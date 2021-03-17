// ReSharper disable NotAccessedField.Local

using System.Collections.Generic;
using Kapture.Mock;
using NUnit.Framework;

namespace Kapture.Test
{
    [TestFixture]
    public class KaptureServiceTest
    {
        [SetUp]
        public void Setup()
        {
            _kapturePlugin = new MockKapturePlugin();
        }

        [TearDown]
        public void TearDown()
        {
        }

        private MockKapturePlugin _kapturePlugin;

        [Test]
        public void ProcessSystem_Searched_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "Searching for ", "", "copies of the ", "Book of Eternity", "..."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Search, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_Added_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "A ", "", "shadowless sash of fending", " has been added to the loot list."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Add, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_Lost_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "Unable to obtain the ", "", "wind-up onion knight", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Lost, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_Purchased_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "You purchase 4 ", "", "ocean cloud", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Purchase, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_Discarded_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "You throw away a ", "", "soiled femur", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Discard, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_ObtainedFromDesynth_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "You obtain a ", "", "pair of shadowless earrings of casting", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_ObtainedFromMateria_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "You receive a ", "", "quickarm materia VII", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void ProcessSystem_LostMateria_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.System,
                MessageParts = new List<string>
                {
                    "The ", "", "savage might materia VIII", " shatters..."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Lost, result.LootEventType);
        }

        [Test]
        public void ProcessLocalPlayerObtainLoot_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerObtainLoot,
                MessageParts = new List<string>
                {
                    "You obtain a ", "", "pair of shadowless earrings of casting", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }


        [Test]
        public void ProcessLocalPlayerRoll_Cast_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerRoll,
                MessageParts = new List<string>
                {
                    "You cast your lot for the ", "", "pair of shadowless hose of aiming", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(LootEventType.Cast, result.LootEventType);
        }

        [Test]
        public void ProcessLocalPlayerRoll_Need_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerRoll,
                MessageParts = new List<string>
                {
                    "You roll Need on the ", "", "shadowless bracelet of aiming", ". 87!"
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(87, result.Roll);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void ProcessLocalPlayerRoll_Greed_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerRoll,
                MessageParts = new List<string>
                {
                    "You roll Greed on the ", "", "shadowless bracelet of aiming", ". 87!"
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(87, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }


        [Test]
        public void ProcessOtherPlayerObtainLoot_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerObtainLoot,
                MessageParts = new List<string>
                {
                    "Gary Oak", "Cactuar obtains a ", "", "piety materia VII", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void ProcessOtherPlayerRoll_Cast_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerRoll,
                MessageParts = new List<string>
                {
                    "Gary Oak", "Cactuar casts his lot for the ", "", "Allagan catalyst", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(LootEventType.Cast, result.LootEventType);
        }

        [Test]
        public void ProcessOtherPlayerRoll_Need_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerRoll,
                MessageParts = new List<string>
                {
                    "Gary Oak", " rolls Need on the ", "", "shadowless necklace of casting", ". 77!"
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(77, result.Roll);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void ProcessOtherPlayerRoll_Greed_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerRoll,
                MessageParts = new List<string>
                {
                    "Gary Oak", " rolls Greed on the ", "", "shadowless necklace of casting", ". 77!"
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(77, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void ProcessOtherPlayerRoll_GreedOtherWorld_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerRoll,
                MessageParts = new List<string>
                {
                    "Gary Oak", "Faerie rolls Greed on ", "", "shadowless necklace of casting", ". 77!"
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(77, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void ProcessAddDesynthSell_Desynth_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.AddDesynthSell,
                MessageParts = new List<string>
                {
                    "You desynthesize a ", "", "pair of Scylla's culottes of casting", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Desynth, result.LootEventType);
        }

        [Test]
        public void ProcessAddDesynthSell_Orchestration_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.AddDesynthSell,
                MessageParts = new List<string>
                {
                    "The ", "", "Blind to the Dark", " orchestrion roll", " is added to your orchestrion list."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void ProcessAddDesynthSell_Sell_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.AddDesynthSell,
                MessageParts = new List<string>
                {
                    "You sell 7 ", "", "circles of sea swallow leather", " for 28 gil."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Sell, result.LootEventType);
        }

        [Test]
        public void ProcessLocalPlayerUse_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerUse,
                MessageParts = new List<string>
                {
                    "You use a ", "", "potion of dexterity", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void ProcessLocalPlayerSpecialObtain_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerSpecialObtain,
                MessageParts = new List<string>
                {
                    "You obtain an ", "", "Edenchoir choker of aiming", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void ProcessOtherPlayerUse_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerUse,
                MessageParts = new List<string>
                {
                    "Gary Oak", " uses an ", "", "Edenmorn hand gear coffer", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void ProcessFastCraft_UseMateria_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.FastCraft,
                MessageParts = new List<string>
                {
                    "You successfully attach a ", "", "quickarm materia VII", " to the ", "", "Edengrace ring of aiming",
                    "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void ProcessFastCraft_ExtractMateria_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.FastCraft,
                MessageParts = new List<string>
                {
                    "You successfully extract a ", "", "piety materia VII", " from the ", "", "Edenmete ring of healing",
                    "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void ProcessFastCraft_QuickSynth_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.FastCraft,
                MessageParts = new List<string>
                {
                    "You synthesize a ", "", "circle of leather", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void ProcessGather_MIN_BTN_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.Gather,
                MessageParts = new List<string>
                {
                    "You obtain a ", "", "chunk of granite ", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Gather, result.LootEventType);
        }

        [Test]
        public void ProcessGather_FSH_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.Gather,
                MessageParts = new List<string>
                {
                    "You land a ", "", "dark sleeper ", " measuring 11.1 ilms!"
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Gather, result.LootEventType);
        }

        [Test]
        public void ProcessLocalPlayerSynthesize_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.LocalPlayerSynthesize,
                MessageParts = new List<string>
                {
                    "You synthesize a ", "", "circle of leather", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void ProcessOtherPlayerSynthesize_Test()
        {
            var message = new LootMessage
            {
                LootMessageType = LootMessageType.OtherPlayerSynthesize,
                MessageParts = new List<string>
                {
                    "Gary Oak", " synthesizes a ", "", "cobalt alloy ingot ", "."
                }
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(message);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }
    }
}