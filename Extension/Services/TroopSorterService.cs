using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TroopManager.Services {
    [SaveableClass(13337100)]
    public class TroopSorterService : MBObjectBase {
        [SaveableProperty(10)]
        public SortMode CurrentSortMode { get; private set; }

        [SaveableProperty(20)]
        public SortDirection SortDirection { get; private set; }

        [SaveableProperty(30)]
        public bool UpgradableOnTop { get; set; }

        [SaveableProperty(40)]
        public SortSide SortSide { get; set; }
        
        public void UpdateSortingMode(SortMode newSortMode = SortMode.ALPHABETICAL, bool skipDirectionFlip = false) {
            if (!skipDirectionFlip) {
                SortDirection = CurrentSortMode == newSortMode ? SortDirection == SortDirection.ASCENDING ? SortDirection.DESCENDING : SortDirection.ASCENDING : SortDirection.ASCENDING;
            }

            CurrentSortMode = newSortMode;
        }

        public void Sort(ref List<TroopRosterElement> sortedTroops, ref List<TroopRosterElement> heroTroops) {
            sortedTroops.Sort(new TroopRosterElementComparer(CurrentSortMode, SortDirection));
            heroTroops.Sort(new TroopRosterElementComparer(CurrentSortMode, SortDirection));

            if (!UpgradableOnTop) return;

            List<TroopRosterElement> upgradableTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade > 0).ToList();
            upgradableTroops = upgradableTroops.OrderByDescending(x => x.Character.UpgradeRequiresItemFromCategory == null).ToList();
            sortedTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade <= 0).ToList();
            sortedTroops.InsertRange(0, upgradableTroops);
        }
    }

    public class TroopRosterElementComparer : IComparer<TroopRosterElement> {
        private readonly SortDirection _sortDirection;
        private readonly SortMode _sortMode;

        public TroopRosterElementComparer(SortMode sortMode, SortDirection sortDirection) {
            _sortDirection = sortDirection;
            _sortMode = sortMode;
        }

        public int Compare(TroopRosterElement x, TroopRosterElement y) {
            if (x.Character == null && y.Character == null) return 0;
            if (y.Character == null) return ApplySortDirection(1);
            if (x.Character == null) return ApplySortDirection(-1);

            switch (_sortMode) {
                case SortMode.ALPHABETICAL: return ApplySortDirection(SortAlphabetically(x, y));
                case SortMode.TYPE: return ApplySortDirection(SortByType(x, y));
                case SortMode.GROUP: return ApplySortDirection(SortByGroup(x, y));
                case SortMode.TIER: return ApplySortDirection(SortByTier(x, y));
                case SortMode.CULTURE: return ApplySortDirection(SortByCulture(x, y));
                case SortMode.NONE: return 0;
                default: throw new ArgumentOutOfRangeException($"Can't sort on column {_sortMode}");
            }
        }

        private static int SortAlphabetically(TroopRosterElement x, TroopRosterElement y) => string.Compare(x.Character.ToString(), y.Character.ToString(), StringComparison.InvariantCultureIgnoreCase);

        // Cavalry, Ranged Cavalry, Infantry, Ranged
        private static int SortByType(TroopRosterElement x, TroopRosterElement y) {
            // Cavalry
            if (x.Character.IsMounted && !x.Character.IsArcher && !(y.Character.IsMounted && !y.Character.IsArcher)) return -1;
            if (y.Character.IsMounted && !y.Character.IsArcher && !(x.Character.IsMounted && !x.Character.IsArcher)) return 1;
            if (x.Character.IsMounted && !x.Character.IsArcher && y.Character.IsMounted && !y.Character.IsArcher) return SortAlphabetically(x, y);

            // Ranged Cavalry
            if (x.Character.IsMounted && x.Character.IsArcher && !(y.Character.IsMounted && y.Character.IsArcher)) return -1;
            if (y.Character.IsMounted && y.Character.IsArcher && !(x.Character.IsMounted && x.Character.IsArcher)) return 1;
            if (x.Character.IsMounted && x.Character.IsArcher && y.Character.IsMounted && y.Character.IsArcher) return SortAlphabetically(x, y);

            // Infantry
            if (x.Character.IsInfantry && !y.Character.IsInfantry) return -1;
            if (y.Character.IsInfantry && !x.Character.IsInfantry) return 1;
            if (x.Character.IsInfantry && y.Character.IsInfantry) return SortAlphabetically(x, y);

            // Ranged
            if (x.Character.IsArcher && !y.Character.IsArcher) return -1;
            if (y.Character.IsArcher && !x.Character.IsArcher) return 1;
            if (x.Character.IsArcher && y.Character.IsArcher) return SortAlphabetically(x, y);

            return SortAlphabetically(x, y);
        }

        // Formation Group
        private static int SortByGroup(TroopRosterElement x, TroopRosterElement y) => x.Character.CurrentFormationClass < y.Character.CurrentFormationClass ? -1 : x.Character.CurrentFormationClass > y.Character.CurrentFormationClass ? 1 : SortAlphabetically(x, y);

        // Tier
        private static int SortByTier(TroopRosterElement x, TroopRosterElement y) => x.Character.Tier < y.Character.Tier ? -1 : x.Character.Tier > y.Character.Tier ? 1 : SortAlphabetically(x, y);
        
        // Culture
        private static int SortByCulture(TroopRosterElement x, TroopRosterElement y) {
            int value = string.Compare(x.Character.Culture.Name.ToString(), y.Character.Culture.Name.ToString(), StringComparison.InvariantCultureIgnoreCase);
            return value == 0 ? SortAlphabetically(x, y) : value;
        }
        
        private int ApplySortDirection(int result) => _sortDirection == SortDirection.ASCENDING ? result : result * -1;
    }

    [SaveableEnum(13337150)]
    public enum SortMode {
        NONE,
        ALPHABETICAL,
        TYPE,
        GROUP,
        TIER,
        CULTURE
    }

    [SaveableEnum(13337200)]
    public enum SortDirection {
        ASCENDING,
        DESCENDING
    }

    [SaveableEnum(13337250)]
    public enum SortSide {
        OTHER,
        PARTY
    }
}
