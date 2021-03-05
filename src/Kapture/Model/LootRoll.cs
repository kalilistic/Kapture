using System.Collections.Generic;
using Newtonsoft.Json;

namespace KapturePlugin
{
    public class LootRoll
    {
        public long Timestamp { get; set; }
        public uint ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public List<LootRoller> Rollers { get; } = new List<LootRoller>();
        public string RollersDisplay { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;
        public bool IsWon { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}