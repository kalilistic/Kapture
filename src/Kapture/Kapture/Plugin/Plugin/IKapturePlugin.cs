using System.Collections.Generic;

using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Text;

namespace Kapture
{
    /// <summary>
    /// Kapture plugin.
    /// </summary>
    public interface IKapturePlugin
    {
        /// <summary>
        /// Gets kapture plugin configuration.
        /// </summary>
        KaptureConfig Configuration { get; }

        /// <summary>
        /// Gets a value indicating whether plugin is initializing.
        /// </summary>
        bool IsInitializing { get; }

        /// <summary>
        /// Gets list of loot events.
        /// </summary>
        List<LootEvent> LootEvents { get; }

        /// <summary>
        /// Gets data manager.
        /// </summary>
        PluginDataManager PluginDataManager { get; }

        /// <summary>
        /// Gets loot rolls.
        /// </summary>
        List<LootRoll> LootRolls { get; }

        /// <summary>
        /// Gets or sets loot rolls for display.
        /// </summary>
        List<LootRoll>? LootRollsDisplay { get; set; }

        /// <summary>
        /// Gets or sets current party list.
        /// </summary>
        public PartyMember[] CurrentPartyList { get; set; }

        /// <summary>
        /// Gets a value indicating whether in content.
        /// </summary>
        bool InContent { get; }

        /// <summary>
        /// Gets or sets a value indicating whether is rolling.
        /// </summary>
        bool IsRolling { get; set; }

        /// <summary>
        /// Gets roll monitor.
        /// </summary>
        RollMonitor RollMonitor { get; }

        /// <summary>
        /// Get SE Icon.
        /// </summary>
        /// <param name="seIconChar">char.</param>
        /// <returns>icon.</returns>
        string GetSeIcon(SeIconChar seIconChar);

        /// <summary>
        /// Get local player name.
        /// </summary>
        /// <returns>local player name.</returns>
        string GetLocalPlayerName();

        /// <summary>
        /// Client language.
        /// </summary>
        /// <returns>client language.</returns>
        ushort ClientLanguage();

        /// <summary>
        /// Format player name.
        /// </summary>
        /// <param name="nameFormatCode">format to use.</param>
        /// <param name="playerName">unformatted player name.</param>
        /// <returns>formatted player name.</returns>
        string FormatPlayerName(int nameFormatCode, string playerName);

        /// <summary>
        /// Gets indicator if is logged in.
        /// </summary>
        /// <returns>indicator if logged in.</returns>
        bool IsLoggedIn();

        /// <summary>
        /// Gets indicator if in combat.
        /// </summary>
        /// <returns>indicator if in combat.</returns>
        bool InCombat();

        /// <summary>
        /// Get party members.
        /// </summary>
        /// <returns>list of party members.</returns>
        IEnumerable<PartyMember> GetPartyMembers();
    }
}
