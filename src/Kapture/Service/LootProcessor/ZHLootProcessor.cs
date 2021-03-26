using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kapture
{
    public class ZHLootProcessor : LootProcessor
    {
        private Regex _processLocalPlayerSynthesizeRegex2;
        private Regex _processOtherPlayerSynthesizeRegex2;

        public ZHLootProcessor(IKapturePlugin kapturePlugin) : base(kapturePlugin)
        {
        }

        protected override void CompileRegex()
        {
            ProcessSystemSearchRegex = BuildRegex(@"^正在确认“$");
            ProcessSystemAddedRegex = BuildRegex(@"^获得了新的战利品$");
            ProcessSystemLostRegex = BuildRegex(@"^无法获得“$");
            ProcessSystemPurchasedRegex = BuildRegex(@"(^从市场购买了“|的价格买入了“|的价格回购了“)$");
            ProcessSystemDiscardedRegex = BuildRegex(@"^舍弃了“$");
            ProcessSystemObtainedFromDesynthRegex = BuildRegex(@"   获得了$");
            ProcessSystemObtainedFromMateriaRegex = BuildRegex(@"成功回收了$");
            ProcessSystemLostMateriaRegex = BuildRegex(@"^化成了粉末……$");
            ProcessLocalPlayerRollCastRegex = BuildRegex(@"^掷骰。$");
            ProcessLocalPlayerRollNeedRegex = BuildRegex(@"在需求条件下对“$");
            ProcessLocalPlayerRollGreedRegex = BuildRegex(@"在贪婪条件下对“$");
            ProcessOtherPlayerRollCastRegex = BuildRegex(@"^掷骰。$");
            ProcessOtherPlayerRollNeedRegex = BuildRegex(@"在需求条件下对“$");
            ProcessOtherPlayerRollGreedRegex = BuildRegex(@"在贪婪条件下对“$");
            ProcessAddDesynthSellDesynthRegex = BuildRegex(@"成功分解了$");
            ProcessAddDesynthSellOrchestrationRegex = BuildRegex(@"^»收录进了管弦乐琴乐谱集之中。$");
            ProcessAddDesynthSellSellRegex = BuildRegex(@"的价格卖出了“$");
            ProcessLocalPlayerUseRegex = BuildRegex(@"使用了“$");
            ProcessOtherPlayerUseRegex = BuildRegex(@"使用了“$");
            ProcessFastCraftUseMateriaRegex = BuildRegex(@"^镶嵌到了$");
            ProcessFastCraftExtractMateriaRegex = BuildRegex(@"^进行了精制魔晶石！\n获得了$");
            ProcessFastCraftCraftRegex = BuildRegex(@"制作“$");
            ProcessGatherMinBtnRegex = BuildRegex(@"获得了“$");
            ProcessGatherFshRegex = BuildRegex(@"成功钓上了$");
            ProcessLocalPlayerSynthesizeRegex = BuildRegex(@"制作“$");
            ProcessOtherPlayerSynthesizeRegex = BuildRegex(@"制作“$");
            RollRegex = BuildRegex(@"(?<Roll>\d{1,3})");
            
            // cn-specific
            _processLocalPlayerSynthesizeRegex2 = BuildRegex(@"成功！$");
            _processOtherPlayerSynthesizeRegex2 = BuildRegex(@"成功！$");
        }

        protected override LootEvent ProcessSystem(LootMessage message)
        {
            // Searched (Local Player)
            if (ProcessSystemSearchRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Search,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Added
            if (ProcessSystemAddedRegex.IsMatch(message.MessageParts.First()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Add
                };

            // Lost
            if (ProcessSystemLostRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost
                };

            // Purchased (Local Player)
            if (ProcessSystemPurchasedRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Purchase,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Discarded (Local Player)
            if (ProcessSystemDiscardedRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Discard,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtained from Desynth (Local Player)
            if (ProcessSystemObtainedFromDesynthRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtained by Retrieving Materia (Local Player)
            if (ProcessSystemObtainedFromMateriaRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Lost Materia (Local Player)
            if (message.MessageParts.Count >= 3 && ProcessSystemLostMateriaRegex.IsMatch(message.MessageParts[2]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Sell (Local Player)
            if (ProcessAddDesynthSellSellRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Sell,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            return null;
        }

        protected override LootEvent ProcessLocalPlayerObtainLoot(LootMessage message)
        {
            // Obtain Loot (Local Player)
            return new LootEvent
            {
                LootEventType = LootEventType.Obtain,
                IsLocalPlayer = true,
                PlayerName = Plugin.GetLocalPlayerName()
            };
        }

        protected override LootEvent ProcessLocalPlayerRoll(LootMessage message)
        {
            var lootEvent = new LootEvent
            {
                IsLocalPlayer = true,
                PlayerName = Plugin.GetLocalPlayerName()
            };

            // Cast Lot (Local Player)
            if (message.MessageParts.Count >= 4 && ProcessLocalPlayerRollCastRegex.IsMatch(message.MessageParts[3]))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Local Player)
            else if (ProcessLocalPlayerRollNeedRegex.IsMatch(message.MessageParts[0]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            // Greed Roll (Local Player)
            else if (ProcessLocalPlayerRollGreedRegex.IsMatch(message.MessageParts[0]))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            else
            {
                return null;
            }

            return lootEvent;
        }

        protected override LootEvent ProcessOtherPlayerObtainLoot(LootMessage message)
        {
            // Obtain Loot (Other Player)
            return new LootEvent
            {
                LootEventType = LootEventType.Obtain,
                IsLocalPlayer = false,
                PlayerName = message.MessageParts[0]
            };
        }

        protected override LootEvent ProcessOtherPlayerRoll(LootMessage message)
        {
            var lootEvent = new LootEvent
            {
                IsLocalPlayer = false,
                PlayerName = message.MessageParts[0]
            };

            // Cast Lot (Other Player)
            if (ProcessOtherPlayerRollCastRegex.IsMatch(message.MessageParts.Last()))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && ProcessOtherPlayerRollNeedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            // Greed Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && ProcessOtherPlayerRollGreedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            else
            {
                return null;
            }

            return lootEvent;
        }

        protected override LootEvent ProcessAddDesynthSell(LootMessage message)
        {
            // Desynth (Local Player)
            if (ProcessAddDesynthSellDesynthRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Desynth,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Orchestration (Local Player)
            if (ProcessAddDesynthSellOrchestrationRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            return null;
        }

        protected override LootEvent ProcessLocalPlayerUse(LootMessage message)
        {
            // Use (Local Player)
            return new LootEvent
            {
                LootEventType = LootEventType.Use,
                IsLocalPlayer = true,
                PlayerName = Plugin.GetLocalPlayerName()
            };
        }

        protected override LootEvent ProcessLocalPlayerSpecialObtain(LootMessage message)
        {
            // Obtain via Item (Local Player)
            return new LootEvent
            {
                LootEventType = LootEventType.Obtain,
                IsLocalPlayer = true,
                PlayerName = Plugin.GetLocalPlayerName()
            };
        }

        protected override LootEvent ProcessOtherPlayerUse(LootMessage message)
        {
            // Use (Other Player)
            if (ProcessOtherPlayerUseRegex.IsMatch(message.MessageParts[1]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    PlayerName = message.MessageParts[0]
                };
            return null;
        }

        protected override LootEvent ProcessFastCraft(LootMessage message)
        {
            // Use Materia (Local Player)
            if (message.MessageParts.Count >= 4 && ProcessFastCraftUseMateriaRegex.IsMatch(message.MessageParts[3]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Extract Materia (Local Player)
            if (message.MessageParts.Count >= 4 && ProcessFastCraftExtractMateriaRegex.IsMatch(message.MessageParts[3]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtain via Crafting (Local Player)
            if (ProcessFastCraftCraftRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            return null;
        }

        protected override LootEvent ProcessGather(LootMessage message)
        {
            // Obtain by MIN/BTN (Local Player)
            if (ProcessGatherMinBtnRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Gather,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtain by FSH (Local Player)
            if (ProcessGatherFshRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Gather,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };
            return null;
        }

        protected override LootEvent ProcessLocalPlayerSynthesize(LootMessage message)
        {
            // Obtain by Crafting (Local Player)
            if (message.MessageParts.Count >= 4 && ProcessLocalPlayerSynthesizeRegex.IsMatch(message.MessageParts[0])
                                                && _processLocalPlayerSynthesizeRegex2.IsMatch(message.MessageParts[3]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };
            return null;
        }

        protected override LootEvent ProcessOtherPlayerSynthesize(LootMessage message)
        {
            // Obtain by Crafting (Other Player)
            if (message.MessageParts.Count >= 4 &&
                ProcessOtherPlayerSynthesizeRegex.IsMatch(message.MessageParts[1]) &&
                _processOtherPlayerSynthesizeRegex2.IsMatch(message.MessageParts[3]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    PlayerName = message.MessageParts[0]
                };
            return null;
        }
    }
}