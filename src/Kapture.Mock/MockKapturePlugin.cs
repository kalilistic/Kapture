// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;

namespace Kapture.Mock
{
    public class MockKapturePlugin : IKapturePlugin, IPluginBase
    {
        public MockKapturePlugin(int langCode = 0)
        {
            PluginName = "Kapture";
            Localization = new Localization(this);
            Configuration = new MockConfig
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
            RollMonitor = new RollMonitor.RollMonitor(this);
            if (langCode == 0)
            {
                LootProcessor = new ENLootProcessor(this);
            }
            else if (langCode == 1)
            {
                LootProcessor = new ENLootProcessor(this); 
            }
            else if (langCode == 2)
            {
                LootProcessor = new ENLootProcessor(this); 
            }
            else if (langCode == 3)
            {
                LootProcessor = new ENLootProcessor(this); 
            }
            else if (langCode == 4)
            {
                LootProcessor = new ZHLootProcessor(this); 
            }

        }

        public LootProcessor LootProcessor { get; }
        public RollMonitor.RollMonitor RollMonitor { get; }

        public string GetLocalPlayerName()
        {
            return "Pika Chu";
        }

        public ushort ClientLanguage()
        {
            throw new NotImplementedException();
        }

        public List<LootRoll> LootRolls { get; } = new List<LootRoll>();
        public List<LootRoll> LootRollsDisplay { get; set; } = new List<LootRoll>();

        public string FormatPlayerName(int nameFormatCode, string playerName)
        {
            return playerName;
        }

        public uint[] ContentIds { get; set; }
        public string[] ContentNames { get; set; }
        public uint[] ItemIds { get; set; }
        public string[] ItemNames { get; set; }
        public uint[] ItemCategoryIds { get; set; }
        public string[] ItemCategoryNames { get; set; }
        public List<KeyValuePair<uint, ItemList>> ItemLists { get; set; }

        public bool IsInitializing { get; set; }
        public KaptureConfig Configuration { get; set; }
        public Localization Localization { get; }

        public void SaveConfig()
        {
            throw new NotImplementedException();
        }

        string IKapturePlugin.GetSeIcon(SeIconChar seIconChar)
        {
            throw new NotImplementedException();
        }

        void IKapturePlugin.LogInfo(string messageTemplate)
        {
        }

        void IKapturePlugin.LogError(Exception exception, string messageTemplate, params object[] values)
        {
        }

        public List<LootEvent> LootEvents { get; set; }
        public DataManager DataManager { get; set; }

        public bool IsLoggedIn()
        {
            throw new NotImplementedException();
        }

        public void LoadTestData()
        {
            throw new NotImplementedException();
        }

        public void ClearData()
        {
            throw new NotImplementedException();
        }

        public bool InContent { get; set; }
        public bool IsRolling { get; set; }

        public bool InCombat()
        {
            throw new NotImplementedException();
        }

        public LootLogger LootLogger { get; set; }

        public string PluginVersion()
        {
            throw new NotImplementedException();
        }

        public string PluginName { get; }

        public void LogVerbose(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        public dynamic LoadConfig()
        {
            throw new NotImplementedException();
        }

        void IPluginBase.Dispose()
        {
            throw new NotImplementedException();
        }

        public string PluginFolder()
        {
            throw new NotImplementedException();
        }

        void IPluginBase.UpdateResources()
        {
            throw new NotImplementedException();
        }

        public double ConvertHeightToInches(int raceId, int tribeId, int genderId, int sliderHeight)
        {
            throw new NotImplementedException();
        }

        string IPluginBase.GetSeIcon(SeIconChar seIconChar)
        {
            throw new NotImplementedException();
        }

        uint? IPluginBase.GetLocalPlayerHomeWorld()
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.LogInfo(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.LogInfo(string messageTemplate, params object[] values)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.LogError(string messageTemplate)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.LogError(string messageTemplate, params object[] values)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.LogError(Exception exception, string messageTemplate, params object[] values)
        {
            throw new NotImplementedException();
        }

        bool IPluginBase.IsKeyPressed(ModifierKey.Enum key)
        {
            throw new NotImplementedException();
        }

        bool IPluginBase.IsKeyPressed(PrimaryKey.Enum key)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.SaveConfig(dynamic config)
        {
            throw new NotImplementedException();
        }

        void IPluginBase.PrintMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}