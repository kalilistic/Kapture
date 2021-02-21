// ReSharper disable UnusedMember.Global
// ReSharper disable DelegateSubtraction

using System;
using Dalamud.Plugin;

namespace Sample
{
    public class Plugin : IDalamudPlugin
    {
        private SamplePlugin _samplePlugin;

        public string Name => "Sample";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _samplePlugin = new SamplePlugin(Name, pluginInterface);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _samplePlugin.Dispose();
        }
    }
}