// ReSharper disable NotAccessedField.Local
// ReSharper disable StringLiteralTypo

using System.Collections.Generic;
using Xunit;

namespace Kapture.Test
{
    public class DELootProcessorTest
    {
        public DELootProcessorTest()
        {
            _kapturePlugin = new KapturePluginMock(2);
            _lootMessage = new LootMessage
            {
                ItemName = "ItemName"
            };
        }

        private LootMessage _lootMessage;

        private KapturePluginMock _kapturePlugin;

        [Fact]
        public void System_Search_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Der ", "", " Salzwasser-Universalköder", " wird gesucht."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Search, result.LootEventType);
        }

        [Fact]
        public void System_Add_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Ihr habt Beutegut (eine ", "", " geplünderte Schutzbrille"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Add, result.LootEventType);
        }

        [Fact]
        public void System_Lost_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Du konntest den ", "", " geplünderten Bindegürtel", " nicht erhalten."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Lost, result.LootEventType);
        }

        [Fact]
        public void System_Purchase_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast eine ", "", " Notenrolle von „Below“", " gekauft."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Purchase, result.LootEventType);

            _lootMessage.MessageParts = new List<string>
            {
                "Du hast eine ", "", " Flasche Mineralwasser", " für 4 Gil gekauft."
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Purchase, result.LootEventType);
        }

        [Fact]
        public void System_Discard_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Du wirfst eine ", "", " abgenutzte Notenrolle von „Riptide“", " weg."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Discard, result.LootEventType);
        }

        [Fact]
        public void System_Obtain_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast ein ", "", " Paar Akolythen-Trippen", " verwertet!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);
        }

        [Fact]
        public void System_Obtain_ReceiveFromMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast die ", "", " Fechtkunst-Materia VIII", " zurückgewonnen!"
            };

            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);
        }

        [Fact]
        public void System_Lost_RetrieveMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "Die ", "", " Fechtkunst-Materia VIII", " zerfällt zu wertlosem Staub", "..."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Lost, result.LootEventType);
        }

        [Fact]
        public void LocalPlayerObtainLoot_Obtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast eine ", "", " geplünderte Schutzbrille", " erhalten."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result.IsLocalPlayer);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);
        }


        [Fact]
        public void LocalPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Du würfelst um das ", "", " Paar geplünderte Ohrringe", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result.IsLocalPlayer);
            Assert.Equal(LootEventType.Cast, result.LootEventType);
        }

        [Fact]
        public void LocalPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Du würfelst mit „Bedarf“ eine 14 auf den ", "", " Erbstück-Ohrring der Heilung", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result.IsLocalPlayer);
            Assert.Equal(14, result.Roll);
            Assert.Equal(LootEventType.Need, result.LootEventType);
        }

        [Fact]
        public void LocalPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Du würfelst mit „Gier“ eine 62 auf das ", "", " Paar Erbstück-Kniestiefel des Spähens", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.True(result.IsLocalPlayer);
            Assert.Equal(62, result.Roll);
            Assert.Equal(LootEventType.Greed, result.LootEventType);
        }


        [Fact]
        public void OtherPlayerObtainLoot_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " hat ein ", "", " Paar Vollstrecker-Leggins", " erhalten."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result.IsLocalPlayer);
            Assert.Equal("Hans Yolo", result.PlayerName);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);

            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " hat eine ", "", " Flasche Grottenwasser", " erhalten."
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result.IsLocalPlayer);
            Assert.Equal("Hans Yolo", result.PlayerName);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " würfelt um das ", "", " Paar Akolythen-Chausses", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result.IsLocalPlayer);
            Assert.Equal("Hans Yolo", result.PlayerName);
            Assert.Equal(LootEventType.Cast, result.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " würfelt mit „Bedarf“ eine 54 auf das ", "", " Paar geplünderte Ohrringe", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result.IsLocalPlayer);
            Assert.Equal("Hans Yolo", result.PlayerName);
            Assert.Equal(54, result.Roll);
            Assert.Equal(LootEventType.Need, result.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " würfelt mit „Gier“ eine 14 auf den ", "", " Erbstück-Ring des Zielens", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result.IsLocalPlayer);
            Assert.Equal("Hans Yolo", result.PlayerName);
            Assert.Equal(14, result.Roll);
            Assert.Equal(LootEventType.Greed, result.LootEventType);
        }

        [Fact]
        public void OtherPlayerRoll_Greed_OtherWorld_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", "Lich würfelt mit „Gier“ eine 88 auf das ", "", " Paar Vollstrecker-Leggins", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.False(result.IsLocalPlayer);
            Assert.Equal("Hans Yolo", result.PlayerName);
            Assert.Equal(88, result.Roll);
            Assert.Equal(LootEventType.Greed, result.LootEventType);
        }

        [Fact]
        public void AddDesynthSell_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast einen ", "", " Sternenglobus des Lichts", " verwertet!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Desynth, result.LootEventType);
        }

        [Fact]
        public void AddDesynthSell_Orchestration_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "Die ", "", " Notenrolle von „Below“", " wurde deinem Orchestrion hinzugefügt."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result.LootEventType);
        }

        [Fact]
        public void AddDesynthSell_Sell_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast eine ", "", " Flasche Mineralwasser", " für 1 Gil verkauft."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Sell, result.LootEventType);
        }

        [Fact]
        public void LocalPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "Du verwendest einen ", "", " Kaffeekeks ", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result.LootEventType);
        }

        [Fact]
        public void LocalPlayerSpecialObtain_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerSpecialObtain;
            _lootMessage.MessageParts = new List<string>
            {
                "Du verwendest eine ", "", " Kiste mit legerer Kleidung", "."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);

            _lootMessage.MessageParts = new List<string>
            {
                "Eine ", "", " legere Jacke", " wurde deinem Inventar hinzugefügt."
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);

            _lootMessage.MessageParts = new List<string>
            {
                "Ein ", "", " Paar legerer Schuhe", " wurde deinem Inventar hinzugefügt."
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);
        }

        [Fact]
        public void OtherPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " macht die ", "", " Edens Verheißung-Karte", " bereit."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result.LootEventType);
        }

        [Fact]
        public void FastCraft_AttachMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast die ", "", " Fechtkunst-Materia VIII", " in das ", "", " Paar Edenmorgen-Stiefel der Magie",
                " eingesetzt!"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Use, result.LootEventType);
        }

        [Fact]
        public void FastCraft_ExtractMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast die Materia von dem ", "", " Paar Edenmorgen-Stiefel der Magie", " entfernt."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Obtain, result.LootEventType);
        }

        [Fact]
        public void FastCraft_QuickSynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast erfolgreich ein ", "", " Mythrit-Nugget", " hergestellt."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Craft, result.LootEventType);
        }

        [Fact]
        public void Gather_MinBtn_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast einen ", "", " Mythrit-Sandklumpen ", " erhalten."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Gather, result.LootEventType);
        }

        [Fact]
        public void Gather_Fsh_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast eine ", "", " limsische Sardelle", " (6,1 Ilme) gefangen."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Gather, result.LootEventType);
        }

        [Fact]
        public void LocalPlayerSynthesize_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerSynthesize;
            _lootMessage.MessageParts = new List<string>
            {
                "Du hast erfolgreich eine ", "", " Kiste Mythrit-Nieten ", " hergestellt."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Craft, result.LootEventType);
        }

        [Fact]
        public void OtherPlayerSynthesize_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerSynthesize;
            _lootMessage.MessageParts = new List<string>
            {
                "Hans Yolo", " hat erfolgreich einen ", "", " ästhetischen Vorschlaghammer ", " hergestellt."
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.NotNull(result);
            Assert.Equal(LootEventType.Craft, result.LootEventType);
        }
    }
}
