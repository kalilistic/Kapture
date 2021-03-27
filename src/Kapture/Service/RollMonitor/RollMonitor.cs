// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable UnusedParameter.Local
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable UnusedParameter.Global
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable ConvertIfStatementToReturnStatement

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Timers;
using CheapLoc;
using Newtonsoft.Json;

namespace Kapture
{
    public class RollMonitor
    {
        private readonly IKapturePlugin _plugin;
        private readonly Timer _processTimer;
        public readonly ConcurrentQueue<LootEvent> LootEvents = new ConcurrentQueue<LootEvent>();
        private bool _isProcessing;

        public RollMonitor(IKapturePlugin plugin)
        {
            _plugin = plugin;
            _processTimer = new Timer
                {Interval = _plugin.Configuration.RollMonitorProcessFrequency, Enabled = true};
            _processTimer.Elapsed += ProcessRolls;
        }

        private void ProcessRolls(object source, ElapsedEventArgs e)
        {
            if (_isProcessing) return;
            if (ShouldWait()) return;
            _isProcessing = true;
            while (LootEvents.Count > 0 && !ShouldWait())
            {
                var tryDequeue = LootEvents.TryDequeue(out var lootEvent);
                if (!tryDequeue) continue;
                ProcessRoll(lootEvent);
            }

            if (!ShouldWait()) UpdateRolls();
            _isProcessing = false;
        }

        public void Dispose()
        {
            _processTimer.Elapsed -= ProcessRolls;
            _processTimer.Stop();
        }

        private bool ShouldWait()
        {
            if (!_plugin.Configuration.Enabled) return true;
            if (_plugin.Configuration.RestrictInCombat && _plugin.InCombat()) return true;
            return false;
        }

        private void CreateDisplayList()
        {
            _plugin.LootRollsDisplay =
                JsonConvert.DeserializeObject<List<LootRoll>>(JsonConvert.SerializeObject(_plugin.LootRolls));
        }

        public void UpdateRolls()
        {
            try
            {
                if (_plugin.LootRolls.Count == 0) return;
                var currentTime = DateUtil.CurrentTime();
                _plugin.LootRolls.RemoveAll(roll => !roll.IsWon &&
                                                    currentTime - roll.Timestamp >
                                                    _plugin.Configuration.RollMonitorAddedTimeout);
                _plugin.LootRolls.RemoveAll(roll => roll.IsWon &&
                                                    currentTime - roll.Timestamp >
                                                    _plugin.Configuration.RollMonitorObtainedTimeout);
                _plugin.IsRolling = _plugin.LootRolls.Count > 0;
                CreateDisplayList();
            }
            catch (Exception ex)
            {
                _plugin.LogError(ex, "Failed to remove old rolls.");
            }
        }

        public void ProcessRoll(LootEvent lootEvent)
        {
            try
            {
                if (lootEvent.ContentId == 0) return;
                switch (lootEvent.LootEventType)
                {
                    case LootEventType.Add:
                        _plugin.LootRolls.Add(new LootRoll
                        {
                            Timestamp = lootEvent.Timestamp,
                            ItemId = lootEvent.LootMessage.ItemId,
                            ItemName = lootEvent.ItemName,
                            ItemNameAbbreviated = lootEvent.ItemNameAbbreviated
                        });
                        break;
                    case LootEventType.Cast:
                    {
                        var lootRoll = _plugin.LootRolls.FirstOrDefault(roll =>
                            roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon &&
                            !roll.Rollers.Any(roller => roller.PlayerName.Equals(lootEvent.PlayerName)));
                        if (lootRoll == null) return;
                        lootRoll.Rollers.Add(new LootRoller {PlayerName = lootEvent.PlayerName});
                        lootRoll.RollerCount += 1;
                        lootRoll.RollersDisplay.Clear();
                        foreach (var roller in lootRoll.Rollers)
                            lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(_plugin.FormatPlayerName(
                                _plugin.Configuration.RollNameFormat,
                                roller.PlayerName), UIColor.GetColorByNumber(0)));
                        lootRoll.RollersDisplay = lootRoll.RollersDisplay.OrderBy(pair => pair.Key).ToList();
                        break;
                    }
                    case LootEventType.Need:
                    case LootEventType.Greed:
                    {
                        var lootRoll = _plugin.LootRolls.FirstOrDefault(roll =>
                            roll.ItemId == lootEvent.LootMessage.ItemId &&
                            roll.Rollers.Any(roller =>
                                roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0));
                        var lootRoller = lootRoll?.Rollers.FirstOrDefault(roller =>
                            roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0);
                        if (lootRoller == null) return;
                        lootRoller.Roll = lootEvent.Roll;
                        lootRoll.RollersDisplay.Clear();
                        foreach (var roller in lootRoll.Rollers)
                            if (roller.Roll == 0)
                                lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(_plugin.FormatPlayerName(
                                    _plugin.Configuration.RollNameFormat,
                                    roller.PlayerName) + " [x]", UIColor.GetColorByNumber(0)));
                            else
                                lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(_plugin.FormatPlayerName(
                                    _plugin.Configuration.RollNameFormat,
                                    roller.PlayerName) + " [" + roller.Roll + "]", UIColor.GetColorByNumber(roller.Roll)));
                        lootRoll.RollersDisplay = lootRoll.RollersDisplay.OrderBy(pair => pair.Key).ToList();
                        break;
                    }
                    case LootEventType.Obtain:
                    {
                        var lootRoll =
                            _plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon);
                        if (lootRoll == null) return;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        var winningRoller =
                            lootRoll.Rollers.FirstOrDefault(roller => roller.PlayerName.Equals(lootEvent.PlayerName));
                        if (winningRoller != null) winningRoller.IsWinner = true;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        lootRoll.IsWon = true;
                        lootRoll.Winner =
                            _plugin.FormatPlayerName(_plugin.Configuration.RollNameFormat, lootEvent.PlayerName);
                        break;
                    }
                    case LootEventType.Lost:
                    {
                        var lootRoll =
                            _plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon);
                        if (lootRoll == null) return;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        lootRoll.IsWon = true;
                        lootRoll.Winner = Loc.Localize("RollMonitorLost", "Dropped to floor");
                        lootRoll.RollersDisplay.Clear();
                        foreach (var roller in lootRoll.Rollers)
                            lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(_plugin.FormatPlayerName(
                                _plugin.Configuration.RollNameFormat,
                                roller.PlayerName) + " [x]", UIColor.GetColorByNumber(0)));
                        lootRoll.RollersDisplay = lootRoll.RollersDisplay.OrderBy(pair => pair.Key).ToList();
                        break;
                    }
                }

                CreateDisplayList();
            }
            catch (Exception ex)
            {
                _plugin.LogError(ex, "Failed to process for roll monitor.");
            }
        }
    }
}