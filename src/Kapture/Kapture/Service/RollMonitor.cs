using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Timers;

using CheapLoc;
using Dalamud.DrunkenToad;
using Newtonsoft.Json;

namespace Kapture
{
    /// <summary>
    /// Roll monitor service.
    /// </summary>
    public class RollMonitor
    {
        /// <summary>
        /// Queue of loot events.
        /// </summary>
        public readonly ConcurrentQueue<LootEvent> LootEvents = new ();

        private readonly IKapturePlugin plugin;
        private readonly Timer processTimer;
        private bool isProcessing;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollMonitor"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public RollMonitor(IKapturePlugin plugin)
        {
            this.plugin = plugin;
            this.processTimer = new Timer
            {
                Interval = this.plugin.Configuration.RollMonitorProcessFrequency, Enabled = true,
            };
            this.processTimer.Elapsed += this.ProcessRolls;
        }

        /// <summary>
        /// Dispose service.
        /// </summary>
        public void Dispose()
        {
            this.processTimer.Elapsed -= this.ProcessRolls;
            this.processTimer.Stop();
        }

        /// <summary>
        /// Update queued rolls.
        /// </summary>
        public void UpdateRolls()
        {
            try
            {
                if (this.plugin.LootRolls.Count == 0) return;
                var currentTime = DateUtil.CurrentTime();
                this.plugin.LootRolls.RemoveAll(roll => !roll.IsWon &&
                                                    currentTime - roll.Timestamp >
                                                    this.plugin.Configuration.RollMonitorAddedTimeout);
                this.plugin.LootRolls.RemoveAll(roll => roll.IsWon &&
                                                    currentTime - roll.Timestamp >
                                                    this.plugin.Configuration.RollMonitorObtainedTimeout);
                this.plugin.IsRolling = this.plugin.LootRolls.Count > 0;
                this.CreateDisplayList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to remove old rolls.");
            }
        }

        /// <summary>
        /// Process new roll.
        /// </summary>
        /// <param name="lootEvent">loot event.</param>
        /// <exception cref="ArgumentOutOfRangeException">unrecognized loot event type.</exception>
        public void ProcessRoll(LootEvent lootEvent)
        {
            try
            {
                if (lootEvent.ContentId == 0) return;
                switch (lootEvent.LootEventType)
                {
                    case LootEventType.Add:
                        this.plugin.LootRolls.Add(new LootRoll
                        {
                            Timestamp = lootEvent.Timestamp,
                            ItemId = lootEvent.LootMessage.ItemId,
                            ItemName = lootEvent.ItemName,
                            ItemNameAbbreviated = lootEvent.ItemNameAbbreviated,
                        });
                        break;
                    case LootEventType.Cast:
                    {
                        var lootRoll = this.plugin.LootRolls.FirstOrDefault(roll =>
                            roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon &&
                            !roll.Rollers.Any(roller => roller.PlayerName.Equals(lootEvent.PlayerName)));
                        if (lootRoll == null) return;
                        lootRoll.Rollers.Add(new LootRoller { PlayerName = lootEvent.PlayerName });
                        lootRoll.RollerCount += 1;
                        lootRoll.RollersDisplay.Clear();
                        foreach (var roller in lootRoll.Rollers)
                        {
                            lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(
                                                            this.plugin.FormatPlayerName(
                                                                this.plugin.Configuration.RollNameFormat,
                                                                roller.PlayerName), ImGuiColorUtil.GetColorByNumber(0)));
                        }

                        lootRoll.RollersDisplay = lootRoll.RollersDisplay.OrderBy(pair => pair.Key).ToList();
                        break;
                    }

                    case LootEventType.Need:
                    case LootEventType.Greed:
                    {
                        var lootRoll = this.plugin.LootRolls.FirstOrDefault(roll =>
                            roll.ItemId == lootEvent.LootMessage.ItemId &&
                            roll.Rollers.Any(roller =>
                                roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0));
                        var lootRoller = lootRoll?.Rollers.FirstOrDefault(roller =>
                            roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0);
                        if (lootRoller == null) return;
                        lootRoller.Roll = lootEvent.Roll;
                        lootRoll?.RollersDisplay.Clear();
                        foreach (var roller in lootRoll?.Rollers!)
                        {
                            if (roller.Roll == 0)
                            {
                                lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(
                                                                this.plugin.FormatPlayerName(
                                                                    this.plugin.Configuration.RollNameFormat,
                                                                    roller.PlayerName) + " [x]", ImGuiColorUtil.GetColorByNumber(0)));
                            }
                            else
                            {
                                lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(
                                                                this.plugin.FormatPlayerName(
                                                                    this.plugin.Configuration.RollNameFormat,
                                                                    roller.PlayerName) + " [" + roller.Roll + "]", ImGuiColorUtil.GetColorByNumber(roller.Roll)));
                            }
                        }

                        lootRoll.RollersDisplay = lootRoll.RollersDisplay.OrderBy(pair => pair.Key).ToList();
                        break;
                    }

