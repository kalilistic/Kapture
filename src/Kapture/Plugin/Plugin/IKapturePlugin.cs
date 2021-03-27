using System;
using System.Collections.Generic;

namespace Kapture
{
    public interface IKapturePlugin
    {
        KaptureConfig Configuration { get; }
        Localization Localization { get; }
        uint[] ContentIds { get; }
        string[] ContentNames { get; }
        uint[] ItemIds { get; }
        string[] ItemNames { get; }
        uint[] ItemCategoryIds { get; }
        string[] ItemCategoryNames { get; }
        List<KeyValuePair<uint, ItemList>> ItemLists { get; }
        bool IsInitializing { get; }
        List<LootEvent> LootEvents { get; }
        DataManager DataManager { get; }
        List<LootRoll> LootRolls { get; }
        List<LootRoll> LootRollsDisplay { get; set; }
        LootLogger LootLogger { get; set; }
        bool InContent { get; set; }
        bool IsRolling { get; set; }
        RollMonitor RollMonitor { get; }
        LootProcessor LootProcessor { get; }
        void SaveConfig();
        string GetSeIcon(SeIconChar seIconChar);
        void LogInfo(string messageTemplate);
        void LogError(Exception exception, string messageTemplate, params object[] values);
        string GetLocalPlayerName();
        ushort ClientLanguage();
        string FormatPlayerName(int nameFormatCode, string playerName);
        bool IsLoggedIn();
        void LoadTestData();
        void ClearData();
        bool InCombat();
    }
}