using System;

namespace Sample
{
    public interface ISamplePlugin
    {
        ISampleService SampleService { get; }
        SampleConfig Configuration { get; set; }
        Localization Localization { get; }
        string PluginName { get; }
        void PrintHelpMessage();
        void Dispose();
        void SaveConfig();
        void SetupCommands();
        void RemoveCommands();
        void ToggleSample(string command, string args);
        void ToggleConfig(string command, string args);
        void ExportLocalizable(string command, string args);
        void PrintMessage(string message);
        string GetSeIcon(SeIconChar seIconChar);
        uint? GetLocalPlayerHomeWorld();
        void LogInfo(string messageTemplate);
        void LogInfo(string messageTemplate, params object[] values);
        void LogError(string messageTemplate);
        void LogError(string messageTemplate, params object[] values);
        void LogError(Exception exception, string messageTemplate, params object[] values);
        bool IsKeyPressed(ModifierKey.Enum key);
        bool IsKeyPressed(PrimaryKey.Enum key);
        void SaveConfig(dynamic config);
        void UpdateResources();
        string PluginVersion();
    }
}