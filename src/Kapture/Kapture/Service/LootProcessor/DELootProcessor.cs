using System;
using System.Linq;

namespace Kapture
{
    /// <summary>
    /// Process german loot messages.
    /// </summary>
    public class DELootProcessor : LootProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DELootProcessor"/> class.
        /// </summary>
        /// <param name="kapturePlugin">kapture plugin.</param>
        public DELootProcessor(IKapturePlugin kapturePlugin)
            : base(kapturePlugin)
        {
        }

        /// <inheritdoc />
        protected override void CompileRegex()
        {
            this.ProcessSystemSearchRegex = BuildRegex(@"^ wird gesucht.$");
            this.ProcessSystemAddedRegex = BuildRegex(@"^Ihr habt Beutegut *?");
            this.ProcessSystemLostRegex = BuildRegex(@"^ nicht erhalten.$");
            this.ProcessSystemPurchasedRegex = BuildRegex(@" gekauft.$");
            this.ProcessSystemDiscardedRegex = BuildRegex(@"^Du wirfst *?");
            this.ProcessSystemObtainedFromDesynthRegex = BuildRegex(@"^ verwertet!$");
            this.ProcessSystemObtainedFromMateriaRegex = BuildRegex(@"^ zurückgewonnen!$");
            this.ProcessSystemLostMateriaRegex = BuildRegex(@"^ zerfällt zu wertlosem Staub$");
            this.ProcessLocalPlayerRollCastRegex = BuildRegex(@"^Du würfelst um *?");
            this.ProcessLocalPlayerRollNeedRegex = BuildRegex(@"^Du würfelst mit „Bedarf“*?");
            this.ProcessLocalPlayerRollGreedRegex = BuildRegex(@"^Du würfelst mit „Gier“*?");
            this.ProcessOtherPlayerRollCastRegex = BuildRegex(@"^ würfelt um das $");
            this.ProcessOtherPlayerRollNeedRegex = BuildRegex(@" würfelt mit „Bedarf“ *?");
            this.ProcessOtherPlayerRollGreedRegex = BuildRegex(@" würfelt mit „Gier“ *?");
            this.ProcessAddDesynthSellDesynthRegex = BuildRegex(@"^ verwertet!$");
            this.ProcessAddDesynthSellOrchestrationRegex = BuildRegex(@"^ wurde deinem Orchestrion hinzugefügt.$");
            this.ProcessAddDesynthSellSellRegex = BuildRegex(@" verkauft.$");
            this.ProcessLocalPlayerUseRegex = BuildRegex(@"^Du verwendest *?");
            this.ProcessOtherPlayerUseRegex = BuildRegex(@"^ macht die $");
            this.ProcessFastCraftUseMateriaRegex = BuildRegex(@"^ eingesetzt!$");
            this.ProcessFastCraftExtractMateriaRegex = BuildRegex(@"^Du hast die Materia *?");
            this.ProcessFastCraftCraftRegex = BuildRegex(@"^ hergestellt.$");
            this.ProcessGatherMinBtnRegex = BuildRegex(@"^ erhalten.$");
            this.ProcessGatherFshRegex = BuildRegex(@" gefangen.$");
            this.ProcessLocalPlayerSynthesizeRegex = BuildRegex(@"^ hergestellt.$");
            this.ProcessOtherPlayerSynthesizeRegex = BuildRegex(@"^ hergestellt.$");
            this.RollRegex = BuildRegex(@"(?<Roll>\d{1,3})");
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessSystem(LootMessage message)
        {
            // Searched (Local Player)
            if (this.ProcessSystemSearchRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessSystemLostRegex.IsMatch(message.MessageParts.Last()))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
                };
            }

            // Purchased (Local Player)
            if (this.ProcessSystemPurchasedRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessSystemDiscardedRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessSystemObtainedFromDesynthRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessSystemObtainedFromMateriaRegex.IsMatch(message.MessageParts.Last()))
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
            if (message.MessageParts.Count >= 4 && this.ProcessSystemLostMateriaRegex.IsMatch(message.MessageParts[3]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
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
            return new()
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
            if (this.ProcessLocalPlayerRollCastRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Local Player)
            else if (message.MessageParts.Count >= 1 && this.ProcessLocalPlayerRollNeedRegex.IsMatch(message.MessageParts[0]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.First()).Groups["Roll"].Value);
            }

            // Greed Roll (Local Player)
            else if (this.ProcessLocalPlayerRollGreedRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.First()).Groups["Roll"].Value);
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
            if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerRollCastRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerRollNeedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts[1]).Groups["Roll"].Value);
            }

            // Greed Roll (Other Player)
            else if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerRollGreedRegex.IsMatch(message.MessageParts[1]))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts[1]).Groups["Roll"].Value);
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
            if (this.ProcessAddDesynthSellDesynthRegex.IsMatch(message.MessageParts.Last()))
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

            // Sell (Local Player)
            if (this.ProcessAddDesynthSellSellRegex.IsMatch(message.MessageParts.Last()))
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
        protected override LootEvent ProcessLocalPlayerUse(LootMessage message)
        {
            // Use (Local Player)
            return new LootEvent
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
            if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerUseRegex.IsMatch(message.MessageParts[1]))
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
            if (this.ProcessFastCraftUseMateriaRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessFastCraftExtractMateriaRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessFastCraftCraftRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessGatherMinBtnRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessGatherFshRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessLocalPlayerSynthesizeRegex.IsMatch(message.MessageParts.Last()))
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
            if (this.ProcessOtherPlayerSynthesizeRegex.IsMatch(message.MessageParts.Last()))
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
