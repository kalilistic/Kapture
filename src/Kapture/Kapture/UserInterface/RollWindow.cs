using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using ImGuiNET;

namespace Kapture
{
    /// <inheritdoc />
    public class RollWindow : PluginWindow
    {
        private readonly IKapturePlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollWindow"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public RollWindow(KapturePlugin plugin)
            : base(plugin, Loc.Localize("RollMonitorOverlayWindow", "Roll Monitor") + "###Kapture_RollMonitor_Window", ImGuiWindowFlags.NoFocusOnAppearing)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(350 * ImGuiHelpers.GlobalScale, 150 * ImGuiHelpers.GlobalScale), ImGuiCond.FirstUseEver);
            if (this.plugin.Configuration.Enabled)
            {
                var lootRolls = this.plugin.LootRollsDisplay;

                if (lootRolls!.Count > 0)
                {
                    var col1 = 200f * ImGuiHelpers.GlobalScale;
                    var col2 = 258f * ImGuiHelpers.GlobalScale;

                    ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("MonitorItemName", "Item"));
                    ImGui.SameLine(col1);
                    ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("MonitorRollers", "Rollers"));
                    ImGui.SameLine(col2);
                    ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("MonitorWinner", "Winner"));

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
                        ImGui.TextColored(ImGuiColors.DalamudViolet, lootRoll.ItemName);
                        ImGui.Separator();
                        foreach (var roller in lootRoll.Rollers)
                        {
                            if (roller.HasRolled)
                            {
                                ImGui.TextColored(roller.RollColor, roller.PlayerName);
                                ImGui.SameLine();
                                ImGui.PushFont(UiBuilder.IconFont);
                                ImGui.TextColored(roller.RollColor, FontAwesomeIcon.Check.ToIconString());
                                ImGui.PopFont();
                            }
                            else if (lootRoll.IsWon)
                            {
                                ImGui.TextColored(roller.RollColor, roller.PlayerName);
                                ImGui.SameLine();
                                ImGui.PushFont(UiBuilder.IconFont);
                                ImGui.TextColored(roller.RollColor, FontAwesomeIcon.Times.ToIconString());
                                ImGui.PopFont();
                            }
                            else
                            {
                                ImGui.TextColored(roller.RollColor, roller.PlayerName);
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
        }

        /// <summary>
        /// Checks whether should display roll window.
        /// </summary>
        /// <returns>indicator whether to show roll window.</returns>
        public bool ShowRollWindow()
        {
            return !this.plugin.IsInitializing && this.plugin.IsLoggedIn() && this.plugin.Configuration.Enabled &&
                   (this.plugin.Configuration.RollDisplayMode == DisplayMode.AlwaysOn.Code ||
                    (this.plugin.Configuration.RollDisplayMode == DisplayMode.ContentOnly.Code && this.plugin.InContent) ||
                    (this.plugin.Configuration.RollDisplayMode == DisplayMode.DuringRollsOnly.Code && this.plugin.IsRolling));
        }
    }
}
