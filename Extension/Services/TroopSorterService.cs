using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace YAPO.Services
{
    [SaveableClass(13337100)]
    public class TroopSorterService : MBObjectBase
    {
        [SaveableProperty(20)]
        public SortDirection SortDirection { get; private set; }

        [SaveableProperty(30)]
        public bool UpgradableOnTop { get; set; }

        [SaveableProperty(35)]
        public bool SortOrderOpposite { get; set; }

        [SaveableProperty(40)]
        public SortSide SortSide { get; set; }

        [SaveableProperty(50)]
        public SortMode CurrentSortByMode { get; set; }

        [SaveableProperty(60)]
        public SortMode CurrentThenByMode { get; set; }

        public void UpdateSortingDirection(SortDirection sortDirection)
        {
            SortDirection = sortDirection;
        }

        public void Sort(ref List<TroopRosterElement> sortedTroops, ref List<TroopRosterElement> heroTroops)
        {
            sortedTroops = CurrentThenByMode == SortMode.NONE || CurrentThenByMode == CurrentSortByMode ? sortedTroops.SortBy(CurrentSortByMode, SortDirection).ToList() : sortedTroops.SortBy(CurrentSortByMode, SortDirection).ThenSortBy(CurrentThenByMode, SortOrderOpposite ? SortDirection.InvertDirection() : SortDirection).ToList();
            heroTroops = CurrentThenByMode == SortMode.NONE || CurrentThenByMode == CurrentSortByMode ? heroTroops.SortBy(CurrentSortByMode, SortDirection).ToList() : heroTroops.SortBy(CurrentSortByMode, SortDirection).ThenSortBy(CurrentThenByMode, SortOrderOpposite ? SortDirection.InvertDirection() : SortDirection).ToList();

            if (!UpgradableOnTop) return;

            // TODO: Re-visit this to use PartyCharacterVM to allow better sorting and upgrade detection
            List<TroopRosterElement> upgradableTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade > 0).ToList();
            upgradableTroops = upgradableTroops.OrderByDescending(x => x.Character.UpgradeRequiresItemFromCategory == null).ToList();
            sortedTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade <= 0).ToList();
            sortedTroops.InsertRange(0, upgradableTroops);
        }
    }
}
