using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using UIExtenderLib;
using UIExtenderLib.ViewModel;
using YAPO.Global;
using YAPO.GUI;
using YAPO.Services;

// ReSharper disable ClassNeverInstantiated.Global

namespace YAPO.Mixins
{
    [ViewModelMixin]
    public partial class PartyVmMixin : BaseViewModelMixin<PartyVM>
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
                case SortSide.BOTH:
                    SortParty(_otherTroopSorterService.SortDirection, true);
                    SortOther(_otherTroopSorterService.SortDirection, true);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(sortSide), sortSide, null);
            }

            RefreshView();
        }

        private void SortParty(SortDirection sortDirection, bool skipDirectionUpdate = false)
        {
            if (_partyTroopSorterService.CurrentSortByMode == SortMode.NONE) return;

            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _partyTroopSorterService.UpdateSortingDirection(sortDirection);
            IEnumerable<TroopRosterElement> sortedPrisoners = SortRoster(_partyTroopSorterService, _partyScreenLogic.PrisonerRosters[1]);
            IEnumerable<TroopRosterElement> sortedTroops = SortRoster(_partyTroopSorterService, _partyScreenLogic.MemberRosters[1]);

            RefreshPartyTroopsView(_vm.MainPartyPrisoners, sortedPrisoners, newTroops => { _vm.MainPartyPrisoners = newTroops; });
            RefreshPartyTroopsView(_vm.MainPartyTroops, sortedTroops, newTroops => { _vm.MainPartyTroops = newTroops; });
        }

        private void SortOther(SortDirection sortDirection, bool skipDirectionUpdate = false)
        {
            if (_otherTroopSorterService.CurrentSortByMode == SortMode.NONE) return;

            GetPartyScreenLogic();
            if (!skipDirectionUpdate) _otherTroopSorterService.UpdateSortingDirection(sortDirection);
            IEnumerable<TroopRosterElement> sortedPrisoners = SortRoster(_otherTroopSorterService, _partyScreenLogic.MemberRosters[0]);
            IEnumerable<TroopRosterElement> sortedTroops = SortRoster(_otherTroopSorterService, _partyScreenLogic.PrisonerRosters[0]);

            RefreshPartyTroopsView(_vm.OtherPartyPrisoners, sortedPrisoners, newTroops => { _vm.OtherPartyPrisoners = newTroops; });
            RefreshPartyTroopsView(_vm.OtherPartyTroops, sortedTroops, newTroops => { _vm.OtherPartyTroops = newTroops; });
        }

        private static IEnumerable<TroopRosterElement> SortRoster(TroopSorterService troopSorterService, TroopRoster troopRoster)
        {
            FieldInfo troopRosterDataField = troopRoster.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
            TroopRosterElement[] originalTroops = (TroopRosterElement[]) troopRosterDataField?.GetValue(troopRoster);
            if (originalTroops == null || originalTroops.All(x => x.Character == null)) throw new Exception("Could not find 'data' value in troopRoster");

            List<TroopRosterElement> originalTroopList = originalTroops.Where(x => x.Character != null).ToList();
            List<TroopRosterElement> sortedTroops = originalTroopList.Where(x => !x.Character.IsHero).ToList();
            List<TroopRosterElement> heroTroops = originalTroopList.Where(x => x.Character.IsHero && !x.Character.IsPlayerCharacter).ToList();
            TroopRosterElement player = originalTroopList.FirstOrDefault(x => x.Character.IsPlayerCharacter);

            troopSorterService.Sort(ref sortedTroops, ref heroTroops);
            if (heroTroops.Count > 0) sortedTroops.InsertRange(0, heroTroops);
            if (player.Character != null) sortedTroops.Insert(0, player);
            troopRosterDataField.SetValue(troopRoster, sortedTroops.ToArray());

            return sortedTroops;
        }

        private static void RefreshPartyTroopsView(ICollection<PartyCharacterVM> partyList, IEnumerable<TroopRosterElement> sortedTroops, Action<MBBindingList<PartyCharacterVM>> apply)
        {
            DateTime start = DateTime.Now;
            List<PartyCharacterVM> tempTroops = partyList.ToList();
            MBBindingList<PartyCharacterVM> newTroops = new MBBindingList<PartyCharacterVM>();
            partyList.Clear();
            foreach (PartyCharacterVM troop in sortedTroops.Select(sortedTroop => tempTroops.FirstOrDefault(x => x.Troop.Character.ToString() == sortedTroop.Character.ToString())))
            {
                newTroops.Add(troop);
            }

            apply(newTroops);
            DateTime end = DateTime.Now;
            Global.Helpers.Message($"Apply took {(end - start).Milliseconds}ms");
            
        }

        private void UpgradeTroops()
        {
            GetPartyScreenLogic();
            States.MassActionInProgress = true;
            (int upgradedTotal, int upgradedTypes, int multiPathSkipped) = TroopActionService.UpgradeTroops(_vm, _partyScreenLogic);
            if (upgradedTotal == 0) return;

            _vm.CurrentCharacter = _vm.MainPartyTroops[0];
            SortParty(_partyTroopSorterService.SortDirection, true);
            RefreshView();
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
            RefreshView();
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
    }
}
