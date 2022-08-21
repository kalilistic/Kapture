using System;
using System.Collections.Generic;
using System.Timers;

using Dalamud.DrunkenToad;
using Dalamud.Game.Text;

namespace Kapture
{
    /// <summary>
    /// Send loot messages to discord via plugin.
    /// </summary>
    public class LootDiscord
    {
        private readonly object locker = new();
        private readonly Queue<LootEvent> discordEventQueue = new();
        private readonly KapturePlugin plugin;
        private readonly Timer sendTimer;
        private readonly DiscordBridgeConsumer discordBridgeConsumer;
        private bool isProcessing;
        private bool isDiscordBridgeAvailable;

        /// <summary>
        /// Initializes a new instance of the <see cref="LootDiscord"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public LootDiscord(KapturePlugin plugin)
        {
            this.plugin = plugin;
            this.sendTimer = new Timer
            {
                Interval = this.plugin.Configuration.SendDiscordFrequency, Enabled = true,
            };
            this.discordBridgeConsumer = new DiscordBridgeConsumer();
            this.sendTimer.Elapsed += this.SendToDiscord;
            if (this.plugin.Configuration.SendDiscordEnabled)
            {
                this.isDiscordBridgeAvailable = this.discordBridgeConsumer.IsAvailable();
            }

            this.isProcessing = false;
        }

        /// <summary>
        /// Send Loot to Discord Endpoint queue.
        /// </summary>
        /// <param name="lootEvent">loot event to send.</param>
        public void SendToDiscordQueue(LootEvent lootEvent)
        {
            lock (this.locker)
            {
                this.discordEventQueue.Enqueue(lootEvent);
            }
        }

        /// <summary>
        /// Dispose service.
        /// </summary>
        public void Dispose()
        {
            this.sendTimer.Enabled = false;
            this.sendTimer.Dispose();
        }

        private void SendToDiscord(object? sender, ElapsedEventArgs? e)
        {
            if (this.isProcessing) return;
            this.isProcessing = true;
            if (!this.isDiscordBridgeAvailable)
            {
                this.isDiscordBridgeAvailable = this.discordBridgeConsumer.IsAvailable();
                if (!this.isDiscordBridgeAvailable) return;
            }

            while (this.discordEventQueue.Count > 0)
            {
                try
                {
                    var lootEvent = this.discordEventQueue.Dequeue();
                    var message = $"{lootEvent.PlayerName} | {lootEvent.LootEventTypeName} | {lootEvent.ItemName}";
                    if (lootEvent.LootMessage.IsHq)
                    {
                        message += " " + this.plugin.GetSeIcon(SeIconChar.HighQuality);
                    }

                    this.discordBridgeConsumer.SendMessage(
                        this.plugin.Name,
                        "https://raw.githubusercontent.com/goatcorp/DalamudPlugins/api4/plugins/Kapture/images/icon.png",
                        message);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to sync with discord bridge.");
                    this.isDiscordBridgeAvailable = this.discordBridgeConsumer.IsAvailable();
                }
            }

            this.isProcessing = false;
        }
    }
}
