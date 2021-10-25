using System;

using Dalamud.DrunkenToad;
using Dalamud.Plugin.Ipc;

namespace Kapture
{
    /// <summary>
    /// IPC with DiscordBridge Plugin.
    /// </summary>
    public class DiscordBridgeConsumer
    {
        private const string RequiredDiscordBridgeVersion = "1";
        private ICallGateSubscriber<string> consumerApiVersion = null!;
        private ICallGateSubscriber<string, string, string, object> consumerSendMessage = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordBridgeConsumer"/> class.
        /// </summary>
        public DiscordBridgeConsumer()
            => this.Subscribe();

        /// <summary>
        /// Subscribe to DiscordBridge plugin methods.
        /// </summary>
        public void Subscribe()
        {
            try
            {
                this.consumerApiVersion =
                   KapturePlugin.PluginInterface.GetIpcSubscriber<string>("DiscordBridge.APIVersion");
                this.consumerSendMessage =
                    KapturePlugin.PluginInterface.GetIpcSubscriber<string, string, string, object>(
                        "DiscordBridge.SendMessage");
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Failed to subscribe to DiscordBridge.:\n{ex}");
            }
        }

        /// <summary>
        /// Check if DiscordBridge is available.
        /// </summary>
        /// <returns>Gets indicator whether DiscordBridge is available.</returns>
        public bool IsAvailable()
        {
            try
            {
                var version = this.consumerApiVersion.InvokeFunc();
                return version.Equals(RequiredDiscordBridgeVersion);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send discord message.
        /// </summary>
        /// <param name="pluginName">plugin / assembly name.</param>
        /// <param name="avatarUrl">avatar url.</param>
        /// <param name="message">message to send.</param>
        public void SendMessage(string pluginName, string avatarUrl, string message)
        {
            this.consumerSendMessage.InvokeAction(pluginName, avatarUrl, message);
        }
    }
}
