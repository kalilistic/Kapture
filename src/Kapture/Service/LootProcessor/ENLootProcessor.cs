using System;
using System.Linq;

namespace KapturePlugin
{
    public class ENLootProcessor : LootProcessor
    {
        public ENLootProcessor(IKapturePlugin kapturePlugin) : base(kapturePlugin)
        {
        }

        protected override void CompileRegex()
        {
            ProcessSystemSearchRegex = BuildRegex(@"^Searching for $");
            ProcessSystemAddedRegex = BuildRegex(@"^ has been added to the loot list.$");
            ProcessSystemLostRegex = BuildRegex(@"^Unable to obtain *?");
            ProcessSystemPurchasedRegex = BuildRegex(@"^You purchase *?");
            ProcessSystemDiscardedRegex = BuildRegex(@"^You throw away *?");
            ProcessSystemObtainedFromDesynthRegex = BuildRegex(@"^You obtain *?");
            ProcessSystemObtainedFromMateriaRegex = BuildRegex(@"^You receive *?");
            ProcessSystemLostMateriaRegex = BuildRegex(@"^ shatters...$");
            ProcessLocalPlayerObtainLootRegex = BuildRegex(@""); // not used
            ProcessLocalPlayerRollCastRegex = BuildRegex(@"^You cast your lot *?");
            ProcessLocalPlayerRollNeedRegex = BuildRegex(@"^You roll Need on *?");
            ProcessLocalPlayerRollGreedRegex = BuildRegex(@"^You roll Greed on *?");
            ProcessOtherPlayerObtainLootRegex = BuildRegex(@""); // not used
            ProcessOtherPlayerRollCastRegex = BuildRegex(@" casts (his|her) lot *?");
            ProcessOtherPlayerRollNeedRegex = BuildRegex(@"^ rolls Need *?");
            ProcessOtherPlayerRollGreedRegex = BuildRegex(@"^ rolls Greed *?");
            ProcessAddDesynthSellDesynthRegex = BuildRegex(@"^You desynthesize *?");
            ProcessAddDesynthSellOrchestrationRegex = BuildRegex(@"^ is added to your orchestrion list.$");
            ProcessAddDesynthSellSellRegex = BuildRegex(@"^You sell *?");
            ProcessLocalPlayerUseRegex = BuildRegex(@"^ uses *?");
            ProcessLocalPlayerSpecialObtainRegex = BuildRegex(@""); // not used
            ProcessOtherPlayerUseRegex = BuildRegex(@"^ uses *?");
            ProcessFastCraftUseMateriaRegex = BuildRegex(@"^You successfully attach *?");
            ProcessFastCraftExtractMateriaRegex = BuildRegex(@"^You successfully extract *?");
            ProcessFastCraftCraftRegex = BuildRegex(@"^You synthesize *?");
            ProcessGatherMinBtnRegex = BuildRegex(@"^You obtain *?");
            ProcessGatherFshRegex = BuildRegex(@"^You land *?");
            ProcessLocalPlayerSynthesizeRegex = BuildRegex(@"^You synthesize *?");
            ProcessOtherPlayerSynthesizeRegex = BuildRegex(@" synthesizes *?");
            RollRegex = BuildRegex(@"(?<Roll>\d{1,3})");
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
            if (ProcessSystemAddedRegex.IsMatch(message.MessageParts.Last()))
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
            if (ProcessSystemLostMateriaRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
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
            if (ProcessLocalPlayerRollCastRegex.IsMatch(message.MessageParts[0]))
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
            if (ProcessOtherPlayerRollCastRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Other Player)
            else if (ProcessOtherPlayerRollNeedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            // Greed Roll (Other Player)
            else if (ProcessOtherPlayerRollGreedRegex.IsMatch(message.MessageParts[1]))
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
            if (ProcessFastCraftUseMateriaRegex.IsMatch(message.MessageParts[0]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Extract Materia (Local Player)
            if (ProcessFastCraftExtractMateriaRegex.IsMatch(message.MessageParts[0]))
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
            if (ProcessLocalPlayerSynthesizeRegex.IsMatch(message.MessageParts[0]))
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
            if (ProcessOtherPlayerSynthesizeRegex.IsMatch(message.MessageParts[1]))
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    PlayerName = message.MessageParts[0]
                };
            return null;
        }
    }
}