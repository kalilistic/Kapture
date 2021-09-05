using System.Collections.Generic;
using System.Numerics;

using ImGuiNET;

namespace Kapture
{
    /// <summary>
    /// Window base.
    /// </summary>
    public abstract class WindowBase
    {
        /// <summary>
        /// Color palette.
        /// </summary>
        public readonly List<Vector4> ColorPalette = new ();

        /// <summary>
        /// Gets or sets a value indicating whether is Visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets scale.
        /// </summary>
        protected static float Scale => ImGui.GetIO().FontGlobalScale;

        /// <summary>
        /// Draw.
        /// </summary>
        public abstract void DrawView();

        /// <summary>
        /// Toggle.
        /// </summary>
        public void ToggleView()
        {
            this.IsVisible = !this.IsVisible;
        }

        /// <summary>
        /// Show.
        /// </summary>
        public void ShowView()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// Hide.
        /// </summary>
        public void HideView()
        {
            this.IsVisible = false;
        }
    }
}
