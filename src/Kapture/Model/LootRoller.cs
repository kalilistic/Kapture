namespace Kapture
{
    public class LootRoller
    {
        public string PlayerName { get; set; } = string.Empty;
        public ushort Roll { get; set; }
        public bool IsWinner { get; set; }
    }
}