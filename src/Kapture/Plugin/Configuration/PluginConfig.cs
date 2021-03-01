using System;
using Dalamud.Configuration;

namespace KapturePlugin
{
    [Serializable]
    public class PluginConfig : KaptureConfig, IPluginConfiguration
    {
        public int Version { get; set; } = 0;
    }
}