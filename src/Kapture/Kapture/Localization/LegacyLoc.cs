using System;
using System.IO;
using System.Reflection;

using CheapLoc;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Kapture.Localization;

public class LegacyLoc
{
    private readonly DalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly string pluginName;
    private readonly Assembly assembly;

    public LegacyLoc(DalamudPluginInterface pluginInterface, ICommandManager commandManager)
    {
        this.pluginInterface = pluginInterface;
        this.commandManager = commandManager;
        this.assembly = Assembly.GetCallingAssembly();
        this.pluginName = this.assembly.GetName().Name ?? string.Empty;
        this.SetLanguage(this.pluginInterface.UiLanguage);
        this.pluginInterface.LanguageChanged += this.LanguageChanged;
        this.commandManager.AddHandler(
            "/" + this.pluginName.ToLower() + "exloc",
            new CommandInfo(this.ExportLocalizable)
            {
                ShowInHelp = false,
            });
    }

    public void SetLanguage(string languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode) && languageCode != "en")
        {
            try
            {
                string locData;
                var resourceFile = $"{this.pluginName}.{this.pluginName}.Resource.translation.{languageCode}.json";
                var resourceStream = this.assembly.GetManifestResourceStream(resourceFile);
                using (var reader = new StreamReader(resourceStream ?? throw new InvalidOperationException()))
                {
                    locData = reader.ReadToEnd();
                }

                Loc.Setup(locData, this.assembly);
            }
            catch (Exception)
            {
                Loc.SetupWithFallbacks(this.assembly);
            }
        }
        else
        {
            Loc.SetupWithFallbacks(this.assembly);
        }
    }

    public void Dispose()
    {
        this.pluginInterface.LanguageChanged -= this.LanguageChanged;
        this.commandManager.RemoveHandler("/" + this.pluginName.ToLower() + "exloc");
    }

    private void ExportLocalizable(string command, string args)
    {
        Loc.ExportLocalizableForAssembly(this.assembly);
    }

    private void LanguageChanged(string langCode)
    {
        this.SetLanguage(langCode);
    }
}
