using UIExtenderLib;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Mixins
{
    public partial class PartyVmMixin
    {
        #region Command - Other Sort Option Buttons

        [DataSourceMethod]
        public void ExecuteOtherSortOrderOpposite()
        {
            Global.Helpers.DebugMessage("Other sort order oppiste toggled");
            _otherTroopSorterService.SortOrderOpposite = !_otherTroopSorterService.SortOrderOpposite;
            SortInPlace(SortSide.OTHER);
        }

        #endregion

        #region Command - Party Sort Buttons

        [DataSourceMethod]
        public void ExecuteSortPartyAscending()
        {
            Global.Helpers.DebugMessage("Party Sort Ascending Pressed");
            SortParty(SortDirection.ASCENDING);
            RefreshView();
        }

        [DataSourceMethod]
        public void ExecuteSortPartyDescending()
        {
            Global.Helpers.DebugMessage("Party Sort Descending Pressed");
            SortParty(SortDirection.DESCENDING);
            RefreshView();
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
            RefreshView();
        }

        [DataSourceMethod]
        public void ExecuteSortOtherDescending()
        {
            Global.Helpers.DebugMessage("Other Sort Descending Pressed");
            SortOther(SortDirection.DESCENDING);
            RefreshView();
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
    }
}
