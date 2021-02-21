using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace Sample
{
    public class OverlayWindow : WindowBase
    {
        private readonly ISamplePlugin _samplePlugin;
        private float _uiScale;

        public OverlayWindow(ISamplePlugin samplePlugin)
        {
            _samplePlugin = samplePlugin;
        }

        public override void DrawView()
        {
            if (!IsVisible) return;
            var isVisible = IsVisible;
            _uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(300 * _uiScale, 150 * _uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("OverlayWindow", "Sample Overlay") + "###Sample_Overlay_Window",
                ref isVisible,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.Text(!_samplePlugin.Configuration.Enabled
                    ? Loc.Localize("PluginDisabled", "SamplePlugin is disabled.")
                    : Loc.Localize("PluginEnabled", "SamplePlugin is enabled."));
                IsVisible = isVisible;
            }

            ImGui.End();
        }
    }
}