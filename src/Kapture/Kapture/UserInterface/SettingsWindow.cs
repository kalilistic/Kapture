using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

using static Kapture.DisplayMode;
using static Kapture.NameFormat;

namespace Kapture
{
    /// <inheritdoc />
    public class SettingsWindow : PluginWindow
    {
        private readonly KapturePlugin plugin;
        private Tab currentTab = Tab.General;
        private int selectedContentIndex;
        private int selectedItemCategoryItemIndex;
        private int selectedItemIndex;
        private uint[] contentIds = null!;
        private string[] contentNames = null!;
        private uint[] itemIds = null!;
        private string[] itemNames = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public SettingsWindow(KapturePlugin plugin)
            : base(plugin, "Kapture Config")
        {
            this.plugin = plugin;
            this.UpdateItemList();
        }

        private enum Tab
        {
            General,
            Loot,
            Rolls,
            Events,
            Content,
            Watchlist,
            Filters,
            Log,
            Links,
        }

        /// <inheritdoc />
        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(500 * ImGuiHelpers.GlobalScale, 320 * ImGuiHelpers.GlobalScale), ImGuiCond.Appearing);
            this.DrawTabs();
            switch (this.currentTab)
            {
                case Tab.General:
                    this.DrawGeneral();
                    break;
                case Tab.Loot:
                    this.DrawLoot();
                    break;
                case Tab.Rolls:
                    this.DrawRolls();
                    break;
                case Tab.Events:
                    this.DrawEventTypes();
                    break;
                case Tab.Content:
                    this.DrawContent();
                    break;
                case Tab.Watchlist:
                    this.DrawWatchlist();
                    break;
                case Tab.Filters:
                    this.DrawFilters();
                    break;
                case Tab.Log:
                    this.DrawLog();
                    break;
                case Tab.Links:
                {
                    this.DrawLinks();
                    break;
                }

                default:
                    this.DrawGeneral();
                    break;
            }
        }

        private void UpdateItemList()
        {
            this.selectedItemIndex = 0;
            this.itemIds = this.plugin.ItemLists[this.selectedItemCategoryItemIndex].Value.ItemIds;
            this.itemNames = this.plugin.ItemLists[this.selectedItemCategoryItemIndex].Value.ItemNames;
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("KaptureSettingsTabBar", ImGuiTabBarFlags.NoTooltip))
            {
                if (ImGui.BeginTabItem(Loc.Localize("General", "General") + "###Kapture_General_Tab"))
                {
                    this.currentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Loot", "Loot") + "###Kapture_Loot_Tab"))
                {
                    this.currentTab = Tab.Loot;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Rolls", "Rolls") + "###Kapture_Rolls_Tab"))
                {
                    this.currentTab = Tab.Rolls;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Events", "Events") + "###Kapture_Events_Tab"))
                {
                    this.currentTab = Tab.Events;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Content", "Content") + "###Kapture_Content_Tab"))
                {
                    this.currentTab = Tab.Content;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Watchlist", "Watchlist") + "###Kapture_Watchlist_Tab"))
                {
                    this.currentTab = Tab.Watchlist;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Filters", "Filters") + "###Kapture_Filters_Tab"))
                {
                    this.currentTab = Tab.Filters;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Log", "Log") + "###Kapture_Log_Tab"))
                {
                    this.currentTab = Tab.Log;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Links", "Links") + "###Kapture_Links_Tab"))
                {
                    this.currentTab = Tab.Links;
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
                ImGui.Spacing();
            }
        }

        private void DrawGeneral()
        {
            // plugin enabled
            var enabled = this.plugin.Configuration.Enabled;
            if (ImGui.Checkbox(
                Loc.Localize("PluginEnabled", "Plugin Enabled") + "###Kapture_PluginEnabled_Checkbox",
                ref enabled))
            {
                this.plugin.Configuration.Enabled = enabled;
                this.plugin
                    .SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "PluginEnabled_HelpMarker",
                "toggle the plugin on/off"));
            ImGui.Spacing();

            // combat
            var restrictInCombat = this.plugin.Configuration.RestrictInCombat;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictInCombat", "Don't process in combat") + "###Kapture_RestrictInCombat_Checkbox",
                ref restrictInCombat))
            {
                this.plugin.Configuration.RestrictInCombat = restrictInCombat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictInComba_HelpMarker",
                "stop processing data while in combat"));
            ImGui.Spacing();
        }

        private void DrawLoot()
        {
            // show loot overlay
            var showLootOverlay = this.plugin.Configuration.ShowLootOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowLootOverlay", "Show Loot Overlay") + "###Kapture_ShowLootOverlay_Checkbox",
                ref showLootOverlay))
            {
                this.plugin.Configuration.ShowLootOverlay = showLootOverlay;
                this.Plugin.WindowManager.LootWindow!.IsOpen = showLootOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowLootOverlay_HelpMarker",
                "show loot overlay window"));
            ImGui.Spacing();

            // display mode
            ImGui.Text(Loc.Localize("LootDisplayMode", "Display Mode"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "LootDisplayMode_HelpMarker",
                "when to show loot overlay"));
            ImGui.Spacing();
            var pluginLootDisplayMode = this.plugin.Configuration.LootDisplayMode;
            if (ImGui.Combo(
                "###Kapture_LootDisplayMode_Combo",
                ref pluginLootDisplayMode,
                DisplayModeNames.ToArray(),
                DisplayModeNames.Count))
            {
                this.plugin.Configuration.LootDisplayMode = pluginLootDisplayMode;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            // loot name format
            ImGui.Text(Loc.Localize("LootNameFormat", "Name Format"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "LootNameFormat_HelpMarker",
                "how to display player names in the loot overlay"));
            ImGui.Spacing();
            var pluginLootNameFormat = this.plugin.Configuration.LootNameFormat;
            if (ImGui.Combo(
                "###Kapture_LootNameFormat_Combo",
                ref pluginLootNameFormat,
                NameFormatNames.ToArray(),
                NameFormatNames.Count))
            {
                this.plugin.Configuration.LootNameFormat = pluginLootNameFormat;
                this.plugin.SaveConfig();
            }
        }

        private void DrawRolls()
        {
            // show roll overlay
            var showRollOverlay = this.plugin.Configuration.ShowRollMonitorOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowRollOverlay", "Show Roll Monitor Overlay") + "###Kapture_ShowRollOverlay_Checkbox",
                ref showRollOverlay))
            {
                this.plugin.Configuration.ShowRollMonitorOverlay = showRollOverlay;
                this.Plugin.WindowManager.RollWindow!.IsOpen = showRollOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowRollOverlay_HelpMarker",
                                           "show roll monitor overlay window"));
            ImGui.Spacing();

            // display mode
            ImGui.Text(Loc.Localize("RollDisplayMode", "Display Mode"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "RollDisplayMode_HelpMarker",
                "when to show roll monitor overlay"));
            ImGui.Spacing();
            var pluginRollDisplayMode = this.plugin.Configuration.RollDisplayMode;
            if (ImGui.Combo(
                "###Kapture_RollDisplayMode_Combo",
                ref pluginRollDisplayMode,
                DisplayModeNames.ToArray(),
                DisplayModeNames.Count))
            {
                this.plugin.Configuration.RollDisplayMode = pluginRollDisplayMode;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            // roll name format
            ImGui.Text(Loc.Localize("RollNameFormat", "Name Format"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "RollNameFormat_HelpMarker",
                "how to display player names in the roll monitor overlay"));
            ImGui.Spacing();
            var pluginRollNameFormat = this.plugin.Configuration.RollNameFormat;
            if (ImGui.Combo(
                "###Kapture_RollNameFormat_Combo",
                ref pluginRollNameFormat,
                NameFormatNames.ToArray(),
                NameFormatNames.Count))
            {
                this.plugin.Configuration.RollNameFormat = pluginRollNameFormat;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            // roll timeout
            ImGui.Text(Loc.Localize("RollMonitorAddedTimeout", "Show Added Items (minutes)"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "RollMonitorAddedTimeout_HelpMarker",
                                           "amount of time before removing added items from roll monitor"));
            var rollMonitorAddedTimeout =
                this.plugin.Configuration.RollMonitorAddedTimeout.FromMillisecondsToMinutes();
            if (ImGui.SliderInt("###PlayerTrack_RollMonitorAddedTimeout_Slider", ref rollMonitorAddedTimeout, 5, 60))
            {
                this.plugin.Configuration.RollMonitorAddedTimeout =
                    rollMonitorAddedTimeout.FromMinutesToMilliseconds();
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            // roll timeout
            ImGui.Text(Loc.Localize("RollMonitorObtainedTimeout", "Show Obtained Items (seconds)"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "RollMonitorObtainedTimeout_HelpMarker",
                "amount of time before removing obtained/lost items from roll monitor"));
            var rollMonitorObtainedTimeout =
                this.plugin.Configuration.RollMonitorObtainedTimeout.FromMillisecondsToSeconds();
            if (ImGui.SliderInt("###PlayerTrack_RollMonitorObtainedTimeout_Slider", ref rollMonitorObtainedTimeout, 5, 300))
            {
                this.plugin.Configuration.RollMonitorObtainedTimeout =
                    rollMonitorObtainedTimeout.FromSecondsToMilliseconds();
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();
        }

        private void DrawEventTypes()
        {
            var offset1 = 110f * ImGuiHelpers.GlobalScale;
            var offset2 = 220f * ImGuiHelpers.GlobalScale;

            var addEnabled = this.plugin.Configuration.AddEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("AddEnabled", "Add") + "###Kapture_AddEnabled_Checkbox",
                ref addEnabled))
            {
                this.plugin.Configuration.AddEnabled = addEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var castEnabled = this.plugin.Configuration.CastEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("CastEnabled", "Cast") + "###Kapture_CastEnabled_Checkbox",
                ref castEnabled))
            {
                this.plugin.Configuration.CastEnabled = castEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var craftEnabled = this.plugin.Configuration.CraftEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("CraftEnabled", "Craft") + "###Kapture_CraftEnabled_Checkbox",
                ref craftEnabled))
            {
                this.plugin.Configuration.CraftEnabled = craftEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            var desynthEnabled = this.plugin.Configuration.DesynthEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("DesynthEnabled", "Desynth") + "###Kapture_DesynthEnabled_Checkbox",
                ref desynthEnabled))
            {
                this.plugin.Configuration.DesynthEnabled = desynthEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var discardEnabled = this.plugin.Configuration.DiscardEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("DiscardEnabled", "Discard") + "###Kapture_DiscardEnabled_Checkbox",
                ref discardEnabled))
            {
                this.plugin.Configuration.DiscardEnabled = discardEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var gatherEnabled = this.plugin.Configuration.GatherEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("GatherEnabled", "Gather") + "###Kapture_GatherEnabled_Checkbox",
                ref gatherEnabled))
            {
                this.plugin.Configuration.GatherEnabled = gatherEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            var greedEnabled = this.plugin.Configuration.GreedEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("GreedEnabled", "Greed") + "###Kapture_GreedEnabled_Checkbox",
                ref greedEnabled))
            {
                this.plugin.Configuration.GreedEnabled = greedEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var lostEnabled = this.plugin.Configuration.LostEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("LostEnabled", "Lost") + "###Kapture_LostEnabled_Checkbox",
                ref lostEnabled))
            {
                this.plugin.Configuration.LostEnabled = lostEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var needEnabled = this.plugin.Configuration.NeedEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("NeedEnabled", "Need") + "###Kapture_NeedEnabled_Checkbox",
                ref needEnabled))
            {
                this.plugin.Configuration.NeedEnabled = needEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            var obtainEnabled = this.plugin.Configuration.ObtainEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("ObtainEnabled", "Obtain") + "###Kapture_ObtainEnabled_Checkbox",
                ref obtainEnabled))
            {
                this.plugin.Configuration.ObtainEnabled = obtainEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var purchaseEnabled = this.plugin.Configuration.PurchaseEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("PurchaseEnabled", "Purchase") + "###Kapture_PurchaseEnabled_Checkbox",
                ref purchaseEnabled))
            {
                this.plugin.Configuration.PurchaseEnabled = purchaseEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);

            var searchEnabled = this.plugin.Configuration.SearchEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("SearchEnabled", "Search") + "###Kapture_SearchEnabled_Checkbox",
                ref searchEnabled))
            {
                this.plugin.Configuration.SearchEnabled = searchEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            var sellEnabled = this.plugin.Configuration.SellEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("SellEnabled", "Sell") + "###Kapture_SellEnabled_Checkbox",
                ref sellEnabled))
            {
                this.plugin.Configuration.SellEnabled = sellEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset1);

            var useEnabled = this.plugin.Configuration.UseEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("UseEnabled", "Use") + "###Kapture_UseEnabled_Checkbox",
                ref useEnabled))
            {
                this.plugin.Configuration.UseEnabled = useEnabled;
                this.plugin.SaveConfig();
            }

            ImGui.SameLine(offset2);
        }

        private void DrawContent()
        {
            this.contentIds = this.plugin.ContentIds;
            this.contentNames = this.plugin.ContentNames;
            this.RestrictToContent();
            this.RestrictToHighEndDuty();
            this.RestrictToCustom();
        }

        private void RestrictToContent()
        {
            var restrictToContent = this.plugin.Configuration.RestrictToContent;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToContent", "Content Only") + "###Kapture_RestrictToContent_Checkbox",
                ref restrictToContent))
            {
                this.plugin.Configuration.RestrictToContent = restrictToContent;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictToContent_HelpMarker",
                "restrict to instanced content and exclude overworld encounters"));
            ImGui.Spacing();
        }

        private void RestrictToHighEndDuty()
        {
            var restrictToHighEndDuty = this.plugin.Configuration.RestrictToHighEndDuty;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToHighEndDuty", "High-End Duty Only") +
                "###Kapture_RestrictToHighEndDuty_Checkbox",
                ref restrictToHighEndDuty))
            {
                this.plugin.Configuration.RestrictToHighEndDuty = restrictToHighEndDuty;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictToHighEndDuty_HelpMarker",
                "restrict to high-end duties only (e.g. savage)"));
            ImGui.Spacing();
        }

        private void RestrictToCustom()
        {
            var restrictToCustomList = this.plugin.Configuration.RestrictToCustomContent;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToCustom", "Restrict to Following Content") +
                "###Kapture_RestrictToCustom_Checkbox",
                ref restrictToCustomList))
            {
                this.plugin.Configuration.RestrictToCustomContent = restrictToCustomList;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictToCustom_HelpMarker",
                "add content to list by using dropdown or remove by clicking on them"));
            ImGui.Spacing();

            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###Kapture_Content_Combo",
                ref this.selectedContentIndex,
                this.contentNames,
                this.contentIds.Length);
            ImGui.SameLine();

            if (ImGui.SmallButton(Loc.Localize("Add", "Add") + "###Kapture_ContentAdd_Button"))
            {
                if (this.plugin.Configuration.PermittedContent.Contains(
                    this.contentIds[this.selectedContentIndex]))
                {
                    ImGui.OpenPopup("###Kapture_DupeContent_Popup");
                }
                else
                {
                    this.plugin.Configuration.PermittedContent.Add(
                        this.contentIds[this.selectedContentIndex]);
                    this.plugin.SaveConfig();
                }
            }

            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Reset", "Reset") + "###Kapture_ContentReset_Button"))
            {
                this.selectedContentIndex = 0;
                this.plugin.Configuration.PermittedContent = new List<uint>();
                this.plugin.SaveConfig();
            }

            if (ImGui.BeginPopup("###Kapture_DupeContent_Popup"))
            {
                ImGui.Text(Loc.Localize("DupeContent", "This content is already added!"));
                ImGui.EndPopup();
            }

            ImGui.Spacing();

            foreach (var permittedContent in this.plugin.Configuration.PermittedContent.ToList())
            {
                var index = Array.IndexOf(this.contentIds, permittedContent);
                ImGui.Text(this.contentNames[index]);
                if (ImGui.IsItemClicked())
                {
                    this.plugin.Configuration.PermittedContent.Remove(permittedContent);
                    this.plugin.SaveConfig();
                }
            }

            ImGui.Spacing();
        }

        private void DrawWatchlist()
        {
            var offset = 110f * ImGuiHelpers.GlobalScale;

            // select category by item ui category
            ImGui.Text(Loc.Localize("SelectCategory", "Select Category"));
            ImGui.SameLine(offset);
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * ImGuiHelpers.GlobalScale);
            if (ImGui.Combo(
                "###Kapture_WatchListCategoryItems_Combo",
                ref this.selectedItemCategoryItemIndex,
                this.plugin.ItemCategoryNames,
                this.plugin.ItemCategoryIds.Length))
                this.UpdateItemList();

            // select item based on category
            ImGui.Text(Loc.Localize("SelectItem", "Select Item"));
            ImGui.SameLine(offset);
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###Kapture_WatchListItems_Combo",
                ref this.selectedItemIndex,
                this.itemNames,
                this.itemIds.Length);
            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Add", "Add") + "###Kapture_AddWatchListItem_Button"))
            {
                if (this.plugin.Configuration.WatchListItems.Contains(
                    this.itemIds[this.selectedItemIndex]))
                {
                    ImGui.OpenPopup("###Kapture_DupeCustomWatchListItem_Popup");
                }
                else
                {
                    this.plugin.Configuration.WatchListItems.Add(
                        this.itemIds[this.selectedItemIndex]);
                    this.plugin.SaveConfig();
                }
            }

            // dupe popup
            if (ImGui.BeginPopup("###Kapture_DupeCustomWatchListItem_Popup"))
            {
                ImGui.Text(Loc.Localize("DupeCustomWatchListItem", "This item is already added!"));
                ImGui.EndPopup();
            }

            ImGui.Spacing();

            // item list
            ImGui.Text(Loc.Localize("WatchListItems", "Watchlist Items"));
            ImGui.SameLine(offset - (5f * ImGuiHelpers.GlobalScale));
            ImGui.Indent(offset - (5f * ImGuiHelpers.GlobalScale));
            if (this.plugin.Configuration.WatchListItems.Count > 0)
            {
                foreach (var watchListItem in this.plugin.Configuration.WatchListItems.ToList())
                {
                    var index = Array.IndexOf(this.plugin.ItemIds, watchListItem);
                    ImGui.Text(this.plugin.ItemNames[index]);
                    if (ImGui.IsItemClicked())
                    {
                        this.plugin.Configuration.WatchListItems.Remove(watchListItem);
                        this.plugin.SaveConfig();
                    }
                }
            }
            else
            {
                ImGui.Text(Loc.Localize("NoWatchListItems", "None"));
            }

            ImGui.Spacing();
        }

        private void DrawFilters()
        {
            // filter to own loot
            var selfOnly = this.plugin.Configuration.SelfOnly;
            if (ImGui.Checkbox(
                Loc.Localize("SelfOnly", "Self Only") +
                "###Kapture_SelfOnly_Checkbox",
                ref selfOnly))
            {
                this.plugin.Configuration.SelfOnly = selfOnly;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "SelfOnly_HelpMarker",
                                           "filter to own items only"));

            var offset = 110f * ImGuiHelpers.GlobalScale;

            var restrictToCustomItemsList = this.plugin.Configuration.RestrictToCustomItems;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictToCustomItems", "Restrict to Following Items") +
                "###Kapture_RestrictToCustomItems_Checkbox",
                ref restrictToCustomItemsList))
            {
                this.plugin.Configuration.RestrictToCustomItems = restrictToCustomItemsList;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictToCustomItems_HelpMarker",
                "add item to list by using dropdown or remove by clicking on them"));

            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Reset", "Reset") + "###Kapture_CustomItemReset_Button"))
            {
                this.selectedItemIndex = 0;
                this.selectedItemCategoryItemIndex = 0;
                this.UpdateItemList();
                this.plugin.Configuration.PermittedItems = new List<uint>();
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();

            // select category by item ui category
            ImGui.Text(Loc.Localize("SelectCategory", "Select Category"));
            ImGui.SameLine(offset);
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * ImGuiHelpers.GlobalScale);
            if (ImGui.Combo(
                "###Kapture_ItemCategoryItems_Combo",
                ref this.selectedItemCategoryItemIndex,
                this.plugin.ItemCategoryNames,
                this.plugin.ItemCategoryIds.Length))
                this.UpdateItemList();

            // select item based on category
            ImGui.Text(Loc.Localize("SelectItem", "Select Item"));
            ImGui.SameLine(offset);
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 2 * ImGuiHelpers.GlobalScale);
            ImGui.Combo(
                "###Kapture_ItemItems_Combo",
                ref this.selectedItemIndex,
                this.itemNames,
                this.itemIds.Length);
            ImGui.SameLine();
            if (ImGui.SmallButton(Loc.Localize("Add", "Add") + "###Kapture_AddItemItem_Button"))
            {
                if (this.plugin.Configuration.PermittedItems.Contains(
                    this.itemIds[this.selectedItemIndex]))
                {
                    ImGui.OpenPopup("###Kapture_DupeCustomItem_Popup");
                }
                else
                {
                    this.plugin.Configuration.PermittedItems.Add(
                        this.itemIds[this.selectedItemIndex]);
                    this.plugin.SaveConfig();
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
            ImGui.SameLine(offset - (5f * ImGuiHelpers.GlobalScale));
            ImGui.Indent(offset - (5f * ImGuiHelpers.GlobalScale));
            if (this.plugin.Configuration.PermittedItems.Count > 0)
            {
                foreach (var permittedItem in this.plugin.Configuration.PermittedItems.ToList())
                {
                    var index = Array.IndexOf(this.plugin.ItemIds, permittedItem);
                    ImGui.Text(this.plugin.ItemNames[index]);
                    if (ImGui.IsItemClicked())
                    {
                        this.plugin.Configuration.PermittedItems.Remove(permittedItem);
                        this.plugin.SaveConfig();
                    }
                }
            }
            else
            {
                ImGui.Text(Loc.Localize("NoPermittedItems", "None"));
            }

            ImGui.Spacing();
        }

        private void DrawLog()
        {
            // logging enabled
            var loggingEnabled = this.plugin.Configuration.LoggingEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("LoggingEnabled", "Enable Loot Logging") +
                "###Kapture_LoggingEnabled_Checkbox",
                ref loggingEnabled))
            {
                this.plugin.Configuration.LoggingEnabled = loggingEnabled;
                this.plugin.SaveConfig();
                if (loggingEnabled) this.plugin.LootLogger.SetLogFormat();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "LoggingEnabled_HelpMarker",
                "save your loot messages to a file in config (see links)"));
            ImGui.Spacing();

            // log format
            ImGui.Text(Loc.Localize("LogFormat", "Log Format"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "LogFormat_HelpMarker",
                "set format for log file with loot info"));
            ImGui.Spacing();
            var pluginLogFormat = this.plugin.Configuration.LogFormat;
            if (ImGui.Combo(
                "###Kapture_LogFormat_Combo",
                ref pluginLogFormat,
                LogFormat.LogFormatNames.ToArray(),
                LogFormat.LogFormatNames.Count))
            {
                this.plugin.Configuration.LogFormat = pluginLogFormat;
                this.plugin.SaveConfig();
                this.plugin.LootLogger.SetLogFormat();
            }
        }

        private void DrawLinks()
        {
            var buttonSize = new Vector2(120f * ImGuiHelpers.GlobalScale, 25f * ImGuiHelpers.GlobalScale);
            if (ImGui.Button(
                Loc.Localize("LoadTestData", "Test Data") + "###Kapture_LoadTestData_Button",
                buttonSize))
                this.plugin.LoadTestData();
            if (ImGui.Button(
                Loc.Localize("ClearData", "Clear Data") + "###Kapture_ClearData_Button",
                buttonSize))
                this.plugin.ClearData();
        }
    }
}
