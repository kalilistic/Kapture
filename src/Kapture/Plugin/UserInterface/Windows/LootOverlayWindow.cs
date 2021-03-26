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
            ImGui.SetNextWindowSize(new Vector2(380 * _uiScale, 220 * _uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("LootOverlayWindow", "Loot") + "###Kapture_Loot_Window"))
            {
                if (_plugin.Configuration.Enabled)
                {
                    var lootEvents = _plugin.LootEvents.ToList();
                    if (lootEvents.Count > 0)
                    {
                        var col1 = 200f * Scale;
                        var col2 = 270f * Scale;

                        ImGui.TextColored(UIColor.Violet, Loc.Localize("LootItemName", "Item"));
                        ImGui.SameLine(col1);
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("LootEventType", "Event"));
                        ImGui.SameLine(col2);
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("LootPlayer", "Player"));
                        ImGui.Separator();

                        foreach (var lootEvent in lootEvents)
                        {
                            ImGui.Text(lootEvent.ItemNameAbbreviated);
                            ImGui.SameLine(col1);
                            ImGui.Text(lootEvent.LootEventTypeName);
                            ImGui.SameLine(col2);
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