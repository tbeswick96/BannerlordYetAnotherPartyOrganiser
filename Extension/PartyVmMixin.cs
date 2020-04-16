using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using UIExtenderLib;
using UIExtenderLib.ViewModel;
using YAPO.Global;
using YAPO.Services;

namespace YAPO {
    [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PartyVmMixin : BaseViewModelMixin<PartyVM> {
        private bool firstRefreshDone = false;
        private readonly TroopSorterService _otherTroopSorterService;
        private readonly TroopSorterService _partyTroopSorterService;
        private MBBindingList<SortByModeOptionVm> _otherSortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _otherThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private PartyScreenLogic _partyScreenLogic;
        private MBBindingList<SortByModeOptionVm> _partySortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _partyThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();

        public PartyVmMixin(PartyVM viewModel) : base(viewModel) {
            States.PartyVmMixin = this;

            _partyTroopSorterService = InitialiseTroopSorterService(SortSide.PARTY);
            _otherTroopSorterService = InitialiseTroopSorterService(SortSide.OTHER);

            InitialiseOptionVmList(_partySortByModeOptionVms);
            InitialiseOptionVmList(_partyThenByModeOptionVms, true);
            InitialiseOptionVmList(_otherSortByModeOptionVms);
            InitialiseOptionVmList(_otherThenByModeOptionVms, true);

            InitialiseOptionDropdowns();
        }

        private static TroopSorterService InitialiseTroopSorterService(SortSide sortSide) {
            TroopSorterService troopSorterService = MBObjectManager.Instance.GetObject<TroopSorterService>(x => x.SortSide == sortSide);
            if (troopSorterService != null) return troopSorterService;

            troopSorterService = MBObjectManager.Instance.CreateObject<TroopSorterService>();
            troopSorterService.SortSide = sortSide;
            return troopSorterService;
        }

        private void InitialiseOptionVmList(ICollection<SortByModeOptionVm> optionVms, bool thenBy = false) {
            foreach (SortMode sortMode in ((SortMode[]) Enum.GetValues(typeof(SortMode))).Where(x => !thenBy && x != SortMode.NONE)) {
                optionVms.Add(new SortByModeOptionVm(this, sortMode));
            }
        }

        private void InitialiseOptionDropdowns() {
            CurrentPartySortByMode = _partyTroopSorterService.CurrentSortByMode == SortMode.NONE ? SortMode.ALPHABETICAL : _partyTroopSorterService.CurrentSortByMode;
            CurrentPartySortByModeText = CurrentPartySortByMode.AsString();
            CurrentPartyThenByMode = _partyTroopSorterService.CurrentThenByMode;
            CurrentPartyThenByModeText = CurrentPartyThenByMode.AsString();
            CurrentOtherSortByMode = _otherTroopSorterService.CurrentSortByMode == SortMode.NONE ? SortMode.ALPHABETICAL : _otherTroopSorterService.CurrentSortByMode;
            CurrentOtherSortByModeText = CurrentOtherSortByMode.AsString();
            CurrentOtherThenByMode = _otherTroopSorterService.CurrentThenByMode;
            CurrentOtherThenByModeText = CurrentOtherThenByMode.AsString();
        }

        public override void Refresh() {
            if (States.PartyVmMixin != this) return;
            
            RefreshView();
            
            Global.Helpers.DebugMessage("Refresh view");
        }

        public void FirstRefresh() {
            if (States.PartyVmMixin != this || firstRefreshDone) return;

            // SortParty(_partyTroopSorterService.SortDirection);
            // SortOther(_partyTroopSorterService.SortDirection);
            
            firstRefreshDone = true;
            Global.Helpers.DebugMessage("First Refresh");
        }

        public override void Destroy() {
            States.PartyVmMixin = null;
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
            RefreshView();
        }

        private void SortOther(SortDirection sortDirection, bool skipDirectionUpdate = false) {
            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _otherTroopSorterService.UpdateSortingDirection(sortDirection);
            SortRoster(_otherTroopSorterService, _partyScreenLogic.PrisonerRosters[0], _vm.OtherPartyPrisoners, newPartyList => { _vm.OtherPartyPrisoners = newPartyList; });
            SortRoster(_otherTroopSorterService, _partyScreenLogic.MemberRosters[0], _vm.OtherPartyTroops, newPartyList => { _vm.OtherPartyTroops = newPartyList; });
            RefreshView();
        }

        private static void SortRoster(TroopSorterService troopSorterService, TroopRoster troopRoster, ICollection<PartyCharacterVM> partyList, Action<MBBindingList<PartyCharacterVM>> apply) {
            FieldInfo troopRosterDataField = troopRoster.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            TroopRosterElement[] originalTroops = (TroopRosterElement[]) troopRosterDataField?.GetValue(troopRoster);
            if (originalTroops == null || originalTroops.All(x => x.Character == null)) return;

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
            RefreshPartyVmInformation();
            RefreshView();
            Global.Helpers.Message($"Upgraded {upgradedTotal} troops over {upgradedTypes} types. {multiPathSkipped} troop types with mulit-path upgrades were skipped. Press 'Apply' to confirm changes");
        }

        private void RecruitPrisoners() {
            GetPartyScreenLogic();
            (int recruitedTotal, int recruitedTypes) = TroopActionService.RecruitPrisoners(_vm, _partyScreenLogic);
            if (recruitedTotal == 0) return;

            _vm.CurrentCharacter = _vm.MainPartyTroops[0];
            RefreshPartyVmInformation();
            RefreshView();
            Global.Helpers.Message($"Recruited {recruitedTotal} prisoners over {recruitedTypes} types. Press 'Apply' to confirm changes");
        }

        private void RefreshView() {
            _vm.OnPropertyChanged(nameof(SortByHintText));
            _vm.OnPropertyChanged(nameof(ThenByHintText));
            _vm.OnPropertyChanged(nameof(SortPartyAscendingText));
            _vm.OnPropertyChanged(nameof(SortPartyAscendingHintText));
            _vm.OnPropertyChanged(nameof(SortPartyDescendingText));
            _vm.OnPropertyChanged(nameof(SortPartyDescendingHintText));
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeText));
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeHintText));
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeDisabledHintText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopHintText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopDisabledHintText));
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
            _vm.OnPropertyChanged(nameof(ActionUpgradeDisabledHintText));
            _vm.OnPropertyChanged(nameof(ActionRecruitHintText));
            _vm.OnPropertyChanged(nameof(ActionRecruitDisabledHintText));
            RefreshPartyScreenWidgetStates();
        }

        private void RefreshPartyScreenWidgetStates() {
            if (States.PartyScreenWidget == null) return;

            List<Widget> allChildren = States.PartyScreenWidget.AllChildren.ToList();

            Widget partySortOrderOppositeButtonWidget = allChildren.FirstOrDefault(x => x.Id == "PartySortOrderOppositeButtonWidget");
            Widget partySortOrderOppositeButtonWidgetDisabled = allChildren.FirstOrDefault(x => x.Id == "PartySortOrderOppositeButtonWidgetDisabled");
            if (partySortOrderOppositeButtonWidget != null && partySortOrderOppositeButtonWidgetDisabled != null) {
                partySortOrderOppositeButtonWidget.IsHidden = CurrentPartyThenByMode == SortMode.NONE;
                partySortOrderOppositeButtonWidgetDisabled.IsHidden = CurrentPartyThenByMode != SortMode.NONE;
            }

            Widget partyUpgradableOnTopButtonWidget = allChildren.FirstOrDefault(x => x.Id == "PartyUpgradableOnTopButtonWidget");
            Widget partyUpgradableOnTopButtonWidgetDisabled = allChildren.FirstOrDefault(x => x.Id == "PartyUpgradableOnTopButtonWidgetDisabled");
            if (partyUpgradableOnTopButtonWidget != null && partyUpgradableOnTopButtonWidgetDisabled != null) {
                partyUpgradableOnTopButtonWidget.IsHidden = !_vm.MainPartyTroops.Any(x => x.IsTroopUpgradable);
                partyUpgradableOnTopButtonWidgetDisabled.IsHidden = _vm.MainPartyTroops.Any(x => x.IsTroopUpgradable);
            }

            Widget otherSortOrderOppositeButtonWidget = allChildren.FirstOrDefault(x => x.Id == "OtherSortOrderOppositeButtonWidget");
            Widget otherSortOrderOppositeButtonWidgetDisabled = allChildren.FirstOrDefault(x => x.Id == "OtherSortOrderOppositeButtonWidgetDisabled");
            if (otherSortOrderOppositeButtonWidget != null && otherSortOrderOppositeButtonWidgetDisabled != null) {
                otherSortOrderOppositeButtonWidget.IsHidden = CurrentOtherThenByMode == SortMode.NONE;
                otherSortOrderOppositeButtonWidgetDisabled.IsHidden = CurrentOtherThenByMode != SortMode.NONE;
            }

            Widget actionUpgradeButtonWidget = allChildren.FirstOrDefault(x => x.Id == "ActionUpgradeButtonWidget");
            Widget actionUpgradeButtonWidgetDisabled = allChildren.FirstOrDefault(x => x.Id == "ActionUpgradeButtonWidgetDisabled");
            if (actionUpgradeButtonWidget != null && actionUpgradeButtonWidgetDisabled != null) {
                actionUpgradeButtonWidget.IsHidden = !_vm.MainPartyTroops.Any(x => x.IsTroopUpgradable);
                actionUpgradeButtonWidgetDisabled.IsHidden = _vm.MainPartyTroops.Any(x => x.IsTroopUpgradable);
            }

            Widget actionRecruitButtonWidget = allChildren.FirstOrDefault(x => x.Id == "ActionRecruitButtonWidget");
            Widget actionRecruitButtonWidgetDisabled = allChildren.FirstOrDefault(x => x.Id == "ActionRecruitButtonWidgetDisabled");
            if (actionRecruitButtonWidget != null && actionRecruitButtonWidgetDisabled != null) {
                actionRecruitButtonWidget.IsHidden = !_vm.MainPartyPrisoners.Any(x => x.IsTroopRecruitable);
                actionRecruitButtonWidgetDisabled.IsHidden = _vm.MainPartyPrisoners.Any(x => x.IsTroopRecruitable);
            }
        }

        private void RefreshPartyVmInformation() {
            MethodInfo refreshPrisonersRecruitable = _vm.GetType().BaseType?.GetMethod("RefreshPrisonersRecruitable", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPrisonersRecruitable?.Invoke(_vm, new object[0]);
            MethodInfo refreshTopInformation = _vm.GetType().BaseType?.GetMethod("RefreshTopInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshTopInformation?.Invoke(_vm, new object[0]);
            MethodInfo refreshPartyInformation = _vm.GetType().BaseType?.GetMethod("RefreshPartyInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPartyInformation?.Invoke(_vm, new object[0]);
        }

        #region Command - Other Sort Option Buttons

        [DataSourceMethod]
        public void ExecuteOtherSortOrderOpposite() {
            Global.Helpers.DebugMessage("Other sort order oppiste toggled");
            _otherTroopSorterService.SortOrderOpposite = !_otherTroopSorterService.SortOrderOpposite;
            SortOther(_otherTroopSorterService.SortDirection);
        }

        #endregion

        #region Text - Dropdowns

        [DataSourceProperty]
        public HintViewModel SortByHintText => new HintViewModel(Strings.SORT_BY_HINT_TEXT);

        [DataSourceProperty]
        public HintViewModel ThenByHintText => new HintViewModel(Strings.THEN_BY_HINT_TEXT);

        #endregion

        #region Text - Party Sort Buttons

        [DataSourceProperty]
        public string SortPartyAscendingText => Strings.SORT_TEXT_ASCENDING;

        [DataSourceProperty]
        public HintViewModel SortPartyAscendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_ASCENDING);

        [DataSourceProperty]
        public string SortPartyDescendingText => Strings.SORT_TEXT_DESCENDING;

        [DataSourceProperty]
        public HintViewModel SortPartyDescendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_DESCENDING);

        #endregion

        #region Text - Other Sort Buttons

        [DataSourceProperty]
        public string SortOtherAscendingText => Strings.SORT_TEXT_ASCENDING;

        [DataSourceProperty]
        public HintViewModel SortOtherAscendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_ASCENDING);

        [DataSourceProperty]
        public string SortOtherDescendingText => Strings.SORT_TEXT_DESCENDING;

        [DataSourceProperty]
        public HintViewModel SortOtherDescendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_DESCENDING);

        #endregion

        #region Text - Party Sort Option Buttons

        [DataSourceProperty]
        public string PartySortOrderOppositeText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);

        [DataSourceProperty]
        public string UpgradableOnTopText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopDisabledHintText => new HintViewModel(Strings.UPGRADABLE_ON_TOP_HINT_TEXT_DISABLED);

        #endregion

        #region Text - Other Sort Option Buttons

        [DataSourceProperty]
        public string OtherSortOrderOppositeText => _otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeHintText => new HintViewModel(_otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);

        #endregion

        #region Text - Action Buttons

        [DataSourceProperty]
        public HintViewModel ActionUpgradeHintText => new HintViewModel(Strings.UPGRADE_HINT_TEXT);

        [DataSourceProperty]
        public HintViewModel ActionUpgradeDisabledHintText => new HintViewModel(Strings.UPGRADE_HINT_TEXT_DISABLED);

        [DataSourceProperty]
        public HintViewModel ActionRecruitHintText => new HintViewModel(Strings.RECRUIT_HINT_TEXT);

        [DataSourceProperty]
        public HintViewModel ActionRecruitDisabledHintText => new HintViewModel(Strings.RECRUIT_HINT_TEXT_DISABLED);

        #endregion

        #region Data - Party Sort Dropdowns

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> PartySortByModeOptionVms {
            get => _partySortByModeOptionVms;
            set {
                if (value == _partySortByModeOptionVms) {
                    return;
                }

                _partySortByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(PartySortByModeOptionVms));
                RefreshPartyScreenWidgetStates();
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

                RefreshPartyScreenWidgetStates();
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
                RefreshPartyScreenWidgetStates();
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
                RefreshPartyScreenWidgetStates();
            }
        }

        #endregion

        #region Data - Other Sort Dropdowns

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> OtherSortByModeOptionVms {
            get => _otherSortByModeOptionVms;
            set {
                if (value == _otherSortByModeOptionVms) {
                    return;
                }

                _otherSortByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(OtherSortByModeOptionVms));
                RefreshPartyScreenWidgetStates();
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

                RefreshPartyScreenWidgetStates();
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
                RefreshPartyScreenWidgetStates();
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
                RefreshPartyScreenWidgetStates();
            }
        }

        #endregion

        #region Command - Party Sort Buttons

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

        #endregion

        #region Command - Party Sort Option Buttons

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

        #endregion

        #region Command - Other Sort Buttons

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

        #endregion

        #region Command - Action Buttons

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

        #endregion
    }
}
