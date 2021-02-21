using System.Diagnostics.CodeAnalysis;

namespace Sample
{
    public class SampleService : ISampleService
    {
        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private readonly ISamplePlugin _samplePlugin;

        public SampleService(ISamplePlugin samplePlugin)
        {
            _samplePlugin = samplePlugin;
        }
    }
}