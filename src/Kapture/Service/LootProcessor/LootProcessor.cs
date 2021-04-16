// ReSharper disable UnusedParameter.Local
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Text.RegularExpressions;

namespace Kapture
{
    public abstract class LootProcessor
    {
        protected readonly IKapturePlugin Plugin;

        protected LootProcessor(IKapturePlugin plugin)
        {
            Plugin = plugin;
            CompileRegex();
        }

        protected Regex ProcessSystemSearchRegex { get; set; }
        protected Regex ProcessSystemAddedRegex { get; set; }
        protected Regex ProcessSystemLostRegex { get; set; }
        protected Regex ProcessSystemPurchasedRegex { get; set; }
        protected Regex ProcessSystemDiscardedRegex { get; set; }
        protected Regex ProcessSystemObtainedFromDesynthRegex { get; set; }
        protected Regex ProcessSystemObtainedFromMateriaRegex { get; set; }
        protected Regex ProcessSystemLostMateriaRegex { get; set; }
        protected Regex ProcessLocalPlayerRollCastRegex { get; set; }
        protected Regex ProcessLocalPlayerRollNeedRegex { get; set; }
        protected Regex ProcessLocalPlayerRollGreedRegex { get; set; }
        protected Regex ProcessOtherPlayerRollCastRegex { get; set; }
        protected Regex ProcessOtherPlayerRollNeedRegex { get; set; }
        protected Regex ProcessOtherPlayerRollGreedRegex { get; set; }
        protected Regex ProcessAddDesynthSellDesynthRegex { get; set; }
        protected Regex ProcessAddDesynthSellOrchestrationRegex { get; set; }
        protected Regex ProcessAddDesynthSellSellRegex { get; set; }
        protected Regex ProcessLocalPlayerUseRegex { get; set; }
        protected Regex ProcessOtherPlayerUseRegex { get; set; }
        protected Regex ProcessFastCraftUseMateriaRegex { get; set; }
        protected Regex ProcessFastCraftExtractMateriaRegex { get; set; }
        protected Regex ProcessFastCraftCraftRegex { get; set; }
        protected Regex ProcessGatherMinBtnRegex { get; set; }
        protected Regex ProcessGatherFshRegex { get; set; }
        protected Regex ProcessLocalPlayerSynthesizeRegex { get; set; }
        protected Regex ProcessOtherPlayerSynthesizeRegex { get; set; }
        protected Regex RollRegex { get; set; }

        protected Regex BuildRegex(string pattern)
        {
            return new Regex(pattern, RegexOptions.Compiled);
        }

        public LootEvent ProcessLoot(LootMessage message)
        {
            try
            {
                LootEvent lootEvent;

                // process by message type
                switch (message.LootMessageType)
                {
                    case LootMessageType.System:
                        lootEvent = ProcessSystem(message);
                        break;
                    case LootMessageType.LocalPlayerObtainLoot:
                        lootEvent = ProcessLocalPlayerObtainLoot(message);
                        break;
                    case LootMessageType.LocalPlayerRoll:
                        lootEvent = ProcessLocalPlayerRoll(message);
                        break;
                    case LootMessageType.OtherPlayerObtainLoot:
                        lootEvent = ProcessOtherPlayerObtainLoot(message);
                        break;
                    case LootMessageType.OtherPlayerRoll:
                        lootEvent = ProcessOtherPlayerRoll(message);
                        break;
                    case LootMessageType.AddDesynthSell:
                        lootEvent = ProcessAddDesynthSell(message);
                        break;
                    case LootMessageType.LocalPlayerUse:
                        lootEvent = ProcessLocalPlayerUse(message);
                        break;
                    case LootMessageType.LocalPlayerSpecialObtain:
                        lootEvent = ProcessLocalPlayerSpecialObtain(message);
                        break;
                    case LootMessageType.OtherPlayerUse:
                        lootEvent = ProcessOtherPlayerUse(message);
                        break;
                    case LootMessageType.FastCraft:
                        lootEvent = ProcessFastCraft(message);
                        break;
                    case LootMessageType.Gather:
                        lootEvent = ProcessGather(message);
                        break;
                    case LootMessageType.LocalPlayerSynthesize:
                        lootEvent = ProcessLocalPlayerSynthesize(message);
                        break;
                    case LootMessageType.OtherPlayerSynthesize:
                        lootEvent = ProcessOtherPlayerSynthesize(message);
                        break;
                    case LootMessageType.AllianceOtherPlayerObtain:
                        lootEvent = ProcessOtherPlayerObtainLoot(message);
                        break;
                    case LootMessageType.AllianceOtherPlayerRoll:
                        lootEvent = ProcessOtherPlayerRoll(message);
                        break;
                    default:
                        lootEvent = null;
                        break;
                }

                // reject if unsupported type
                if (lootEvent == null) return null;

                // set common fields
                lootEvent.LootMessage = message;
                lootEvent.LootEventTypeName = Enum.GetName(typeof(LootEventType), lootEvent.LootEventType);
                lootEvent.ItemName = lootEvent.LootMessage.ItemName;
                if (string.IsNullOrEmpty(lootEvent.ItemName))
                    lootEvent.ItemNameAbbreviated = lootEvent.ItemName;
                else
                    lootEvent.ItemNameAbbreviated = lootEvent.LootMessage.ItemName.Length <= 30
                        ? lootEvent.LootMessage.ItemName
                        : lootEvent.LootMessage.ItemName.Substring(0, 26) + "...";
                lootEvent.PlayerDisplayName = Plugin.FormatPlayerName(Plugin.Configuration.LootNameFormat, lootEvent.PlayerName);
                if (lootEvent.LootMessage.IsHq)
                    lootEvent.ItemNameAbbreviated += " " + Plugin.GetSeIcon(SeIconChar.HighQuality);
                return lootEvent;
            }
            catch (Exception ex)
            {
                Plugin.LogError(ex, "Failed to process item.");
                return null;
            }
        }

