using System.Collections.Generic;

using Dalamud.Configuration;

namespace Kapture
{
    /// <summary>
    /// Kapture config.
    /// </summary>
    public class KaptureConfig : IPluginConfiguration
    {
        /// <summary>
        /// Timeout for added items.
        /// </summary>
        public int RollMonitorAddedTimeout = 600000;

        /// <summary>
        /// Timeout for obtained items.
        /// </summary>
        public int RollMonitorObtainedTimeout = 10000;

        /// <summary>
        /// Frequency to process queued loot messages.
        /// </summary>
        public int RollMonitorProcessFrequency = 3000;

        /// <summary>
        /// Gets or sets a value indicating whether fresh install.
        /// </summary>
        public bool FreshInstall { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets plugin language.
        /// </summary>
        public int PluginLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show loot overlay.
        /// </summary>
        public bool ShowLootOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show roll monitor.
        /// </summary>
        public bool ShowRollMonitorOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable add loot event.
        /// </summary>
        public bool AddEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable cast loot event.
        /// </summary>
        public bool CastEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable craft loot event.
        /// </summary>
        public bool CraftEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable desynth loot event.
        /// </summary>
        public bool DesynthEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable discard loot event.
        /// </summary>
        public bool DiscardEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable gather loot event.
        /// </summary>
        public bool GatherEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable greed loot event.
        /// </summary>
        public bool GreedEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable lost loot event.
        /// </summary>
        public bool LostEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable need loot event.
        /// </summary>
        public bool NeedEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable obtain loot event.
        /// </summary>
        public bool ObtainEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable purchase loot event.
        /// </summary>
        public bool PurchaseEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable search loot event.
        /// </summary>
        public bool SearchEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable sell loot event.
        /// </summary>
        public bool SellEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable use loot event.
        /// </summary>
        public bool UseEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restrict in combat.
        /// </summary>
        public bool RestrictInCombat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restrict in content.
        /// </summary>
        public bool RestrictToContent { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to restrict to specific list of content.
        /// </summary>
        public bool RestrictToCustomContent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restrict to high end duty content.
        /// </summary>
        public bool RestrictToHighEndDuty { get; set; }

        /// <summary>
        /// Gets or sets list of permitted content.
        /// </summary>
        public List<uint> PermittedContent { get; set; } = new ();

        /// <summary>
        /// Gets or sets list of permitted items.
        /// </summary>
        public List<uint> PermittedItems { get; set; } = new ();

        /// <summary>
        /// Gets or sets a value indicating whether to restrict to list of items.
        /// </summary>
        public bool RestrictToCustomItems { get; set; }

        /// <summary>
        /// Gets frequency to write to log.
        /// </summary>
        public int WriteToLogFrequency { get; } = 30000;

        /// <summary>
        /// Gets or sets a value indicating whether to enable logging.
        /// </summary>
        public bool LoggingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restrict to local player events.
        /// </summary>
        public bool SelfOnly { get; set; }

        /// <summary>
        /// Gets or sets loot name format.
        /// </summary>
        public int LootNameFormat { get; set; }

        /// <summary>
        /// Gets or sets roll name format.
        /// </summary>
        public int RollNameFormat { get; set; }

        /// <summary>
        /// Gets or sets log format.
        /// </summary>
        public int LogFormat { get; set; }

        /// <summary>
        /// Gets or sets roll display mode.
        /// </summary>
        public int RollDisplayMode { get; set; }

        /// <summary>
        ///  Gets or sets loot display mode.
        /// </summary>
        public int LootDisplayMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable debug logging to build regex.
        /// </summary>
        public bool DebugLoggingEnabled { get; set; }

        /// <inheritdoc />
        public int Version { get; set; }
    }
}
