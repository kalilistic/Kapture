using System;

namespace Sample
{
	public class PluginUIBase : IDisposable
	{
		public OverlayWindow OverlayWindow;
		public ISamplePlugin SamplePlugin;
		public SettingsWindow SettingsWindow;

		public PluginUIBase(ISamplePlugin samplePlugin)
		{
			SamplePlugin = samplePlugin;
			BuildWindows();
			SetWindowVisibility();
			AddEventHandlers();
		}

		public void Dispose()
		{
		}

		private void BuildWindows()
		{
			OverlayWindow = new OverlayWindow(SamplePlugin);
			SettingsWindow = new SettingsWindow(SamplePlugin);
		}

		private void SetWindowVisibility()
		{
			OverlayWindow.IsVisible = SamplePlugin.Configuration.ShowOverlay;
			SettingsWindow.IsVisible = false;
		}

		private void AddEventHandlers()
		{
			SettingsWindow.OverlayVisibilityUpdated += UpdateOverlayVisibility;
		}

		private void UpdateOverlayVisibility(object sender, bool e)
		{
			OverlayWindow.IsVisible = e;
		}

		public void Draw()
		{
			OverlayWindow.DrawView();
			SettingsWindow.DrawView();
		}
	}
}