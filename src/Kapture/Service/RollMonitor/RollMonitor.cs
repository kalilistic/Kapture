// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable UnusedParameter.Local
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable UnusedParameter.Global
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
using System;
using System.Collections.Generic;
using System.Linq;
using CheapLoc;
using Newtonsoft.Json;

namespace KapturePlugin.RollMonitor
{
    public class RollMonitor
    {
        private static readonly object Lock = new object();

        private readonly IKapturePlugin _plugin;

        public RollMonitor(IKapturePlugin plugin)
        {
            _plugin = plugin;
        }

        private void CreateDisplayList()
        {
            _plugin.LootRollsDisplay =
                JsonConvert.DeserializeObject<List<LootRoll>>(JsonConvert.SerializeObject(_plugin.LootRolls));
        }

        public void UpdateRolls()
        {
            lock (Lock)
            {
                try
                {
                    if (_plugin.LootRolls.Count == 0) return;
                    var currentTime = DateUtil.CurrentTime();
                    _plugin.LootRolls.RemoveAll(roll => string.IsNullOrEmpty(roll.Winner) &&
                        currentTime - roll.Timestamp > _plugin.Configuration.RollMonitorAddedTimeout);
                    _plugin.LootRolls.RemoveAll(roll => !string.IsNullOrEmpty(roll.Winner) && 
                        currentTime - roll.Timestamp > _plugin.Configuration.RollMonitorObtainedTimeout);
                    _plugin.IsRolling = _plugin.LootRolls.Count > 0;
                    CreateDisplayList();
                }
                catch (Exception ex)
                {
                    _plugin.LogError(ex, "Failed to remove old rolls.");
                }
            }
        }
        public void ProcessRoll(LootEvent lootEvent)
        {
            lock (Lock)
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
                                ItemName = lootEvent.ItemDisplayName,
                                RollersDisplay = Loc.Localize("RollMonitorNone", "No one has rolled")
                            });
                            break;
                        case LootEventType.Cast:
                        {
                            var lootRoll = _plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon &&
                                !roll.Rollers.Any(roller => roller.PlayerName.Equals(lootEvent.PlayerName)));
                            if (lootRoll == null) return;
                            lootRoll.Rollers.Add(new LootRoller { PlayerName = lootEvent.PlayerName});
                            var rollers = lootRoll.Rollers.Select(roller =>
                                _plugin.FormatPlayerName(_plugin.Configuration.RollNameFormat, roller.PlayerName)).ToList();
                            lootRoll.RollersDisplay = string.Join(", ", rollers);
                            if (_plugin.Configuration.ShowRollerCount)
                                lootRoll.RollersDisplay = "[" + rollers.Count + "] " + lootRoll.RollersDisplay;
                            break;
                        }
                        case LootEventType.Need:
                        case LootEventType.Greed:
                        {
                            var lootRoll = _plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon &&
                                roll.Rollers.Any(roller =>
                                    roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0));
                            var lootRoller = lootRoll?.Rollers.FirstOrDefault(roller =>
                                roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0);
                            if (lootRoller == null) return;
                            lootRoller.Roll = lootEvent.Roll;
                            var rollers = new List<string>();
                            if (_plugin.Configuration.ShowRollNumbers)
                            {
                                foreach (var roller in lootRoll.Rollers)
                                {
                                    if (roller.Roll == 0)
                                    {
                                        rollers.Add(_plugin.FormatPlayerName(_plugin.Configuration.RollNameFormat,
                                            roller.PlayerName));
                                    }
                                    else
                                    {
                                        rollers.Add(_plugin.FormatPlayerName(_plugin.Configuration.RollNameFormat,
                                            roller.PlayerName) + "[" + roller.Roll + "]");
                                    }
                                }
                                lootRoll.RollersDisplay = string.Join(", ", rollers);
                                if (_plugin.Configuration.ShowRollerCount)
                                    lootRoll.RollersDisplay = "[" + rollers.Count + "] " + lootRoll.RollersDisplay; 
                            }
                            break;
                        }
                        case LootEventType.Obtain:
                        {
                            var lootRoll =
                                _plugin.LootRolls.FirstOrDefault(roll =>
                                    roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon);
                            if (lootRoll == null) return;
                            var winningRoller = lootRoll.Rollers.FirstOrDefault(roller => roller.PlayerName.Equals(lootEvent.PlayerName));
                            if (winningRoller != null)
                            {
                                winningRoller.IsWinner = true;
                            }
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
                            lootRoll.IsWon = true;
                            lootRoll.Winner = Loc.Localize("RollMonitorLost", "Dropped to floor");
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
}