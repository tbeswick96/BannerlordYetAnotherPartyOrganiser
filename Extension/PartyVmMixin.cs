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
using TroopManager.Global;
using TroopManager.Services;
using UIExtenderLib;
using UIExtenderLib.ViewModel;

namespace TroopManager {
    [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PartyVmMixin : BaseViewModelMixin<PartyVM> {
        private readonly TroopSorterService _otherTroopSorterService;
        private readonly TroopSorterService _partyTroopSorterService;
        private PartyScreenLogic _partyScreenLogic;

        public PartyVmMixin(PartyVM viewModel) : base(viewModel) {
            _partyTroopSorterService = MBObjectManager.Instance.GetObject<TroopSorterService>(x => x.SortSide == SortSide.PARTY);
            if (_partyTroopSorterService == null) {
                _partyTroopSorterService = MBObjectManager.Instance.CreateObject<TroopSorterService>();
                _partyTroopSorterService.SortSide = SortSide.PARTY;
            }

            _otherTroopSorterService = MBObjectManager.Instance.GetObject<TroopSorterService>(x => x.SortSide == SortSide.OTHER);
            if (_otherTroopSorterService == null) {
                _otherTroopSorterService = MBObjectManager.Instance.CreateObject<TroopSorterService>();
                _partyTroopSorterService.SortSide = SortSide.OTHER;
            }
        }

        // Party Buttons
        [DataSourceProperty]
        public string SortAlphabeticalPartyText => ResolveText(_partyTroopSorterService, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_TEXT_ASCENDING, Strings.ALPHABETICAL_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortAlphabeticalPartyHintText => new HintViewModel(ResolveText(_partyTroopSorterService, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_HINT_TEXT_ASCENDING, Strings.ALPHABETICAL_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTypePartyText => ResolveText(_partyTroopSorterService, SortMode.TYPE, Strings.TYPE_TEXT_ASCENDING, Strings.TYPE_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTypePartyHintText => new HintViewModel(ResolveText(_partyTroopSorterService, SortMode.TYPE, Strings.TYPE_HINT_TEXT_ASCENDING, Strings.TYPE_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortGroupPartyText => ResolveText(_partyTroopSorterService, SortMode.GROUP, Strings.GROUP_TEXT_ASCENDING, Strings.GROUP_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortGroupPartyHintText => new HintViewModel(ResolveText(_partyTroopSorterService, SortMode.GROUP, Strings.GROUP_HINT_TEXT_ASCENDING, Strings.GROUP_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTierPartyText => ResolveText(_partyTroopSorterService, SortMode.TIER, Strings.TIER_TEXT_ASCENDING, Strings.TIER_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTierPartyHintText => new HintViewModel(ResolveText(_partyTroopSorterService, SortMode.TIER, Strings.TIER_HINT_TEXT_ASCENDING, Strings.TIER_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string UpgradableOnTopText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        // Other buttons
        [DataSourceProperty]
        public string SortAlphabeticalOtherText => ResolveText(_otherTroopSorterService, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_TEXT_ASCENDING, Strings.ALPHABETICAL_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortAlphabeticalOtherHintText => new HintViewModel(ResolveText(_otherTroopSorterService, SortMode.ALPHABETICAL, Strings.ALPHABETICAL_HINT_TEXT_ASCENDING, Strings.ALPHABETICAL_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTypeOtherText => ResolveText(_otherTroopSorterService, SortMode.TYPE, Strings.TYPE_TEXT_ASCENDING, Strings.TYPE_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTypeOtherHintText => new HintViewModel(ResolveText(_otherTroopSorterService, SortMode.TYPE, Strings.TYPE_HINT_TEXT_ASCENDING, Strings.TYPE_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortGroupOtherText => ResolveText(_otherTroopSorterService, SortMode.GROUP, Strings.GROUP_TEXT_ASCENDING, Strings.GROUP_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortGroupOtherHintText => new HintViewModel(ResolveText(_otherTroopSorterService, SortMode.GROUP, Strings.GROUP_HINT_TEXT_ASCENDING, Strings.GROUP_HINT_TEXT_DESCENDING));

        [DataSourceProperty]
        public string SortTierOtherText => ResolveText(_otherTroopSorterService, SortMode.TIER, Strings.TIER_TEXT_ASCENDING, Strings.TIER_TEXT_DESCENDING);

        [DataSourceProperty]
        public HintViewModel SortTierOtherHintText => new HintViewModel(ResolveText(_otherTroopSorterService, SortMode.TIER, Strings.TIER_HINT_TEXT_ASCENDING, Strings.TIER_HINT_TEXT_DESCENDING));

        // Action buttons
        [DataSourceProperty]
        public HintViewModel ActionUpgradeHintText => new HintViewModel(Strings.UPGRADE_HINT_TEXT);

        [DataSourceProperty]
        public HintViewModel ActionRecruitHintText => new HintViewModel(Strings.RECRUIT_HINT_TEXT);

        // Party Buttons
        [DataSourceMethod]
        public void ExecuteSortAlphabeticalParty() {
            Global.Helpers.DebugMessage("Party Alphabetical Sort Pressed");
            SortParty(SortMode.ALPHABETICAL);
        }

        [DataSourceMethod]
        public void ExecuteSortTypeParty() {
            Global.Helpers.DebugMessage("Party Type Sort Pressed");
            SortParty(SortMode.TYPE);
        }

        [DataSourceMethod]
        public void ExecuteSortGroupParty() {
            Global.Helpers.DebugMessage("Party Group Sort Pressed");
            SortParty(SortMode.GROUP);
        }

        [DataSourceMethod]
        public void ExecuteSortTierParty() {
            Global.Helpers.DebugMessage("Party Tier Sort Pressed");
            SortParty(SortMode.TIER);
        }

        [DataSourceMethod]
        public void ExecuteUpgradableOnTop() {
            Global.Helpers.DebugMessage("Party Upgradable on top toggled");
            _partyTroopSorterService.UpgradableOnTop = !_partyTroopSorterService.UpgradableOnTop;
            SortParty(_partyTroopSorterService.CurrentSortMode, true);
        }

        // Other buttons
        [DataSourceMethod]
        public void ExecuteSortAlphabeticalOther() {
            Global.Helpers.DebugMessage("Other Alphabetical Sort Pressed");
            SortOther(SortMode.ALPHABETICAL);
        }

        [DataSourceMethod]
        public void ExecuteSortTypeOther() {
            Global.Helpers.DebugMessage("Other Type Sort Pressed");
            SortOther(SortMode.TYPE);
        }

        [DataSourceMethod]
        public void ExecuteSortGroupOther() {
            Global.Helpers.DebugMessage("Other Group Sort Pressed");
            SortOther(SortMode.GROUP);
        }

        [DataSourceMethod]
        public void ExecuteSortTierOther() {
            Global.Helpers.DebugMessage("Other Tier Sort Pressed");
            SortOther(SortMode.TIER);
        }

        // Action buttons
        [DataSourceMethod]
        public void ExecuteActionUpgrade() {
            Global.Helpers.DebugMessage("Action upgrade Pressed");
            UpgradeTroops();
        }

        [DataSourceMethod]
        public void ExecuteActionRecruit() {
            Global.Helpers.DebugMessage("Action recruit Pressed");
            RecruitPrisoners();
        }

        private static string ResolveText(TroopSorterService troopSorterService, SortMode targetSortMode, string ascendingText, string descendingText) {
            if (troopSorterService == null) return "NULL";

            return troopSorterService.CurrentSortMode == targetSortMode && troopSorterService.SortDirection == SortDirection.ASCENDING ? descendingText : ascendingText;
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
            _partyTroopSorterService.UpdateSortingMode(sortMode, skipDirectionFlip);
            SortRoster(_partyTroopSorterService, _partyScreenLogic.PrisonerRosters[1], _vm.MainPartyPrisoners, newPartyList => { _vm.MainPartyPrisoners = newPartyList; });
            SortRoster(_partyTroopSorterService, _partyScreenLogic.MemberRosters[1], _vm.MainPartyTroops, newPartyList => { _vm.MainPartyTroops = newPartyList; });
            UpdateView();
        }

        private void SortOther(SortMode sortMode, bool skipDirectionFlip = false) {
            GetPartyScreenLogic();
            _otherTroopSorterService.UpdateSortingMode(sortMode, skipDirectionFlip);
            SortRoster(_otherTroopSorterService, _partyScreenLogic.PrisonerRosters[0], _vm.OtherPartyPrisoners, newPartyList => { _vm.OtherPartyPrisoners = newPartyList; });
            SortRoster(_otherTroopSorterService, _partyScreenLogic.MemberRosters[0], _vm.OtherPartyTroops, newPartyList => { _vm.OtherPartyTroops = newPartyList; });
            UpdateView();
        }

        private static void SortRoster(TroopSorterService troopSorterService, TroopRoster troopRoster, ICollection<PartyCharacterVM> partyList, Action<MBBindingList<PartyCharacterVM>> apply) {
            FieldInfo troopRosterDataField = troopRoster.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            TroopRosterElement[] originalTroops = (TroopRosterElement[]) troopRosterDataField?.GetValue(troopRoster);
            if (originalTroops == null) return;

            List<TroopRosterElement> originalTroopList = originalTroops.Where(x => x.Character != null).ToList();
            List<TroopRosterElement> sortedTroops = originalTroopList.Where(x => !x.Character.IsHero).ToList();
            List<TroopRosterElement> heroTroops = originalTroopList.Where(x => x.Character.IsHero && !x.Character.IsPlayerCharacter).ToList();
            TroopRosterElement player = originalTroopList.FirstOrDefault(x => x.Character.IsPlayerCharacter);

            troopSorterService.Sort(ref sortedTroops, ref heroTroops);
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

        private void UpgradeTroops() {
            GetPartyScreenLogic();
            (int upgradedTotal, int upgradedTypes, int multiPathSkipped) = TroopActionService.UpgradeTroops(_vm, _partyScreenLogic);
            if (upgradedTotal == 0) return;

            _vm.CurrentCharacter = _vm.MainPartyTroops[0];
            RefreshInformation();
            Global.Helpers.Message($"Upgraded {upgradedTotal} troops over {upgradedTypes} types. {multiPathSkipped} troop types with mulit-path upgrades were skipped");
        }

        private void RecruitPrisoners() {
            GetPartyScreenLogic();
            (int recruitedTotal, int recruitedTypes) = TroopActionService.RecruitPrisoners(_vm, _partyScreenLogic);
            if (recruitedTotal == 0) return;
            
            _vm.CurrentCharacter = _vm.MainPartyTroops[0];
            RefreshInformation();
            Global.Helpers.Message($"Recruited {recruitedTotal} prisoners over {recruitedTypes} types");
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

        private void RefreshInformation() {
            MethodInfo refreshPrisonersRecruitable = _vm.GetType().BaseType?.GetMethod("RefreshPrisonersRecruitable", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPrisonersRecruitable?.Invoke(_vm, new object[0]);
            MethodInfo refreshTopInformation = _vm.GetType().BaseType?.GetMethod("RefreshTopInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshTopInformation?.Invoke(_vm, new object[0]);
            MethodInfo refreshPartyInformation = _vm.GetType().BaseType?.GetMethod("RefreshPartyInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPartyInformation?.Invoke(_vm, new object[0]);
        }
    }
}
