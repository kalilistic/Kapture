using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kapture
{
    /// <summary>
    /// Process chinese loot messages.
    /// </summary>
    public class ZHLootProcessor : LootProcessor
    {
        private Regex processFastCraftRegex2 = null!;
        private Regex processLocalPlayerSynthesizeRegex2 = null!;
        private Regex processOtherPlayerSynthesizeRegex2 = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZHLootProcessor"/> class.
        /// </summary>
        /// <param name="kapturePlugin">kapture plugin.</param>
        public ZHLootProcessor(IKapturePlugin kapturePlugin)
            : base(kapturePlugin)
        {
        }

        /// <inheritdoc />
        protected override void CompileRegex()
        {
            this.ProcessSystemSearchRegex = BuildRegex(@"^正在确认“$");
            this.ProcessSystemAddedRegex = BuildRegex(@"^获得了新的战利品$");
            this.ProcessSystemLostRegex = BuildRegex(@"^无法获得“$");
            this.ProcessSystemPurchasedRegex = BuildRegex(@"(^从市场购买了“|的价格买入了“|的价格回购了“)$");
            this.ProcessSystemDiscardedRegex = BuildRegex(@"^舍弃了“$");
            this.ProcessSystemObtainedFromDesynthRegex = BuildRegex(@"   获得了$");
            this.ProcessSystemObtainedFromMateriaRegex = BuildRegex(@"成功回收了$");
            this.ProcessSystemLostMateriaRegex = BuildRegex(@"^化成了粉末……$");
            this.ProcessLocalPlayerRollCastRegex = BuildRegex(@"^掷骰。$");
            this.ProcessLocalPlayerRollNeedRegex = BuildRegex(@"在需求条件下对“$");
            this.ProcessLocalPlayerRollGreedRegex = BuildRegex(@"在贪婪条件下对“$");
            this.ProcessOtherPlayerRollCastRegex = BuildRegex(@"^掷骰。$");
            this.ProcessOtherPlayerRollNeedRegex = BuildRegex(@"在需求条件下对“$");
            this.ProcessOtherPlayerRollGreedRegex = BuildRegex(@"在贪婪条件下对“$");
            this.ProcessAddDesynthSellDesynthRegex = BuildRegex(@"成功分解了$");
            this.ProcessAddDesynthSellOrchestrationRegex = BuildRegex(@"^»收录进了管弦乐琴乐谱集之中。$");
            this.ProcessAddDesynthSellSellRegex = BuildRegex(@"的价格卖出了“$");
            this.ProcessLocalPlayerUseRegex = BuildRegex(@"使用了“$");
            this.ProcessOtherPlayerUseRegex = BuildRegex(@"使用了“$");
            this.ProcessFastCraftUseMateriaRegex = BuildRegex(@"^镶嵌到了$");
            this.ProcessFastCraftExtractMateriaRegex = BuildRegex(@"^进行了精制魔晶石！\n获得了$");
            this.ProcessFastCraftCraftRegex = BuildRegex(@"制作“$");
            this.ProcessGatherMinBtnRegex = BuildRegex(@"获得了“$");
            this.ProcessGatherFshRegex = BuildRegex(@"成功钓上了$");
            this.ProcessLocalPlayerSynthesizeRegex = BuildRegex(@"制作“$");
            this.ProcessOtherPlayerSynthesizeRegex = BuildRegex(@"制作“$");
            this.RollRegex = BuildRegex(@"(?<Roll>\d{1,3})");

            // cn-specific
            this.processFastCraftRegex2 = BuildRegex(@"成功！$");
            this.processLocalPlayerSynthesizeRegex2 = BuildRegex(@"成功！$");
            this.processOtherPlayerSynthesizeRegex2 = BuildRegex(@"成功！$");
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessSystem(LootMessage message)
        {
            // Searched (Local Player)
            if (this.ProcessSystemSearchRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Search,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Added
            if (this.ProcessSystemAddedRegex.IsMatch(message.MessageParts.First()))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Add,
                };
            }

            // Lost
            if (this.ProcessSystemLostRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
                };
            }

            // Purchased (Local Player)
            if (this.ProcessSystemPurchasedRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Purchase,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Discarded (Local Player)
            if (this.ProcessSystemDiscardedRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Discard,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Obtained from Desynth (Local Player)
            if (this.ProcessSystemObtainedFromDesynthRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Obtained by Retrieving Materia (Local Player)
            if (this.ProcessSystemObtainedFromMateriaRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Lost Materia (Local Player)
            if (message.MessageParts.Count >= 3 && this.ProcessSystemLostMateriaRegex.IsMatch(message.MessageParts[2]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Sell (Local Player)
            if (this.ProcessAddDesynthSellSellRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Sell,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent ProcessLocalPlayerObtainLoot(LootMessage message)
        {
            // Obtain Loot (Local Player)
            return new LootEvent
            {
                LootEventType = LootEventType.Obtain,
                IsLocalPlayer = true,
                PlayerName = this.Plugin.GetLocalPlayerName(),
                World = this.Plugin.GetLocalPlayerWorld(),
            };
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessLocalPlayerRoll(LootMessage message)
        {
            var lootEvent = new LootEvent
            {
                IsLocalPlayer = true,
                PlayerName = this.Plugin.GetLocalPlayerName(),
                World = this.Plugin.GetLocalPlayerWorld(),
            };

            // Cast Lot (Local Player)
            if (message.MessageParts.Count >= 4 && this.ProcessLocalPlayerRollCastRegex.IsMatch(message.MessageParts[3]))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Local Player)
            else if (this.ProcessLocalPlayerRollNeedRegex.IsMatch(message.MessageParts[0]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            // Greed Roll (Local Player)
            else if (this.ProcessLocalPlayerRollGreedRegex.IsMatch(message.MessageParts[0]))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }
            else
            {
                return null;
            }

            return lootEvent;
        }

        /// <inheritdoc />
        protected override LootEvent ProcessOtherPlayerObtainLoot(LootMessage message)
        {
            // Obtain Loot (Other Player)
            return new()
            {
                LootEventType = LootEventType.Obtain,
                IsLocalPlayer = false,
                PlayerName = message.MessageParts[0],
                World = message.Player?.World.Name.ToString() ?? string.Empty,
            };
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessOtherPlayerRoll(LootMessage message)
        {
            var lootEvent = new LootEvent
            {
                IsLocalPlayer = false,
                PlayerName = message.MessageParts[0],
                World = message.Player?.World.Name.ToString() ?? string.Empty,
            };

            // Cast Lot (Other Player)
            if (this.ProcessOtherPlayerRollCastRegex.IsMatch(message.MessageParts.Last()))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerRollNeedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            // Greed Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerRollGreedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }
            else
            {
                return null;
            }

            return lootEvent;
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessAddDesynthSell(LootMessage message)
        {
            // Desynth (Local Player)
            if (this.ProcessAddDesynthSellDesynthRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Desynth,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Orchestration (Local Player)
            if (this.ProcessAddDesynthSellOrchestrationRegex.IsMatch(message.MessageParts.Last()))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent ProcessLocalPlayerUse(LootMessage message)
        {
            // Use (Local Player)
            return new()
            {
                LootEventType = LootEventType.Use,
                IsLocalPlayer = true,
                PlayerName = this.Plugin.GetLocalPlayerName(),
                World = this.Plugin.GetLocalPlayerWorld(),
            };
        }

        /// <inheritdoc />
        protected override LootEvent ProcessLocalPlayerSpecialObtain(LootMessage message)
        {
            // Obtain via Item (Local Player)
            return new()
            {
                LootEventType = LootEventType.Obtain,
                IsLocalPlayer = true,
                PlayerName = this.Plugin.GetLocalPlayerName(),
                World = this.Plugin.GetLocalPlayerWorld(),
            };
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessOtherPlayerUse(LootMessage message)
        {
            // Use (Other Player)
            if (this.ProcessOtherPlayerUseRegex.IsMatch(message.MessageParts[1]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    PlayerName = message.MessageParts[0],
                    World = message.Player?.World.Name.ToString() ?? string.Empty,
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessFastCraft(LootMessage message)
        {
            // Use Materia (Local Player)
            if (message.MessageParts.Count >= 4 && this.ProcessFastCraftUseMateriaRegex.IsMatch(message.MessageParts[3]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Extract Materia (Local Player)
            if (message.MessageParts.Count >= 4 && this.ProcessFastCraftExtractMateriaRegex.IsMatch(message.MessageParts[3]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Obtain via Crafting (Local Player)
            if (message.MessageParts.Count >= 4 && this.ProcessFastCraftCraftRegex.IsMatch(message.MessageParts[0])
                                                && this.processFastCraftRegex2.IsMatch(message.MessageParts[3]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessGather(LootMessage message)
        {
            // Obtain by MIN/BTN (Local Player)
            if (this.ProcessGatherMinBtnRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Gather,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            // Obtain by FSH (Local Player)
            if (this.ProcessGatherFshRegex.IsMatch(message.MessageParts[0]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Gather,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessLocalPlayerSynthesize(LootMessage message)
        {
            // Obtain by Crafting (Local Player)
            if (message.MessageParts.Count >= 4 && this.ProcessLocalPlayerSynthesizeRegex.IsMatch(message.MessageParts[0])
                                                && this.processLocalPlayerSynthesizeRegex2.IsMatch(message.MessageParts[3]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    IsLocalPlayer = true,
                    PlayerName = this.Plugin.GetLocalPlayerName(),
                    World = this.Plugin.GetLocalPlayerWorld(),
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessOtherPlayerSynthesize(LootMessage message)
        {
            // Obtain by Crafting (Other Player)
            if (message.MessageParts.Count >= 4 &&
                this.ProcessOtherPlayerSynthesizeRegex.IsMatch(message.MessageParts[1]) &&
                this.processOtherPlayerSynthesizeRegex2.IsMatch(message.MessageParts[3]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    PlayerName = message.MessageParts[0],
                    World = message.Player?.World.Name.ToString() ?? string.Empty,
                };
            }

            return null;
        }
    }
}
