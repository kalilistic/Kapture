// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace Kapture
{
    public class SettingsWindow : WindowBase
    {
        private readonly IKapturePlugin _plugin;
        private Tab _currentTab = Tab.General;
        private int _selectedContentIndex;
        private int _selectedItemCategoryItemIndex;
        private int _selectedItemIndex;
        private float _uiScale;
        private uint[] ContentIds;
        private string[] ContentNames;
        private uint[] ItemIds;
        private string[] ItemNames;

        public SettingsWindow(IKapturePlugin plugin)
        {
            _plugin = plugin;
            UpdateItemList();
        }

        private void UpdateItemList()
        {
            _selectedItemIndex = 0;
            ItemIds = _plugin.ItemLists[_selectedItemCategoryItemIndex].Value.ItemIds;
            ItemNames = _plugin.ItemLists[_selectedItemCategoryItemIndex].Value.ItemNames;
        }

        public event EventHandler<bool> LootOverlayVisibilityUpdated;
        public event EventHandler<bool> RollMonitorOverlayVisibilityUpdated;

        public override void DrawView()
        {
            if (_plugin.IsInitializing) return;
            if (!_plugin.IsLoggedIn()) return;
            if (!IsVisible) return;
            var isVisible = IsVisible;
            _uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(440 * _uiScale, 320 * _uiScale), ImGuiCond.Appearing);
            if (ImGui.Begin(Loc.Localize("SettingsWindow", "Kapture Settings") + "###Kapture_Settings_Window",
                ref isVisible,
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar))
            {
                IsVisible = isVisible;
                DrawTabs();
                switch (_currentTab)
                {
                    case Tab.General:
                    {
                        DrawGeneral();
                        break;
                    }
                    case Tab.Loot:
                        DrawLoot();
                        break;
                    case Tab.Rolls:
                        DrawRolls();
                        break;
                    case Tab.Events:
                        DrawEventTypes();
                        break;
                    case Tab.Content:
                        DrawContent();
                        break;
                    case Tab.Items:
                        DrawItems();
                        break;
                    case Tab.Filters:
                        DrawFilters();
                        break;
                    case Tab.Log:
                        DrawLog();
                        break;
                    case Tab.Links:
                    {
                        DrawLinks();
                        break;
                    }
                    default:
                        DrawGeneral();
                        break;
                }
            }

            ImGui.End();
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("KaptureSettingsTabBar", ImGuiTabBarFlags.NoTooltip))
            {
                if (ImGui.BeginTabItem(Loc.Localize("General", "General") + "###Kapture_General_Tab"))
                {
                    _currentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Loot", "Loot") + "###Kapture_Loot_Tab"))
                {
                    _currentTab = Tab.Loot;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Rolls", "Rolls") + "###Kapture_Rolls_Tab"))
                {
                    _currentTab = Tab.Rolls;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Events", "Events") + "###Kapture_Events_Tab"))
                {
                    _currentTab = Tab.Events;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Content", "Content") + "###Kapture_Content_Tab"))
                {
                    _currentTab = Tab.Content;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Items", "Items") + "###Kapture_Items_Tab"))
                {
                    _currentTab = Tab.Items;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Filters", "Filters") + "###Kapture_Filters_Tab"))
                {
                    _currentTab = Tab.Filters;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Log", "Log") + "###Kapture_Log_Tab"))
                {
                    _currentTab = Tab.Log;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Links", "Links") + "###Kapture_Links_Tab"))
                {
                    _currentTab = Tab.Links;
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
                ImGui.Spacing();
            }
        }

        private void DrawGeneral()
        {
            // plugin enabled
            var enabled = _plugin.Configuration.Enabled;
            if (ImGui.Checkbox(
                Loc.Localize("PluginEnabled", "Plugin Enabled") + "###Kapture_PluginEnabled_Checkbox",
                ref enabled))
            {
                _plugin.Configuration.Enabled = enabled;
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("PluginEnabled_HelpMarker",
                "toggle the plugin on/off"));
            ImGui.Spacing();

            // combat
            var restrictInCombat = _plugin.Configuration.RestrictInCombat;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictInCombat", "Don't process in combat") + "###Kapture_RestrictInCombat_Checkbox",
                ref restrictInCombat))
            {
                _plugin.Configuration.RestrictInCombat = restrictInCombat;
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("PluginEnabled_HelpMarker",
                "stop processing data while in combat"));
            ImGui.Spacing();

            // language
            ImGui.Text(Loc.Localize("Language", "Language"));
            CustomWidgets.HelpMarker(Loc.Localize("Language_HelpMarker",
                "use default or override plugin ui language"));
            ImGui.Spacing();
            var pluginLanguage = _plugin.Configuration.PluginLanguage;
            if (ImGui.Combo("###Kapture_Language_Combo", ref pluginLanguage,
                PluginLanguage.LanguageNames.ToArray(),
                PluginLanguage.LanguageNames.Count))
            {
                _plugin.Configuration.PluginLanguage = pluginLanguage;
                _plugin.SaveConfig();
                _plugin.Localization.SetLanguage(pluginLanguage);
            }
        }

        private void DrawLoot()
        {
            // show loot overlay
            var showLootOverlay = _plugin.Configuration.ShowLootOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowLootOverlay", "Show Loot Overlay") + "###Kapture_ShowLootOverlay_Checkbox",
                ref showLootOverlay))
            {
                _plugin.Configuration.ShowLootOverlay = showLootOverlay;
                LootOverlayVisibilityUpdated?.Invoke(this, showLootOverlay);
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("ShowLootOverlay_HelpMarker",
                "show loot overlay window"));
            ImGui.Spacing();

            // display mode
            ImGui.Text(Loc.Localize("LootDisplayMode", "Display Mode"));
            CustomWidgets.HelpMarker(Loc.Localize("LootDisplayMode_HelpMarker",
                "when to show loot overlay"));
            ImGui.Spacing();
            var pluginLootDisplayMode = _plugin.Configuration.LootDisplayMode;
            if (ImGui.Combo("###Kapture_LootDisplayMode_Combo", ref pluginLootDisplayMode,
                DisplayMode.DisplayModeNames.ToArray(),
                DisplayMode.DisplayModeNames.Count))
            {
                _plugin.Configuration.LootDisplayMode = pluginLootDisplayMode;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            // loot name format
            ImGui.Text(Loc.Localize("LootNameFormat", "Name Format"));
            CustomWidgets.HelpMarker(Loc.Localize("LootNameFormat_HelpMarker",
                "how to display player names in the loot overlay"));
            ImGui.Spacing();
            var pluginLootNameFormat = _plugin.Configuration.LootNameFormat;
            if (ImGui.Combo("###Kapture_LootNameFormat_Combo", ref pluginLootNameFormat,
                NameFormat.NameFormatNames.ToArray(),
                NameFormat.NameFormatNames.Count))
            {
                _plugin.Configuration.LootNameFormat = pluginLootNameFormat;
                _plugin.SaveConfig();
            }
        }

        private void DrawRolls()
        {
            // show roll overlay
            var showRollOverlay = _plugin.Configuration.ShowRollMonitorOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowRollOverlay", "Show Roll Monitor Overlay") + "###Kapture_ShowRollOverlay_Checkbox",
                ref showRollOverlay))
            {
                _plugin.Configuration.ShowRollMonitorOverlay = showRollOverlay;
                RollMonitorOverlayVisibilityUpdated?.Invoke(this, showRollOverlay);
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("ShowRollOverlay_HelpMarker",
                "show roll monitor overlay window"));
            ImGui.Spacing();

            // display mode
            ImGui.Text(Loc.Localize("RollDisplayMode", "Display Mode"));
            CustomWidgets.HelpMarker(Loc.Localize("RollDisplayMode_HelpMarker",
                "when to show roll monitor overlay"));
            ImGui.Spacing();
            var pluginRollDisplayMode = _plugin.Configuration.RollDisplayMode;
            if (ImGui.Combo("###Kapture_RollDisplayMode_Combo", ref pluginRollDisplayMode,
                DisplayMode.DisplayModeNames.ToArray(),
                DisplayMode.DisplayModeNames.Count))
            {
                _plugin.Configuration.RollDisplayMode = pluginRollDisplayMode;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            // roll name format
            ImGui.Text(Loc.Localize("RollNameFormat", "Name Format"));
            CustomWidgets.HelpMarker(Loc.Localize("RollNameFormat_HelpMarker",
                "how to display player names in the roll monitor overlay"));
            ImGui.Spacing();
            var pluginRollNameFormat = _plugin.Configuration.RollNameFormat;
            if (ImGui.Combo("###Kapture_RollNameFormat_Combo", ref pluginRollNameFormat,
                NameFormat.NameFormatNames.ToArray(),
                NameFormat.NameFormatNames.Count))
            {
                _plugin.Configuration.RollNameFormat = pluginRollNameFormat;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            // roll timeout
            ImGui.Text(Loc.Localize("RollMonitorAddedTimeout", "Show Added Items (minutes)"));
            CustomWidgets.HelpMarker(Loc.Localize("RollMonitorAddedTimeout_HelpMarker",
                "amount of time before removing added items from roll monitor"));
            var RollMonitorAddedTimeout =
                _plugin.Configuration.RollMonitorAddedTimeout.FromMillisecondsToMinutes();
            if (ImGui.SliderInt("###PlayerTrack_RollMonitorAddedTimeout_Slider", ref RollMonitorAddedTimeout, 5, 60))
            {
                _plugin.Configuration.RollMonitorAddedTimeout =
                    RollMonitorAddedTimeout.FromMinutesToMilliseconds();
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            // roll timeout
            ImGui.Text(Loc.Localize("RollMonitorObtainedTimeout", "Show Obtained Items (seconds)"));
            CustomWidgets.HelpMarker(Loc.Localize("RollMonitorObtainedTimeout_HelpMarker",
                "amount of time before removing obtained/lost items from roll monitor"));
            var RollMonitorObtainedTimeout =
                _plugin.Configuration.RollMonitorObtainedTimeout.FromMillisecondsToSeconds();
            if (ImGui.SliderInt("###PlayerTrack_RollMonitorObtainedTimeout_Slider", ref RollMonitorObtainedTimeout, 5, 300))
            {
                _plugin.Configuration.RollMonitorObtainedTimeout =
                    RollMonitorObtainedTimeout.FromSecondsToMilliseconds();
                _plugin.SaveConfig();
            }

            ImGui.Spacing();
        }

        private void DrawEventTypes()
        {
            var offset1 = 110f * Scale;
            var offset2 = 220f * Scale;

            var addEnabled = _plugin.Configuration.AddEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("AddEnabled", "Add") + "###Kapture_AddEnabled_Checkbox",
                ref addEnabled))
            {
                _plugin.Configuration.AddEnabled = addEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var castEnabled = _plugin.Configuration.CastEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("CastEnabled", "Cast") + "###Kapture_CastEnabled_Checkbox",
                ref castEnabled))
            {
                _plugin.Configuration.CastEnabled = castEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var craftEnabled = _plugin.Configuration.CraftEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("CraftEnabled", "Craft") + "###Kapture_CraftEnabled_Checkbox",
                ref craftEnabled))
            {
                _plugin.Configuration.CraftEnabled = craftEnabled;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            var desynthEnabled = _plugin.Configuration.DesynthEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("DesynthEnabled", "Desynth") + "###Kapture_DesynthEnabled_Checkbox",
                ref desynthEnabled))
            {
                _plugin.Configuration.DesynthEnabled = desynthEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var discardEnabled = _plugin.Configuration.DiscardEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("DiscardEnabled", "Discard") + "###Kapture_DiscardEnabled_Checkbox",
                ref discardEnabled))
            {
                _plugin.Configuration.DiscardEnabled = discardEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var gatherEnabled = _plugin.Configuration.GatherEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("GatherEnabled", "Gather") + "###Kapture_GatherEnabled_Checkbox",
                ref gatherEnabled))
            {
                _plugin.Configuration.GatherEnabled = gatherEnabled;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            var greedEnabled = _plugin.Configuration.GreedEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("GreedEnabled", "Greed") + "###Kapture_GreedEnabled_Checkbox",
                ref greedEnabled))
            {
                _plugin.Configuration.GreedEnabled = greedEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var lostEnabled = _plugin.Configuration.LostEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("LostEnabled", "Lost") + "###Kapture_LostEnabled_Checkbox",
                ref lostEnabled))
            {
                _plugin.Configuration.LostEnabled = lostEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var needEnabled = _plugin.Configuration.NeedEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("NeedEnabled", "Need") + "###Kapture_NeedEnabled_Checkbox",
                ref needEnabled))
            {
                _plugin.Configuration.NeedEnabled = needEnabled;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            var obtainEnabled = _plugin.Configuration.ObtainEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("ObtainEnabled", "Obtain") + "###Kapture_ObtainEnabled_Checkbox",
                ref obtainEnabled))
            {
                _plugin.Configuration.ObtainEnabled = obtainEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var purchaseEnabled = _plugin.Configuration.PurchaseEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("PurchaseEnabled", "Purchase") + "###Kapture_PurchaseEnabled_Checkbox",
                ref purchaseEnabled))
            {
                _plugin.Configuration.PurchaseEnabled = purchaseEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var searchEnabled = _plugin.Configuration.SearchEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("SearchEnabled", "Search") + "###Kapture_SearchEnabled_Checkbox",
                ref searchEnabled))
            {
                _plugin.Configuration.SearchEnabled = searchEnabled;
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            var sellEnabled = _plugin.Configuration.SellEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("SellEnabled", "Sell") + "###Kapture_SellEnabled_Checkbox",
                ref sellEnabled))
            {
                _plugin.Configuration.SellEnabled = sellEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var useEnabled = _plugin.Configuration.UseEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("UseEnabled", "Use") + "###Kapture_UseEnabled_Checkbox",
                ref useEnabled))
            {
                _plugin.Configuration.UseEnabled = useEnabled;
                _plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);
        }

        private void DrawContent()
        {
            ContentIds = _plugin.ContentIds;
            ContentNames = _plugin.ContentNames;
            RestrictToContent();
            RestrictToHighEndDuty();
            RestrictToCustom();
        }

        private void RestrictToContent()
        {
            var restrictToContent = _plugin.Configuration.RestrictToContent;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToContent", "Content Only") + "###Kapture_RestrictToContent_Checkbox",
                ref restrictToContent))
            {
                _plugin.Configuration.RestrictToContent = restrictToContent;
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("RestrictToContent_HelpMarker",
                "restrict to instanced content and exclude overworld encounters"));
            ImGui.Spacing();
        }

        private void RestrictToHighEndDuty()
        {
            var restrictToHighEndDuty = _plugin.Configuration.RestrictToHighEndDuty;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToHighEndDuty", "High-End Duty Only") +
                "###Kapture_RestrictToHighEndDuty_Checkbox",
                ref restrictToHighEndDuty))
            {
                _plugin.Configuration.RestrictToHighEndDuty = restrictToHighEndDuty;
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("RestrictToHighEndDuty_HelpMarker",
                "restrict to high-end duties only (e.g. savage)"));
            ImGui.Spacing();
        }

        private void RestrictToCustom()
        {
            var restrictToCustomList = _plugin.Configuration.RestrictToCustomContent;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToCustom", "Restrict to Following Content") +
                "###Kapture_RestrictToCustom_Checkbox",
                ref restrictToCustomList))
            {
                _plugin.Configuration.RestrictToCustomContent = restrictToCustomList;
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("RestrictToCustom_HelpMarker",
                "add content to list by using dropdown or remove by clicking on them"));
            ImGui.Spacing();

            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * Scale);
            ImGui.Combo("###Kapture_Content_Combo", ref _selectedContentIndex,
                ContentNames, ContentIds.Length);
            ImGui.SameLine();

            if (ImGui.SmallButton(Loc.Localize("Add", "Add") + "###Kapture_ContentAdd_Button"))
            {
                if (_plugin.Configuration.PermittedContent.Contains(
                    ContentIds[_selectedContentIndex]))
                {
                    ImGui.OpenPopup("###Kapture_DupeContent_Popup");
                }
                else
                {
                    _plugin.Configuration.PermittedContent.Add(
                        ContentIds[_selectedContentIndex]);
                    _plugin.SaveConfig();
                }
            }

            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Reset", "Reset") + "###Kapture_ContentReset_Button"))
            {
                _selectedContentIndex = 0;
                _plugin.Configuration.PermittedContent = new List<uint>();
                _plugin.SaveConfig();
            }

            if (ImGui.BeginPopup("###Kapture_DupeContent_Popup"))
            {
                ImGui.Text(Loc.Localize("DupeContent", "This content is already added!"));
                ImGui.EndPopup();
            }

            ImGui.Spacing();

            foreach (var permittedContent in _plugin.Configuration.PermittedContent.ToList())
            {
                var index = Array.IndexOf(ContentIds, permittedContent);
                ImGui.Text(ContentNames[index]);
                if (ImGui.IsItemClicked())
                {
                    _plugin.Configuration.PermittedContent.Remove(permittedContent);
                    _plugin.SaveConfig();
                }
            }

            ImGui.Spacing();
        }

        private void DrawItems()
        {
            var offset = 110f * Scale;

            var restrictToCustomItemsList = _plugin.Configuration.RestrictToCustomItems;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToCustomItems", "Restrict to Following Items") +
                "###Kapture_RestrictToCustomItems_Checkbox",
                ref restrictToCustomItemsList))
            {
                _plugin.Configuration.RestrictToCustomItems = restrictToCustomItemsList;
                _plugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("RestrictToCustomItems_HelpMarker",
                "add item to list by using dropdown or remove by clicking on them"));

            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Reset", "Reset") + "###Kapture_CustomItemReset_Button"))
            {
                _selectedItemIndex = 0;
                _selectedItemCategoryItemIndex = 0;
                UpdateItemList();
                _plugin.Configuration.PermittedItems = new List<uint>();
                _plugin.SaveConfig();
            }

            ImGui.Spacing();

            // select category by item ui category
            ImGui.Text(Loc.Localize("SelectCategory", "Select Category"));
            ImGui.SameLine(offset);
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * Scale);
            if (ImGui.Combo("###Kapture_ItemCategoryItems_Combo", ref _selectedItemCategoryItemIndex,
                _plugin.ItemCategoryNames, _plugin.ItemCategoryIds.Length))
                UpdateItemList();

            // select item based on category
            ImGui.Text(Loc.Localize("SelectItem", "Select Item"));
            ImGui.SameLine(offset);
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * Scale);
            ImGui.Combo("###Kapture_ItemItems_Combo", ref _selectedItemIndex,
                ItemNames, ItemIds.Length);
            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Add", "Add") + "###Kapture_AddItemItem_Button"))
            {
                if (_plugin.Configuration.PermittedItems.Contains(
                    ItemIds[_selectedItemIndex]))
                {
                    ImGui.OpenPopup("###Kapture_DupeCustomItem_Popup");
                }
                else
                {
                    _plugin.Configuration.PermittedItems.Add(
                        ItemIds[_selectedItemIndex]);
                    _plugin.SaveConfig();
                }
            }

            // dupe popup
            if (ImGui.BeginPopup("###Kapture_DupeCustomItem_Popup"))
            {
                ImGui.Text(Loc.Localize("DupeCustomItem", "This item is already added!"));
                ImGui.EndPopup();
            }

            ImGui.Spacing();

            // item list
            ImGui.Text(Loc.Localize("PermittedItems", "Permitted Items"));
            ImGui.SameLine(offset - 5f * Scale);
            ImGui.Indent(offset - 5f * Scale);
            if (_plugin.Configuration.PermittedItems != null &&
                _plugin.Configuration.PermittedItems.Count > 0)
                foreach (var permittedItem in _plugin.Configuration.PermittedItems.ToList())
                {
                    var index = Array.IndexOf(_plugin.ItemIds, permittedItem);
                    ImGui.Text(_plugin.ItemNames[index]);
                    if (ImGui.IsItemClicked())
                    {
                        _plugin.Configuration.PermittedItems.Remove(permittedItem);
                        _plugin.SaveConfig();
                    }
                }
            else
                ImGui.Text(Loc.Localize("NoPermittedItems", "None"));


            ImGui.Spacing();
        }

        private void DrawFilters()
        {
            var selfOnly = _plugin.Configuration.SelfOnly;
            if (ImGui.Checkbox(
                Loc.Localize("SelfOnly", "Self Only") +
                "###Kapture_SelfOnly_Checkbox",
                ref selfOnly))
            {
                _plugin.Configuration.SelfOnly = selfOnly;
                _plugin.SaveConfig();
            }
        }

        private void DrawLog()
        {
            // logging enabled
            var loggingEnabled = _plugin.Configuration.LoggingEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("LoggingEnabled", "Enable Loot Logging") +
                "###Kapture_LoggingEnabled_Checkbox",
                ref loggingEnabled))
            {
                _plugin.Configuration.LoggingEnabled = loggingEnabled;
                _plugin.SaveConfig();
                if (loggingEnabled) _plugin.LootLogger.SetLogFormat();
            }

            CustomWidgets.HelpMarker(Loc.Localize("LoggingEnabled_HelpMarker",
                "save your loot messages to a file in config (see links)"));
            ImGui.Spacing();

            // log format
            ImGui.Text(Loc.Localize("LogFormat", "Log Format"));
            CustomWidgets.HelpMarker(Loc.Localize("LogFormat_HelpMarker",
                "set format for log file with loot info"));
            ImGui.Spacing();
            var pluginLogFormat = _plugin.Configuration.LogFormat;
            if (ImGui.Combo("###Kapture_LogFormat_Combo", ref pluginLogFormat,
                LogFormat.LogFormatNames.ToArray(),
                LogFormat.LogFormatNames.Count))
            {
                _plugin.Configuration.LogFormat = pluginLogFormat;
                _plugin.SaveConfig();
                _plugin.LootLogger.SetLogFormat();
            }
        }

        private void DrawLinks()
        {
            var buttonSize = new Vector2(120f * _uiScale, 25f * _uiScale);
            if (ImGui.Button(Loc.Localize("OpenGithub", "Github") + "###Kapture_OpenGithub_Button", buttonSize))
                Process.Start("https://github.com/kalilistic/Dalamud.Kapture");
            if (ImGui.Button(
                Loc.Localize("ImproveTranslate", "Translations") + "###Kapture_ImproveTranslate_Button",
                buttonSize))
                Process.Start("https://crowdin.com/project/kapture");
            if (ImGui.Button(
                Loc.Localize("LoadTestData", "Test Data") + "###Kapture_LoadTestData_Button",
                buttonSize))
                _plugin.LoadTestData();
            if (ImGui.Button(
                Loc.Localize("ClearData", "Clear Data") + "###Kapture_ClearData_Button",
                buttonSize))
                _plugin.ClearData();
            if (ImGui.Button(
                Loc.Localize("OpenLogs", "Open Logs") + "###Kapture_OpenLogs_Button",
                buttonSize))
                Process.Start(_plugin.DataManager.DataPath);
        }

        private enum Tab
        {
            General,
            Loot,
            Rolls,
            Events,
            Content,
            Items,
            Filters,
            Log,
            Links
        }
    }
}