﻿// ReSharper disable DelegateSubtraction
// ReSharper disable InconsistentNaming
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable InvertIf

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CheapLoc;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace KapturePlugin
{
    public sealed class KapturePlugin : PluginBase, IKapturePlugin
    {
        private DalamudPluginInterface _pluginInterface;
        private PluginUI _pluginUI;

        public KapturePlugin(string pluginName, DalamudPluginInterface pluginInterface) : base(pluginName,
            pluginInterface)
        {
            Task.Run(() =>
            {
                _pluginInterface = pluginInterface;
                DataManager = new DataManager(this);
                ResourceManager.UpdateResources();
                InitItems();
                InitContent();
                LoadConfig();
                LoadServices();
                SetupCommands();
                LoadUI();
                HandleFreshInstall();
                SetupListeners();
                IsInitializing = false;
            });
        }

        private Kapture Kapture { get; set; }
        private RollMonitor.RollMonitor RollMonitor { get; set; }
        private LootProcessor LootProcessor { get; set; }
        public bool IsInitializing { get; private set; } = true;
        public List<LootEvent> LootEvents { get; } = new List<LootEvent>();
        public List<LootRoll> LootRolls { get; } = new List<LootRoll>();
        public List<LootRoll> LootRollsDisplay { get; set; } = new List<LootRoll>();
        public DataManager DataManager { get; private set; }
        public KaptureConfig Configuration { get; private set; }

        public void SaveConfig()
        {
            SaveConfig(Configuration);
        }

        public string FormatPlayerName(int nameFormatCode, string playerName)
        {
            try
            {
                if (string.IsNullOrEmpty(playerName)) return string.Empty;

                if (nameFormatCode == NameFormat.FullName.Code) return playerName;

                if (nameFormatCode == NameFormat.FirstName.Code) return playerName.Split(' ')[0];

                if (nameFormatCode == NameFormat.Initials.Code)
                {
                    var splitName = playerName.Split(' ');
                    return splitName[0].Substring(0, 1) + splitName[1].Substring(0, 1);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to format name.");
            }

            return string.Empty;
        }

        public new void Dispose()
        {
            DisposeListeners();
            Kapture.Dispose();
            base.Dispose();
            RemoveCommands();
            ClearData();
            _pluginInterface.UiBuilder.OnOpenConfigUi -= (sender, args) => DrawConfigUI();
            _pluginInterface.UiBuilder.OnBuildUi -= DrawUI;
            _pluginInterface.Dispose();
        }

        private new void SetupCommands()
        {
            _pluginInterface.CommandManager.AddHandler("/loot", new CommandInfo(ToggleLootOverlay)
            {
                HelpMessage = "Show loot overlay.",
                ShowInHelp = true
            });
            _pluginInterface.CommandManager.AddHandler("/roll", new CommandInfo(ToggleRollOverlay)
            {
                HelpMessage = "Show roll monitor overlay.",
                ShowInHelp = true
            });
            _pluginInterface.CommandManager.AddHandler("/lootconfig", new CommandInfo(ToggleConfig)
            {
                HelpMessage = "Show loot config.",
                ShowInHelp = true
            });
        }

        private new void RemoveCommands()
        {
            _pluginInterface.CommandManager.RemoveHandler("/loot");
            _pluginInterface.CommandManager.RemoveHandler("/roll");
            _pluginInterface.CommandManager.RemoveHandler("/lootconfig");
        }

        private void ToggleLootOverlay(string command, string args)
        {
            LogInfo("Running command {0} with args {1}", command, args);
            Configuration.ShowLootOverlay = !Configuration.ShowLootOverlay;
            _pluginUI.LootOverlayWindow.IsVisible = !_pluginUI.LootOverlayWindow.IsVisible;
        }

        private void ToggleRollOverlay(string command, string args)
        {
            LogInfo("Running command {0} with args {1}", command, args);
            Configuration.ShowRollMonitorOverlay = !Configuration.ShowRollMonitorOverlay;
            _pluginUI.RollMonitorOverlayWindow.IsVisible = !_pluginUI.RollMonitorOverlayWindow.IsVisible;
        }

        private void ToggleConfig(string command, string args)
        {
            LogInfo("Running command {0} with args {1}", command, args);
            _pluginUI.SettingsWindow.IsVisible = !_pluginUI.SettingsWindow.IsVisible;
        }

        private void LoadServices()
        {
            RollMonitor = new RollMonitor.RollMonitor(this);
            LootProcessor = new ENLootProcessor(this);
            Kapture = new Kapture(this);
        }

        private void LoadUI()
        {
            Localization.SetLanguage(Configuration.PluginLanguage);
            _pluginUI = new PluginUI(this);
            _pluginInterface.UiBuilder.OnBuildUi += DrawUI;
            _pluginInterface.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();
        }

        private void HandleFreshInstall()
        {
            if (!Configuration.FreshInstall) return;
            PrintMessage(Loc.Localize("InstallThankYou", "Thank you for installing the Kapture Loot Tracker Plugin!"));
            Thread.Sleep(500);
            PrintMessage(Loc.Localize("Instructions",
                "Use /loot and /roll for the overlays and /lootconfig for settings."));
            Configuration.FreshInstall = false;
            SaveConfig();
            _pluginUI.SettingsWindow.IsVisible = true;
        }

        private void DrawUI()
        {
            _pluginUI.Draw();
        }

        private void DrawConfigUI()
        {
            _pluginUI.SettingsWindow.IsVisible = true;
        }

        private new void LoadConfig()
        {
            try
            {
                Configuration = base.LoadConfig() as PluginConfig ?? new PluginConfig();
            }
            catch (Exception ex)
            {
                LogError("Failed to load config so creating new one.", ex);
                Configuration = new PluginConfig();
                SaveConfig();
            }
        }

        private void SetupListeners()
        {
            if (ClientLanguage() != 1) return;
            _pluginInterface.Framework.Gui.Chat.OnCheckMessageHandled += OnChatMessageHandled;
        }

        private void DisposeListeners()
        {
            if (ClientLanguage() != 1) return;
            _pluginInterface.Framework.Gui.Chat.OnCheckMessageHandled -= OnChatMessageHandled;
        }

        public void LoadTestData()
        {
            // add
            var event1 = new LootEvent
            {
                LootEventType = LootEventType.Add,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Add),
                Timestamp = DateUtil.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = "Boiled Egg"
                },
                ItemDisplayName = "Boiled Egg"
            };
            RollMonitor.ProcessRoll(event1);
            LootEvents.Add(event1);
            
            // add again
            var event2 = new LootEvent
            {
                LootEventType = LootEventType.Add,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Add),
                Timestamp = DateUtil.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = "Wind-up Aldgoat"
                },
                ItemDisplayName = "Wind-up Aldgoat"
            };
            RollMonitor.ProcessRoll(event2);
            LootEvents.Add(event2);
            
            // cast
            var event3 = new LootEvent
            {
                LootEventType = LootEventType.Cast,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Cast),
                Timestamp = DateUtil.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = "Boiled Egg"
                },
                ItemDisplayName = "Boiled Egg",
                PlayerName = "Wyatt Earp",
                PlayerDisplayName = "Wyatt Earp"
            };
            RollMonitor.ProcessRoll(event3);
            LootEvents.Add(event3);

            // cast again
            var event4 = new LootEvent
            {
                LootEventType = LootEventType.Cast,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Cast),
                Timestamp = DateUtil.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = "Boiled Egg"
                },
                ItemDisplayName = "Boiled Egg",
                PlayerName = "April O'Neil",
                PlayerDisplayName = "April O'Neil"
            };
            RollMonitor.ProcessRoll(event4);
            LootEvents.Add(event4);
            
            // obtain
            var event5 = new LootEvent
            {
                LootEventType = LootEventType.Obtain,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Obtain),
                Timestamp = DateUtil.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = "Boiled Egg"
                },
                ItemDisplayName = "Boiled Egg",
                PlayerName = "Wyatt Earp",
                PlayerDisplayName = "Wyatt Earp"
            };
            RollMonitor.ProcessRoll(event5);
            LootEvents.Add(event5);
        }

        public void ClearData()
        {
            LootEvents.Clear();
            LootRolls.Clear();
            LootRollsDisplay.Clear();
        }

        private void OnChatMessageHandled(XivChatType type, uint senderId, ref SeString sender, ref SeString message,
            ref bool isHandled)
        {
            // check if enabled
            if (!Configuration.Enabled) return;

            // remove old rolls if monitor is enabled
            if (Configuration.ShowRollMonitorOverlay) RollMonitor.RemoveOldRolls();

            // lookup territory and content
            var xivChatType = (ushort) type;
            var territoryTypeId = GetTerritoryType();
            var contentId = GetContentId(territoryTypeId);


            // print chat message for troubleshooting
            var lootMessage1 = new LootMessage
            {
                XivChatType = xivChatType,
                Message = message.TextValue
            };
            foreach (var payload in message.Payloads)
                switch (payload)
                {
                    case TextPayload textPayload:
                        lootMessage1.MessageParts.Add(textPayload.Text);
                        break;
                }

            LogInfo("Raw Message: " + lootMessage1);

            // restrict by user settings
            if (Configuration.RestrictToContent && contentId == 0) return;
            if (Configuration.RestrictToHighEndDuty && !IsHighEndDuty(contentId)) return;
            if (Configuration.RestrictToCustomContent && !Configuration.PermittedContent.Contains(contentId)) return;

            // filter out bad messages
            if (!Enum.IsDefined(typeof(LootMessageType), xivChatType)) return;
            if (!message.Payloads.Any(payload => payload is ItemPayload)) return;
            var logKind = (LogKind) ((uint) type & ~(~0 << 7));
            if (!Enum.IsDefined(typeof(LogKind), logKind)) return;

            // build initial loot message
            var lootMessage = new LootMessage
            {
                XivChatType = xivChatType,
                LogKind = logKind,
                LootMessageType = (LootMessageType) xivChatType,
                Message = message.TextValue
            };

            // add name fields for logging/display
            lootMessage.LogKindName = Enum.GetName(typeof(LogKind), lootMessage.LogKind);
            lootMessage.LootMessageTypeName = Enum.GetName(typeof(LootMessageType), lootMessage.LootMessageType);

            // add item and message part payloads
            foreach (var payload in message.Payloads)
                switch (payload)
                {
                    case TextPayload textPayload:
                        lootMessage.MessageParts.Add(textPayload.Text);
                        break;
                    case ItemPayload itemPayload:
                        if (lootMessage.ItemId != 0) break;
                        lootMessage.ItemId = itemPayload.Item.RowId;
                        lootMessage.ItemName = itemPayload.Item.Name.ToString();
                        lootMessage.Item = itemPayload.Item;
                        lootMessage.IsHq = itemPayload.IsHQ;
                        break;
                }

            // filter out non-permitted item ids
            if (Configuration.RestrictToCustomItems && !Configuration.PermittedItems.Contains(lootMessage.ItemId)) return;

            // send to loot processor
            var lootEvent = LootProcessor.ProcessLoot(lootMessage);

            // kick out if didn't process
            if (lootEvent == null) return;

            // enrich
            lootEvent.Timestamp = DateUtil.CurrentTime();
            lootEvent.LootEventId = Guid.NewGuid();
            lootEvent.TerritoryTypeId = territoryTypeId;
            lootEvent.ContentId = contentId;

            // add to list
            if (LootProcessor.IsEnabledEvent(lootEvent)) LootEvents.Add(lootEvent);

            // process for roll monitor
            RollMonitor.ProcessRoll(lootEvent);

            // output
            if (Configuration.LoggingEnabled) Kapture.LogLoot(lootEvent);
        }
    }
}