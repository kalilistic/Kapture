using System.Collections.Generic;
using Newtonsoft.Json;

namespace KapturePlugin
{
    public class LootRoll
    {
        public long Timestamp { get; set; }
        public uint ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public List<string> Rollers { get; } = new List<string>();
        public string RollersDisplay { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}