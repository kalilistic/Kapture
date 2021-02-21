using NUnit.Framework;
using Sample.Mock;

// ReSharper disable NotAccessedField.Local

namespace Sample.Test
{
    [TestFixture]
    public class SampleServiceTest
    {
        [SetUp]
        public void Setup()
        {
            _samplePlugin = new MockSamplePlugin();
            _sampleService = new SampleService(_samplePlugin);
        }

        [TearDown]
        public void TearDown()
        {
        }

        private SampleService _sampleService;
        private MockSamplePlugin _samplePlugin;

        [Test]
        public void SampleTest()
        {
            Assert.IsTrue(true);
        }
    }
}