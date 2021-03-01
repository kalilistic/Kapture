using System.Collections.Generic;

namespace KapturePlugin
{
    public abstract class KaptureConfig
    {
        public int RollMonitorTimeout = 600000;
        public bool FreshInstall { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public int PluginLanguage { get; set; }
        public bool ShowLootOverlay { get; set; } = true;
        public bool ShowRollMonitorOverlay { get; set; } = true;
        public bool AddEnabled { get; set; }
        public bool CastEnabled { get; set; }
        public bool CraftEnabled { get; set; }
        public bool DesynthEnabled { get; set; }
        public bool DiscardEnabled { get; set; }
        public bool GatherEnabled { get; set; }
        public bool GreedEnabled { get; set; }
        public bool LostEnabled { get; set; }
        public bool NeedEnabled { get; set; }
        public bool ObtainEnabled { get; set; } = true;
        public bool PurchaseEnabled { get; set; }
        public bool SearchEnabled { get; set; }
        public bool SellEnabled { get; set; }
        public bool UseEnabled { get; set; }
        public bool RestrictToContent { get; set; } = true;
        public bool RestrictToCustomContent { get; set; }
        public bool RestrictToHighEndDuty { get; set; }
        public List<uint> PermittedContent { get; set; } = new List<uint>();
        public List<uint> PermittedItems { get; set; } = new List<uint>();
        public bool RestrictToCustomItems { get; set; }
        public int WriteToLogFrequency { get; } = 30000;
        public bool LoggingEnabled { get; set; }
        public bool SelfOnly { get; set; }
        public bool ShowRollerCount { get; set; }
        public int LootNameFormat { get; set; }
        public int RollNameFormat { get; set; } = 2;
    }
}