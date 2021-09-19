using Dalamud.Interface.Windowing;

namespace Kapture
{
    /// <summary>
    /// Window manager to hold plugin windows and window system.
    /// </summary>
    public class WindowManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowManager"/> class.
        /// </summary>
        /// <param name="kapturePlugin">Kapture plugin.</param>
        public WindowManager(KapturePlugin kapturePlugin)
        {
            this.Plugin = kapturePlugin;

            // create windows
            this.LootWindow = new LootWindow(kapturePlugin);
            this.RollWindow = new RollWindow(kapturePlugin);
            this.SettingsWindow = new SettingsWindow(kapturePlugin);

            // set initial state
            this.LootWindow.IsOpen = this.Plugin.Configuration.ShowLootOverlay;
            this.RollWindow.IsOpen = this.Plugin.Configuration.ShowRollMonitorOverlay;

            // setup window systems
            this.LootWindowSystem = new WindowSystem("KaptureLootWindowSystem");
            this.RollWindowSystem = new WindowSystem("KaptureRollWindowSystem");
            this.ConfigWindowSystem = new WindowSystem("KaptureConfigWindowSystem");

            // add event listeners
            KapturePlugin.PluginInterface.UiBuilder.Draw += this.Draw;
            KapturePlugin.PluginInterface.UiBuilder.OpenConfigUi += this.OpenConfigUi;
        }

        /// <summary>
        /// Gets loot Kapture window.
        /// </summary>
        public LootWindow? LootWindow { get; }

        /// <summary>
        /// Gets roll Kapture window.
        /// </summary>
        public RollWindow? RollWindow { get; }

        /// <summary>
        /// Gets config Kapture window.
        /// </summary>
        public SettingsWindow? SettingsWindow { get; }

        private WindowSystem LootWindowSystem { get; }

        private WindowSystem RollWindowSystem { get; }

        private WindowSystem ConfigWindowSystem { get; }

        private KapturePlugin Plugin { get; }

        /// <summary>
        /// Add windows after plugin start.
        /// </summary>
        public void AddWindows()
        {
            this.LootWindowSystem.AddWindow(this.LootWindow!);
            this.ConfigWindowSystem.AddWindow(this.SettingsWindow!);
            this.RollWindowSystem.AddWindow(this.RollWindow!);
        }

        /// <summary>
        /// Dispose plugin windows and commands.
        /// </summary>
        public void Dispose()
        {
            KapturePlugin.PluginInterface.UiBuilder.Draw -= this.Draw;
            KapturePlugin.PluginInterface.UiBuilder.OpenConfigUi -= this.OpenConfigUi;
            this.LootWindowSystem.RemoveAllWindows();
            this.RollWindowSystem.RemoveAllWindows();
            this.ConfigWindowSystem.RemoveAllWindows();
        }

        private void Draw()
        {
            // only show when logged in
            if (!KapturePlugin.ClientState.IsLoggedIn) return;

            this.ConfigWindowSystem.Draw();

            if (this.Plugin.Configuration.ShowLootOverlay)
            {
                this.LootWindowSystem.Draw();
            }

            if (this.Plugin.Configuration.ShowRollMonitorOverlay)
            {
                this.RollWindowSystem.Draw();
            }
        }

        private void OpenConfigUi()
        {
            this.SettingsWindow!.IsOpen ^= true;
        }
    }
}
