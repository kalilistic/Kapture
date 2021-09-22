using System.Numerics;

namespace Kapture
{
    /// <summary>
    /// Loot roller.
    /// </summary>
    public class LootRoller
    {
        /// <summary>
        /// Gets player name.
        /// </summary>
        public string PlayerName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets player name.
        /// </summary>
        public string FormattedPlayerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets roll color.
        /// </summary>
        public Vector4 RollColor { get; set; }

        /// <summary>
        /// Gets or sets roll.
        /// </summary>
        public ushort Roll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether they've casted lot yet.
        /// </summary>
        public bool HasRolled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is winner.
        /// </summary>
        public bool IsWinner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether alert roll has been made.
        /// </summary>
        public bool IsReminderSent { get; set; }

        /// <summary>
        /// Gets a value indicating whether is local player.
        /// </summary>
        public bool IsLocalPlayer { get; init; }
    }
}
