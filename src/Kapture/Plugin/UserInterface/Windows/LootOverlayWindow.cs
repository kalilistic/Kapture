using System.Linq;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace KapturePlugin
{
    public class LootOverlayWindow : WindowBase
    {
        private readonly IKapturePlugin _plugin;
        private float _uiScale;

        public LootOverlayWindow(IKapturePlugin plugin)
        {
            _plugin = plugin;
        }

        public override void DrawView()
        {
            if (_plugin.IsInitializing) return;
            if (!_plugin.IsLoggedIn()) return;
            if (!_plugin.Configuration.Enabled) return;
            if (!IsVisible) return;
            var isVisible = IsVisible;
            _uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(300 * _uiScale, 150 * _uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("LootOverlayWindow", "Loot") + "###Kapture_Loot_Window"))
            {
                if (_plugin.ClientLanguage() != 1)
                {
                    ImGui.Text(Loc.Localize("LanguageNotSupported1",
                        "Sorry, only English is supported for this test release."));
                    ImGui.Text(Loc.Localize("LanguageNotSupported2",
                        "I'll be adding support for other client languages soon."));
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