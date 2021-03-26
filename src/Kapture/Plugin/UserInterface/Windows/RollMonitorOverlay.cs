using System.Linq;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace Kapture
{
    public class RollMonitorOverlayWindow : WindowBase
    {
        private readonly IKapturePlugin _plugin;
        private float _uiScale;

        public RollMonitorOverlayWindow(IKapturePlugin plugin)
        {
            _plugin = plugin;
        }

        private bool ShowOverlay()
        {
            return !_plugin.IsInitializing && _plugin.IsLoggedIn() && _plugin.Configuration.Enabled && IsVisible &&
                   (_plugin.Configuration.RollDisplayMode == DisplayMode.AlwaysOn.Code ||
                    _plugin.Configuration.RollDisplayMode == DisplayMode.ContentOnly.Code && _plugin.InContent ||
                    _plugin.Configuration.RollDisplayMode == DisplayMode.DuringRollsOnly.Code && _plugin.IsRolling);
        }

        public override void DrawView()
        {
            if (!ShowOverlay()) return;
            var isVisible = IsVisible;
            _uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(350 * _uiScale, 150 * _uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("RollMonitorOverlayWindow", "Roll Monitor") + "###Kapture_RollMonitor_Window"))
            {
                if (_plugin.Configuration.Enabled)
                {
                    var lootRolls = _plugin.LootRollsDisplay;

                    if (lootRolls.Count > 0)
                    {
                        var col1 = 200f * Scale;
                        var col2 = 258f * Scale;

                        ImGui.TextColored(UIColor.Violet, Loc.Localize("MonitorItemName", "Item"));
                        ImGui.SameLine(col1);
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("MonitorRollers", "Rollers"));
                        ImGui.SameLine(col2);
                        ImGui.TextColored(UIColor.Violet, Loc.Localize("MonitorWinner", "Winner"));

                        foreach (var lootRoll in lootRolls.ToList())
                        {
                            ImGui.BeginGroup();
                            ImGui.Text(lootRoll.ItemNameAbbreviated);
                            ImGui.SameLine(col1 - 7f);
                            ImGui.Text(lootRoll.RollerCount.ToString());
                            ImGui.SameLine(col2 - 7f);
                            ImGui.Text(lootRoll.Winner);
                            ImGui.EndGroup();
                            if (!ImGui.IsItemHovered()) continue;
                            ImGui.BeginTooltip();
                            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                            ImGui.TextColored(UIColor.Violet, lootRoll.ItemName);
                            ImGui.Separator();
                            if (lootRoll.RollerCount > 0)
                                foreach (var roller in lootRoll.RollersDisplay)
                                    ImGui.TextColored(roller.Value, roller.Key);
                            else
                                ImGui.Text(Loc.Localize("RollMonitorNone", "No one has rolled"));
                            ImGui.PopTextWrapPos();
                            ImGui.EndTooltip();
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