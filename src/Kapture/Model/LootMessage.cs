// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;

namespace KapturePlugin
{
    public class LootMessage
    {
        public uint XivChatType { get; set; }
        public LogKind LogKind { get; set; }
        public string LogKindName { get; set; }
        public LootMessageType LootMessageType { get; set; }
        public string LootMessageTypeName { get; set; }
        public string Message { get; set; }

        public List<string> MessageParts { get; set; } = new List<string>();

        public uint ItemId { get; set; }

        public string ItemName { get; set; }

        [JsonIgnore] public Item Item { get; set; }

        public bool IsHq { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}