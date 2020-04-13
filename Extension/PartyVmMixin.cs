using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using UIExtenderLib;
using UIExtenderLib.ViewModel;

namespace TroopManager {
    [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PartyVmMixin : BaseViewModelMixin<PartyVM> {
        private readonly Sorter _sorter;

        public PartyVmMixin(PartyVM viewModel) : base(viewModel) {
            _sorter = MBObjectManager.Instance.GetObject<Sorter>(x => true) ?? MBObjectManager.Instance.CreateObject<Sorter>();
        }

        [DataSourceProperty]
        public string SortAlphabeticalText => ResolveText(SortingMode.ALPHABETICAL, "AZ", "ZA");

        [DataSourceProperty]
        public HintViewModel SortAlphabeticalHintText => new HintViewModel(ResolveText(SortingMode.ALPHABETICAL, "Sort troops by A-Z alphabetical order", "Sort troops by Z-A alphabetical order"));

        [DataSourceProperty]
        public string SortTypeText => ResolveText(SortingMode.TYPE, "T+", "T-");

        [DataSourceProperty]
        public HintViewModel SortTypeHintText => new HintViewModel(ResolveText(SortingMode.TYPE, "Sort troops by ascending type order (Cav, Rng Cav, Inf, Rng)", "Sort troops by descending type order (Rng, Inf, Rng Cav, Cav)"));

        [DataSourceProperty]
        public string SortGroupText => ResolveText(SortingMode.GROUP, "G+", "G-");

        [DataSourceProperty]
        public HintViewModel SortGroupHintText => new HintViewModel(ResolveText(SortingMode.GROUP, "Sort troops by ascending group order (Formation)", "Sort troops by descending group order (Formation)"));

        [DataSourceProperty]
        public string SortTierText => ResolveText(SortingMode.TIER, "T+", "T-");

        [DataSourceProperty]
        public HintViewModel SortTierHintText => new HintViewModel(ResolveText(SortingMode.TIER, "Sort troops by ascending tier order", "Sort troops by descending tier order"));

        [DataSourceProperty]
        public string UpgradableOnTopText => _sorter == null ? "NULL" : _sorter.UpgradableOnTop ? "UN" : "UT";

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText => new HintViewModel(_sorter == null ? "NULL" : _sorter.UpgradableOnTop ? "Place upgradable troops normally" : "Place upgradable troops on top");

        private string ResolveText(SortingMode sortingMode, string ascendingText, string descendingText) {
            if (_sorter == null) return "NULL";

            return _sorter.CurrentlySortingMode == sortingMode && _sorter.SortDirection == SortingDirection.ASCENDING ? descendingText : ascendingText;
        }

        public override void Refresh() {
            UpdateView();
        }

        [DataSourceMethod]
        public void ExecuteSortAlphabetical() {
            Helpers.DebugMessage("Alphabetical Sort Pressed");
            SortParty();
        }

        [DataSourceMethod]
        public void ExecuteSortType() {
            Helpers.DebugMessage("Type Sort Pressed");
            SortParty(SortingMode.TYPE);
        }

        [DataSourceMethod]
        public void ExecuteSortGroup() {
            Helpers.DebugMessage("Group Sort Pressed");
            SortParty(SortingMode.GROUP);
        }

        [DataSourceMethod]
        public void ExecuteSortTier() {
            Helpers.DebugMessage("Tier Sort Pressed");
            SortParty(SortingMode.TIER);
        }

        [DataSourceMethod]
        public void ExecuteUpgradableOnTop() {
            Helpers.DebugMessage("Upgradable on top toggled");
            _sorter.UpgradableOnTop = !_sorter.UpgradableOnTop;
            SortParty(_sorter.CurrentlySortingMode, true);
        }

        private void SortParty(SortingMode sortingMode = SortingMode.ALPHABETICAL, bool skipDirectionFlip = false) {
            FieldInfo partyScreenLogicField = _vm.GetType().BaseType?.GetField("_partyScreenLogic", BindingFlags.NonPublic | BindingFlags.Instance);
            PartyScreenLogic partyScreenLogic = (PartyScreenLogic) partyScreenLogicField?.GetValue(_vm);
            if (partyScreenLogic == null) return;

            _sorter.UpdateSortingMode(sortingMode, skipDirectionFlip);
            SortRoster(partyScreenLogic.PrisonerRosters[1], _vm.MainPartyPrisoners, newPartyList => { _vm.MainPartyPrisoners = newPartyList; });
            SortRoster(partyScreenLogic.MemberRosters[1], _vm.MainPartyTroops, newPartyList => { _vm.MainPartyTroops = newPartyList; });

            UpdateView();
        }

        private void SortRoster(TroopRoster troopRoster, ICollection<PartyCharacterVM> partyList, Action<MBBindingList<PartyCharacterVM>> apply) {
            FieldInfo troopRosterDataField = troopRoster.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            TroopRosterElement[] originalTroops = (TroopRosterElement[]) troopRosterDataField?.GetValue(troopRoster);
            if (originalTroops == null) return;

            List<TroopRosterElement> originalTroopList = originalTroops.Where(x => x.Character != null).ToList();
            List<TroopRosterElement> sortedTroops = originalTroopList.Where(x => !x.Character.IsHero).ToList();
            List<TroopRosterElement> heroTroops = originalTroopList.Where(x => x.Character.IsHero && !x.Character.IsPlayerCharacter).ToList();
            TroopRosterElement player = originalTroopList.FirstOrDefault(x => x.Character.IsPlayerCharacter);

            _sorter.Sort(ref sortedTroops, ref heroTroops);
            if (heroTroops.Count > 0) sortedTroops.InsertRange(0, heroTroops);
            if (player.Character != null) sortedTroops.Insert(0, player);
            troopRosterDataField.SetValue(troopRoster, sortedTroops.ToArray());

            List<PartyCharacterVM> tempTroopList = partyList.ToList();
            MBBindingList<PartyCharacterVM> newTroopList = new MBBindingList<PartyCharacterVM>();
            partyList.Clear();
            foreach (PartyCharacterVM troop in sortedTroops.Select(sortedTroop => tempTroopList.FirstOrDefault(x => x.Troop.Character.ToString() == sortedTroop.Character.ToString()))) {
                newTroopList.Add(troop);
            }

            apply(newTroopList);
        }

        private void UpdateView() {
            _vm.OnPropertyChanged(nameof(SortAlphabeticalText));
            _vm.OnPropertyChanged(nameof(SortAlphabeticalHintText));
            _vm.OnPropertyChanged(nameof(SortTypeText));
            _vm.OnPropertyChanged(nameof(SortTypeHintText));
            _vm.OnPropertyChanged(nameof(SortGroupText));
            _vm.OnPropertyChanged(nameof(SortGroupHintText));
            _vm.OnPropertyChanged(nameof(SortTierText));
            _vm.OnPropertyChanged(nameof(SortTierHintText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopHintText));
        }
    }
}
