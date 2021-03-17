using System;
using Dalamud.Configuration;

namespace Kapture
{
    [Serializable]
    public class PluginConfig : KaptureConfig, IPluginConfiguration
    {
        public int Version { get; set; } = 0;
    }
}