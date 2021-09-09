using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using CheapLoc;
using Dalamud.Data;
using Dalamud.DrunkenToad;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.IoC;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

using Condition = Dalamud.Game.ClientState.Conditions.Condition;

namespace Kapture
{
    /// <inheritdoc cref="Kapture.IKapturePlugin" />
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class KapturePlugin : IKapturePlugin, IDalamudPlugin
    {
        private const string RepoName = "Dalamud.Kapture";
        private readonly Localization localization = null!;
        private PluginUI pluginUI = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="KapturePlugin"/> class.
        /// </summary>
        public KapturePlugin()
        {
            try
            {
                this.localization = new Localization(PluginInterface, CommandManager);
                this.InitContent();
                this.InitItems();
                this.PluginDataManager = new PluginDataManager(this);
                this.LoadConfig();
                this.LoadServices();
                this.SetupCommands();
                this.LoadUI();
                this.HandleFreshInstall();
                this.SetupListeners();
                this.IsInitializing = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to initialize.");
                this.Dispose();
            }
        }

        /// <summary>
        /// Gets pluginInterface.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

        /// <summary>
        /// Gets data manager.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static DataManager DataManager { get; private set; } = null!;

        /// <summary>
        /// Gets command manager.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static CommandManager CommandManager { get; private set; } = null!;

        /// <summary>
        /// Gets client state.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static ClientState ClientState { get; private set; } = null!;

        /// <summary>
        /// Gets chat gui.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static ChatGui Chat { get; private set; } = null!;

        /// <summary>
        /// Gets condition.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static Condition Condition { get; private set; } = null!;

        /// <inheritdoc />
        public string Name { get; } = "Kapture";

        /// <inheritdoc />
        public RollMonitor RollMonitor { get; private set; } = null!;

        /// <summary>
        /// Gets loot processor.
        /// </summary>
        public LootProcessor LootProcessor { get; private set; } = null!;

        /// <summary>
        /// Gets or sets loot Logger.
        /// </summary>
        public LootLogger LootLogger { get; set; } = null!;

        /// <summary>
        /// Gets or sets list of item category names.
        /// </summary>
        public string[] ItemCategoryNames { get; set; } = null!;

        /// <summary>
        /// Gets or sets list of items for settings.
        /// </summary>
        public List<KeyValuePair<uint, ItemList>> ItemLists { get; set; } = null!;

        /// <inheritdoc />
        public bool IsInitializing { get; private set; }

        /// <inheritdoc />
        public List<LootEvent> LootEvents { get; } = new ();

        /// <inheritdoc />
        public List<LootRoll> LootRolls { get; } = new ();

        /// <inheritdoc />
        public List<LootRoll>? LootRollsDisplay { get; set; } = new ();

        /// <inheritdoc />
        public PluginDataManager PluginDataManager { get; private set; } = null!;

        /// <inheritdoc />
        public KaptureConfig Configuration { get; private set; } = null!;

        /// <summary>
        /// Gets or sets list of content ids.
        /// </summary>
        public uint[] ContentIds { get; set; } = null!;

        /// <summary>
        /// Gets or sets list of content names.
        /// </summary>
        public string[] ContentNames { get; set; } = null!;

        /// <summary>
        /// Gets or sets list of item ids.
        /// </summary>
        public uint[] ItemIds { get; set; } = null!;

        /// <summary>
        /// Gets or sets list of item names.
        /// </summary>
        public string[] ItemNames { get; set; } = null!;

        /// <summary>
        /// Gets or sets list of item category ids.
        /// </summary>
        public uint[] ItemCategoryIds { get; set; } = null!;

        /// <inheritdoc />
        public bool InContent { get; set; }

        /// <inheritdoc />
        public bool IsRolling { get; set; }

        /// <summary>
        /// Save configuration.
        /// </summary>
        public void SaveConfig()
        {
            PluginInterface.SavePluginConfig(this.Configuration);
        }

        /// <inheritdoc />
        public string GetSeIcon(SeIconChar seIconChar)
        {
            return Convert.ToChar(seIconChar, CultureInfo.InvariantCulture)
                          .ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public string GetLocalPlayerName()
        {
            return ClientState.LocalPlayer?.Name.ToString() ?? string.Empty;
        }

        /// <inheritdoc />
        public ushort ClientLanguage()
        {
            return (ushort)ClientState.ClientLanguage;
        }

        /// <inheritdoc />
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

                if (nameFormatCode == NameFormat.SurnameAbbreviated.Code)
                {
                    var splitName = playerName.Split(' ');
                    return splitName[0] + " " + splitName[1].Substring(0, 1) + ".";
                }

                if (nameFormatCode == NameFormat.ForenameAbbreviated.Code)
                {
                    var splitName = playerName.Split(' ');
                    return splitName[0].Substring(0, 1) + ". " + splitName[1];
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to format name.");
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public bool IsLoggedIn()
        {
            return ClientState.IsLoggedIn;
        }

        /// <summary>
        /// Load test data.
        /// </summary>
        public void LoadTestData()
        {
            TestData.LoadTestData(this);
        }

        /// <summary>
        /// Clear current data.
        /// </summary>
        public void ClearData()
        {
            this.LootEvents.Clear();
            this.LootRolls.Clear();
            this.LootRollsDisplay?.Clear();
            this.IsRolling = false;
        }

        /// <inheritdoc />
        public bool InCombat()
        {
            return Condition.InCombat();
        }

        /// <summary>
        /// Dispose plugin.
        /// </summary>
        public void Dispose()
        {
            this.DisposeListeners();
            this.LootLogger.Dispose();
            this.RollMonitor.Dispose();
            this.RemoveCommands();
            this.ClearData();
            PluginInterface.Dispose();
            this.localization.Dispose();
        }

        private void PrintMessage(string message)
        {
            Chat.Print(message);
        }

        private void SetupCommands()
        {
            CommandManager.AddHandler("/loot", new CommandInfo(this.ToggleLootOverlay)
            {
                HelpMessage = "Show loot overlay.",
                ShowInHelp = true,
            });
            CommandManager.AddHandler("/roll", new CommandInfo(this.ToggleRollOverlay)
            {
                HelpMessage = "Show roll monitor overlay.",
                ShowInHelp = true,
            });
            CommandManager.AddHandler("/lootconfig", new CommandInfo(this.ToggleConfig)
            {
                HelpMessage = "Show loot config.",
                ShowInHelp = true,
            });
        }

        private void RemoveCommands()
        {
            CommandManager.RemoveHandler("/loot");
            CommandManager.RemoveHandler("/roll");
            CommandManager.RemoveHandler("/lootconfig");
        }

        private void ToggleLootOverlay(string command, string args)
        {
            Logger.LogInfo("Running command {0} with args {1}", command, args);
            this.Configuration.ShowLootOverlay = !this.Configuration.ShowLootOverlay;
            this.pluginUI.LootOverlayWindow.IsVisible = !this.pluginUI.LootOverlayWindow.IsVisible;
            this.SaveConfig();
        }

        private void ToggleRollOverlay(string command, string args)
        {
            Logger.LogInfo("Running command {0} with args {1}", command, args);
            this.Configuration.ShowRollMonitorOverlay = !this.Configuration.ShowRollMonitorOverlay;
            this.pluginUI.RollMonitorOverlay.IsVisible = !this.pluginUI.RollMonitorOverlay.IsVisible;
            this.SaveConfig();
        }

        private void ToggleConfig(string command, string args)
        {
            Logger.LogInfo("Running command {0} with args {1}", command, args);
            this.pluginUI.SettingsWindow.IsVisible = !this.pluginUI.SettingsWindow.IsVisible;
        }

        private void LoadServices()
        {
            this.RollMonitor = new RollMonitor(this);
            var langCode = this.ClientLanguage();
            this.LootProcessor = langCode switch
            {
                // japanese
                0 => new ENLootProcessor(this),

                // english
                1 => new ENLootProcessor(this),

                // german
                2 => new DELootProcessor(this),

                // french
                3 => new ENLootProcessor(this),

                // chinese
                4 => new ZHLootProcessor(this),
                _ => this.LootProcessor,
            };

            this.LootLogger = new LootLogger(this);
        }

        private void LoadUI()
        {
            this.pluginUI = new PluginUI(this);
            PluginInterface.UiBuilder.Draw += this.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
        }

        private void OpenConfigUi()
        {
            this.pluginUI.SettingsWindow.IsVisible = true;
        }

        private void HandleFreshInstall()
        {
            if (!this.Configuration.FreshInstall) return;
            this.PrintMessage(Loc.Localize("InstallThankYou", "Thank you for installing the Kapture Loot Tracker Plugin!"));
            Thread.Sleep(500);
            this.PrintMessage(Loc.Localize(
                             "Instructions",
                             "Use /loot and /roll for the overlays and /lootconfig for settings."));
            this.Configuration.FreshInstall = false;
            this.SaveConfig();
            this.pluginUI.SettingsWindow.IsVisible = true;
        }

        private void Draw()
        {
            this.pluginUI.Draw();
        }

        private void LoadConfig()
        {
            try
            {
                this.Configuration = PluginInterface.GetPluginConfig() as KaptureConfig ?? new KaptureConfig();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load config so creating new one.", ex);
                this.Configuration = new KaptureConfig();
                this.SaveConfig();
            }
        }

        private void SetupListeners()
        {
            Chat.CheckMessageHandled += this.ChatMessageHandled;
        }

        private void DisposeListeners()
        {
            Chat.CheckMessageHandled -= this.ChatMessageHandled;
        }

        private void ChatMessageHandled(
            XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            // check if enabled
            if (!this.Configuration.Enabled) return;

            // log for debugging
            if (this.Configuration.DebugLoggingEnabled) Logger.LogInfo("[ChatMessage]" + type + ":" + message);

            // combat check
            if (this.Configuration.RestrictInCombat && this.InCombat()) return;

            // lookup territory and content
            var xivChatType = (ushort)type;
            var territoryTypeId = this.GetTerritoryType();
            var contentId = this.GetContentId();

            // update content
            this.InContent = contentId != 0;

            // restrict by user settings
            if (this.Configuration.RestrictToContent && contentId == 0) return;
            if (this.Configuration.RestrictToHighEndDuty && !this.IsHighEndDuty()) return;
            if (this.Configuration.RestrictToCustomContent && !this.Configuration.PermittedContent.Contains(contentId)) return;

            // filter out bad messages
            if (!Enum.IsDefined(typeof(LootMessageType), xivChatType)) return;
            if (!message.Payloads.Any(payload => payload is ItemPayload)) return;
            var logKind = (LogKind)((uint)type & ~(~0 << 7));
            if (!Enum.IsDefined(typeof(LogKind), logKind)) return;

            // build initial loot message
            var lootMessage = new LootMessage
            {
                XivChatType = xivChatType,
                LogKind = logKind,
                LootMessageType = (LootMessageType)xivChatType,
                Message = message.TextValue,
            };

            // add name fields for logging/display
            lootMessage.LogKindName = Enum.GetName(typeof(LogKind), lootMessage.LogKind) ?? string.Empty;
            lootMessage.LootMessageTypeName = Enum.GetName(typeof(LootMessageType), lootMessage.LootMessageType) ?? string.Empty;

            // add item and message part payloads
            foreach (var payload in message.Payloads)
            {
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
            }

            // filter out non-permitted item ids
            if (this.Configuration.RestrictToCustomItems && !this.Configuration.PermittedItems.Contains(lootMessage.ItemId)) return;

            // log for debugging
            if (this.Configuration.DebugLoggingEnabled) Logger.LogInfo("[LootChatMessage]" + lootMessage);

            // send to loot processor
            var lootEvent = this.LootProcessor.ProcessLoot(lootMessage);

            // kick out if didn't process
            if (lootEvent == null) return;

            // log for debugging
            if (this.Configuration.DebugLoggingEnabled) Logger.LogInfo("[LootEvent]" + lootEvent);

            // enrich
            lootEvent.Timestamp = DateUtil.CurrentTime();
            lootEvent.LootEventId = Guid.NewGuid();
            lootEvent.TerritoryTypeId = territoryTypeId;
            lootEvent.ContentId = contentId;

            // add to list
            if (this.LootProcessor.IsEnabledEvent(lootEvent)) this.LootEvents.Add(lootEvent);

            // process for roll monitor
            this.RollMonitor.LootEvents.Enqueue(lootEvent);

            // output
            if (this.Configuration.LoggingEnabled) this.LootLogger.LogLoot(lootEvent);
        }

        private bool IsHighEndDuty()
        {
            return DataManager.InHighEndDuty(ClientState.TerritoryType);
        }

        private uint GetContentId()
        {
            return DataManager.ContentId(ClientState.TerritoryType);
        }

        private uint GetTerritoryType()
        {
            return ClientState.TerritoryType;
        }

        private void InitContent()
        {
            try
            {
                var excludedContent = new List<uint> { 69, 70, 71 };
                var contentTypes = new List<uint> { 2, 4, 5, 6, 26, 28, 29 };
                var contentList = DataManager.GetExcelSheet<ContentFinderCondition>() !
                                             .Where(content =>
                                                        contentTypes.Contains(content.ContentType.Row) && !excludedContent.Contains(content.RowId))
                                             .ToList();
                var contentNames = PluginInterface.Sanitizer.Sanitize(contentList.Select(content => content.Name.ToString())).ToArray();
                var contentIds = contentList.Select(content => content.RowId).ToArray();
                Array.Sort(contentNames, contentIds);
                this.ContentIds = contentIds;
                this.ContentNames = contentNames;
            }
            catch
            {
                Logger.LogVerbose("Failed to initialize content list.");
            }
        }

        private void InitItems()
        {
            try
            {
                // create item list
                var itemDataList = DataManager.GetExcelSheet<Item>() !.Where(item => !string.IsNullOrEmpty(item.Name)).ToList();

                // add all items
                var itemIds = itemDataList.Select(item => item.RowId).ToArray();
                var itemNames = PluginInterface.Sanitizer.Sanitize(itemDataList.Select(item => item.Name.ToString())).ToArray();
                this.ItemIds = itemIds;
                this.ItemNames = itemNames;

                // item categories
                var categoryList = DataManager.GetExcelSheet<ItemUICategory>() !
                                              .Where(category => category.RowId != 0).ToList();
                var categoryNames = PluginInterface.Sanitizer.Sanitize(categoryList.Select(category => category.Name.ToString())).ToArray();
                var categoryIds = categoryList.Select(category => category.RowId).ToArray();
                Array.Sort(categoryNames, categoryIds);
                this.ItemCategoryIds = categoryIds;
                this.ItemCategoryNames = categoryNames;

                // populate item lists by category
                var itemLists = new List<KeyValuePair<uint, ItemList>>();
                foreach (var categoryId in categoryIds)
                {
                    var itemCategoryDataList =
                        itemDataList.Where(item => item.ItemUICategory.Row == categoryId).ToList();
                    var itemCategoryIds = itemCategoryDataList.Select(item => item.RowId).ToArray();
                    var itemCategoryNames =
                        PluginInterface.Sanitizer.Sanitize(itemCategoryDataList.Select(item => item.Name.ToString())).ToArray();
                    Array.Sort(itemCategoryNames, itemCategoryIds);
                    var itemList = new ItemList
                    {
                        ItemIds = itemCategoryIds,
                        ItemNames = itemCategoryNames,
                    };
                    itemLists.Add(new KeyValuePair<uint, ItemList>(categoryId, itemList));
                }

                this.ItemLists = itemLists;
            }
            catch
            {
                Logger.LogVerbose("Failed to initialize content list.");
            }
        }
    }
}
