using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace YAPO.Services {
    public static class SortHelpers {
        public static SortDirection InvertDirection(this SortDirection sortDirection) => sortDirection == SortDirection.ASCENDING ? SortDirection.DESCENDING : SortDirection.ASCENDING;
        
        public static IOrderedEnumerable<TroopRosterElement> SortBy(this IEnumerable<TroopRosterElement> troops, SortMode sortMode, SortDirection sortDirection) {
            return sortMode switch {
                SortMode.NONE => troops.OrderBy(x => x),
                SortMode.ALPHABETICAL => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortAlphabetically) : troops.OrderByDescending(SortAlphabetically),
                SortMode.TYPE => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(x => x, new TroopTypeComparer()) : troops.OrderByDescending(x => x, new TroopTypeComparer()),
                SortMode.GROUP => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByGroup) : troops.OrderByDescending(SortByGroup),
                SortMode.TIER => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByTier) : troops.OrderByDescending(SortByTier),
                SortMode.CULTURE => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByCulture) : troops.OrderByDescending(SortByCulture),
                SortMode.COUNT => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByCount) : troops.OrderByDescending(SortByCount),
                _ => throw new ArgumentOutOfRangeException(nameof(sortMode))
            };
        }

        public static IEnumerable<TroopRosterElement> ThenSortBy(this IOrderedEnumerable<TroopRosterElement> troops, SortMode sortMode, SortDirection sortDirection) {
            return sortMode switch {
                SortMode.NONE => troops.ThenBy(x => x),
                SortMode.ALPHABETICAL => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortAlphabetically) : troops.ThenByDescending(SortAlphabetically),
                SortMode.TYPE => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(x => x, new TroopTypeComparer()) : troops.ThenByDescending(x => x, new TroopTypeComparer()),
                SortMode.GROUP => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByGroup) : troops.ThenByDescending(SortByGroup),
                SortMode.TIER => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByTier) : troops.ThenByDescending(SortByTier),
                SortMode.CULTURE => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByCulture) : troops.ThenByDescending(SortByCulture),
                SortMode.COUNT => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByCount) : troops.ThenByDescending(SortByCount),
                _ => throw new ArgumentOutOfRangeException(nameof(sortMode))
            };
        }

        // Name
        private static string SortAlphabetically(TroopRosterElement x) => x.Character.ToString();

        // Formation Group
        private static FormationClass SortByGroup(TroopRosterElement x) => x.Character.CurrentFormationClass;

        // Tier
        private static int SortByTier(TroopRosterElement x) => x.Character.Tier;

        // Culture
        private static string SortByCulture(TroopRosterElement x) => x.Character.Culture.Name.ToString();

        // Count
        private static int SortByCount(TroopRosterElement x) => x.Number;

        // Type: Cavalry, Ranged Cavalry, Infantry, Ranged
        private class TroopTypeComparer : IComparer<TroopRosterElement> {
            public int Compare(TroopRosterElement x, TroopRosterElement y) {
                if (x.Character == null && y.Character == null) return 0;
                if (y.Character == null) return 1;
                if (x.Character == null) return -1;

                // Cavalry
                if (x.Character.IsMounted && !x.Character.IsArcher && !(y.Character.IsMounted && !y.Character.IsArcher)) return -1;
                if (y.Character.IsMounted && !y.Character.IsArcher && !(x.Character.IsMounted && !x.Character.IsArcher)) return 1;
                if (x.Character.IsMounted && !x.Character.IsArcher && y.Character.IsMounted && !y.Character.IsArcher) return 0;

                // Ranged Cavalry
                if (x.Character.IsMounted && x.Character.IsArcher && !(y.Character.IsMounted && y.Character.IsArcher)) return -1;
                if (y.Character.IsMounted && y.Character.IsArcher && !(x.Character.IsMounted && x.Character.IsArcher)) return 1;
                if (x.Character.IsMounted && x.Character.IsArcher && y.Character.IsMounted && y.Character.IsArcher) return 0;

                // Infantry
                if (x.Character.IsInfantry && !y.Character.IsInfantry) return -1;
                if (y.Character.IsInfantry && !x.Character.IsInfantry) return 1;
                if (x.Character.IsInfantry && y.Character.IsInfantry) return 0;

                // Ranged
                if (x.Character.IsArcher && !y.Character.IsArcher) return -1;
                if (y.Character.IsArcher && !x.Character.IsArcher) return 1;
                if (x.Character.IsArcher && y.Character.IsArcher) return 0;

                return 0;
            }
        }
    }
}
