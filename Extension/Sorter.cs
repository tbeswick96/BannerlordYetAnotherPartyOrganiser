using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TroopManager {
    [SaveableClass(13337100)]
    public class Sorter : MBObjectBase {
        [SaveableProperty(10)]
        public SortingMode CurrentlySortingMode { get; private set; }

        [SaveableProperty(20)]
        public SortingDirection SortDirection { get; private set; }

        [SaveableProperty(30)]
        public bool UpgradableOnTop { get; set; }

        public void UpdateSortingMode(SortingMode sortingMode = SortingMode.ALPHABETICAL, bool skipDirectionFlip = false) {
            if (!skipDirectionFlip) {
                SortDirection = CurrentlySortingMode == sortingMode ? SortDirection == SortingDirection.ASCENDING ? SortingDirection.DESCENDING : SortingDirection.ASCENDING : SortingDirection.ASCENDING;
            }

            CurrentlySortingMode = sortingMode;
        }

        public void Sort(ref List<TroopRosterElement> sortedTroops, ref List<TroopRosterElement> heroTroops) {
            sortedTroops.Sort(new TroopRosterElementComparer(CurrentlySortingMode, SortDirection));
            heroTroops.Sort(new TroopRosterElementComparer(CurrentlySortingMode, SortDirection));

            if (!UpgradableOnTop) return;

            List<TroopRosterElement> upgradableTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade > 0).ToList();
            sortedTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade <= 0).ToList();
            sortedTroops.InsertRange(0, upgradableTroops);
        }
    }

    public class TroopRosterElementComparer : IComparer<TroopRosterElement> {
        private readonly SortingDirection _sortingDirection;
        private readonly SortingMode _sortingMode;

        public TroopRosterElementComparer(SortingMode sortingMode, SortingDirection sortingDirection) {
            _sortingDirection = sortingDirection;
            _sortingMode = sortingMode;
        }

        public int Compare(TroopRosterElement x, TroopRosterElement y) {
            if (x.Character == null && y.Character == null) return 0;
            if (y.Character == null) return ApplySortDirection(1);
            if (x.Character == null) return ApplySortDirection(-1);

            switch (_sortingMode) {
                case SortingMode.ALPHABETICAL: return ApplySortDirection(SortAlphabetically(x, y));
                case SortingMode.TYPE: return ApplySortDirection(SortByType(x, y));
                case SortingMode.GROUP: return ApplySortDirection(SortByGroup(x, y));
                case SortingMode.TIER: return ApplySortDirection(SortByTier(x, y));
                case SortingMode.NONE: return 0;
                default: throw new ArgumentOutOfRangeException($"Can't sort on column {_sortingMode}");
            }
        }

        private static int SortAlphabetically(TroopRosterElement x, TroopRosterElement y) => string.Compare(x.Character.ToString(), y.Character.ToString(), StringComparison.Ordinal);

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

        private int ApplySortDirection(int result) => _sortingDirection == SortingDirection.ASCENDING ? result : result * -1;
    }

    [SaveableEnum(13337150)]
    public enum SortingMode {
        NONE,
        ALPHABETICAL,
        TYPE,
        GROUP,
        TIER
    }

    [SaveableEnum(13337200)]
    public enum SortingDirection {
        ASCENDING,
        DESCENDING
    }
}
