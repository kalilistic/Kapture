using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using Dalamud.Interface;
using ImGuiNET;

namespace Kapture
{
    /// <inheritdoc />
    public class RollMonitorOverlay : WindowBase
    {
        private readonly IKapturePlugin plugin;
        private float uiScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollMonitorOverlay"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public RollMonitorOverlay(IKapturePlugin plugin)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc />
        public override void DrawView()
        {
            if (!this.ShowOverlay()) return;
            var isVisible = this.IsVisible;
            this.uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(350 * this.uiScale, 150 * this.uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("RollMonitorOverlayWindow", "Roll Monitor") + "###Kapture_RollMonitor_Window"))
            {
                if (this.plugin.Configuration.Enabled)
                {
                    var lootRolls = this.plugin.LootRollsDisplay;

                    if (lootRolls!.Count > 0)
                    {
                        var col1 = 200f * Scale;
                        var col2 = 258f * Scale;

                        ImGui.TextColored(ImGuiColors2.ToadViolet, Loc.Localize("MonitorItemName", "Item"));
                        ImGui.SameLine(col1);
                        ImGui.TextColored(ImGuiColors2.ToadViolet, Loc.Localize("MonitorRollers", "Rollers"));
                        ImGui.SameLine(col2);
                        ImGui.TextColored(ImGuiColors2.ToadViolet, Loc.Localize("MonitorWinner", "Winner"));

                        foreach (var lootRoll in lootRolls.ToList())
                        {
                            ImGui.BeginGroup();
                            ImGui.Text(lootRoll.ItemNameAbbreviated);
                            ImGui.SameLine(col1 - 7f);
                            ImGui.Text(lootRoll.RollerCount + " / " + lootRoll.Rollers.Count);
                            ImGui.SameLine(col2 - 7f);
                            ImGui.Text(lootRoll.Winner);
                            ImGui.EndGroup();
                            if (!ImGui.IsItemHovered()) continue;
                            ImGui.BeginTooltip();
                            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
                            ImGui.TextColored(ImGuiColors2.ToadViolet, lootRoll.ItemName);
                            ImGui.Separator();
                            foreach (var roller in lootRoll.Rollers)
                            {
                                if (roller.HasRolled)
                                {
                                    ImGui.TextColored(roller.RollColor, roller.FormattedPlayerName);
                                    ImGui.SameLine();
                                    ImGui.PushFont(UiBuilder.IconFont);
                                    ImGui.TextColored(roller.RollColor, FontAwesomeIcon.Check.ToIconString());
                                    ImGui.PopFont();
                                }
                                else if (lootRoll.IsWon)
                                {
                                    ImGui.TextColored(roller.RollColor, roller.FormattedPlayerName);
                                    ImGui.SameLine();
                                    ImGui.PushFont(UiBuilder.IconFont);
                                    ImGui.TextColored(roller.RollColor, FontAwesomeIcon.Times.ToIconString());
                                    ImGui.PopFont();
                                }
                                else
                                {
                                    ImGui.TextColored(roller.RollColor, roller.FormattedPlayerName);
                                }
                            }

                            ImGui.PopTextWrapPos();
                            ImGui.EndTooltip();
                        }
                    }
                    else
                    {
                        ImGui.Text(Loc.Localize("WaitingForItems", "Waiting for items."));
                    }
                }

                this.IsVisible = isVisible;
            }

            ImGui.End();
        }

        private bool ShowOverlay()
        {
            return !this.plugin.IsInitializing && this.plugin.IsLoggedIn() && this.plugin.Configuration.Enabled && this.IsVisible &&
                   (this.plugin.Configuration.RollDisplayMode == DisplayMode.AlwaysOn.Code ||
                    (this.plugin.Configuration.RollDisplayMode == DisplayMode.ContentOnly.Code && this.plugin.InContent) ||
                    (this.plugin.Configuration.RollDisplayMode == DisplayMode.DuringRollsOnly.Code && this.plugin.IsRolling));
        }
    }
}
