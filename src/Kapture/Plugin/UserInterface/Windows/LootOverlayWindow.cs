using System.Linq;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace Kapture
{
    public class LootOverlayWindow : WindowBase
    {
        private readonly IKapturePlugin _plugin;
        private float _uiScale;

        public LootOverlayWindow(IKapturePlugin plugin)
        {
            _plugin = plugin;
        }

        private bool ShowOverlay()
        {
            return !_plugin.IsInitializing && _plugin.IsLoggedIn() && _plugin.Configuration.Enabled && IsVisible &&
                   (_plugin.Configuration.LootDisplayMode == DisplayMode.AlwaysOn.Code ||
                    _plugin.Configuration.LootDisplayMode == DisplayMode.ContentOnly.Code && _plugin.InContent ||
                    _plugin.Configuration.LootDisplayMode == DisplayMode.DuringRollsOnly.Code && _plugin.IsRolling);
        }

        public override void DrawView()
        {
            if (!ShowOverlay()) return;
            var isVisible = IsVisible;
            _uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(300 * _uiScale, 150 * _uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("LootOverlayWindow", "Loot") + "###Kapture_Loot_Window"))
            {
                if (_plugin.ClientLanguage() != 1)
                {
                    ImGui.Text(Loc.Localize("LanguageNotSupported1",
                        "Sorry, only English is supported at this time."));
                    ImGui.Spacing();
                    ImGui.Text(Loc.Localize("LanguageNotSupported2",
                        "Please reach out on discord if you want to help."));
                    ImGui.Text(Loc.Localize("LanguageNotSupported3",
                        "add German, French, or Japanese support."));
                }
                else if (_plugin.Configuration.Enabled)
                {
                    var lootEvents = _plugin.LootEvents.ToList();
                    if (lootEvents.Count > 0)
                    {
                        ImGui.Columns(3);
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("LootItemName", "Item"));
                        ImGui.NextColumn();
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("LootEventType", "Event"));
                        ImGui.NextColumn();
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("LootPlayer", "Player"));
                        ImGui.NextColumn();
                        ImGui.Separator();

                        foreach (var lootEvent in lootEvents)
                        {
                            ImGui.Text(lootEvent.ItemDisplayName);
                            ImGui.NextColumn();
                            ImGui.Text(lootEvent.LootEventTypeName);
                            ImGui.NextColumn();
                            ImGui.Text(lootEvent.PlayerDisplayName);
                            ImGui.NextColumn();
                        }
                    }
                    else
                    {
                        ImGui.Text(Loc.Localize("WaitingForItems", "Waiting for items."));
                    }
                }

                IsVisible = isVisible;
            }

            ImGui.End();
        }
    }
}