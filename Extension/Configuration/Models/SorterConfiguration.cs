using System;
using JetBrains.Annotations;

namespace YAPO.Configuration.Models {
    [UsedImplicitly]
    public class SorterConfiguration {
        public SortMode CurrentSortByMode;
        public SortMode CurrentThenByMode;
        public SortDirection SortDirection;
        public bool SortOrderOpposite;
        public bool UpgradableOnTop;
        public TypeSortOption[] SortByTypeOrder = Enum.GetValues(typeof(TypeSortOption)) as TypeSortOption[];
        public TypeSortOption[] ThenByTypeOrder = Enum.GetValues(typeof(TypeSortOption)) as TypeSortOption[];
    }
}
