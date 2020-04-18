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
using YAPO.GUI;
using YAPO.Services;

namespace YAPO.Mixins
{
    [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PartyVmMixin : BaseViewModelMixin<PartyVM>
    {
        private readonly TroopSorterService _otherTroopSorterService;
        private readonly TroopSorterService _partyTroopSorterService;
        private bool _firstRefreshDone;
        private MBBindingList<SortByModeOptionVm> _otherSortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _otherThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private PartyScreenLogic _partyScreenLogic;
        private MBBindingList<SortByModeOptionVm> _partySortByModeOptionVms = new MBBindingList<SortByModeOptionVm>();
        private MBBindingList<SortByModeOptionVm> _partyThenByModeOptionVms = new MBBindingList<SortByModeOptionVm>();

        public PartyVmMixin(PartyVM viewModel) : base(viewModel)
        {
            States.PartyVmMixin = this;

            _partyTroopSorterService = InitialiseTroopSorterService(SortSide.PARTY);
            _otherTroopSorterService = InitialiseTroopSorterService(SortSide.OTHER);

            InitialiseOptionVmList(_partySortByModeOptionVms);
            InitialiseOptionVmList(_partyThenByModeOptionVms);
            InitialiseOptionVmList(_otherSortByModeOptionVms);
            InitialiseOptionVmList(_otherThenByModeOptionVms);

            InitialiseOptionDropdowns();
        }

        private static TroopSorterService InitialiseTroopSorterService(SortSide sortSide)
        {
            TroopSorterService troopSorterService = MBObjectManager.Instance.GetObject<TroopSorterService>(x => x.SortSide == sortSide);
            if (troopSorterService != null) return troopSorterService;

            troopSorterService = MBObjectManager.Instance.CreateObject<TroopSorterService>();
            troopSorterService.SortSide = sortSide;
            return troopSorterService;
        }

        private void InitialiseOptionVmList(ICollection<SortByModeOptionVm> optionVms)
        {
            foreach (SortMode sortMode in (SortMode[]) Enum.GetValues(typeof(SortMode)))
            {
                optionVms.Add(new SortByModeOptionVm(this, sortMode));
            }
        }

        private void InitialiseOptionDropdowns()
        {
            CurrentPartySortByMode = _partyTroopSorterService.CurrentSortByMode;
            CurrentPartySortByModeText = CurrentPartySortByMode.AsString();
            CurrentPartyThenByMode = _partyTroopSorterService.CurrentThenByMode;
            CurrentPartyThenByModeText = CurrentPartyThenByMode.AsString();
            CurrentOtherSortByMode = _otherTroopSorterService.CurrentSortByMode;
            CurrentOtherSortByModeText = CurrentOtherSortByMode.AsString();
            CurrentOtherThenByMode = _otherTroopSorterService.CurrentThenByMode;
            CurrentOtherThenByModeText = CurrentOtherThenByMode.AsString();
        }

        public override void Refresh()
        {
            if (States.PartyVmMixin != this) return;

            RefreshView();

            Global.Helpers.DebugMessage("Refresh view");
        }

        public void FirstRefresh()
        {
            if (States.PartyVmMixin != this || _firstRefreshDone) return;

            if (Settings.Instance.AutoSortEnabled)
            {
                SortParty(_partyTroopSorterService.SortDirection);
                SortOther(_partyTroopSorterService.SortDirection);
            }

            Refresh();

            _firstRefreshDone = true;
            Global.Helpers.DebugMessage("First Refresh");
        }

        public override void Destroy()
        {
            States.PartyVmMixin = null;
        }

        private void GetPartyScreenLogic()
        {
            if (_partyScreenLogic != null) return;

            FieldInfo partyScreenLogicField = _vm.GetType().BaseType?.GetField("_partyScreenLogic", BindingFlags.NonPublic | BindingFlags.Instance);
            _partyScreenLogic = (PartyScreenLogic) partyScreenLogicField?.GetValue(_vm);
        }

        public void SortInPlace(SortSide sortSide)
        {
            switch (sortSide)
            {
                case SortSide.OTHER:
                    SortOther(_otherTroopSorterService.SortDirection, true);
                    break;
                case SortSide.PARTY:
                    SortParty(_otherTroopSorterService.SortDirection, true);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(sortSide), sortSide, null);
            }
        }

        private void SortParty(SortDirection sortDirection, bool skipDirectionUpdate = false)
        {
            if (_partyTroopSorterService.CurrentSortByMode == SortMode.NONE) return;

            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _partyTroopSorterService.UpdateSortingDirection(sortDirection);
            SortRoster(_partyTroopSorterService, _partyScreenLogic.PrisonerRosters[1], _vm.MainPartyPrisoners, newPartyList => { _vm.MainPartyPrisoners = newPartyList; });
            SortRoster(_partyTroopSorterService, _partyScreenLogic.MemberRosters[1], _vm.MainPartyTroops, newPartyList => { _vm.MainPartyTroops = newPartyList; });
            RefreshView();
        }

        private void SortOther(SortDirection sortDirection, bool skipDirectionUpdate = false)
        {
            if (_otherTroopSorterService.CurrentSortByMode == SortMode.NONE) return;

            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _otherTroopSorterService.UpdateSortingDirection(sortDirection);
            SortRoster(_otherTroopSorterService, _partyScreenLogic.PrisonerRosters[0], _vm.OtherPartyPrisoners, newPartyList => { _vm.OtherPartyPrisoners = newPartyList; });
            SortRoster(_otherTroopSorterService, _partyScreenLogic.MemberRosters[0], _vm.OtherPartyTroops, newPartyList => { _vm.OtherPartyTroops = newPartyList; });
            RefreshView();
        }

        private static void SortRoster(TroopSorterService troopSorterService, TroopRoster troopRoster, ICollection<PartyCharacterVM> partyList, Action<MBBindingList<PartyCharacterVM>> apply)
        {
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
            foreach (PartyCharacterVM troop in sortedTroops.Select(sortedTroop => tempTroopList.FirstOrDefault(x => x.Troop.Character.ToString() == sortedTroop.Character.ToString())))
            {
                newTroopList.Add(troop);
            }

            apply(newTroopList);
        }

        private void UpgradeTroops()
        {
            GetPartyScreenLogic();
            States.MassActionInProgress = true;  
            (int upgradedTotal, int upgradedTypes, int multiPathSkipped) = TroopActionService.UpgradeTroops(_vm, _partyScreenLogic);
            if (upgradedTotal == 0) return;

            _vm.CurrentCharacter = _vm.MainPartyTroops[0];
            SortParty(_partyTroopSorterService.SortDirection, true);
            RefreshPartyVmInformation();
            States.MassActionInProgress = false;
            Global.Helpers.Message($"Upgraded {upgradedTotal} troops over {upgradedTypes} types. {multiPathSkipped} troop types with mulit-path upgrades were skipped. Press 'Apply' to confirm changes");
        }

        private void RecruitPrisoners()
        {
            GetPartyScreenLogic();
            States.MassActionInProgress = true;
            (int recruitedTotal, int recruitedTypes) = TroopActionService.RecruitPrisoners(_vm, _partyScreenLogic);
            if (recruitedTotal == 0) return;

            _vm.CurrentCharacter = _vm.MainPartyTroops[0];
            SortParty(_partyTroopSorterService.SortDirection, true);
            RefreshPartyVmInformation();
            States.MassActionInProgress = false;
            Global.Helpers.Message($"Recruited {recruitedTotal} prisoners over {recruitedTypes} types. Press 'Apply' to confirm changes");
        }

        private void RefreshView()
        {
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeText));
            _vm.OnPropertyChanged(nameof(PartySortOrderOppositeHintText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopText));
            _vm.OnPropertyChanged(nameof(UpgradableOnTopHintText));
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
            RefreshPartyScreenWidgetStates();
        }

        private void RefreshPartyScreenWidgetStates()
        {
            if (States.PartyScreenWidget == null) return;

            List<Widget> allChildren = States.PartyScreenWidget.AllChildren.ToList();
            allChildren.RefreshWidgetState("PartySortOrderOppositeButtonWidget", () => CurrentPartyThenByMode != SortMode.NONE);
            allChildren.RefreshWidgetState("PartyUpgradableOnTopButtonWidget", () => _vm.MainPartyTroops.Any(x => x.IsTroopUpgradable));
            allChildren.RefreshWidgetState("OtherSortOrderOppositeButtonWidget", () => CurrentOtherThenByMode != SortMode.NONE);
            allChildren.RefreshWidgetState("ActionUpgradeButtonWidget", () => _vm.MainPartyTroops.Any(x => x.IsTroopUpgradable));
            allChildren.RefreshWidgetState("ActionRecruitButtonWidget", () => _vm.MainPartyPrisoners.Any(x => x.IsTroopRecruitable));
        }

        private void RefreshPartyVmInformation()
        {
            MethodInfo refreshPrisonersRecruitable = _vm.GetType().BaseType?.GetMethod("RefreshPrisonersRecruitable", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPrisonersRecruitable?.Invoke(_vm, new object[0]);
            MethodInfo refreshTopInformation = _vm.GetType().BaseType?.GetMethod("RefreshTopInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshTopInformation?.Invoke(_vm, new object[0]);
            MethodInfo refreshPartyInformation = _vm.GetType().BaseType?.GetMethod("RefreshPartyInformation", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshPartyInformation?.Invoke(_vm, new object[0]);
        }

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global

        #region Command - Other Sort Option Buttons

        [DataSourceMethod]
        public void ExecuteOtherSortOrderOpposite()
        {
            Global.Helpers.DebugMessage("Other sort order oppiste toggled");
            _otherTroopSorterService.SortOrderOpposite = !_otherTroopSorterService.SortOrderOpposite;
            SortInPlace(SortSide.OTHER);
        }

        #endregion

        #region Text - Party Sort Option Buttons

        [DataSourceProperty]
        public string PartySortOrderOppositeText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel PartySortOrderOppositeHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

        [DataSourceProperty]
        public string UpgradableOnTopText => _partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_TEXT_ON : Strings.UPGRADABLE_ON_TOP_TEXT_OFF;

        [DataSourceProperty]
        public HintViewModel UpgradableOnTopHintText => new HintViewModel(_partyTroopSorterService == null ? "NULL" : _partyTroopSorterService.UpgradableOnTop ? Strings.UPGRADABLE_ON_TOP_HINT_TEXT_ON : Strings.UPGRADABLE_ON_TOP_HINT_TEXT_OFF);

        #endregion

        #region Text - Other Sort Option Buttons

        [DataSourceProperty]
        public string OtherSortOrderOppositeText => _otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_TEXT_OPPOSITE;

        [DataSourceProperty]
        public HintViewModel OtherSortOrderOppositeHintText => new HintViewModel(_otherTroopSorterService == null ? "NULL" : _otherTroopSorterService.SortOrderOpposite ? Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_SAME : Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE);

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

        #region Command - Party Sort Buttons

        [DataSourceMethod]
        public void ExecuteSortPartyAscending()
        {
            Global.Helpers.DebugMessage("Party Sort Ascending Pressed");
            SortParty(SortDirection.ASCENDING);
        }

        [DataSourceMethod]
        public void ExecuteSortPartyDescending()
        {
            Global.Helpers.DebugMessage("Party Sort Descending Pressed");
            SortParty(SortDirection.DESCENDING);
        }

        #endregion

        #region Command - Party Sort Option Buttons

        [DataSourceMethod]
        public void ExecutePartySortOrderOpposite()
        {
            Global.Helpers.DebugMessage("Party sort order oppiste toggled");
            _partyTroopSorterService.SortOrderOpposite = !_partyTroopSorterService.SortOrderOpposite;
            SortInPlace(SortSide.PARTY);
        }

        [DataSourceMethod]
        public void ExecuteUpgradableOnTop()
        {
            Global.Helpers.DebugMessage("Party Upgradable on top toggled");
            _partyTroopSorterService.UpgradableOnTop = !_partyTroopSorterService.UpgradableOnTop;
            SortInPlace(SortSide.PARTY);
        }

        #endregion

        #region Command - Other Sort Buttons

        [DataSourceMethod]
        public void ExecuteSortOtherAscending()
        {
            Global.Helpers.DebugMessage("Other Sort Ascending Pressed");
            SortOther(SortDirection.ASCENDING);
        }

        [DataSourceMethod]
        public void ExecuteSortOtherDescending()
        {
            Global.Helpers.DebugMessage("Other Sort Descending Pressed");
            SortOther(SortDirection.DESCENDING);
        }

        #endregion

        #region Command - Action Buttons

        [DataSourceMethod]
        public void ExecuteActionUpgrade()
        {
            Global.Helpers.DebugMessage("Action upgrade Pressed");
            UpgradeTroops();
        }

        [DataSourceMethod]
        public void ExecuteActionRecruit()
        {
            Global.Helpers.DebugMessage("Action recruit Pressed");
            RecruitPrisoners();
        }

        #endregion

        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
    }
}
