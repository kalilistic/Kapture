// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Collections.Generic;

namespace Kapture
{
    public abstract class KaptureConfig
    {
        public int RollMonitorAddedTimeout = 600000;
        public int RollMonitorObtainedTimeout = 10000;
        public int RollMonitorProcessFrequency = 3000;
        public bool FreshInstall { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public int PluginLanguage { get; set; }
        public bool ShowLootOverlay { get; set; } = true;
        public bool ShowRollMonitorOverlay { get; set; } = true;
        public bool AddEnabled { get; set; } = true;
        public bool CastEnabled { get; set; } = true;
        public bool CraftEnabled { get; set; }
        public bool DesynthEnabled { get; set; }
        public bool DiscardEnabled { get; set; }
        public bool GatherEnabled { get; set; }
        public bool GreedEnabled { get; set; } = true;
        public bool LostEnabled { get; set; } = true;
        public bool NeedEnabled { get; set; } = true;
        public bool ObtainEnabled { get; set; } = true;
        public bool PurchaseEnabled { get; set; }
        public bool SearchEnabled { get; set; }
        public bool SellEnabled { get; set; }
        public bool UseEnabled { get; set; }
        public bool RestrictInCombat { get; set; }
        public bool RestrictToContent { get; set; } = true;
        public bool RestrictToCustomContent { get; set; }
        public bool RestrictToHighEndDuty { get; set; }
        public List<uint> PermittedContent { get; set; } = new List<uint>();
        public List<uint> PermittedItems { get; set; } = new List<uint>();
        public bool RestrictToCustomItems { get; set; }
        public int WriteToLogFrequency { get; } = 30000;
        public bool LoggingEnabled { get; set; }
        public bool SelfOnly { get; set; }
        public int LootNameFormat { get; set; }
        public int RollNameFormat { get; set; }
        public int LogFormat { get; set; }
        public int RollDisplayMode { get; set; }
        public int LootDisplayMode { get; set; }
        public bool DebugLoggingEnabled { get; set; }
    }
}