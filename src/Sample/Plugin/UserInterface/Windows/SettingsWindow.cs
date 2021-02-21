using System;
using System.Diagnostics;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

// ReSharper disable InconsistentNaming

// ReSharper disable InvertIf

namespace Sample
{
    public class SettingsWindow : WindowBase
    {
        private readonly ISamplePlugin _samplePlugin;
        private Tab _currentTab = Tab.General;
        private float _uiScale;

        public SettingsWindow(ISamplePlugin samplePlugin)
        {
            _samplePlugin = samplePlugin;
        }

        public event EventHandler<bool> OverlayVisibilityUpdated;

        public override void DrawView()
        {
            if (!IsVisible) return;
            var isVisible = IsVisible;
            _uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(350 * _uiScale, 210 * _uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(Loc.Localize("SettingsWindow", "Sample Settings") + "###Sample_Settings_Window",
                ref isVisible,
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar))
            {
                IsVisible = isVisible;
                DrawTabs();
                switch (_currentTab)
                {
                    case Tab.General:
                    {
                        DrawGeneral();
                        break;
                    }
                    case Tab.Other:
                    {
                        DrawOther();
                        break;
                    }
                    default:
                        DrawGeneral();
                        break;
                }
            }

            ImGui.End();
        }

        public void DrawTabs()
        {
            if (ImGui.BeginTabBar("SampleSettingsTabBar", ImGuiTabBarFlags.NoTooltip))
            {
                if (ImGui.BeginTabItem(Loc.Localize("General", "General") + "###Sample_General_Tab"))
                {
                    _currentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Other", "Other") + "###Sample_Other_Tab"))
                {
                    _currentTab = Tab.Other;
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
                ImGui.Spacing();
            }
        }

        public void DrawGeneral()
        {
            // plugin enabled
            var enabled = _samplePlugin.Configuration.Enabled;
            if (ImGui.Checkbox(
                Loc.Localize("PluginEnabled", "Plugin Enabled") + "###Sample_PluginEnabled_Checkbox",
                ref enabled))
            {
                _samplePlugin.Configuration.Enabled = enabled;
                _samplePlugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("PluginEnabled_HelpMarker",
                "toggle the plugin on/off"));
            ImGui.Spacing();

            // show overlay
            var showOverlay = _samplePlugin.Configuration.ShowOverlay;
            if (ImGui.Checkbox(Loc.Localize("ShowOverlay", "Show Overlay") + "###Sample_ShowOverlay_Checkbox",
                ref showOverlay))
            {
                _samplePlugin.Configuration.ShowOverlay = showOverlay;
                OverlayVisibilityUpdated?.Invoke(this, showOverlay);
                _samplePlugin.SaveConfig();
            }

            CustomWidgets.HelpMarker(Loc.Localize("ShowOverlay_HelpMarker",
                "show overlay window"));
            ImGui.Spacing();

            // language
            ImGui.Text(Loc.Localize("Language", "Language"));
            CustomWidgets.HelpMarker(Loc.Localize("Language_HelpMarker",
                "use default or override plugin ui language"));
            ImGui.Spacing();
            var pluginLanguage = _samplePlugin.Configuration.PluginLanguage;
            if (ImGui.Combo("###Sample_Language_Combo", ref pluginLanguage,
                PluginLanguage.LanguageNames.ToArray(),
                PluginLanguage.LanguageNames.Count))
            {
                _samplePlugin.Configuration.PluginLanguage = pluginLanguage;
                _samplePlugin.SaveConfig();
                _samplePlugin.Localization.SetLanguage(pluginLanguage);
            }
        }

        public void DrawOther()
        {
            var buttonSize = new Vector2(120f * _uiScale, 25f * _uiScale);
            if (ImGui.Button(Loc.Localize("OpenGithub", "Github") + "###Sample_OpenGithub_Button", buttonSize))
                Process.Start("https://github.com");
            if (ImGui.Button(
                Loc.Localize("ImproveTranslate", "Translations") + "###Sample_ImproveTranslate_Button",
                buttonSize))
                Process.Start("https://crowdin.com");
            if (ImGui.Button(Loc.Localize("PrintHelp", "Instructions") + "###Sample_PrintHelp_Button", buttonSize))
                _samplePlugin.PrintHelpMessage();
        }

        private enum Tab
        {
            General,
            Other
        }
    }
}