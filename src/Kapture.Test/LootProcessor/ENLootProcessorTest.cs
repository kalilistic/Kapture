using System.Collections.Generic;
using Xunit;
#pragma warning disable 1591

namespace Kapture.Test
{
    public class ENLootProcessorTest
    {
        public ENLootProcessorTest()
        {
            _kapturePlugin = new KapturePluginMock();
            _lootMessage = new LootMessage
            {
                ItemName = "ItemName",
            };
        }

        private readonly LootMessage _lootMessage;

        private readonly KapturePluginMock _kapturePlugin;

        [Fact]
        public void System_Search_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Searching for ", "", "copies of the ", "Book of Eternity", "..."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Search, result?.LootEventType);
        }

        [Fact]
        public void System_Add_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "A ", "", "shadowless sash of fending", " has been added to the loot list."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Add, result?.LootEventType);
        }

        [Fact]
        public void System_Lost_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Unable to obtain the ", "", "wind-up onion knight", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Lost, result?.LootEventType);
        }

        [Fact]
        public void System_Purchase_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You purchase 4 ", "", "ocean cloud", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Purchase, result?.LootEventType);
        }

        [Fact]
        public void System_Discard_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You throw away a ", "", "soiled femur", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Discard, result?.LootEventType);
        }

        [Fact]
        public void System_Obtain_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain a ", "", "pair of shadowless earrings of casting", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }

        [Fact]
        public void System_Obtain_ReceiveFromMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "You receive a ", "", "quickarm materia VII", "."
            };

            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }

        [Fact]
        public void System_Lost_RetrieveMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "The ", "", "savage might materia VIII", " shatters..."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Lost, result?.LootEventType);
        }

        [Fact]
        public void LocalPlayerObtainLoot_Obtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain a ", "", "pair of shadowless earrings of casting", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result?.IsLocalPlayer);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }


        [Fact]
        public void LocalPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "You cast your lot for the ", "", "pair of shadowless hose of aiming", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result?.IsLocalPlayer);
            Assert.Equal(LootEventType.Cast, result?.LootEventType);
        }

        [Fact]
        public void LocalPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "You roll Need on the ", "", "shadowless bracelet of aiming", ". 87!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result?.IsLocalPlayer);
            Assert.Equal(87, result?.Roll ?? 0);
            Assert.Equal(LootEventType.Need, result?.LootEventType);
        }

        [Fact]
        public void LocalPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "You roll Greed on the ", "", "shadowless bracelet of aiming", ". 87!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result?.IsLocalPlayer);
            Assert.Equal(87, result?.Roll ?? 0);
            Assert.Equal(LootEventType.Greed, result?.LootEventType);
        }


        [Fact]
        public void OtherPlayerObtainLoot_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", "Cactuar obtains a ", "", "piety materia VII", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result?.IsLocalPlayer);
            Assert.Equal("Gary Oak", result?.PlayerName);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", "Cactuar casts his lot for the ", "", "Allagan catalyst", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result?.IsLocalPlayer);
            Assert.Equal("Gary Oak", result?.PlayerName);
            Assert.Equal(LootEventType.Cast, result?.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Need on the ", "", "shadowless necklace of casting", ". 77!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result?.IsLocalPlayer);
            Assert.Equal("Gary Oak", result?.PlayerName);
            Assert.Equal(77, result?.Roll ?? 0);
            Assert.Equal(LootEventType.Need, result?.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Greed on the ", "", "shadowless necklace of casting", ". 77!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result?.IsLocalPlayer);
            Assert.Equal("Gary Oak", result?.PlayerName);
            Assert.Equal(77, result?.Roll ?? 0);
            Assert.Equal(LootEventType.Greed, result?.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Greed_OtherWorld_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", "Faerie rolls Greed on ", "", "shadowless necklace of casting", ". 77!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result?.IsLocalPlayer);
            Assert.Equal("Gary Oak", result?.PlayerName);
            Assert.Equal(77, result?.Roll ?? 0);
            Assert.Equal(LootEventType.Greed, result?.LootEventType);
        }

        [Fact]
        public void AddDesynthSell_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "You desynthesize a ", "", "pair of Scylla's culottes of casting", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Desynth, result?.LootEventType);
        }

        [Fact]
        public void AddDesynthSell_Orchestration_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "The ", "", "Blind to the Dark", " orchestrion roll", " is added to your orchestrion list."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result?.LootEventType);
        }

        [Fact]
        public void AddDesynthSell_Sell_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "You sell 7 ", "", "circles of sea swallow leather", " for 28 gil."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Sell, result?.LootEventType);
        }

        [Fact]
        public void LocalPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "You use a ", "", "potion of dexterity", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result?.LootEventType);
        }

        [Fact]
        public void LocalPlayerSpecialObtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerSpecialObtain;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain an ", "", "Edenchoir choker of aiming", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }

        [Fact]
        public void OtherPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " uses an ", "", "Edenmorn hand gear coffer", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result?.LootEventType);
        }

        [Fact]
        public void FastCraft_AttachMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "You successfully attach a ", "", "quickarm materia VII", " to the ", "", "Edengrace ring of aiming",
                "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result?.LootEventType);
        }

        [Fact]
        public void FastCraft_ExtractMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "You successfully extract a ", "", "piety materia VII", " from the ", "", "Edenmete ring of healing",
                "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }

        [Fact]
        public void FastCraft_QuickSynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "You synthesize a ", "", "circle of leather", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Craft, result?.LootEventType);
        }

        [Fact]
        public void Gather_MinBtn_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "You obtain a ", "", "chunk of granite ", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Gather, result?.LootEventType);
        }

        [Fact]
        public void Gather_Fsh_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "You land a ", "", "dark sleeper ", " measuring 11.1 ilms!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Gather, result?.LootEventType);
        }

        [Fact]
        public void LocalPlayerSynthesize_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerSynthesize;
            _lootMessage.MessageParts = new List<string>
            {
                "You synthesize a ", "", "circle of leather", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Craft, result?.LootEventType);
        }

        [Fact]
        public void OtherPlayerSynthesize_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerSynthesize;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " synthesizes a ", "", "cobalt alloy ingot ", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Craft, result?.LootEventType);
        }

        [Fact]
        public void AllianceOtherPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " casts his lot for the ", "", "Antipyretic", " orchestrion roll", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Cast, result?.LootEventType);
        }

        [Fact]
        public void AllianceOtherPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Need on the ", "", "Antipyretic", " orchestrion roll", ". 40!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Need, result?.LootEventType);
        }

        [Fact]
        public void AllianceOtherPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " rolls Greed on the ", "", "Antipyretic", " orchestrion roll", ". 40!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Greed, result?.LootEventType);
        }

        [Fact]
        public void AllianceOtherPlayerObtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AllianceOtherPlayerObtain;
            _lootMessage.MessageParts = new List<string>
            {
                "Gary Oak", " obtains an ", "", "Antipyretic", " orchestrion roll", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result?.LootEventType);
        }
        
    }
}
