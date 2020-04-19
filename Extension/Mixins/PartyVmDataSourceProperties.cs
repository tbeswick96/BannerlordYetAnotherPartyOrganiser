using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using YAPO.Global;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Mixins
{
    public partial class PartyVmMixin
    {
        #region Text - Party Sort Option Buttons

        [DataSourceProperty]
        public string PartySortOrderOppositeText =>
            _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeHintText =>
            new HintViewModel(_partyTroopSorterService == null ? "NULL" :
                              _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);

        [DataSourceProperty]
        public string UpgradableOnTopText =>
            _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText =>
            new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopDisabledHintText => new HintViewModel(Strings.UPGRADABLE_ON_TOP_HINT_TEXT_DISABLED);

        #endregion

        #region Text - Other Sort Option Buttons

        [DataSourceProperty]
        public string OtherSortOrderOppositeText =>
            _otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeHintText =>
            new HintViewModel(_otherTroopSorterService == null ? "NULL" :
                              _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);

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
        public MBBindingList<SortByModeOptionVm> PartySortByModeOptionVms
        {
            get => _partySortByModeOptionVms;
            set
            {
                if (value == _partySortByModeOptionVms)
                {
                    return;
                }

                _partySortByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(PartySortByModeOptionVms));
                RefreshView();
            }
        }

        [DataSourceProperty]
        public string CurrentPartySortByModeText { get; set; }

        public SortMode CurrentPartySortByMode
        {
            get => _partyTroopSorterService.CurrentSortByMode;
            set
            {
                if (value == _partyTroopSorterService.CurrentSortByMode)
                {
                    return;
                }

                SortMode currentSortByMode = CurrentPartySortByMode;
                _partyTroopSorterService.CurrentSortByMode = value;
                CurrentPartySortByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentPartySortByModeText));

                if (CurrentPartyThenByMode == value)
                {
                    CurrentPartyThenByMode = currentSortByMode;
                }

                RefreshView();
            }
        }

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> PartyThenByModeOptionVms
        {
            get => _partyThenByModeOptionVms;
            set
            {
                if (value == _partyThenByModeOptionVms)
                {
                    return;
                }

                _partyThenByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(PartyThenByModeOptionVms));
                RefreshView();
            }
        }

        [DataSourceProperty]
        public string CurrentPartyThenByModeText { get; set; }

        public SortMode CurrentPartyThenByMode
        {
            get => _partyTroopSorterService.CurrentThenByMode;
            set
            {
                if (value == _partyTroopSorterService.CurrentThenByMode)
                {
                    return;
                }

                if (CurrentPartySortByMode == value)
                {
                    value = CurrentPartyThenByMode;
                    Global.Helpers.Message("Then By should be different from Sort By");
                }

                _partyTroopSorterService.CurrentThenByMode = value;
                CurrentPartyThenByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentPartyThenByModeText));
                RefreshView();
            }
        }

        #endregion

        #region Data - Other Sort Dropdowns

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> OtherSortByModeOptionVms
        {
            get => _otherSortByModeOptionVms;
            set
            {
                if (value == _otherSortByModeOptionVms)
                {
                    return;
                }

                _otherSortByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(OtherSortByModeOptionVms));
                RefreshView();
            }
        }

        [DataSourceProperty]
        public string CurrentOtherSortByModeText { get; set; }

        public SortMode CurrentOtherSortByMode
        {
            get => _otherTroopSorterService.CurrentSortByMode;
            set
            {
                if (value == _otherTroopSorterService.CurrentSortByMode)
                {
                    return;
                }

                SortMode currentSortByMode = CurrentOtherSortByMode;
                _otherTroopSorterService.CurrentSortByMode = value;
                CurrentOtherSortByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentOtherSortByModeText));

                if (CurrentOtherThenByMode == value)
                {
                    CurrentOtherThenByMode = currentSortByMode;
                }

                RefreshView();
            }
        }

        [DataSourceProperty]
        public MBBindingList<SortByModeOptionVm> OtherThenByModeOptionVms
        {
            get => _otherThenByModeOptionVms;
            set
            {
                if (value == _otherThenByModeOptionVms)
                {
                    return;
                }

                _otherThenByModeOptionVms = value;
                _vm.OnPropertyChanged(nameof(OtherThenByModeOptionVms));
                RefreshView();
            }
        }

        [DataSourceProperty]
        public string CurrentOtherThenByModeText { get; set; }

        public SortMode CurrentOtherThenByMode
        {
            get => _otherTroopSorterService.CurrentThenByMode;
            set
            {
                if (value == _otherTroopSorterService.CurrentThenByMode)
                {
                    return;
                }

                if (CurrentOtherSortByMode == value)
                {
                    value = SortMode.NONE;
                    Global.Helpers.Message("Then By should be different from Sort By");
                }

                _otherTroopSorterService.CurrentThenByMode = value;
                CurrentOtherThenByModeText = value.AsString();
                _vm.OnPropertyChanged(nameof(CurrentOtherThenByModeText));
                RefreshView();
            }
        }

        #endregion
    }
}
