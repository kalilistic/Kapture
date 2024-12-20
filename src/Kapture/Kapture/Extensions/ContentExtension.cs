using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapture.Kapture.Extensions
{
    internal static class ContentExtension
    {
        /// <summary>
        /// Gets current content id.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="territoryType">territory type id.</param>
        /// <returns>content id or zero if not in content.</returns>
        public static uint ContentId(this IDataManager value, ushort territoryType) => GetContentId(value, territoryType);

        /// <summary>
        /// Get content name.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="territoryType">territory type id.</param>
        /// <returns>content name.</returns>
        public static string ContentName(this IDataManager value, ushort territoryType)
        {
            var contentId = GetContentId(value, territoryType);
            if (contentId == 0)
            {
                return string.Empty;
            }

            return value.GetExcelSheet<ContentFinderCondition>().GetRowOrDefault(contentId)?.Name.ExtractText() ?? string.Empty;
        }

        /// <summary>
        /// Gets indicator whether current territory is content.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="territoryType">territory type id.</param>
        /// <returns>indicator whether local player is in content.</returns>
        public static bool InContent(this IDataManager value, ushort territoryType) => GetContentId(value, territoryType) != 0;

        /// <summary>
        /// Gets indicator whether current territory is high-end duty content.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="territoryType">territory type id.</param>
        /// <returns>indicator whether local player is in high-end duty content.</returns>
        public static bool InHighEndDuty(this IDataManager value, ushort territoryType)
        {
            var contentId = GetContentId(value, territoryType);
            if (contentId == 0)
            {
                return false;
            }

            var content = value.GetExcelSheet<ContentFinderCondition>();
            var contentRow = content.GetRowOrDefault(contentId);
            return contentRow is { HighEndDuty: true };
        }

        /// <summary>
        /// Get world names.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <returns>list of world names.</returns>
        public static string[] WorldNames(this IDataManager value)
        {
            var worldSheet = value.GetExcelSheet<World>();
            return worldSheet.Where(world => world.IsPublic)
                .Select(world => world.Name.ToString())
                .OrderBy(worldName => worldName).ToArray();
        }

        /// <summary>
        /// Get race name by race/gender id.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="id">race id.</param>
        /// <param name="genderId">gender id.</param>
        /// <returns>race name.</returns>
        public static string Race(this IDataManager value, int id, int genderId)
        {
            if (id == 0)
            {
                return string.Empty;
            }

            var raceSheet = value.GetExcelSheet<Race>();
            var raceResult = raceSheet.TryGetFirst(raceEntry => raceEntry.RowId == id, out var race);

            if (!raceResult)
            {
                return string.Empty;
            }

            return genderId == 0 ? race.Masculine.ExtractText() : race.Feminine.ExtractText();
        }

        /// <summary>
        /// Get tribe name by race/gender id.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="id">tribe id.</param>
        /// <param name="genderId">gender id.</param>
        /// <returns>race name.</returns>
        public static string Tribe(this IDataManager value, int id, int genderId)
        {
            if (id == 0)
            {
                return string.Empty;
            }

            var tribeSheet = value.GetExcelSheet<Tribe>();
            var tribeResult = tribeSheet.TryGetFirst(tribeEntry => tribeEntry.RowId == id, out var tribe);

            if (!tribeResult)
            {
                return string.Empty;
            }

            return genderId == 0 ? tribe.Masculine.ExtractText() : tribe.Feminine.ExtractText();
        }

        /// <summary>
        /// Get job code.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="classJobId">job code id.</param>
        /// <returns>job code.</returns>
        public static string ClassJobCode(this IDataManager value, uint classJobId) =>
            value.GetExcelSheet<ClassJob>().GetRowOrDefault(classJobId)?.Abbreviation.ExtractText() ?? string.Empty;

        /// <summary>
        /// Get place name.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="territoryTypeId">territory type id.</param>
        /// <returns>place name.</returns>
        public static string PlaceName(this IDataManager value, uint territoryTypeId) =>
            value.GetExcelSheet<TerritoryType>().GetRowOrDefault(territoryTypeId)?.PlaceName.Value.Name.ToDalamudString().TextValue.Replace("\u0002\u001F\u0001\u0003", "-") ??
            string.Empty;

        /// <summary>
        /// Get world id.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="worldName">world name.</param>
        /// <returns>world id.</returns>
        public static uint WorldId(this IDataManager value, string worldName)
        {
            var worldSheet = value.GetExcelSheet<World>();
            var worldResult = worldSheet.TryGetFirst(w => w.Name.ToString().Equals(worldName, StringComparison.Ordinal), out var world);
            return !worldResult ? 0 : world.RowId;
        }

        /// <summary>
        /// Get world id.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="worldId">world id.</param>
        /// <returns>world name.</returns>
        public static string WorldName(this IDataManager value, uint worldId) => value.GetExcelSheet<World>().GetRowOrDefault(worldId)?.Name.ExtractText() ?? string.Empty;

        /// <summary>
        /// Get indicator whether world is a test data center.
        /// </summary>
        /// <param name="value">data manager.</param>
        /// <param name="worldId">world id.</param>
        /// <returns>indicator whether world is a test data center.</returns>
        public static bool IsTestDc(this IDataManager value, uint worldId)
        {
            var world = value.GetExcelSheet<World>().GetRowOrDefault(worldId);
            return world?.DataCenter.RowId == 13;
        }

        private static uint GetContentId(IDataManager value, ushort territoryType)
        {
            var cfcs = value.GetExcelSheet<ContentFinderCondition>();
            var cfcResult = cfcs.TryGetFirst(c => c.TerritoryType.RowId == territoryType, out var cfc);
            return !cfcResult ? 0 : cfc.RowId;
        }
    }
}
