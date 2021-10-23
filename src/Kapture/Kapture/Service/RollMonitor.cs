using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                this.SendRollReminder();
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
                LootRoll? lootRoll;
                switch (lootEvent.LootEventType)
                {
                    case LootEventType.Add:
                        lootRoll = new LootRoll
                        {
                            Timestamp = lootEvent.Timestamp,
                            ItemId = lootEvent.LootMessage.ItemId,
                            ItemName = lootEvent.ItemName,
                            ItemNameAbbreviated = lootEvent.ItemNameAbbreviated,
                        };
                        this.plugin.LootRolls.Add(lootRoll);

                        foreach (var player in this.plugin.GetPartyMembers())
                        {
                            lootRoll.Rollers.Add(new LootRoller
                            {
                                PlayerName = this.plugin.FormatPlayerName(
                                    this.plugin.Configuration.RollNameFormat,
                                    player.Name.ToString()),
                                RollColor = ImGuiColorUtil.GetColorByNumber(0),
                                IsLocalPlayer = player.ObjectId == KapturePlugin.ClientState.LocalPlayer?.ObjectId,
                            });
                        }

                        if (this.plugin.Configuration.WatchListItems.Contains(lootEvent.LootMessage.ItemId))
                        {
                            KapturePlugin.Chat.PluginPrintNotice(string.Format(
                             Loc.Localize("WatchListAddedAlert", "{0} dropped and is on your watch list."),
                             lootEvent.LootMessage.ItemName));
                        }

                        break;
                    case LootEventType.Cast:
                    {
                        lootRoll = this.plugin.LootRolls.FirstOrDefault(roll =>
                            roll.ItemId == lootEvent.LootMessage.ItemId &&
                            !roll.IsWon &&
                            !roll.Rollers.Any(roller => roller.PlayerName.Equals(lootEvent.PlayerName) && roller.HasRolled));
                        if (lootRoll == null) return;
                        var lootRoller =
                            lootRoll.Rollers.FirstOrDefault(roller => roller.PlayerName.Equals(lootEvent.PlayerName));
                        if (lootRoller != null)
                        {
                            lootRoller.HasRolled = true;
                        }
                        else
                        {
                            lootRoll.Rollers.Add(new LootRoller
                            {
                                PlayerName = this.plugin.FormatPlayerName(
                                    this.plugin.Configuration.RollNameFormat,
                                    lootEvent.PlayerName),
                                RollColor = ImGuiColorUtil.GetColorByNumber(0),
                                HasRolled = true,
                            });
                        }

                        lootRoll.RollerCount += 1;
                        break;
                    }

                    case LootEventType.Need:
                    case LootEventType.Greed:
                    {
                        lootRoll = this.plugin.LootRolls.FirstOrDefault(roll =>
                            roll.ItemId == lootEvent.LootMessage.ItemId &&
                            roll.Rollers.Any(roller =>
                                roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0));
                        var lootRoller = lootRoll?.Rollers.FirstOrDefault(roller =>
                            roller.PlayerName.Equals(lootEvent.PlayerName) && roller.Roll == 0);
                        if (lootRoller == null) return;
                        lootRoller.Roll = lootEvent.Roll;
                        lootRoller.RollColor = ImGuiColorUtil.GetColorByNumber(lootRoller.Roll);
                        if (lootRoller.Roll != 0)
                        {
                            lootRoller.PlayerName = this.plugin.FormatPlayerName(
                                                                 this.plugin.Configuration.RollNameFormat,
                                                                 lootRoller.PlayerName) + " [" + lootRoller.Roll + "]";
                        }

                        break;
                    }

                    case LootEventType.Obtain:
                    {
                        lootRoll =
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
                        if (lootEvent.IsLocalPlayer && this.plugin.Configuration.WatchListItems.Contains(lootEvent.LootMessage.ItemId))
                        {
                            KapturePlugin.Chat.PluginPrintNotice(string.Format(
                                Loc.Localize("WatchListObtainedAlert", "{0} obtained so removing from your watch list."),
                                lootEvent.LootMessage.ItemName));
                            this.plugin.Configuration.WatchListItems.Remove(lootEvent.LootMessage.ItemId);
                        }

                        break;
                    }

                    case LootEventType.Lost:
                    {
                        lootRoll =
                            this.plugin.LootRolls.FirstOrDefault(roll =>
                                roll.ItemId == lootEvent.LootMessage.ItemId && !roll.IsWon);
                        if (lootRoll == null) return;
                        lootRoll.Timestamp = lootEvent.Timestamp;
                        lootRoll.IsWon = true;
                        lootRoll.Winner = Loc.Localize("RollMonitorLost", "Dropped to floor");

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
            while (!this.LootEvents.IsEmpty && !this.ShouldWait())
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

        private void SendRollReminder()
        {
            if (!this.plugin.Configuration.SendRollReminder) return;
            foreach (var lootRoll in this.plugin.LootRolls.Where(roll => !roll.IsWon))
            {
                var rollerMatch = lootRoll.Rollers.FirstOrDefault(roller =>
                                                         roller.IsLocalPlayer &&
                                                         !roller.HasRolled && !roller.IsReminderSent &&
                                                         DateUtil.CurrentTime() > lootRoll.Timestamp + 300000 - this.plugin.Configuration.RollReminderTime);
                if (rollerMatch != null)
                {
                    rollerMatch.IsReminderSent = true;
                    KapturePlugin.Chat.PluginPrintNotice(string.Format(Loc.Localize("RollReminder", "Roll soon on {0}!"), lootRoll.ItemName));
                }
            }
        }
    }
}
