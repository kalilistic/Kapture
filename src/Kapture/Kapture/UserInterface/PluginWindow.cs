using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Kapture
{
    /// <summary>
    /// Plugin window which extends window with Kapture.
    /// </summary>
    public abstract class PluginWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginWindow"/> class.
        /// </summary>
        /// <param name="plugin">Kapture plugin.</param>
        /// <param name="windowName">Name of the window.</param>
        /// <param name="flags">ImGui flags.</param>
        protected PluginWindow(KapturePlugin plugin, string windowName, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
            : base(windowName, flags)
        {
            this.Plugin = plugin;
            this.RespectCloseHotkey = false;
        }

        /// <summary>
        /// Gets Kapture for window.
        /// </summary>
        protected KapturePlugin Plugin { get; }

        /// <inheritdoc/>
        public override void Draw()
        {
        }
    }
}
