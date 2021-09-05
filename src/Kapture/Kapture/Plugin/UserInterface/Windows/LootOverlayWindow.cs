using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using ImGuiNET;

namespace Kapture
{
    /// <inheritdoc />
    public class LootOverlayWindow : WindowBase
    {
        private readonly IKapturePlugin plugin;
        private float uiScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="LootOverlayWindow"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public LootOverlayWindow(IKapturePlugin plugin)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc />
        public override void DrawView()
        {
            if (!this.ShowOverlay()) return;
            var isVisible = this.IsVisible;
            this.uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(380 * this.uiScale, 220 * this.uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("LootOverlayWindow", "Loot") + "###Kapture_Loot_Window"))
            {
                if (this.plugin.Configuration.Enabled)
                {
                    var lootEvents = this.plugin.LootEvents.ToList();
                    if (lootEvents.Count > 0)
                    {
                        var col1 = 200f * Scale;
                        var col2 = 270f * Scale;

                        ImGui.TextColored(ImGuiColors2.ToadViolet, Loc.Localize("LootItemName", "Item"));
                        ImGui.SameLine(col1);
                        ImGui.TextColored(ImGuiColors2.ToadViolet, Loc.Localize("LootEventType", "Event"));
                        ImGui.SameLine(col2);
                        ImGui.TextColored(ImGuiColors2.ToadViolet, Loc.Localize("LootPlayer", "Player"));
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

                this.IsVisible = isVisible;
            }

            ImGui.End();
        }

        private bool ShowOverlay()
        {
            return !this.plugin.IsInitializing && this.plugin.IsLoggedIn() && this.plugin.Configuration.Enabled && this.IsVisible &&
                   (this.plugin.Configuration.LootDisplayMode == DisplayMode.AlwaysOn.Code ||
                    (this.plugin.Configuration.LootDisplayMode == DisplayMode.ContentOnly.Code && this.plugin.InContent) ||
                    (this.plugin.Configuration.LootDisplayMode == DisplayMode.DuringRollsOnly.Code && this.plugin.IsRolling));
        }
    }
}
