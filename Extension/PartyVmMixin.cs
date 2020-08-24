using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using YAPO.Configuration.Models;
using YAPO.Global;
using YAPO.Services;

namespace YAPO {
    [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global"), UsedImplicitly]
    public class PartyVmMixin : BaseViewModelMixin<PartyVM> {
        private readonly PartyVM _viewModel;
        private bool _firstRefreshDone;
        private MBBindingList<SortByModeOptionVm> _otherSortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _otherThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _partySortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _partyThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();

        public PartyVmMixin(PartyVM viewModel) : base(viewModel) {
            States.PartyVmMixin = this;

            if (ViewModel != null) {
                _viewModel = ViewModel;
            } else {
                throw new NullReferenceException("PartyVM ViewModel is null");
            }

            InitialiseOptionVmList(_partySortByModeOptionVms);
            InitialiseOptionVmList(_partyThenByModeOptionVms);
            InitialiseOptionVmList(_otherSortByModeOptionVms);
            InitialiseOptionVmList(_otherThenByModeOptionVms);

            InitialiseOptionDropdowns();
        }

        private void InitialiseOptionVmList(ICollection<SortByModeOptionVm> optionVms) {
            foreach (SortMode sortMode in (SortMode[]) Enum.GetValues(typeof(SortMode))) {
                optionVms.Add(new SortByModeOptionVm(this, sortMode));
            }
        }

        private void InitialiseOptionDropdowns() {
            CurrentPartySortByMode = States.PartySorterConfiguration.CurrentSortByMode;
            CurrentPartyThenByMode = States.PartySorterConfiguration.CurrentThenByMode;
            CurrentOtherSortByMode = States.OtherSorterConfiguration.CurrentSortByMode;
            CurrentOtherThenByMode = States.OtherSorterConfiguration.CurrentThenByMode;
            CurrentPartySortByModeText = CurrentPartySortByMode.AsString();
            CurrentPartyThenByModeText = CurrentPartyThenByMode.AsString();
            CurrentOtherSortByModeText = CurrentOtherSortByMode.AsString();
            CurrentOtherThenByModeText = CurrentOtherThenByMode.AsString();
        }

        public override void OnRefresh() {
            if (States.PartyVmMixin != this) return;

            RefreshView();

            Global.Helpers.DebugMessage("Refresh view");
        }

        public void FirstRefresh() {
            if (States.PartyVmMixin != this || _firstRefreshDone) return;

            if (YapoSettings.Instance.IsAutoSortEnabled) {
                SortParty(States.PartySorterConfiguration.SortDirection);
                SortOther(States.PartySorterConfiguration.SortDirection);
            }

            OnRefresh();

            _firstRefreshDone = true;
            Global.Helpers.DebugMessage("First Refresh");
        }

        public override void OnFinalize() {
            States.PartyVmMixin = null;
        }

        private void SortParty(SortDirection sortDirection, bool skipDirectionUpdate = false) {
            if (States.PartySorterConfiguration.CurrentSortByMode == SortMode.NONE) return;

            if (!skipDirectionUpdate) States.PartySorterConfiguration.SortDirection = sortDirection;
            SortRoster(States.PartySorterConfiguration, _viewModel.PartyScreenLogic.PrisonerRosters[1], _viewModel.MainPartyPrisoners, newPartyList => { _viewModel.MainPartyPrisoners = newPartyList; });
            SortRoster(States.PartySorterConfiguration, _viewModel.PartyScreenLogic.MemberRosters[1], _viewModel.MainPartyTroops, newPartyList => { _viewModel.MainPartyTroops = newPartyList; });
            RefreshView();
        }

        private void SortOther(SortDirection sortDirection, bool skipDirectionUpdate = false) {
            if (States.OtherSorterConfiguration.CurrentSortByMode == SortMode.NONE) return;

            if (!skipDirectionUpdate) States.OtherSorterConfiguration.SortDirection = sortDirection;
            SortRoster(States.OtherSorterConfiguration, _viewModel.PartyScreenLogic.PrisonerRosters[0], _viewModel.OtherPartyPrisoners, newPartyList => { _viewModel.OtherPartyPrisoners = newPartyList; });
            SortRoster(States.OtherSorterConfiguration, _viewModel.PartyScreenLogic.MemberRosters[0], _viewModel.OtherPartyTroops, newPartyList => { _viewModel.OtherPartyTroops = newPartyList; });
            RefreshView();
        }

        private static void SortRoster(SorterConfiguration configuration, TroopRoster troopRoster, ICollection<PartyCharacterVM> partyList, Action<MBBindingList<PartyCharacterVM>> apply) {
            FieldInfo troopRosterDataField = troopRoster.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            TroopRosterElement[] originalTroops = (TroopRosterElement[]) troopRosterDataField?.GetValue(troopRoster);
            if (originalTroops == null || originalTroops.All(x => x.Character == null)) return;

            List<TroopRosterElement> originalTroopList = originalTroops.Where(x => x.Character != null).ToList();
            List<TroopRosterElement> sortedTroops = originalTroopList.Where(x => !x.Character.IsHero).ToList();
            List<TroopRosterElement> heroTroops = originalTroopList.Where(x => x.Character.IsHero && !x.Character.IsPlayerCharacter).ToList();
            TroopRosterElement player = originalTroopList.FirstOrDefault(x => x.Character.IsPlayerCharacter);

            TroopSorterService.Sort(ref sortedTroops, ref heroTroops, configuration);
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
            UpgradeResults results = TroopActionService.UpgradeTroops(_viewModel, _viewModel.PartyScreenLogic);
            if (results.UpgradedTotal == 0) return;

            _viewModel.CurrentCharacter = _viewModel.MainPartyTroops[0];
            RefreshPartyVmInformation();
            RefreshView();

            StringBuilder message = new StringBuilder();
            message.Append($"Upgraded {results.UpgradedTotal} troops over {results.UpgradedTypes} types. ");
            if (results.MultiPathSkipped > 0) {
                message.Append($"{results.MultiPathSkipped} troop types with multi-path upgrades were skipped. ");
            }

            message.Append("Press 'Done' to confirm changes");
            Global.Helpers.Message(message.ToString());
        }

        private void RecruitPrisoners() {
            RecruitmentResults results = TroopActionService.RecruitPrisoners(_viewModel, _viewModel.PartyScreenLogic);
            if (results.RecruitedTotal == 0) return;

            _viewModel.CurrentCharacter = _viewModel.MainPartyTroops[0];
            RefreshPartyVmInformation();
            RefreshView();
            Global.Helpers.Message($"Recruited {results.RecruitedTotal} prisoners over {results.RecruitedTypes} types. " + "Press 'Done' to confirm changes");
        }

        private void RefreshView() {
            _viewModel.OnPropertyChanged(nameof(SortByHintText));
            _viewModel.OnPropertyChanged(nameof(ThenByHintText));
            _viewModel.OnPropertyChanged(nameof(SortPartyAscendingText));
            _viewModel.OnPropertyChanged(nameof(SortPartyAscendingHintText));
            _viewModel.OnPropertyChanged(nameof(SortPartyDescendingText));
            _viewModel.OnPropertyChanged(nameof(SortPartyDescendingHintText));
            _viewModel.OnPropertyChanged(nameof(PartySortOrderOppositeText));
            _viewModel.OnPropertyChanged(nameof(PartySortOrderOppositeHintText));
            _viewModel.OnPropertyChanged(nameof(PartySortOrderOppositeDisabledHintText));
            _viewModel.OnPropertyChanged(nameof(UpgradableOnTopText));
            _viewModel.OnPropertyChanged(nameof(UpgradableOnTopHintText));
            _viewModel.OnPropertyChanged(nameof(UpgradableOnTopDisabledHintText));
            _viewModel.OnPropertyChanged(nameof(SortOtherAscendingText));
            _viewModel.OnPropertyChanged(nameof(SortOtherAscendingHintText));
            _viewModel.OnPropertyChanged(nameof(SortOtherDescendingText));
            _viewModel.OnPropertyChanged(nameof(SortOtherDescendingHintText));
            _viewModel.OnPropertyChanged(nameof(OtherSortOrderOppositeText));
            _viewModel.OnPropertyChanged(nameof(OtherSortOrderOppositeHintText));
            _viewModel.OnPropertyChanged(nameof(PartySortByModeOptionVms));
            _viewModel.OnPropertyChanged(nameof(CurrentPartySortByModeText));
            _viewModel.OnPropertyChanged(nameof(PartyThenByModeOptionVms));
            _viewModel.OnPropertyChanged(nameof(CurrentPartyThenByModeText));
            _viewModel.OnPropertyChanged(nameof(OtherSortByModeOptionVms));
            _viewModel.OnPropertyChanged(nameof(CurrentOtherSortByModeText));
            _viewModel.OnPropertyChanged(nameof(OtherThenByModeOptionVms));
            _viewModel.OnPropertyChanged(nameof(CurrentOtherThenByModeText));
            _viewModel.OnPropertyChanged(nameof(ActionUpgradeHintText));
            _viewModel.OnPropertyChanged(nameof(ActionUpgradeDisabledHintText));
            _viewModel.OnPropertyChanged(nameof(ActionRecruitHintText));
            _viewModel.OnPropertyChanged(nameof(ActionRecruitDisabledHintText));
            RefreshPartyScreenWidgetStates();
        }

        private void RefreshPartyScreenWidgetStates() {
            if (States.PartyScreenWidget == null) return;

            List<Widget> allChildren = States.PartyScreenWidget.AllChildren.ToList();
            RefreshPartyScreenWidgetState(allChildren, "PartySortOrderOppositeButtonWidget", () => CurrentPartyThenByMode != SortMode.NONE);
            RefreshPartyScreenWidgetState(allChildren, "PartyUpgradableOnTopButtonWidget", () => _viewModel.MainPartyTroops.Any(x => x.IsTroopUpgradable));
            RefreshPartyScreenWidgetState(allChildren, "OtherSortOrderOppositeButtonWidget", () => CurrentOtherThenByMode != SortMode.NONE);
            RefreshPartyScreenWidgetState(allChildren, "ActionUpgradeButtonWidget", () => _viewModel.MainPartyTroops.Any(x => x.IsTroopUpgradable));
            RefreshPartyScreenWidgetState(allChildren, "ActionRecruitButtonWidget", () => _viewModel.MainPartyPrisoners.Any(x => x.IsTroopRecruitable));
        }

        private static void RefreshPartyScreenWidgetState(IReadOnlyCollection<Widget> allWidgets, string widgetName, Func<bool> statePredicate) {
            Widget widget = allWidgets.FirstOrDefault(x => x.Id == widgetName);
            Widget widgetDisabled = allWidgets.FirstOrDefault(x => x.Id == $"{widgetName}Disabled");
            if (widget == null || widgetDisabled == null) return;

            bool state = statePredicate();
            widget.IsHidden = !state;
            widgetDisabled.IsHidden = state;
        }

        private void RefreshPartyVmInformation() {
            MethodInfo refreshPrisonersRecruitable = _viewModel.GetType().BaseType?.GetMethod("RefreshPrisonersRecruitable", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPrisonersRecruitable?.Invoke(_viewModel, new object[0]);
            MethodInfo refreshTopInformation = _viewModel.GetType().BaseType?.GetMethod("RefreshTopInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshTopInformation?.Invoke(_viewModel, new object[0]);
            MethodInfo refreshPartyInformation = _viewModel.GetType().BaseType?.GetMethod("RefreshPartyInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPartyInformation?.Invoke(_viewModel, new object[0]);
        }

        #region Command - Other Sort Option Buttons

        [DataSourceMethod]
        public void ExecuteOtherSortOrderOpposite() {
            Global.Helpers.DebugMessage("Other sort order opposite toggled");
            States.OtherSorterConfiguration.SortOrderOpposite = !States.OtherSorterConfiguration.SortOrderOpposite;
            SortOther(States.OtherSorterConfiguration.SortDirection);
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
        public string PartySortOrderOppositeText =>
            States.PartySorterConfiguration == null ? "NULL" :
            States.PartySorterConfiguration.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeHintText =>
            new HintViewModel(States.PartySorterConfiguration == null ? "NULL" :
                              States.PartySorterConfiguration.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);

        [DataSourceProperty]
        public string UpgradableOnTopText =>
            States.PartySorterConfiguration == null ? "NULL" :
            States.PartySorterConfiguration.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText =>
            new HintViewModel(States.PartySorterConfiguration == null ? "NULL" :
                              States.PartySorterConfiguration.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopDisabledHintText => new HintViewModel(Strings.UPGRADABLE_ON_TOP_HINT_TEXT_DISABLED);

        #endregion

        #region Text - Other Sort Option Buttons

        [DataSourceProperty]
        public string OtherSortOrderOppositeText =>
            States.OtherSorterConfiguration == null ? "NULL" :
            States.OtherSorterConfiguration.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeHintText =>
            new HintViewModel(States.OtherSorterConfiguration == null ? "NULL" :
                              States.OtherSorterConfiguration.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

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
                _viewModel.OnPropertyChanged(nameof(PartySortByModeOptionVms));
                RefreshPartyScreenWidgetStates();
            }
        }

        [DataSourceProperty]
        public string CurrentPartySortByModeText { get; set; }

        public SortMode CurrentPartySortByMode {
            get => States.PartySorterConfiguration.CurrentSortByMode;
            set {
                if (value == States.PartySorterConfiguration.CurrentSortByMode) {
                    return;
                }

                SortMode currentSortByMode = CurrentPartySortByMode;
                States.PartySorterConfiguration.CurrentSortByMode = value;
                CurrentPartySortByModeText = value.AsString();
                _viewModel.OnPropertyChanged(nameof(CurrentPartySortByModeText));

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
                _viewModel.OnPropertyChanged(nameof(PartyThenByModeOptionVms));
                RefreshPartyScreenWidgetStates();
            }
        }

        [DataSourceProperty]
        public string CurrentPartyThenByModeText { get; set; }

        public SortMode CurrentPartyThenByMode {
            get => States.PartySorterConfiguration.CurrentThenByMode;
            set {
                if (value == States.PartySorterConfiguration.CurrentThenByMode) {
                    return;
                }

                if (CurrentPartySortByMode == value) {
                    value = CurrentPartyThenByMode;
                    Global.Helpers.Message("Then By should be different from Sort By");
                }

                States.PartySorterConfiguration.CurrentThenByMode = value;
                CurrentPartyThenByModeText = value.AsString();
                _viewModel.OnPropertyChanged(nameof(CurrentPartyThenByModeText));
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
                _viewModel.OnPropertyChanged(nameof(OtherSortByModeOptionVms));
                RefreshPartyScreenWidgetStates();
            }
        }

        [DataSourceProperty]
        public string CurrentOtherSortByModeText { get; set; }

        public SortMode CurrentOtherSortByMode {
            get => States.OtherSorterConfiguration.CurrentSortByMode;
            set {
                if (value == States.OtherSorterConfiguration.CurrentSortByMode) {
                    return;
                }

                SortMode currentSortByMode = CurrentOtherSortByMode;
                States.OtherSorterConfiguration.CurrentSortByMode = value;
                CurrentOtherSortByModeText = value.AsString();
                _viewModel.OnPropertyChanged(nameof(CurrentOtherSortByModeText));

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
                _viewModel.OnPropertyChanged(nameof(OtherThenByModeOptionVms));
                RefreshPartyScreenWidgetStates();
            }
        }

        [DataSourceProperty]
        public string CurrentOtherThenByModeText { get; set; }

        public SortMode CurrentOtherThenByMode {
            get => States.OtherSorterConfiguration.CurrentThenByMode;
            set {
                if (value == States.OtherSorterConfiguration.CurrentThenByMode) {
                    return;
                }

                if (CurrentOtherSortByMode == value) {
                    value = SortMode.NONE;
                    Global.Helpers.Message("Then By should be different from Sort By");
                }

                States.OtherSorterConfiguration.CurrentThenByMode = value;
                CurrentOtherThenByModeText = value.AsString();
                _viewModel.OnPropertyChanged(nameof(CurrentOtherThenByModeText));
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
            Global.Helpers.DebugMessage("Party sort order opposite toggled");
            States.PartySorterConfiguration.SortOrderOpposite = !States.PartySorterConfiguration.SortOrderOpposite;
            SortParty(States.PartySorterConfiguration.SortDirection);
        }

        [DataSourceMethod]
        public void ExecuteUpgradableOnTop() {
            Global.Helpers.DebugMessage("Party Upgradable on top toggled");
            States.PartySorterConfiguration.UpgradableOnTop = !States.PartySorterConfiguration.UpgradableOnTop;
            SortParty(States.PartySorterConfiguration.SortDirection, true);
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