                    case LootEventType.Obtain:
                    {
                        var lootRoll =
                            this.plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon);
                        if (lootRoll == null) return;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        var winningRoller =
                            lootRoll.Rollers.FirstOrDefault(roller => roller.PlayerName.Equals(lootEvent.PlayerName));
                        if (winningRoller != null) winningRoller.IsWinner = true;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        lootRoll.IsWon = true;
                        lootRoll.Winner =
                            this.plugin.FormatPlayerName(this.plugin.Configuration.RollNameFormat, lootEvent.PlayerName);
                        break;
                    }

                    case LootEventType.Lost:
                    {
                        var lootRoll =
                            this.plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon);
                        if (lootRoll == null) return;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        lootRoll.IsWon = true;
                        lootRoll.Winner = Loc.Localize("RollMonitorLost", "Dropped to floor");
                        lootRoll.RollersDisplay.Clear();
                        foreach (var roller in lootRoll.Rollers)
                        {
                            lootRoll.RollersDisplay.Add(new KeyValuePair<string, Vector4>(
                                                            this.plugin.FormatPlayerName(
                                                                this.plugin.Configuration.RollNameFormat,
                                                                roller.PlayerName) + " [x]", ImGuiColorUtil.GetColorByNumber(0)));
                        }

                        lootRoll.RollersDisplay = lootRoll.RollersDisplay.OrderBy(pair => pair.Key).ToList();
                        break;
                    }

                    case LootEventType.Craft:
                        break;
                    case LootEventType.Desynth:
                        break;
                    case LootEventType.Discard:
                        break;
                    case LootEventType.Gather:
                        break;
                    case LootEventType.Purchase:
                        break;
                    case LootEventType.Search:
                        break;
                    case LootEventType.Sell:
                        break;
                    case LootEventType.Use:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.CreateDisplayList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process for roll monitor.");
            }
        }

        private void ProcessRolls(object source, ElapsedEventArgs e)
        {
            if (this.isProcessing) return;
            if (this.ShouldWait()) return;
            this.isProcessing = true;
            while (this.LootEvents.IsEmpty && !this.ShouldWait())
            {
                var tryDequeue = this.LootEvents.TryDequeue(out var lootEvent);
                if (!tryDequeue) continue;
                if (lootEvent != null) this.ProcessRoll(lootEvent);
            }

            if (!this.ShouldWait()) this.UpdateRolls();
            this.isProcessing = false;
        }

        private bool ShouldWait()
        {
            if (!this.plugin.Configuration.Enabled) return true;
            if (this.plugin.Configuration.RestrictInCombat && this.plugin.InCombat()) return true;
            return false;
        }

        private void CreateDisplayList()
        {
            this.plugin.LootRollsDisplay =
                JsonConvert.DeserializeObject<List<LootRoll>>(JsonConvert.SerializeObject(this.plugin.LootRolls));
        }
    }
}
