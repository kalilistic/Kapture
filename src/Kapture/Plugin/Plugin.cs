// ReSharper disable UnusedMember.Global
// ReSharper disable DelegateSubtraction

using System;
using Dalamud.Plugin;

namespace Kapture
{
    public class Plugin : IDalamudPlugin
    {
        private KapturePlugin _kapturePlugin;

        public string Name => "Kapture";

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _kapturePlugin = new KapturePlugin(Name, pluginInterface);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _kapturePlugin.Dispose();
        }
    }
}