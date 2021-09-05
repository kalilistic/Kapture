using System;

namespace Kapture
{
    /// <inheritdoc />
    public class PluginUI : IDisposable
    {
        /// <summary>
        /// Loot overlay window.
        /// </summary>
        public LootOverlayWindow LootOverlayWindow = null!;

        /// <summary>
        /// Roll monitor window.
        /// </summary>
        public RollMonitorOverlay RollMonitorOverlay = null!;

        /// <summary>
        /// Settings window.
        /// </summary>
        public SettingsWindow SettingsWindow = null!;

        private readonly KapturePlugin kapturePlugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginUI"/> class.
        /// </summary>
        /// <param name="kapturePlugin">kapture plugin.</param>
        public PluginUI(KapturePlugin kapturePlugin)
        {
            this.kapturePlugin = kapturePlugin;
            this.BuildWindows();
            this.SetWindowVisibility();
            this.AddEventHandlers();
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        /// Draw windows.
        /// </summary>
        public void Draw()
        {
            this.LootOverlayWindow.DrawView();
            this.RollMonitorOverlay.DrawView();
            this.SettingsWindow.DrawView();
        }

        private void BuildWindows()
        {
            this.LootOverlayWindow = new LootOverlayWindow(this.kapturePlugin);
            this.RollMonitorOverlay = new RollMonitorOverlay(this.kapturePlugin);
            this.SettingsWindow = new SettingsWindow(this.kapturePlugin);
        }

        private void SetWindowVisibility()
        {
            this.LootOverlayWindow.IsVisible = this.kapturePlugin.Configuration.ShowLootOverlay;
            this.RollMonitorOverlay.IsVisible = this.kapturePlugin.Configuration.ShowRollMonitorOverlay;
            this.SettingsWindow.IsVisible = true;
        }

        private void AddEventHandlers()
        {
            this.SettingsWindow.LootOverlayVisibilityUpdated += this.UpdateLootOverlayVisibility;
            this.SettingsWindow.RollMonitorOverlayVisibilityUpdated += this.UpdateRollMonitorOverlayVisibility;
        }

        private void UpdateLootOverlayVisibility(object? sender, bool e)
        {
            this.LootOverlayWindow.IsVisible = e;
        }

        private void UpdateRollMonitorOverlayVisibility(object? sender, bool e)
        {
            this.RollMonitorOverlay.IsVisible = e;
        }
    }
}
