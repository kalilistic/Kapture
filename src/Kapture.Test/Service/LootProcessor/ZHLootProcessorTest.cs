// ReSharper disable NotAccessedField.Local

using System.Collections.Generic;
using Kapture.Mock;
using NUnit.Framework;

namespace Kapture.Test
{
    [TestFixture]
    public class ZHLootProcessorTest
    {
        private LootMessage _lootMessage;
        [SetUp]
        public void Setup()
        {
            _kapturePlugin = new MockKapturePlugin(4);
            _lootMessage = new LootMessage
            {
                ItemName = "ItemName",
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private MockKapturePlugin _kapturePlugin;

        [Test]
        public void System_Search_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "正在确认“","","传送网使用券","”的持有数量。"
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
                "获得了新的战利品","","延夏术士护胫","。"
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
                "无法获得“","","火神书","”。"
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
                "从市场购买了“","","管弦乐琴乐谱：水车低鸣","”×1。"
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
                "舍弃了“","","皇金锭","” ×1。"
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
                "   获得了","","皇金锭","。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
            
            _lootMessage.MessageParts = new List<string>
            {
                "   获得了","","风之晶簇","×2。"
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void System_Obtain_ReceiveFromMateria_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.System;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠从","","联盟御敌项链","上取下了魔晶石。"
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
                "","神眼魔晶石捌型","化成了粉末……"
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
                "风眠获得了“","","火神法杖","”。"
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
                "风眠对","","延夏术士护胫","掷骰。"
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
                "风眠在需求条件下对“","","延夏豪士佩楯","”掷出了22点。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(22, result.Roll);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void LocalPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.LocalPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠在贪婪条件下对“","","延夏学士指饰","”掷出了39点。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsLocalPlayer);
            Assert.AreEqual(39, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }


        [Test]
        public void OtherPlayerObtainLoot_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerObtainLoot;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠","红玉海获得了“","","延夏术士手饰","”。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("风眠", result.PlayerName);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Cast_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠","萌芽池对","","延夏斗士首饰","掷骰。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("风眠", result.PlayerName);
            Assert.AreEqual(LootEventType.Cast, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Need_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠","红玉海在需求条件下对“","","延夏术士护胫","”掷出了50点。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("风眠", result.PlayerName);
            Assert.AreEqual(50, result.Roll);
            Assert.AreEqual(LootEventType.Need, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Greed_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠","萌芽池在贪婪条件下对“","","延夏斗士首饰","”掷出了84点。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("风眠", result.PlayerName);
            Assert.AreEqual(84, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void OtherPlayerRoll_Greed_OtherWorld_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerRoll;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠","红玉海在贪婪条件下对“","","延夏学士指饰","”掷出了97点。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsLocalPlayer);
            Assert.AreEqual("风眠", result.PlayerName);
            Assert.AreEqual(97, result.Roll);
            Assert.AreEqual(LootEventType.Greed, result.LootEventType);
        }

        [Test]
        public void AddDesynthSell_Desynth_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.AddDesynthSell;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠成功分解了","","煽动兵精准盔甲戒指","！"
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
                "将«","","管弦乐琴乐谱：水车低鸣","»收录进了管弦乐琴乐谱集之中。"
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
                "以2,500金币的价格卖出了“","","古代银币","”×5。"
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
                "风眠使用了“","","甘菊茶","”。"
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
                "风眠使用了“","","田园监督者饰品箱","”。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
            
            _lootMessage.MessageParts = new List<string>
            {
                "风眠获得了“","","改良型田园监督者耳坠","”。"
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
            
            _lootMessage.MessageParts = new List<string>
            {
                "将","","改良型田园监督者耳坠","放回到了背包中。"
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Obtain, result.LootEventType);
        }

        [Test]
        public void OtherPlayerUse_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.OtherPlayerUse;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠","使用了“","","骑士面包","”。"
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
                "风眠将","","刚力魔晶石陆型","镶嵌到了","","寄叶五三式强袭军装","上！"
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
                "风眠用","","伊甸之恩御敌战盔","进行了精制魔晶石！\n获得了","","刚柔魔晶石柒型","。"
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
                "风眠开始制作“","","愈疮木木材","”。"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
            
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠制作“","","愈疮木木材","”成功！"
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
            
            _lootMessage.LootMessageType = LootMessageType.FastCraft;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠制作“","","愈疮木木材","”成功！"
            };
            result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void Gather_MinBtn_Test()
        {
            _lootMessage.LootMessageType = LootMessageType.Gather;
            _lootMessage.MessageParts = new List<string>
            {
                "风眠获得了“","","陈旧的缠尾蛟革地图","”。"
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
                "风眠成功钓上了","","罗敏萨鳀鱼","（6.2星寸）。"
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
                "风眠制作“","","愈疮木木材","”成功！"
            };
            var result = _kapturePlugin.LootProcessor.ProcessLoot(_lootMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(LootEventType.Craft, result.LootEventType);
        }

        [Test]
        public void OtherPlayerSynthesize_Test()
        {
            Assert.IsTrue(true);
        }
    }
}