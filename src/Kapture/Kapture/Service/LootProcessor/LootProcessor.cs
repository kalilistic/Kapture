using System;
using System.Text.RegularExpressions;

using Dalamud.DrunkenToad;
using Dalamud.Game.Text;

namespace Kapture
{
    /// <summary>
    /// Process loot events.
    /// </summary>
    public abstract class LootProcessor
    {
        /// <summary>
        /// Kapture plugin.
        /// </summary>
        protected readonly IKapturePlugin Plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="LootProcessor"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        protected LootProcessor(IKapturePlugin plugin)
        {
            this.Plugin = plugin;
            this.CompileRegex();
        }

        /// <summary>
        /// Gets or sets regex to search for item.
        /// </summary>
        protected Regex ProcessSystemSearchRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex to add item to loot table.
        /// </summary>
        protected Regex ProcessSystemAddedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex to lost item in loot table.
        /// </summary>
        protected Regex ProcessSystemLostRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for purchase item.
        /// </summary>
        protected Regex ProcessSystemPurchasedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex to discard item.
        /// </summary>
        protected Regex ProcessSystemDiscardedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex to obtain item from desynth.
        /// </summary>
        protected Regex ProcessSystemObtainedFromDesynthRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex to obtain item from materia.
        /// </summary>
        protected Regex ProcessSystemObtainedFromMateriaRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for lost materia.
        /// </summary>
        protected Regex ProcessSystemLostMateriaRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for local player casting lot.
        /// </summary>
        protected Regex ProcessLocalPlayerRollCastRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for local player rolling need.
        /// </summary>
        protected Regex ProcessLocalPlayerRollNeedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for local player rolling greed.
        /// </summary>
        protected Regex ProcessLocalPlayerRollGreedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for non-local player casting lot.
        /// </summary>
        protected Regex ProcessOtherPlayerRollCastRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for non-local player rolling need.
        /// </summary>
        protected Regex ProcessOtherPlayerRollNeedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for non-local player rolling greed.
        /// </summary>
        protected Regex ProcessOtherPlayerRollGreedRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for desynth and a few other scenarios.
        /// </summary>
        protected Regex ProcessAddDesynthSellDesynthRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for desynth and a few other scenarios for orchestration rolls.
        /// </summary>
        protected Regex ProcessAddDesynthSellOrchestrationRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for desynth and a few other scenarios.
        /// </summary>
        protected Regex ProcessAddDesynthSellSellRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for local player using an item.
        /// </summary>
        protected Regex ProcessLocalPlayerUseRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for non-local player using an item.
        /// </summary>
        protected Regex ProcessOtherPlayerUseRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for fast craft or using materia.
        /// </summary>
        protected Regex ProcessFastCraftUseMateriaRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for fast craft or extracting materia.
        /// </summary>
        protected Regex ProcessFastCraftExtractMateriaRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for fast crafting an item.
        /// </summary>
        protected Regex ProcessFastCraftCraftRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for gathering an item.
        /// </summary>
        protected Regex ProcessGatherMinBtnRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for fishing for an item.
        /// </summary>
        protected Regex ProcessGatherFshRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for local player synthesizing an item.
        /// </summary>
        protected Regex ProcessLocalPlayerSynthesizeRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for non-local player synthesizing an item.
        /// </summary>
        protected Regex ProcessOtherPlayerSynthesizeRegex { get; set; } = null!;

        /// <summary>
        /// Gets or sets regex for a player roll.
        /// </summary>
        protected Regex RollRegex { get; set; } = null!;

        /// <summary>
        /// Process loot message.
        /// </summary>
        /// <param name="message">message from chat.</param>
        /// <returns>loot event.</returns>
        public LootEvent? ProcessLoot(LootMessage message)
        {
            try
            {
                // process by message type
                var lootEvent = message.LootMessageType switch
                {
                    LootMessageType.System => this.ProcessSystem(message),
                    LootMessageType.LocalPlayerObtainLoot => this.ProcessLocalPlayerObtainLoot(message),
                    LootMessageType.LocalPlayerRoll => this.ProcessLocalPlayerRoll(message),
                    LootMessageType.OtherPlayerObtainLoot => this.ProcessOtherPlayerObtainLoot(message),
                    LootMessageType.OtherPlayerRoll => this.ProcessOtherPlayerRoll(message),
                    LootMessageType.AddDesynthSell => this.ProcessAddDesynthSell(message),
                    LootMessageType.LocalPlayerUse => this.ProcessLocalPlayerUse(message),
                    LootMessageType.LocalPlayerSpecialObtain => this.ProcessLocalPlayerSpecialObtain(message),
                    LootMessageType.OtherPlayerUse => this.ProcessOtherPlayerUse(message),
                    LootMessageType.FastCraft => this.ProcessFastCraft(message),
                    LootMessageType.Gather => this.ProcessGather(message),
                    LootMessageType.LocalPlayerSynthesize => this.ProcessLocalPlayerSynthesize(message),
                    LootMessageType.OtherPlayerSynthesize => this.ProcessOtherPlayerSynthesize(message),
                    LootMessageType.AllianceOtherPlayerObtain => this.ProcessOtherPlayerObtainLoot(message),
                    LootMessageType.AllianceOtherPlayerRoll => this.ProcessOtherPlayerRoll(message),
                    _ => null,
                };

                // reject if unsupported type
                if (lootEvent == null) return null;

                // set common fields
                lootEvent.LootMessage = message;
                lootEvent.LootEventTypeName = Enum.GetName(typeof(LootEventType), lootEvent.LootEventType) ?? string.Empty;
                lootEvent.ItemName = lootEvent.LootMessage.ItemName;
                if (string.IsNullOrEmpty(lootEvent.ItemName))
                {
                    lootEvent.ItemNameAbbreviated = lootEvent.ItemName;
                }
                else
                {
                    lootEvent.ItemNameAbbreviated = lootEvent.LootMessage.ItemName.Length <= 30
                        ? lootEvent.LootMessage.ItemName
                        : lootEvent.LootMessage.ItemName.Substring(0, 26) + "...";
                }

                lootEvent.PlayerDisplayName = this.Plugin.FormatPlayerName(this.Plugin.Configuration.LootNameFormat, lootEvent.PlayerName);
                if (lootEvent.LootMessage.IsHq)
                    lootEvent.ItemNameAbbreviated += " " + this.Plugin.GetSeIcon(SeIconChar.HighQuality);
                return lootEvent;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process item.");
                return null;
            }
        }

