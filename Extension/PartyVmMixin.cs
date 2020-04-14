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
using YAPO.Global;
using YAPO.Services;
using YAPO.UI;

namespace YAPO {
    [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PartyVmMixin : BaseViewModelMixin<PartyVM> {
        private readonly TroopSorterService _otherTroopSorterService;
        private readonly TroopSorterService _partyTroopSorterService;
        private PartyScreenLogic _partyScreenLogic;
        private MBBindingList<SortByModeOptionVm> _partySortByModeOptionVms;
        private MBBindingList<SortByModeOptionVm> _partyThenByModeOptionVms;
        private MBBindingList<SortByModeOptionVm> _otherSortByModeOptionVms;
        private MBBindingList<SortByModeOptionVm> _otherThenByModeOptionVms;

        public PartyVmMixin(PartyVM viewModel) : base(viewModel) {
            _partyTroopSorterService = MBObjectManager.Instance.GetObject<TroopSorterService>(x => x.SortSide == SortSide.PARTY);
            if (_partyTroopSorterService == null) {
                _partyTroopSorterService = MBObjectManager.Instance.CreateObject<TroopSorterService>();
                _partyTroopSorterService.SortSide = SortSide.PARTY;
            }

            _otherTroopSorterService = MBObjectManager.Instance.GetObject<TroopSorterService>(x => x.SortSide == SortSide.OTHER);
            if (_otherTroopSorterService == null) {
                _otherTroopSorterService = MBObjectManager.Instance.CreateObject<TroopSorterService>();
                _otherTroopSorterService.SortSide = SortSide.OTHER;
            }

            _partySortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
            foreach (SortMode sortMode in ((SortMode[]) Enum.GetValues(typeof(SortMode))).Where(x => x != SortMode.NONE)) {
                _partySortByModeOptionVms.Add(new SortByModeOptionVm(this, sortMode));
            }

            _partyThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
            foreach (SortMode sortMode in Enum.GetValues(typeof(SortMode))) {
                _partyThenByModeOptionVms.Add(new SortByModeOptionVm(this, sortMode));
            }

            _otherSortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
            foreach (SortMode sortMode in ((SortMode[]) Enum.GetValues(typeof(SortMode))).Where(x => x != SortMode.NONE)) {
                _otherSortByModeOptionVms.Add(new SortByModeOptionVm(this, sortMode));
            }

            _otherThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
            foreach (SortMode sortMode in Enum.GetValues(typeof(SortMode))) {
                _otherThenByModeOptionVms.Add(new SortByModeOptionVm(this, sortMode));
            }

            CurrentPartySortByMode = _partyTroopSorterService.CurrentSortByMode;
            CurrentPartySortByModeText = CurrentPartySortByMode.AsString();
            CurrentPartyThenByMode = _partyTroopSorterService.CurrentThenByMode;
            CurrentPartyThenByModeText = CurrentPartyThenByMode.AsString();
            CurrentOtherSortByMode = _otherTroopSorterService.CurrentSortByMode;
            CurrentOtherSortByModeText = CurrentOtherSortByMode.AsString();
            CurrentOtherThenByMode = _otherTroopSorterService.CurrentThenByMode;
            CurrentOtherThenByModeText = CurrentOtherThenByMode.AsString();
        }

        // Dropdowns
        [DataSourceProperty]
        public HintViewModel SortByHintText => new HintViewModel(Strings.SORT_BY_HINT_TEXT);

        [DataSourceProperty]
        public HintViewModel ThenByHintText => new HintViewModel(Strings.THEN_BY_HINT_TEXT);

        // Party Sort Buttons
        [DataSourceProperty]
        public string SortPartyAscendingText => Strings.SORT_TEXT_ASCENDING;

        [DataSourceProperty]
        public HintViewModel SortPartyAscendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_ASCENDING);

        [DataSourceProperty]
        public string SortPartyDescendingText => Strings.SORT_TEXT_DESCENDING;

        [DataSourceProperty]
        public HintViewModel SortPartyDescendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_DESCENDING);

        // Other Sort Buttons
        [DataSourceProperty]
        public string SortOtherAscendingText => Strings.SORT_TEXT_ASCENDING;

