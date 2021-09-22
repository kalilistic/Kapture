namespace Kapture
{
    /// <summary>
    /// Item list.
    /// </summary>
    public class ItemList
    {
        /// <summary>
        /// Gets item ids.
        /// </summary>
        public uint[] ItemIds { get; init; } = null!;

        /// <summary>
        /// Gets item names.
        /// </summary>
        public string[] ItemNames { get; init; } = null!;
    }
}
