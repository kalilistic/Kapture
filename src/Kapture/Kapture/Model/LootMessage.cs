using System.Collections.Generic;

using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Kapture
{
    /// <summary>
    /// Loot message.
    /// </summary>
    public class LootMessage
    {
        /// <summary>
        /// Gets or sets chat type.
        /// </summary>
        public uint XivChatType { get; set; }

        /// <summary>
        /// Gets or sets log kind.
        /// </summary>
        public LogKind LogKind { get; set; }

        /// <summary>
        /// Gets or sets log kind name.
        /// </summary>
        public string LogKindName { get; set; } = null!;

        /// <summary>
        /// Gets or sets loot message type.
        /// </summary>
        public LootMessageType LootMessageType { get; set; }

        /// <summary>
        /// Gets or sets loot message type name.
        /// </summary>
        public string LootMessageTypeName { get; set; } = null!;

        /// <summary>
        /// Gets or sets message as string.
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Gets or sets message parts from payload.
        /// </summary>
        public List<string> MessageParts { get; set; } = new ();

        /// <summary>
        /// Gets or sets itemId.
        /// </summary>
        public uint ItemId { get; set; }

        /// <summary>
        /// Gets or sets item name.
        /// </summary>
        public string ItemName { get; set; } = null!;

        /// <summary>
        /// Gets or sets item.
        /// </summary>
        [JsonIgnore]
        public Lumina.Excel.Sheets.Item Item { get; set; }

        /// <summary>
        /// Gets or sets playerPayload.
        /// </summary>
        [JsonIgnore]
        public PlayerPayload? Player { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hq.
        /// </summary>
        public bool IsHq { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
