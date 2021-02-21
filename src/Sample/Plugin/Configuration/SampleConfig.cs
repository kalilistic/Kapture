namespace Sample
{
    public abstract class SampleConfig
    {
        public bool FreshInstall { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public int PluginLanguage { get; set; } = 0;
        public bool ShowOverlay { get; set; } = true;
    }
}