        [DataSourceProperty]
        public HintViewModel SortOtherAscendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_ASCENDING);

        [DataSourceProperty]
        public string SortOtherDescendingText => Strings.SORT_TEXT_DESCENDING;

        [DataSourceProperty]
        public HintViewModel SortOtherDescendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_DESCENDING);

        // Party Sort Option Buttons
        [DataSourceProperty]
        public string PartySortOrderOppositeText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);
        
        [DataSourceProperty]
        public string UpgradableOnTopText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        // Other Sort Option Buttons
        [DataSourceProperty]
        public string OtherSortOrderOppositeText => _otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeHintText => new HintViewModel(_otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        // Party Sort Dropdowns
        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> PartySortByModeOptionVms {
            get => _partySortByModeOptionVms;
            set {
                if (value == _partySortByModeOptionVms) {
                    return;
                }

                _partySortByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(PartySortByModeOptionVms));
            }
        }

        [DataSourceProperty]
        public string CurrentPartySortByModeText { get; set; }

        public SortMode CurrentPartySortByMode {
            get => _partyTroopSorterService.CurrentSortByMode;
            set {
                if (value == _partyTroopSorterService.CurrentSortByMode) {
                    return;
                }

                SortMode currentSortByMode = CurrentPartySortByMode;
                _partyTroopSorterService.CurrentSortByMode = value;
                CurrentPartySortByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentPartySortByModeText));

                if (CurrentPartyThenByMode == value) {
                    CurrentPartyThenByMode = currentSortByMode;
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> PartyThenByModeOptionVms {
            get => _partyThenByModeOptionVms;
            set {
                if (value == _partyThenByModeOptionVms) {
                    return;
                }

                _partyThenByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(PartyThenByModeOptionVms));
            }
        }

        [DataSourceProperty]
        public string CurrentPartyThenByModeText { get; set; }

        public SortMode CurrentPartyThenByMode {
            get => _partyTroopSorterService.CurrentThenByMode;
            set {
                if (value == _partyTroopSorterService.CurrentThenByMode) {
                    return;
                }

                if (CurrentPartySortByMode == value) {
                    value = CurrentPartyThenByMode;
                    Global.Helpers.Message("Then By should be different from Sort By");
                }

                _partyTroopSorterService.CurrentThenByMode = value;
                CurrentPartyThenByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentPartyThenByModeText));
            }
        }

        // Other Sort Dropdowns
        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> OtherSortByModeOptionVms {
            get => _otherSortByModeOptionVms;
            set {
                if (value == _otherSortByModeOptionVms) {
                    return;
                }

                _otherSortByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(OtherSortByModeOptionVms));
            }
        }

        [DataSourceProperty]
        public string CurrentOtherSortByModeText { get; set; }

        public SortMode CurrentOtherSortByMode {
            get => _otherTroopSorterService.CurrentSortByMode;
            set {
                if (value == _otherTroopSorterService.CurrentSortByMode) {
                    return;
                }

                SortMode currentSortByMode = CurrentOtherSortByMode;
                _otherTroopSorterService.CurrentSortByMode = value;
                CurrentOtherSortByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentOtherSortByModeText));

                if (CurrentOtherThenByMode == value) {
                    CurrentOtherThenByMode = currentSortByMode;
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> OtherThenByModeOptionVms {
            get => _otherThenByModeOptionVms;
            set {
                if (value == _otherThenByModeOptionVms) {
                    return;
                }

                _otherThenByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(OtherThenByModeOptionVms));
            }
        }

        [DataSourceProperty]
        public string CurrentOtherThenByModeText { get; set; }

        public SortMode CurrentOtherThenByMode {
            get => _otherTroopSorterService.CurrentThenByMode;
            set {
                if (value == _otherTroopSorterService.CurrentThenByMode) {
                    return;
                }

                if (CurrentOtherSortByMode == value) {
                    value = SortMode.NONE;
                    Global.Helpers.Message("Then By should be different from Sort By");
                }

                _otherTroopSorterService.CurrentThenByMode = value;
                CurrentOtherThenByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentOtherThenByModeText));
            }
        }

        // Action Buttons
        [DataSourceProperty]
        public HintViewModel ActionUpgradeHintText => new HintViewModel(Strings.UPGRADE_HINT_TEXT);

        [DataSourceProperty]
        public HintViewModel ActionRecruitHintText => new HintViewModel(Strings.RECRUIT_HINT_TEXT);

        // Party Sort Buttons
        [DataSourceMethod]
        public void ExecuteSortPartyAscending() {
            Global.Helpers.DebugMessage("Party Sort Ascending Pressed");
            SortParty(SortDirection.ASCENDING);
        }

        [DataSourceMethod]
        public void ExecuteSortPartyDescending() {
            Global.Helpers.DebugMessage("Party Sort Descending Pressed");
            SortParty(SortDirection.DESCENDING);
        }

        // Party Sort Option Buttons
        [DataSourceMethod]
        public void ExecutePartySortOrderOpposite() {
            Global.Helpers.DebugMessage("Party sort order oppiste toggled");
            _partyTroopSorterService.SortOrderOpposite = !_partyTroopSorterService.SortOrderOpposite;
            SortParty(_partyTroopSorterService.SortDirection);
        }
        
        [DataSourceMethod]
        public void ExecuteUpgradableOnTop() {
            Global.Helpers.DebugMessage("Party Upgradable on top toggled");
            _partyTroopSorterService.UpgradableOnTop = !_partyTroopSorterService.UpgradableOnTop;
            SortParty(_partyTroopSorterService.SortDirection, true);
        }

        // Other Buttons
        [DataSourceMethod]
        public void ExecuteSortOtherAscending() {
            Global.Helpers.DebugMessage("Other Sort Ascending Pressed");
            SortOther(SortDirection.ASCENDING);
        }

        [DataSourceMethod]
        public void ExecuteSortOtherDescending() {
            Global.Helpers.DebugMessage("Other Sort Descending Pressed");
            SortOther(SortDirection.DESCENDING);
        }

        // Other Sort Option Buttons
        [DataSourceMethod]
        public void ExecuteOtherSortOrderOpposite() {
            Global.Helpers.DebugMessage("Other sort order oppiste toggled");
            _otherTroopSorterService.SortOrderOpposite = !_otherTroopSorterService.SortOrderOpposite;
            SortOther(_otherTroopSorterService.SortDirection);
        }

        // Action Buttons
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

        public override void Refresh() {
            UpdateView();
        }

        private void GetPartyScreenLogic() {
            if (_partyScreenLogic != null) return;

            FieldInfo partyScreenLogicField = _vm.GetType().BaseType?.GetField("_partyScreenLogic", BindingFlags.NonPublic | BindingFlags.Instance);
            _partyScreenLogic = (PartyScreenLogic) partyScreenLogicField?.GetValue(_vm);
        }

        private void SortParty(SortDirection sortDirection, bool skipDirectionUpdate = false) {
            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _partyTroopSorterService.UpdateSortingDirection(sortDirection);
            SortRoster(_partyTroopSorterService, _partyScreenLogic.PrisonerRosters[1], _vm.MainPartyPrisoners, newPartyList => { _vm.MainPartyPrisoners = newPartyList; });
            SortRoster(_partyTroopSorterService, _partyScreenLogic.MemberRosters[1], _vm.MainPartyTroops, newPartyList => { _vm.MainPartyTroops = newPartyList; });
            UpdateView();
        }

        private void SortOther(SortDirection sortDirection, bool skipDirectionUpdate = false) {
            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _otherTroopSorterService.UpdateSortingDirection(sortDirection);
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
            _vm.OnPropertyChanged(nameof(SortByHintText));
            _vm.OnPropertyChanged(nameof(ThenByHintText));
            _vm.OnPropertyChanged(nameof(SortPartyAscendingText));
            _vm.OnPropertyChanged(nameof(SortPartyAscendingHintText));
            _vm.OnPropertyChanged(nameof(SortPartyDescendingText));
            _vm.OnPropertyChanged(nameof(SortPartyDescendingHintText));
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeText));
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeHintText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopHintText));
            _vm.OnPropertyChanged(nameof(SortOtherAscendingText));
            _vm.OnPropertyChanged(nameof(SortOtherAscendingHintText));
            _vm.OnPropertyChanged(nameof(SortOtherDescendingText));
            _vm.OnPropertyChanged(nameof(SortOtherDescendingHintText));
            _vm.OnPropertyChanged(nameof(OtherSortOrderOppositeText));
            _vm.OnPropertyChanged(nameof(OtherSortOrderOppositeHintText));
            _vm.OnPropertyChanged(nameof(PartySortByModeOptionVms));
            _vm.OnPropertyChanged(nameof(CurrentPartySortByModeText));
            _vm.OnPropertyChanged(nameof(PartyThenByModeOptionVms));
            _vm.OnPropertyChanged(nameof(CurrentPartyThenByModeText));
            _vm.OnPropertyChanged(nameof(OtherSortByModeOptionVms));
            _vm.OnPropertyChanged(nameof(CurrentOtherSortByModeText));
            _vm.OnPropertyChanged(nameof(OtherThenByModeOptionVms));
            _vm.OnPropertyChanged(nameof(CurrentOtherThenByModeText));
            _vm.OnPropertyChanged(nameof(ActionUpgradeHintText));
            _vm.OnPropertyChanged(nameof(ActionRecruitHintText));
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
