using System.Collections.Generic;
using Dalamud.Game.ClientState.Party;
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
        public PartyMember[] CurrentPartyList { get; set; } = System.Array.Empty<PartyMember>();

        /// <inheritdoc />
        public KaptureConfig Configuration { get; }

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
        public bool InContent { get; set; }
        
        /// <inheritdoc />
        public bool IsRolling { get; set; }
        
        /// <inheritdoc />
        public RollMonitor RollMonitor { get; }

        /// <summary>
        /// Loot Processor.
        /// </summary>
        public LootProcessor LootProcessor { get; } = null!;

        /// <inheritdoc />
        public string GetLocalPlayerName()
        {
            return "Pika Chu";
        }
        
        /// <inheritdoc />
        public string GetLocalPlayerWorld()
        {
            return "Pandaemonium";
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
        public bool InCombat()
        {
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<PartyMember> GetPartyMembers()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public string GetSeIcon(SeIconChar seIconChar)
        {
            return string.Empty;
        }
    }
}