        public bool IsEnabledEvent(LootEvent lootEvent)
        {
            return IsEnabledType(lootEvent.LootEventType) && IsEnabledPlayer(lootEvent.PlayerName);
        }

        private bool IsEnabledPlayer(string playerName)
        {
            return !(Plugin.Configuration.SelfOnly && !playerName.Equals(Plugin.GetLocalPlayerName()));
        }

        private bool IsEnabledType(LootEventType lootEventType)
        {
            switch (lootEventType)
            {
                case LootEventType.Add:
                    return Plugin.Configuration.AddEnabled;
                case LootEventType.Cast:
                    return Plugin.Configuration.CastEnabled;
                case LootEventType.Craft:
                    return Plugin.Configuration.CraftEnabled;
                case LootEventType.Desynth:
                    return Plugin.Configuration.DesynthEnabled;
                case LootEventType.Discard:
                    return Plugin.Configuration.DiscardEnabled;
                case LootEventType.Gather:
                    return Plugin.Configuration.GatherEnabled;
                case LootEventType.Greed:
                    return Plugin.Configuration.GreedEnabled;
                case LootEventType.Lost:
                    return Plugin.Configuration.LostEnabled;
                case LootEventType.Need:
                    return Plugin.Configuration.NeedEnabled;
                case LootEventType.Obtain:
                    return Plugin.Configuration.ObtainEnabled;
                case LootEventType.Purchase:
                    return Plugin.Configuration.PurchaseEnabled;
                case LootEventType.Search:
                    return Plugin.Configuration.SearchEnabled;
                case LootEventType.Sell:
                    return Plugin.Configuration.SellEnabled;
                case LootEventType.Use:
                    return Plugin.Configuration.UseEnabled;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract void CompileRegex();
        protected abstract LootEvent ProcessSystem(LootMessage message);
        protected abstract LootEvent ProcessLocalPlayerObtainLoot(LootMessage message);
        protected abstract LootEvent ProcessLocalPlayerRoll(LootMessage message);
        protected abstract LootEvent ProcessOtherPlayerObtainLoot(LootMessage message);
        protected abstract LootEvent ProcessOtherPlayerRoll(LootMessage message);
        protected abstract LootEvent ProcessAddDesynthSell(LootMessage message);
        protected abstract LootEvent ProcessLocalPlayerUse(LootMessage message);
        protected abstract LootEvent ProcessLocalPlayerSpecialObtain(LootMessage message);
        protected abstract LootEvent ProcessOtherPlayerUse(LootMessage message);
        protected abstract LootEvent ProcessFastCraft(LootMessage message);
        protected abstract LootEvent ProcessGather(LootMessage message);
        protected abstract LootEvent ProcessLocalPlayerSynthesize(LootMessage message);
        protected abstract LootEvent ProcessOtherPlayerSynthesize(LootMessage message);
    }
}