using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kapture
{
    public class LootEvent
    {
        public long Timestamp { get; set; }
        public LootMessage LootMessage { get; set; }
        public LootEventType LootEventType { get; set; }
        public string LootEventTypeName { get; set; } = string.Empty;
        public bool IsLocalPlayer { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string PlayerDisplayName { get; set; } = string.Empty;
        public ushort Roll { get; set; }
        public uint TerritoryTypeId { get; set; }
        public uint ContentId { get; set; }
        public Guid LootEventId { get; set; }
        public string ItemDisplayName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static string GetCsvHeadings()
        {
            return string.Join(",", new List<string>
            {
                "LootEventId",
                "Timestamp",
                "TerritoryTypeId",
                "ContentId",
                "LootEventTypeName",
                "ItemId",
                "ItemName",
                "IsHQ",
                "PlayerName",
                "Roll"
            });
        }

        public string ToCsv()
        {
            return string.Join(",", new List<string>
            {
                LootEventId.ToString(),
                Timestamp.ToString(),
                TerritoryTypeId.ToString(),
                ContentId.ToString(),
                LootEventTypeName,
                LootMessage.ItemId.ToString(),
                LootMessage.ItemName,
                LootMessage.IsHq.ToString(),
                PlayerName,
                Roll.ToString()
            });
        }
    }
}