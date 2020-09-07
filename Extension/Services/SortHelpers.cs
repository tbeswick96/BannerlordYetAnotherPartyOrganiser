using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using YAPO.Configuration.Models;

namespace YAPO.Services {
    public static class SortHelpers {
        public static IOrderedEnumerable<TroopRosterElement> SortBy(this IEnumerable<TroopRosterElement> troops, SortMode sortMode, SorterConfiguration configuration) {
            SortDirection sortDirection = configuration.SortDirection;
            return sortMode switch {
                SortMode.NONE => troops.OrderBy(x => x),
                SortMode.ALPHABETICAL => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortAlphabetically) : troops.OrderByDescending(SortAlphabetically),
                SortMode.TYPE => sortDirection == SortDirection.ASCENDING
                                     ? troops.OrderBy(x => x, new TroopTypeComparer(configuration.SortByTypeOrder))
                                     : troops.OrderByDescending(x => x, new TroopTypeComparer(configuration.SortByTypeOrder)),
                SortMode.GROUP => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByGroup) : troops.OrderByDescending(SortByGroup),
                SortMode.TIER => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByTier) : troops.OrderByDescending(SortByTier),
                SortMode.CULTURE => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByCulture) : troops.OrderByDescending(SortByCulture),
                SortMode.COUNT => sortDirection == SortDirection.ASCENDING ? troops.OrderBy(SortByCount) : troops.OrderByDescending(SortByCount),
                _ => throw new ArgumentOutOfRangeException(nameof(sortMode))
            };
        }

        public static IEnumerable<TroopRosterElement> ThenSortBy(this IOrderedEnumerable<TroopRosterElement> troops, SortMode sortMode, SorterConfiguration configuration) {
            SortDirection sortDirection = configuration.SortOrderOpposite ? configuration.SortDirection.InvertDirection() : configuration.SortDirection;
            return sortMode switch {
                SortMode.NONE => troops.ThenBy(x => x),
                SortMode.ALPHABETICAL => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortAlphabetically) : troops.ThenByDescending(SortAlphabetically),
                SortMode.TYPE => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(x => x, new TroopTypeComparer(configuration.ThenByTypeOrder)) : troops.ThenByDescending(x => x, new TroopTypeComparer(configuration.ThenByTypeOrder)),
                SortMode.GROUP => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByGroup) : troops.ThenByDescending(SortByGroup),
                SortMode.TIER => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByTier) : troops.ThenByDescending(SortByTier),
                SortMode.CULTURE => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByCulture) : troops.ThenByDescending(SortByCulture),
                SortMode.COUNT => sortDirection == SortDirection.ASCENDING ? troops.ThenBy(SortByCount) : troops.ThenByDescending(SortByCount),
                _ => throw new ArgumentOutOfRangeException(nameof(sortMode))
            };
        }

        private static SortDirection InvertDirection(this SortDirection sortDirection) => sortDirection == SortDirection.ASCENDING ? SortDirection.DESCENDING : SortDirection.ASCENDING;

        // Name
        private static string SortAlphabetically(TroopRosterElement x) => x.Character.ToString();

        // Formation Group
        private static FormationClass SortByGroup(TroopRosterElement x) => x.Character.GetFormationClass(PartyBase.MainParty);

        // Tier
        private static int SortByTier(TroopRosterElement x) => x.Character.Tier;

        // Culture
        private static string SortByCulture(TroopRosterElement x) => x.Character.Culture.Name.ToString();

        // Count
        private static int SortByCount(TroopRosterElement x) => x.Number;

        // Type
        private class TroopTypeComparer : IComparer<TroopRosterElement> {
            private readonly TypeSorter[] _typeSorters;

            public TroopTypeComparer(IEnumerable<TypeSortOption> typeSortOrder) {
                _typeSorters = typeSortOrder.Select(x => new TypeSorter(ResolveTypeSortAction(x))).ToArray();
            }

            public int Compare(TroopRosterElement a, TroopRosterElement b) {
                if (a.Character == null && b.Character == null) return 0;
                if (b.Character == null) return 1;
                if (a.Character == null) return -1;

                return _typeSorters.Select(typeSorter => typeSorter.Sort(a, b)).FirstOrDefault(result => result != int.MaxValue);
            }

            private static Func<TroopRosterElement, TroopRosterElement, int> ResolveTypeSortAction(TypeSortOption typeSortOption) {
                return typeSortOption switch {
                    TypeSortOption.CAVALRY => (a, b) => {
                        if (a.Character.IsMounted && !a.Character.IsArcher && !(b.Character.IsMounted && !b.Character.IsArcher)) return -1;
                        if (b.Character.IsMounted && !b.Character.IsArcher && !(a.Character.IsMounted && !a.Character.IsArcher)) return 1;
                        if (a.Character.IsMounted && !a.Character.IsArcher && b.Character.IsMounted && !b.Character.IsArcher) return 0;
                        return int.MaxValue;
                    },
                    TypeSortOption.RANGED_CAVALRY => (a, b) => {
                        if (a.Character.IsMounted && a.Character.IsArcher && !(b.Character.IsMounted && b.Character.IsArcher)) return -1;
                        if (b.Character.IsMounted && b.Character.IsArcher && !(a.Character.IsMounted && a.Character.IsArcher)) return 1;
                        if (a.Character.IsMounted && a.Character.IsArcher && b.Character.IsMounted && b.Character.IsArcher) return 0;
                        return int.MaxValue;
                    },
                    TypeSortOption.INFANTRY => (a, b) => {
                        if (a.Character.IsInfantry && !b.Character.IsInfantry) return -1;
                        if (b.Character.IsInfantry && !a.Character.IsInfantry) return 1;
                        if (a.Character.IsInfantry && b.Character.IsInfantry) return 0;
                        return int.MaxValue;
                    },
                    TypeSortOption.RANGED => (a, b) => {
                        if (a.Character.IsArcher && !b.Character.IsArcher) return -1;
                        if (b.Character.IsArcher && !a.Character.IsArcher) return 1;
                        if (a.Character.IsArcher && b.Character.IsArcher) return 0;
                        return int.MaxValue;
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(typeSortOption), typeSortOption, $"TypeSortOption {typeSortOption} does not exist")
                };
            }
        }

        private class TypeSorter {
            private readonly Func<TroopRosterElement, TroopRosterElement, int> _sortAction;

            public TypeSorter(Func<TroopRosterElement, TroopRosterElement, int> sortAction) => _sortAction = sortAction;

            public int Sort(TroopRosterElement a, TroopRosterElement b) => _sortAction(a, b);
        }
    }
}
