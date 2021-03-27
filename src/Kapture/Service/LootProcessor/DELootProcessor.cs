using System;
using System.Linq;

// ReSharper disable StringLiteralTypo

namespace Kapture
{
    public class DELootProcessor : LootProcessor
    {
        public DELootProcessor(IKapturePlugin kapturePlugin) : base(kapturePlugin)
        {
        }

        protected override void CompileRegex()
        {
            ProcessSystemSearchRegex = BuildRegex(@"^ wird gesucht.$");
            ProcessSystemAddedRegex = BuildRegex(@"^Ihr habt Beutegut *?");
            ProcessSystemLostRegex = BuildRegex(@"^ nicht erhalten.$");
            ProcessSystemPurchasedRegex = BuildRegex(@" gekauft.$");
            ProcessSystemDiscardedRegex = BuildRegex(@"^Du wirfst *?");
            ProcessSystemObtainedFromDesynthRegex = BuildRegex(@"^ verwertet!$");
            ProcessSystemObtainedFromMateriaRegex = BuildRegex(@"^ zurückgewonnen!$");
            ProcessSystemLostMateriaRegex = BuildRegex(@"^ zerfällt zu wertlosem Staub$");
            ProcessLocalPlayerRollCastRegex = BuildRegex(@"^Du würfelst um *?");
            ProcessLocalPlayerRollNeedRegex = BuildRegex(@"^Du würfelst mit „Bedarf“*?");
            ProcessLocalPlayerRollGreedRegex = BuildRegex(@"^Du würfelst mit „Gier“*?");
            ProcessOtherPlayerRollCastRegex = BuildRegex(@"^ würfelt um das $");
            ProcessOtherPlayerRollNeedRegex = BuildRegex(@" würfelt mit „Bedarf“ *?");
            ProcessOtherPlayerRollGreedRegex = BuildRegex(@" würfelt mit „Gier“ *?");
            ProcessAddDesynthSellDesynthRegex = BuildRegex(@"^ verwertet!$");
            ProcessAddDesynthSellOrchestrationRegex = BuildRegex(@"^ wurde deinem Orchestrion hinzugefügt.$");
            ProcessAddDesynthSellSellRegex = BuildRegex(@" verkauft.$");
            ProcessLocalPlayerUseRegex = BuildRegex(@"^Du verwendest *?");
            ProcessOtherPlayerUseRegex = BuildRegex(@"^ macht die $");
            ProcessFastCraftUseMateriaRegex = BuildRegex(@"^ eingesetzt!$");
            ProcessFastCraftExtractMateriaRegex = BuildRegex(@"^Du hast die Materia *?");
            ProcessFastCraftCraftRegex = BuildRegex(@"^ hergestellt.$");
            ProcessGatherMinBtnRegex = BuildRegex(@"^ erhalten.$");
            ProcessGatherFshRegex = BuildRegex(@" gefangen.$");
            ProcessLocalPlayerSynthesizeRegex = BuildRegex(@"^ hergestellt.$");
            ProcessOtherPlayerSynthesizeRegex = BuildRegex(@"^ hergestellt.$");
            RollRegex = BuildRegex(@"(?<Roll>\d{1,3})");
        }

        protected override LootEvent ProcessSystem(LootMessage message)
        {
            // Searched (Local Player)
            if (ProcessSystemSearchRegex.IsMatch(message.MessageParts.Last()))
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
            if (ProcessSystemLostRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost
                };

            // Purchased (Local Player)
            if (ProcessSystemPurchasedRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Purchase,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Discarded (Local Player)
            if (ProcessSystemDiscardedRegex.IsMatch(message.MessageParts.First()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Discard,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtained from Desynth (Local Player)
            if (ProcessSystemObtainedFromDesynthRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtained by Retrieving Materia (Local Player)
            if (ProcessSystemObtainedFromMateriaRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Lost Materia (Local Player)
            if (message.MessageParts.Count >= 4 && ProcessSystemLostMateriaRegex.IsMatch(message.MessageParts[3]))
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
            if (ProcessLocalPlayerRollCastRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Local Player)
            else if (message.MessageParts.Count >= 1 && ProcessLocalPlayerRollNeedRegex.IsMatch(message.MessageParts[0]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.First()).Groups["Roll"].Value);
            }

            // Greed Roll (Local Player)
            else if (ProcessLocalPlayerRollGreedRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts.First()).Groups["Roll"].Value);
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
            if (message.MessageParts.Count >= 2 && ProcessOtherPlayerRollCastRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && ProcessOtherPlayerRollNeedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts[1]).Groups["Roll"].Value);
            }

            // Greed Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && ProcessOtherPlayerRollGreedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(RollRegex.Match(message.MessageParts[1]).Groups["Roll"].Value);
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
            if (ProcessAddDesynthSellDesynthRegex.IsMatch(message.MessageParts.Last()))
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
            if (ProcessAddDesynthSellSellRegex.IsMatch(message.MessageParts.Last()))
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
            if (message.MessageParts.Count >= 2 && ProcessOtherPlayerUseRegex.IsMatch(message.MessageParts[1]))
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
            if (ProcessFastCraftUseMateriaRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Use,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Extract Materia (Local Player)
            if (ProcessFastCraftExtractMateriaRegex.IsMatch(message.MessageParts.First()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Obtain,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtain via Crafting (Local Player)
            if (ProcessFastCraftCraftRegex.IsMatch(message.MessageParts.Last()))
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
            if (ProcessGatherMinBtnRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Gather,
                    IsLocalPlayer = true,
                    PlayerName = Plugin.GetLocalPlayerName()
                };

            // Obtain by FSH (Local Player)
            if (ProcessGatherFshRegex.IsMatch(message.MessageParts.Last()))
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
            if (ProcessLocalPlayerSynthesizeRegex.IsMatch(message.MessageParts.Last()))
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
            if (ProcessOtherPlayerSynthesizeRegex.IsMatch(message.MessageParts.Last()))
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    PlayerName = message.MessageParts[0]
                };
            return null;
        }
    }
}