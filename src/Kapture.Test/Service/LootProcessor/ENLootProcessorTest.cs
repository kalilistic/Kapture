// ReSharper disable NotAccessedField.Local

using System.Collections.Generic;
using Kapture.Mock;
using NUnit.Framework;

namespace Kapture.Test
{
    [TestFixture]
    public class ENLootProcessorTest
    {
        [SetUp]
        public void Setup()
        {
            _kapturePlugin = new MockKapturePlugin();
            _lootMessage = new LootMessage
            {
                ItemName = "ItemName"
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private LootMessage _lootMessage;

        private MockKapturePlugin _kapturePlugin;

        [Test]
        public void System_Search_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Searching for ", "", "copies of the ", "Book of Eternity", "..."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Search, result.LootEventType);
        }

        [Test]
        public void System_Add_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "A ", "", "shadowless sash of fending", " has been added to the loot list."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Add, result.LootEventType);
        }

        [Test]
        public void System_Lost_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Unable to obtain the ", "", "wind-up onion knight", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Lost, result.LootEventType);
        }

        [Test]
        public void System_Purchase_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You purchase 4 ", "", "ocean cloud", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Purchase, result.LootEventType);
        }

        [Test]
        public void System_Discard_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You throw away a ", "", "soiled femur", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Discard, result.LootEventType);
        }

        [Test]
        public void System_Obtain_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain a ", "", "pair of shadowless earrings of casting", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void System_Obtain_ReceiveFromMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You receive a ", "", "quickarm materia VII", "."
            };

            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void System_Lost_RetrieveMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "The ", "", "savage might materia VIII", " shatters..."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Lost, result.LootEventType);
        }

        [Test]
        public void LocalPlayerObtainLoot_Obtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain a ", "", "pair of shadowless earrings of casting", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }


        [Test]
        public void LocalPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "You cast your lot for the ", "", "pair of shadowless hose of aiming", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(LootEventType.Cast, result.LootEventType);
        }

        [Test]
        public void LocalPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "You roll Need on the ", "", "shadowless bracelet of aiming", ". 87!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(87, result.Roll);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void LocalPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "You roll Greed on the ", "", "shadowless bracelet of aiming", ". 87!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(87, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }


        [Test]
        public void OtherPlayerObtainLoot_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", "Cactuar obtains a ", "", "piety materia VII", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", "Cactuar casts his lot for the ", "", "Allagan catalyst", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(LootEventType.Cast, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Need on the ", "", "shadowless necklace of casting", ". 77!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(77, result.Roll);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Greed on the ", "", "shadowless necklace of casting", ". 77!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(77, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Greed_OtherWorld_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", "Faerie rolls Greed on ", "", "shadowless necklace of casting", ". 77!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("Gary Oak", result.PlayerName);
            Assert.AreEqual(77, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void AddDesynthSell_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "You desynthesize a ", "", "pair of Scylla's culottes of casting", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Desynth, result.LootEventType);
        }

        [Test]
        public void AddDesynthSell_Orchestration_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "The ", "", "Blind to the Dark", " orchestrion roll", " is added to your orchestrion list."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void AddDesynthSell_Sell_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "You sell 7 ", "", "circles of sea swallow leather", " for 28 gil."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Sell, result.LootEventType);
        }

        [Test]
        public void LocalPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "You use a ", "", "potion of dexterity", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void LocalPlayerSpecialObtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerSpecialObtain;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain an ", "", "Edenchoir choker of aiming", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void OtherPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " uses an ", "", "Edenmorn hand gear coffer", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void FastCraft_AttachMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "You successfully attach a ", "", "quickarm materia VII", " to the ", "", "Edengrace ring of aiming",
                "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Use, result.LootEventType);
        }

        [Test]
        public void FastCraft_ExtractMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "You successfully extract a ", "", "piety materia VII", " from the ", "", "Edenmete ring of healing",
                "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void FastCraft_QuickSynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "You synthesize a ", "", "circle of leather", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void Gather_MinBtn_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain a ", "", "chunk of granite ", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Gather, result.LootEventType);
        }

        [Test]
        public void Gather_Fsh_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "You land a ", "", "dark sleeper ", " measuring 11.1 ilms!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Gather, result.LootEventType);
        }

        [Test]
        public void LocalPlayerSynthesize_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerSynthesize;
            _lootMessage.MessageParts = new List<string>
            {
                "You synthesize a ", "", "circle of leather", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void OtherPlayerSynthesize_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerSynthesize;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " synthesizes a ", "", "cobalt alloy ingot ", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void AllianceOtherPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " casts his lot for the ", "", "Antipyretic", " orchestrion roll", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Cast, result.LootEventType);
        }

        [Test]
        public void AllianceOtherPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Need on the ", "", "Antipyretic", " orchestrion roll", ". 40!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void AllianceOtherPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Greed on the ", "", "Antipyretic", " orchestrion roll", ". 40!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void AllianceOtherPlayerObtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerObtain;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " obtains an ", "", "Antipyretic", " orchestrion roll", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }
        
    }
}