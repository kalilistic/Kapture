using System;
using System.Linq;

namespace Kapture
{
    /// <summary>
    /// English loot processor.
    /// </summary>
    public class ENLootProcessor : LootProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ENLootProcessor"/> class.
        /// </summary>
        /// <param name="kapturePlugin">kapture plugin.</param>
        public ENLootProcessor(IKapturePlugin kapturePlugin)
            : base(kapturePlugin)
        {
        }

        /// <inheritdoc/>
        protected override void CompileRegex()
        {
            this.ProcessSystemSearchRegex = BuildRegex(@"^Searching for $");
            this.ProcessSystemAddedRegex = BuildRegex(@"^ has been added to the loot list.$");
            this.ProcessSystemLostRegex = BuildRegex(@"^Unable to obtain*?");
            this.ProcessSystemPurchasedRegex = BuildRegex(@"^You purchase*?");
            this.ProcessSystemDiscardedRegex = BuildRegex(@"^You throw away*?");
            this.ProcessSystemObtainedFromDesynthRegex = BuildRegex(@" obtain*?");
            this.ProcessSystemObtainedFromMateriaRegex = BuildRegex(@"^You receive*?");
            this.ProcessSystemLostMateriaRegex = BuildRegex(@"^ shatters...$");
            this.ProcessLocalPlayerRollCastRegex = BuildRegex(@" cast[s]? (your|his|her) lot*?");
            this.ProcessLocalPlayerRollNeedRegex = BuildRegex(@" roll[s]? Need on*?");
            this.ProcessLocalPlayerRollGreedRegex = BuildRegex(@" roll[s]? Greed on*?");
            this.ProcessOtherPlayerRollCastRegex = BuildRegex(@" casts (his|her) lot*?");
            this.ProcessOtherPlayerRollNeedRegex = BuildRegex(@" rolls Need *?");
            this.ProcessOtherPlayerRollGreedRegex = BuildRegex(@" rolls Greed *?");
            this.ProcessAddDesynthSellDesynthRegex = BuildRegex(@" desynthesize[s]? *?");
            this.ProcessAddDesynthSellOrchestrationRegex = BuildRegex(@"^ is added to your orchestrion list.$");
            this.ProcessAddDesynthSellSellRegex = BuildRegex(@"^You sell *?");
            this.ProcessLocalPlayerUseRegex = BuildRegex(@"^ uses *?");
            this.ProcessOtherPlayerUseRegex = BuildRegex(@"^ uses *?");
            this.ProcessFastCraftUseMateriaRegex = BuildRegex(@"successfully attach*?");
            this.ProcessFastCraftExtractMateriaRegex = BuildRegex(@"successfully extract*?");
            this.ProcessFastCraftCraftRegex = BuildRegex(@" synthesize[s]? *?");
            this.ProcessGatherMinBtnRegex = BuildRegex(@" obtain[s]? *?");
            this.ProcessGatherFshRegex = BuildRegex(@"land*?");
            this.ProcessLocalPlayerSynthesizeRegex = BuildRegex(@" synthesize[s]? *?");
            this.ProcessOtherPlayerSynthesizeRegex = BuildRegex(@" synthesizes *?");
            this.RollRegex = BuildRegex(@"(?<Roll>\d{1,3})");
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessSystem(LootMessage message)
        {
            // Searched (Local Player)
            if (this.ProcessSystemSearchRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessSystemAddedRegex.IsMatch(message.MessageParts.Last()))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Add,
                };
            }

            // Lost
            if (this.ProcessSystemLostRegex.IsMatch(message.MessageParts.First()))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Lost,
                };
            }

            // Purchased (Local Player)
            if (this.ProcessSystemPurchasedRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessSystemObtainedFromDesynthRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessSystemObtainedFromMateriaRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessSystemLostMateriaRegex.IsMatch(message.MessageParts.Last()))
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
            };

            // Cast Lot (Local Player)
            if (this.ProcessLocalPlayerRollCastRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Cast;
            }

            // Need Roll (Local Player)
            else if (this.ProcessLocalPlayerRollNeedRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Need;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }

            // Greed Roll (Local Player)
            else if (this.ProcessLocalPlayerRollGreedRegex.IsMatch(message.MessageParts.First()))
            {
                lootEvent.LootEventType = LootEventType.Greed;
                lootEvent.Roll = Convert.ToUInt16(this.RollRegex.Match(message.MessageParts.Last()).Groups["Roll"].Value);
            }
            else
            {
                return null;
            }

            lootEvent.PlayerName = this.Plugin.GetLocalPlayerName();
            lootEvent.World = this.Plugin.GetLocalPlayerWorld();

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
                PlayerName = message.MessageParts.First(),
                World = message.Player?.World.Name.ToString() ?? string.Empty,
            };
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessOtherPlayerRoll(LootMessage message)
        {
            var lootEvent = new LootEvent
            {
                IsLocalPlayer = false,
                PlayerName = message.MessageParts.First(),
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
            if (this.ProcessAddDesynthSellDesynthRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessAddDesynthSellSellRegex.IsMatch(message.MessageParts.First()))
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
            return new LootEvent
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
                    PlayerName = message.MessageParts.First(),
                    World = message.Player?.World.Name.ToString() ?? string.Empty,
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override LootEvent? ProcessFastCraft(LootMessage message)
        {
            // Use Materia (Local Player)
            if (this.ProcessFastCraftUseMateriaRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessFastCraftCraftRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessGatherMinBtnRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessGatherFshRegex.IsMatch(message.MessageParts.First()))
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
            if (this.ProcessLocalPlayerSynthesizeRegex.IsMatch(message.MessageParts.First()))
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
            if (message.MessageParts.Count >= 2 && this.ProcessOtherPlayerSynthesizeRegex.IsMatch(message.MessageParts[1]))
            {
                return new LootEvent
                {
                    LootEventType = LootEventType.Craft,
                    PlayerName = message.MessageParts.First(),
                    World = message.Player?.World.Name.ToString() ?? string.Empty,
                };
            }

            return null;
        }
    }
}
