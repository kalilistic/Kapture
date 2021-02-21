// ReSharper disable DelegateSubtraction

using System;
using System.Threading.Tasks;
using CheapLoc;
using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace Sample
{
    public sealed class SamplePlugin : PluginBase, ISamplePlugin
    {
        private DalamudPluginInterface _pluginInterface;
        private PluginUI _pluginUI;

        public SamplePlugin(string pluginName, DalamudPluginInterface pluginInterface) : base(pluginName,
            pluginInterface)
        {
            Task.Run(() =>
            {
                _pluginInterface = pluginInterface;
                ResourceManager.UpdateResources();
                LoadConfig();
                LoadServices();
                SetupCommands();
                LoadUI();
                HandleFreshInstall();
            });
        }

        public ISampleService SampleService { get; set; }
        public SampleConfig Configuration { get; set; }

        public void PrintHelpMessage()
        {
            PrintMessage(Loc.Localize("HelpMessage",
                "This is a helpful message!"));
        }

        public new void Dispose()
        {
            base.Dispose();
            RemoveCommands();
            _pluginInterface.UiBuilder.OnOpenConfigUi -= (sender, args) => DrawConfigUI();
            _pluginInterface.UiBuilder.OnBuildUi -= DrawUI;
            _pluginInterface.Dispose();
        }

        public void SaveConfig()
        {
            SaveConfig(Configuration);
        }

        public new void SetupCommands()
        {
            _pluginInterface.CommandManager.AddHandler("/sample", new CommandInfo(ToggleSample)
            {
                HelpMessage = "Show sample plugin.",
                ShowInHelp = true
            });
            _pluginInterface.CommandManager.AddHandler("/sampleconfig", new CommandInfo(ToggleConfig)
            {
                HelpMessage = "Show sample config.",
                ShowInHelp = true
            });
        }

        public new void RemoveCommands()
        {
            _pluginInterface.CommandManager.RemoveHandler("/sample");
            _pluginInterface.CommandManager.RemoveHandler("/sampleconfig");
        }

        public void ToggleSample(string command, string args)
        {
            LogInfo("Running command {0} with args {1}", command, args);
            Configuration.ShowOverlay = !Configuration.ShowOverlay;
            _pluginUI.OverlayWindow.IsVisible = !_pluginUI.OverlayWindow.IsVisible;
        }

        public void ToggleConfig(string command, string args)
        {
            LogInfo("Running command {0} with args {1}", command, args);
            _pluginUI.SettingsWindow.IsVisible = !_pluginUI.SettingsWindow.IsVisible;
        }

        public void LoadServices()
        {
            SampleService = new SampleService(this);
        }

        public void LoadUI()
        {
            Localization.SetLanguage(Configuration.PluginLanguage);
            _pluginUI = new PluginUI(this);
            _pluginInterface.UiBuilder.OnBuildUi += DrawUI;
            _pluginInterface.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();
        }

        private void HandleFreshInstall()
        {
            if (!Configuration.FreshInstall) return;
            PrintMessage(Loc.Localize("InstallThankYou", "Thank you for installing Sample Plugin!"));
            PrintHelpMessage();
            Configuration.FreshInstall = false;
            SaveConfig();
            _pluginUI.SettingsWindow.IsVisible = true;
        }

        private void DrawUI()
        {
            _pluginUI.Draw();
        }

        private void DrawConfigUI()
        {
            _pluginUI.SettingsWindow.IsVisible = true;
        }

        public new void LoadConfig()
        {
            try
            {
                Configuration = base.LoadConfig() as PluginConfig ?? new PluginConfig();
            }
            catch (Exception ex)
            {
                LogError("Failed to load config so creating new one.", ex);
                Configuration = new PluginConfig();
                SaveConfig();
            }
        }
    }
}