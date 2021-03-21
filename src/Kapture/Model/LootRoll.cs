using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;

namespace Kapture
{
    public class LootRoll
    {
        public long Timestamp { get; set; }
        public uint ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemNameAbbreviated { get; set; } = string.Empty;
        public List<LootRoller> Rollers { get; } = new List<LootRoller>();
        public List<KeyValuePair<string, Vector4>> RollersDisplay { get; set; } = new List<KeyValuePair<string, Vector4>>();
        public uint RollerCount { get; set; }
        public string Winner { get; set; } = string.Empty;
        public bool IsWon { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}