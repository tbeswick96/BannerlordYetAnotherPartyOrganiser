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
        private readonly Sorter _sorterOther;
        private readonly Sorter _sorterParty;
        private PartyScreenLogic _partyScreenLogic;

        public PartyVmMixin(PartyVM viewModel) : base(viewModel) {
            _sorterParty = MBObjectManager.Instance.GetObject<Sorter>(x => x.SortSide == SortSide.PARTY);
            if (_sorterParty == null) {
                _sorterParty = MBObjectManager.Instance.CreateObject<Sorter>();
                _sorterParty.SortSide = SortSide.PARTY;
            }

            _sorterOther = MBObjectManager.Instance.GetObject<Sorter>(x => x.SortSide == SortSide.OTHER);
            if (_sorterOther == null) {
                _sorterOther = MBObjectManager.Instance.CreateObject<Sorter>();
                _sorterParty.SortSide = SortSide.OTHER;
            }
        }

        // Party Buttons
        [DataSourceProperty]
        public string SortAlphabeticalPartyText => ResolveText(_sorterParty, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_TEXT_ASCENDING, Strings.ALPHABETICAL_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortAlphabeticalPartyHintText => new HintViewModel(ResolveText(_sorterParty, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_HINT_TEXT_ASCENDING, Strings.ALPHABETICAL_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTypePartyText => ResolveText(_sorterParty, SortMode.TYPE, Strings.TYPE_TEXT_ASCENDING, Strings.TYPE_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTypePartyHintText => new HintViewModel(ResolveText(_sorterParty, SortMode.TYPE, Strings.TYPE_HINT_TEXT_ASCENDING, Strings.TYPE_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortGroupPartyText => ResolveText(_sorterParty, SortMode.GROUP, Strings.GROUP_TEXT_ASCENDING, Strings.GROUP_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortGroupPartyHintText => new HintViewModel(ResolveText(_sorterParty, SortMode.GROUP, Strings.GROUP_HINT_TEXT_ASCENDING, Strings.GROUP_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTierPartyText => ResolveText(_sorterParty, SortMode.TIER, Strings.TIER_TEXT_ASCENDING, Strings.TIER_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTierPartyHintText => new HintViewModel(ResolveText(_sorterParty, SortMode.TIER, Strings.TIER_HINT_TEXT_ASCENDING, Strings.TIER_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string UpgradableOnTopText => _sorterParty == null ? "NULL" : _sorterParty.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText => new HintViewModel(_sorterParty == null ? "NULL" : _sorterParty.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        // Other buttons
        [DataSourceProperty]
        public string SortAlphabeticalOtherText => ResolveText(_sorterOther, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_TEXT_ASCENDING, Strings.ALPHABETICAL_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortAlphabeticalOtherHintText => new HintViewModel(ResolveText(_sorterOther, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_HINT_TEXT_ASCENDING, Strings.ALPHABETICAL_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTypeOtherText => ResolveText(_sorterOther, SortMode.TYPE, Strings.TYPE_TEXT_ASCENDING, Strings.TYPE_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTypeOtherHintText => new HintViewModel(ResolveText(_sorterOther, SortMode.TYPE, Strings.TYPE_HINT_TEXT_ASCENDING, Strings.TYPE_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortGroupOtherText => ResolveText(_sorterOther, SortMode.GROUP, Strings.GROUP_TEXT_ASCENDING, Strings.GROUP_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortGroupOtherHintText => new HintViewModel(ResolveText(_sorterOther, SortMode.GROUP, Strings.GROUP_HINT_TEXT_ASCENDING, Strings.GROUP_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTierOtherText => ResolveText(_sorterOther, SortMode.TIER, Strings.TIER_TEXT_ASCENDING, Strings.TIER_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTierOtherHintText => new HintViewModel(ResolveText(_sorterOther, SortMode.TIER, Strings.TIER_HINT_TEXT_ASCENDING, Strings.TIER_HINT_TEXT_DESCENDING));

        // Party Buttons
        [DataSourceMethod]
        public void ExecuteSortAlphabeticalParty() {
            Helpers.DebugMessage("Party Alphabetical Sort Pressed");
            SortParty(SortMode.ALPHABETICAL);
        }

        [DataSourceMethod]
        public void ExecuteSortTypeParty() {
            Helpers.DebugMessage("Party Type Sort Pressed");
            SortParty(SortMode.TYPE);
        }

        [DataSourceMethod]
        public void ExecuteSortGroupParty() {
            Helpers.DebugMessage("Party Group Sort Pressed");
            SortParty(SortMode.GROUP);
        }

        [DataSourceMethod]
        public void ExecuteSortTierParty() {
            Helpers.DebugMessage("Party Tier Sort Pressed");
            SortParty(SortMode.TIER);
        }

        [DataSourceMethod]
        public void ExecuteUpgradableOnTop() {
            Helpers.DebugMessage("Party Upgradable on top toggled");
            _sorterParty.UpgradableOnTop = !_sorterParty.UpgradableOnTop;
            SortParty(_sorterParty.CurrentSortMode, true);
        }

        // Other buttons
        [DataSourceMethod]
        public void ExecuteSortAlphabeticalOther() {
            Helpers.DebugMessage("Other Alphabetical Sort Pressed");
            SortOther(SortMode.ALPHABETICAL);
        }

        [DataSourceMethod]
        public void ExecuteSortTypeOther() {
            Helpers.DebugMessage("Other Type Sort Pressed");
            SortOther(SortMode.TYPE);
        }

        [DataSourceMethod]
        public void ExecuteSortGroupOther() {
            Helpers.DebugMessage("Other Group Sort Pressed");
            SortOther(SortMode.GROUP);
        }

        [DataSourceMethod]
        public void ExecuteSortTierOther() {
            Helpers.DebugMessage("Other Tier Sort Pressed");
            SortOther(SortMode.TIER);
        }

        private static string ResolveText(Sorter sorter, SortMode targetSortMode, string ascendingText, string descendingText) {
            if (sorter == null) return "NULL";

            return sorter.CurrentSortMode == targetSortMode && sorter.SortDirection == SortDirection.ASCENDING ? descendingText : ascendingText;
        }

        public override void Refresh() {
            UpdateView();
        }

        private void GetPartyScreenLogic() {
            if (_partyScreenLogic != null) return;

            FieldInfo partyScreenLogicField = _vm.GetType().BaseType?.GetField("_partyScreenLogic", BindingFlags.NonPublic | BindingFlags.Instance);
            _partyScreenLogic = (PartyScreenLogic) partyScreenLogicField?.GetValue(_vm);
        }

        private void SortParty(SortMode sortMode, bool skipDirectionFlip = false) {
            GetPartyScreenLogic();
            _sorterParty.UpdateSortingMode(sortMode, skipDirectionFlip);
            SortRoster(_sorterParty, _partyScreenLogic.PrisonerRosters[1], _vm.MainPartyPrisoners, newPartyList => { _vm.MainPartyPrisoners = newPartyList; });
            SortRoster(_sorterParty, _partyScreenLogic.MemberRosters[1], _vm.MainPartyTroops, newPartyList => { _vm.MainPartyTroops = newPartyList; });
            UpdateView();
        }

        private void SortOther(SortMode sortMode, bool skipDirectionFlip = false) {
            GetPartyScreenLogic();
            _sorterOther.UpdateSortingMode(sortMode, skipDirectionFlip);
            SortRoster(_sorterOther, _partyScreenLogic.PrisonerRosters[0], _vm.OtherPartyPrisoners, newPartyList => { _vm.OtherPartyPrisoners = newPartyList; });
            SortRoster(_sorterOther, _partyScreenLogic.MemberRosters[0], _vm.OtherPartyTroops, newPartyList => { _vm.OtherPartyTroops = newPartyList; });
            UpdateView();
        }

        private static void SortRoster(Sorter sorter, TroopRoster troopRoster, ICollection<PartyCharacterVM> partyList, Action<MBBindingList<PartyCharacterVM>> apply) {
            FieldInfo troopRosterDataField = troopRoster.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            TroopRosterElement[] originalTroops = (TroopRosterElement[]) troopRosterDataField?.GetValue(troopRoster);
            if (originalTroops == null) return;

            List<TroopRosterElement> originalTroopList = originalTroops.Where(x => x.Character != null).ToList();
            List<TroopRosterElement> sortedTroops = originalTroopList.Where(x => !x.Character.IsHero).ToList();
            List<TroopRosterElement> heroTroops = originalTroopList.Where(x => x.Character.IsHero && !x.Character.IsPlayerCharacter).ToList();
            TroopRosterElement player = originalTroopList.FirstOrDefault(x => x.Character.IsPlayerCharacter);

            sorter.Sort(ref sortedTroops, ref heroTroops);
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
            _vm.OnPropertyChanged(nameof(SortAlphabeticalPartyText));
            _vm.OnPropertyChanged(nameof(SortAlphabeticalPartyHintText));
            _vm.OnPropertyChanged(nameof(SortTypePartyText));
            _vm.OnPropertyChanged(nameof(SortTypePartyHintText));
            _vm.OnPropertyChanged(nameof(SortGroupPartyText));
            _vm.OnPropertyChanged(nameof(SortGroupPartyHintText));
            _vm.OnPropertyChanged(nameof(SortTierPartyText));
            _vm.OnPropertyChanged(nameof(SortTierPartyHintText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopHintText));
            _vm.OnPropertyChanged(nameof(SortAlphabeticalOtherText));
            _vm.OnPropertyChanged(nameof(SortAlphabeticalOtherHintText));
            _vm.OnPropertyChanged(nameof(SortTypeOtherText));
            _vm.OnPropertyChanged(nameof(SortTypeOtherHintText));
            _vm.OnPropertyChanged(nameof(SortGroupOtherText));
            _vm.OnPropertyChanged(nameof(SortGroupOtherHintText));
            _vm.OnPropertyChanged(nameof(SortTierOtherText));
            _vm.OnPropertyChanged(nameof(SortTierOtherHintText));
        }
    }
}
