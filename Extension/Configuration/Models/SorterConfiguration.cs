// ReSharper disable UnusedMember.Global

using JetBrains.Annotations;

namespace YAPO.Configuration.Models
{
    [UsedImplicitly]
    public class SorterConfiguration
    {
        public SortDirection SortDirection;

        public bool UpgradableOnTop;

        public bool SortOrderOpposite;

        public SortMode CurrentSortByMode;

        public SortMode CurrentThenByMode;
    }
}
