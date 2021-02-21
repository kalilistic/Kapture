using System;
using Dalamud.Configuration;

namespace Sample
{
    [Serializable]
    public class PluginConfig : SampleConfig, IPluginConfiguration
    {
        public int Version { get; set; } = 0;
    }
}