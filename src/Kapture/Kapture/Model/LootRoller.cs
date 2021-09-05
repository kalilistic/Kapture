namespace Kapture
{
    /// <summary>
    /// Loot roller.
    /// </summary>
    public class LootRoller
    {
        /// <summary>
        /// Gets or sets player name.
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets roll.
        /// </summary>
        public ushort Roll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is winner.
        /// </summary>
        public bool IsWinner { get; set; }
    }
}
