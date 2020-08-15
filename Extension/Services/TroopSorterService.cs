using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using YAPO.Configuration.Models;

namespace YAPO.Services {
    public static class TroopSorterService {
        public static void Sort(ref List<TroopRosterElement> sortedTroops, ref List<TroopRosterElement> heroTroops, SorterConfiguration configuration) {
            sortedTroops = configuration.CurrentThenByMode == SortMode.NONE || configuration.CurrentThenByMode == configuration.CurrentSortByMode
                               ? sortedTroops.SortBy(configuration.CurrentSortByMode, configuration.SortDirection).ToList()
                               : sortedTroops.SortBy(configuration.CurrentSortByMode, configuration.SortDirection)
                                             .ThenSortBy(configuration.CurrentThenByMode, configuration.SortOrderOpposite ? configuration.SortDirection.InvertDirection() : configuration.SortDirection)
                                             .ToList();
            heroTroops = configuration.CurrentThenByMode == SortMode.NONE || configuration.CurrentThenByMode == configuration.CurrentSortByMode
                             ? heroTroops.SortBy(configuration.CurrentSortByMode, configuration.SortDirection).ToList()
                             : heroTroops.SortBy(configuration.CurrentSortByMode, configuration.SortDirection)
                                         .ThenSortBy(configuration.CurrentThenByMode, configuration.SortOrderOpposite ? configuration.SortDirection.InvertDirection() : configuration.SortDirection)
                                         .ToList();

            if (!configuration.UpgradableOnTop) return;

            // TODO: Re-visit this to use PartyCharacterVM to allow better sorting and upgrade detection
            List<TroopRosterElement> upgradableTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade > 0).ToList();
            upgradableTroops = upgradableTroops.OrderByDescending(x => x.Character.UpgradeRequiresItemFromCategory == null).ToList();
            sortedTroops = sortedTroops.Where(x => x.NumberReadyToUpgrade <= 0).ToList();
            sortedTroops.InsertRange(0, upgradableTroops);
        }
    }
}
