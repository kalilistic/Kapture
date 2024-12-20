using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapture.Kapture.Extensions
{
        public static class ChatExtensions
        {
            /// <summary>
            /// Print message with plugin name to dalamud default channel.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="message">chat message.</param>
            public static void PluginPrint(this IChatGui value, string message) => value.Print(BuildSeString(KapturePlugin.PluginInterface.InternalName, message));

            /// <summary>
            /// Print payload with plugin name to dalamud default channel.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="payloads">list of payloads.</param>
            public static void PluginPrint(this IChatGui value, IEnumerable<Payload> payloads) =>
                value.Print(BuildSeString(KapturePlugin.PluginInterface.InternalName, payloads));

            /// <summary>
            /// Print message with plugin name to specified channel.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="message">chat message.</param>
            /// <param name="chatType">chat type to use.</param>
            public static void PluginPrint(this IChatGui value, string message, XivChatType chatType) => value.Print(new XivChatEntry
            {
                Message = BuildSeString(KapturePlugin.PluginInterface.InternalName, message), Type = chatType,
            });

            /// <summary>
            /// Print message with plugin name to specified channel.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="payloads">list of payloads.</param>
            /// <param name="chatType">chat type to use.</param>
            public static void PluginPrint(this IChatGui value, IEnumerable<Payload> payloads, XivChatType chatType) => value.Print(new XivChatEntry
            {
                Message = BuildSeString(KapturePlugin.PluginInterface.InternalName, payloads), Type = chatType,
            });

            /// <summary>
            /// Print message with plugin name to notice channel.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="payloads">list of payloads.</param>
            public static void PluginPrintNotice(this IChatGui value, IEnumerable<Payload> payloads) => value.Print(new XivChatEntry
            {
                Message = BuildSeString(KapturePlugin.PluginInterface.InternalName, payloads), Type = XivChatType.Notice,
            });

            /// <summary>
            /// Print message with plugin name to notice channel.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="message">chat message.</param>
            public static void PluginPrintNotice(this IChatGui value, string message) => value.Print(new XivChatEntry
            {
                Message = BuildSeString(KapturePlugin.PluginInterface.InternalName, message), Type = XivChatType.Notice,
            });

            /// <summary>
            /// Print chat with player and message.
            /// </summary>
            /// <param name="value">chat gui service.</param>
            /// <param name="playerName">player name.</param>
            /// <param name="worldId">player home world id.</param>
            /// <param name="message">message.</param>
            /// <param name="chatType">target channel.</param>
            public static void PluginPrint(this IChatGui value, string playerName, uint worldId, string message, XivChatType chatType)
            {
                var basePayloads = BuildBasePayloads(KapturePlugin.PluginInterface.InternalName);
                var customPayloads = new List<Payload> { new PlayerPayload(playerName, worldId), new TextPayload(" " + message) };

                var seString = new SeString(basePayloads.Concat(customPayloads).ToList());
                value.Print(new XivChatEntry { Message = seString, Type = chatType });
            }

            private static SeString BuildSeString(string? pluginName, string message)
            {
                var basePayloads = BuildBasePayloads(pluginName);
                var customPayloads = new List<Payload> { new TextPayload(message) };

                return new SeString(basePayloads.Concat(customPayloads).ToList());
            }

            private static SeString BuildSeString(string? pluginName, IEnumerable<Payload> payloads)
            {
                var basePayloads = BuildBasePayloads(pluginName);
                return new SeString(basePayloads.Concat(payloads).ToList());
            }

            private static IEnumerable<Payload> BuildBasePayloads(string? pluginName) => new List<Payload>
    {
        new UIForegroundPayload(0), new TextPayload($"[{pluginName}] "), new UIForegroundPayload(548),
    };
        }
    }
