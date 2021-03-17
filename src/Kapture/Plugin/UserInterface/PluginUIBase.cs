// ReSharper disable InconsistentNaming

using System;

namespace Kapture
{
    public class PluginUIBase : IDisposable
    {
        private readonly IKapturePlugin KapturePlugin;
        public LootOverlayWindow LootOverlayWindow;
        public RollMonitorOverlayWindow RollMonitorOverlayWindow;
        public SettingsWindow SettingsWindow;

        protected PluginUIBase(IKapturePlugin kapturePlugin)
        {
            KapturePlugin = kapturePlugin;
            BuildWindows();
            SetWindowVisibility();
            AddEventHandlers();
        }

        public void Dispose()
        {
        }

        private void BuildWindows()
        {
            LootOverlayWindow = new LootOverlayWindow(KapturePlugin);
            RollMonitorOverlayWindow = new RollMonitorOverlayWindow(KapturePlugin);
            SettingsWindow = new SettingsWindow(KapturePlugin);
        }

        private void SetWindowVisibility()
        {
            LootOverlayWindow.IsVisible = KapturePlugin.Configuration.ShowLootOverlay;
            RollMonitorOverlayWindow.IsVisible = KapturePlugin.Configuration.ShowRollMonitorOverlay;
            SettingsWindow.IsVisible = false;
        }

        private void AddEventHandlers()
        {
            SettingsWindow.LootOverlayVisibilityUpdated += UpdateLootOverlayVisibility;
            SettingsWindow.RollMonitorOverlayVisibilityUpdated += UpdateRollMonitorOverlayVisibility;
        }

        private void UpdateLootOverlayVisibility(object sender, bool e)
        {
            LootOverlayWindow.IsVisible = e;
        }

        private void UpdateRollMonitorOverlayVisibility(object sender, bool e)
        {
            RollMonitorOverlayWindow.IsVisible = e;
        }

        public void Draw()
        {
            LootOverlayWindow.DrawView();
            RollMonitorOverlayWindow.DrawView();
            SettingsWindow.DrawView();
        }
    }
}