using System;
using System.Linq;
using System.Threading;
using Kapture.Mock;
using NUnit.Framework;

// ReSharper disable NotAccessedField.Local

namespace Kapture.Test
{
    [TestFixture]
    public class RollMonitorTest
    {
        [SetUp]
        public void Setup()
        {
            _kapturePlugin = new MockKapturePlugin();
        }

        [TearDown]
        public void TearDown()
        {
            _kapturePlugin.LootRolls.Clear();
            _kapturePlugin.LootRollsDisplay.Clear();
        }

        private MockKapturePlugin _kapturePlugin;

        private LootEvent BuildTestEvent()
        {
            return new LootEvent
            {
                Timestamp = DateUtil.CurrentTime(),
                LootEventId = Guid.NewGuid(),
                TerritoryTypeId = 1,
                ContentId = 1,
                LootMessage = new LootMessage
                {
                    ItemId = 1,
                    ItemName = "Pokeball"
                }
            };
        }

        [Test]
        public void AddTest()
        {
            var lootEvent = BuildTestEvent();
            lootEvent.LootEventType = LootEventType.Add;
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent);
            Assert.AreEqual(1, _kapturePlugin.LootRolls.Count);
        }

        [Test]
        public void RollTest()
        {
            // add item
            var lootEvent1 = BuildTestEvent();
            lootEvent1.LootEventType = LootEventType.Add;
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent1);

            // add roll 1
            var lootEvent2 = BuildTestEvent();
            lootEvent2.LootEventType = LootEventType.Cast;
            lootEvent2.PlayerName = "Player One";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent2);

            // add roll 2
            var lootEvent3 = BuildTestEvent();
            lootEvent3.LootEventType = LootEventType.Cast;
            lootEvent3.PlayerName = "Player Two";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent3);

            Assert.AreEqual(1, _kapturePlugin.LootRolls.Count);
        }

        [Test]
        public void ObtainTest()
        {
            // add item
            var lootEvent1 = BuildTestEvent();
            lootEvent1.LootEventType = LootEventType.Add;
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent1);

            // add roll 1
            var lootEvent2 = BuildTestEvent();
            lootEvent2.LootEventType = LootEventType.Cast;
            lootEvent2.PlayerName = "Player One";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent2);

            // add roll 2
            var lootEvent3 = BuildTestEvent();
            lootEvent3.LootEventType = LootEventType.Cast;
            lootEvent3.PlayerName = "Player Two";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent3);

            // add obtained
            var lootEvent4 = BuildTestEvent();
            lootEvent4.LootEventType = LootEventType.Obtain;
            lootEvent4.PlayerName = "Player Two";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent4);

            Assert.AreEqual(1, _kapturePlugin.LootRolls.Count);
            Assert.AreEqual("Player Two", _kapturePlugin.LootRolls.First().Winner);
        }

        [Test]
        public void LostTest()
        {
            // add item
            var lootEvent1 = BuildTestEvent();
            lootEvent1.LootEventType = LootEventType.Add;
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent1);

            // add roll 1
            var lootEvent2 = BuildTestEvent();
            lootEvent2.LootEventType = LootEventType.Cast;
            lootEvent2.PlayerName = "Player One";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent2);

            // add roll 2
            var lootEvent3 = BuildTestEvent();
            lootEvent3.LootEventType = LootEventType.Cast;
            lootEvent3.PlayerName = "Player Two";
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent3);

            // add lost
            var lootEvent4 = BuildTestEvent();
            lootEvent4.LootEventType = LootEventType.Lost;
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent4);

            Assert.AreEqual(1, _kapturePlugin.LootRolls.Count);
            Assert.AreEqual("#RollMonitorLost", _kapturePlugin.LootRolls.First().Winner);
        }

        [Test]
        public void TimeoutTest()
        {
            // set timeout
            _kapturePlugin.Configuration.RollMonitorAddedTimeout = 1;

            // add item
            var lootEvent1 = BuildTestEvent();
            lootEvent1.LootEventType = LootEventType.Add;
            _kapturePlugin.RollMonitor.ProcessRoll(lootEvent1);

            // try to remove roll
            Thread.Sleep(1000);
            _kapturePlugin.RollMonitor.UpdateRolls();

            Assert.AreEqual(0, _kapturePlugin.LootRolls.Count);
        }
    }
}