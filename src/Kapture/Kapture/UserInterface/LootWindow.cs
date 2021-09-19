using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using Dalamud.Interface;
using ImGuiNET;

namespace Kapture
{
    /// <inheritdoc />
    public class LootWindow : PluginWindow
    {
        private readonly IKapturePlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="LootWindow"/> class.
        /// </summary>
        /// <param name="plugin">plugin.</param>
        public LootWindow(KapturePlugin plugin)
            : base(plugin, Loc.Localize("LootOverlayWindow", "Loot") + "###Kapture_Loot_Window")
        {
            this.plugin = plugin;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(380 * ImGuiHelpers.GlobalScale, 220 * ImGuiHelpers.GlobalScale), ImGuiCond.FirstUseEver);
            if (this.plugin.Configuration.Enabled)
            {
                var lootEvents = this.plugin.LootEvents.ToList();
                if (lootEvents.Count > 0)
                {
                    var col1 = 200f * ImGuiHelpers.GlobalScale;
                    var col2 = 270f * ImGuiHelpers.GlobalScale;

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
        }

        private bool ShowOverlay()
        {
            return !this.plugin.IsInitializing && this.plugin.IsLoggedIn() && this.plugin.Configuration.Enabled &&
                   (this.plugin.Configuration.LootDisplayMode == DisplayMode.AlwaysOn.Code ||
                    (this.plugin.Configuration.LootDisplayMode == DisplayMode.ContentOnly.Code && this.plugin.InContent) ||
                    (this.plugin.Configuration.LootDisplayMode == DisplayMode.DuringRollsOnly.Code && this.plugin.IsRolling));
        }
    }
}
