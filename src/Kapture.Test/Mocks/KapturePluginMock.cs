using System.Collections.Generic;
using Dalamud.DrunkenToad;
using Dalamud.Game.Text;

namespace Kapture.Test
{
    /// <inheritdoc />
    public class KapturePluginMock : IKapturePlugin
    {
        private readonly List<KeyValuePair<uint, ItemList>> itemLists = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        public KapturePluginMock(int langCode = 0)
        {
            this.IsInitializing = false;
            Configuration = new KaptureConfigMock
            {
                AddEnabled = true,
                CastEnabled = true,
                CraftEnabled = true,
                DesynthEnabled = true,
                DiscardEnabled = true,
                GatherEnabled = true,
                GreedEnabled = true,
                LostEnabled = true,
                NeedEnabled = true,
                ObtainEnabled = true,
                PurchaseEnabled = true,
                SearchEnabled = true,
                SellEnabled = true,
                UseEnabled = true
            };
            RollMonitor = new RollMonitor(this);
            if (langCode == 0)
                LootProcessor = new ENLootProcessor(this);
            else if (langCode == 1)
                LootProcessor = new ENLootProcessor(this);
            else if (langCode == 2)
                LootProcessor = new DELootProcessor(this);
            else if (langCode == 3)
                LootProcessor = new ENLootProcessor(this);
            else if (langCode == 4) LootProcessor = new ZHLootProcessor(this);
        }

        /// <inheritdoc />
        public KaptureConfig Configuration { get; }

        /// <inheritdoc />
        public uint[] ContentIds => null!;

        /// <inheritdoc />
        public string[] ContentNames => null!;

        /// <inheritdoc />
        public uint[] ItemIds => null!;

        /// <inheritdoc />
        public string[] ItemNames => null!;

        /// <inheritdoc />
        public uint[] ItemCategoryIds => null!;

        /// <inheritdoc />
        public string[] ItemCategoryNames => null!;

        /// <inheritdoc />
        public List<KeyValuePair<uint, ItemList>> ItemLists => this.itemLists;

        /// <inheritdoc />
        public bool IsInitializing { get; }
        
        /// <inheritdoc />
        public List<LootEvent> LootEvents => null!;

        /// <inheritdoc />
        public PluginDataManager PluginDataManager => null!;

        /// <inheritdoc />
        public List<LootRoll> LootRolls => new List<LootRoll>();

        /// <inheritdoc />
        public List<LootRoll>? LootRollsDisplay { get; set; } = null!;

        /// <inheritdoc />
        public LootLogger LootLogger { get; set; } = null!;

        /// <inheritdoc />
        public bool InContent { get; set; }
        
        /// <inheritdoc />
        public bool IsRolling { get; set; }
        
        /// <inheritdoc />
        public RollMonitor RollMonitor { get; }

        /// <inheritdoc />
        public LootProcessor LootProcessor { get; } = null!;

        /// <inheritdoc />
        public void SaveConfig()
        {
        }

        /// <inheritdoc />
        public string GetLocalPlayerName()
        {
            return "Pika Chu";
        }

        /// <inheritdoc />
        public ushort ClientLanguage()
        {
            return 0;
        }

        /// <inheritdoc />
        public string FormatPlayerName(int nameFormatCode, string playerName)
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public bool IsLoggedIn()
        {
            return true;
        }

        /// <inheritdoc />
        public void LoadTestData()
        {
        }

        /// <inheritdoc />
        public void ClearData()
        {
        }

        /// <inheritdoc />
        public bool InCombat()
        {
            return false;
        }

        /// <inheritdoc />
        public string GetSeIcon(SeIconChar seIconChar)
        {
            return string.Empty;
        }
    }
}
