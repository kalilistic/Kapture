using System;

using Dalamud.DrunkenToad;
using Dalamud.DrunkenToad.Helpers;

namespace Kapture
{
    /// <summary>
    /// Test data for demo purposes.
    /// </summary>
    public static class TestData
    {
        /// <summary>
        /// Load test data.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public static void LoadTestData(IKapturePlugin plugin)
        {
            // get test data by language
            TestDataSet? testDataSet = null;
            var langCode = plugin.ClientLanguage();
            testDataSet = langCode switch
            {
                0 => new TestDataSet
                {
                    ItemName1 = "Ruby Tide Bracelets of Fending",
                    ItemName2 = "Wind-up Aldgoat",
                    PlayerName1 = "Wyatt Earp",
                    PlayerName2 = "April O'Neil",
                },
                1 => new TestDataSet
                {
                    ItemName1 = "Ruby Tide Bracelets of Fending",
                    ItemName2 = "Wind-up Aldgoat",
                    PlayerName1 = "Wyatt Earp",
                    PlayerName2 = "April O'Neil",
                },
                2 => new TestDataSet
                {
                    ItemName1 = "Topasring",
                    ItemName2 = "Kiesgolem",
                    PlayerName1 = "Hans Yolo",
                    PlayerName2 = "April O'Neil",
                },
                3 => new TestDataSet
                {
                    ItemName1 = "Ruby Tide Bracelets of Fending",
                    ItemName2 = "Wind-up Aldgoat",
                    PlayerName1 = "Wyatt Earp",
                    PlayerName2 = "April O'Neil",
                },
                4 => new TestDataSet
                {
                    ItemName1 = "延夏学士指饰", ItemName2 = "改良型田园监督者耳坠", PlayerName1 = "望舒", PlayerName2 = "语嫣",
                },
                _ => testDataSet,
            };

            if (testDataSet == null) return;

            // add loot
            var event1 = new LootEvent
            {
                LootEventType = LootEventType.Add,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Add) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName1,
                },
                ItemName = testDataSet.ItemName1,
                ItemNameAbbreviated = testDataSet.ItemName1,
            };
            plugin.RollMonitor.ProcessRoll(event1);
            plugin.LootEvents.Add(event1);

            // add again
            var event2 = new LootEvent
            {
                LootEventType = LootEventType.Add,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Add) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName2,
                },
                ItemName = testDataSet.ItemName2,
                ItemNameAbbreviated = testDataSet.ItemName2,
            };
            plugin.RollMonitor.ProcessRoll(event2);
            plugin.LootEvents.Add(event2);

            // cast
            var event3 = new LootEvent
            {
                LootEventType = LootEventType.Cast,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Cast) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName1,
                },
                ItemName = testDataSet.ItemName1,
                ItemNameAbbreviated = testDataSet.ItemName1,
                PlayerName = testDataSet.PlayerName1,
            };
            plugin.RollMonitor.ProcessRoll(event3);
            plugin.LootEvents.Add(event3);

            // cast again
            var event4 = new LootEvent
            {
                LootEventType = LootEventType.Cast,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Cast) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName1,
                },
                ItemName = testDataSet.ItemName1,
                ItemNameAbbreviated = testDataSet.ItemName1,
                PlayerName = testDataSet.PlayerName2,
            };
            plugin.RollMonitor.ProcessRoll(event4);
            plugin.LootEvents.Add(event4);

            // need roll
            var event5 = new LootEvent
            {
                LootEventType = LootEventType.Need,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Need) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName1,
                },
                ItemName = testDataSet.ItemName1,
                ItemNameAbbreviated = testDataSet.ItemName1,
                PlayerName = testDataSet.PlayerName2,
                Roll = 45,
            };
            plugin.RollMonitor.ProcessRoll(event5);
            plugin.LootEvents.Add(event5);

            // need roll again
            var event6 = new LootEvent
            {
                LootEventType = LootEventType.Need,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Need) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName1,
                },
                ItemName = testDataSet.ItemName1,
                ItemNameAbbreviated = testDataSet.ItemName1,
                PlayerName = testDataSet.PlayerName1,
                Roll = 57,
            };
            plugin.RollMonitor.ProcessRoll(event6);
            plugin.LootEvents.Add(event6);

            // obtain
            var event7 = new LootEvent
            {
                LootEventType = LootEventType.Obtain,
                LootEventTypeName = Enum.GetName(typeof(LootEventType), LootEventType.Obtain) ?? string.Empty,
                Timestamp = UnixTimestampHelper.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = testDataSet.ItemName1,
                },
                ItemName = testDataSet.ItemName1,
                ItemNameAbbreviated = testDataSet.ItemName1,
                PlayerName = testDataSet.PlayerName1,
            };
            plugin.RollMonitor.ProcessRoll(event7);
            plugin.LootEvents.Add(event7);
        }

        private class TestDataSet
        {
            public string ItemName1 = null!;
            public string ItemName2 = null!;
            public string PlayerName1 = null!;
            public string PlayerName2 = null!;
        }
    }
}
