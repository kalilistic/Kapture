using System.Collections.Generic;

using Newtonsoft.Json;

namespace Kapture
{
    /// <summary>
    /// Loot roll.
    /// </summary>
    public class LootRoll
    {
        /// <summary>
        /// Gets or sets time stamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets item Id.
        /// </summary>
        public uint ItemId { get; init; }

        /// <summary>
        /// Gets item name.
        /// </summary>
        public string ItemName { get; init; } = string.Empty;

        /// <summary>
        /// Gets item name abbreviated.
        /// </summary>
        public string ItemNameAbbreviated { get; init; } = string.Empty;

        /// <summary>
        /// Gets list of rollers.
        /// </summary>
        public List<LootRoller> Rollers { get; } = new();

        /// <summary>
        /// Gets or sets roller count.
        /// </summary>
        public uint RollerCount { get; set; }

        /// <summary>
        /// Gets or sets winner.
        /// </summary>
        public string Winner { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets winner status.
        /// </summary>
        public bool IsWon { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