        /// <summary>
        /// Check if event is enabled in configuration.
        /// </summary>
        /// <param name="lootEvent">loot event.</param>
        /// <returns>indicator if event is enabled.</returns>
        public bool IsEnabledEvent(LootEvent lootEvent)
        {
            return this.IsEnabledType(lootEvent.LootEventType) && this.IsEnabledPlayer(lootEvent.PlayerName);
        }

        /// <summary>
        /// Build regex expression.
        /// </summary>
        /// <param name="pattern">regex pattern.</param>
        /// <returns>Precompiled regex.</returns>
        protected static Regex BuildRegex(string pattern)
        {
            return new (pattern, RegexOptions.Compiled);
        }

                /// <summary>
        /// Compile regex.
        /// </summary>
        protected abstract void CompileRegex();

        /// <summary>
        /// Process system message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessSystem(LootMessage message);

        /// <summary>
        /// Process local player obtain loot message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent ProcessLocalPlayerObtainLoot(LootMessage message);

        /// <summary>
        /// Process local player roll message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessLocalPlayerRoll(LootMessage message);

        /// <summary>
        /// Process non-local player obtain loot message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent ProcessOtherPlayerObtainLoot(LootMessage message);

        /// <summary>
        /// Process non-local player roll message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessOtherPlayerRoll(LootMessage message);

        /// <summary>
        /// Process add/desynth/sell message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessAddDesynthSell(LootMessage message);

        /// <summary>
        /// Process local player use message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent ProcessLocalPlayerUse(LootMessage message);

        /// <summary>
        /// Process local player special obtain message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent ProcessLocalPlayerSpecialObtain(LootMessage message);

        /// <summary>
        /// Process non-local player use message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessOtherPlayerUse(LootMessage message);

        /// <summary>
        /// Process fast craft message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessFastCraft(LootMessage message);

        /// <summary>
        /// Process gather message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessGather(LootMessage message);

        /// <summary>
        /// Process non-local player synthesis message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessLocalPlayerSynthesize(LootMessage message);

        /// <summary>
        /// Process other player synthesis message.
        /// </summary>
        /// <param name="message">chat message.</param>
        /// <returns>parsed loot event.</returns>
        protected abstract LootEvent? ProcessOtherPlayerSynthesize(LootMessage message);

        private bool IsEnabledPlayer(string playerName)
        {
            return !(this.Plugin.Configuration.SelfOnly && !playerName.Equals(this.Plugin.GetLocalPlayerName()));
        }

        private bool IsEnabledType(LootEventType lootEventType)
        {
            return lootEventType switch
            {
                LootEventType.Add => this.Plugin.Configuration.AddEnabled,
                LootEventType.Cast => this.Plugin.Configuration.CastEnabled,
                LootEventType.Craft => this.Plugin.Configuration.CraftEnabled,
                LootEventType.Desynth => this.Plugin.Configuration.DesynthEnabled,
                LootEventType.Discard => this.Plugin.Configuration.DiscardEnabled,
                LootEventType.Gather => this.Plugin.Configuration.GatherEnabled,
                LootEventType.Greed => this.Plugin.Configuration.GreedEnabled,
                LootEventType.Lost => this.Plugin.Configuration.LostEnabled,
                LootEventType.Need => this.Plugin.Configuration.NeedEnabled,
                LootEventType.Obtain => this.Plugin.Configuration.ObtainEnabled,
                LootEventType.Purchase => this.Plugin.Configuration.PurchaseEnabled,
                LootEventType.Search => this.Plugin.Configuration.SearchEnabled,
                LootEventType.Sell => this.Plugin.Configuration.SellEnabled,
                LootEventType.Use => this.Plugin.Configuration.UseEnabled,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
