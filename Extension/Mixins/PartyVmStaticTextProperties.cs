// using System;
// using System.Diagnostics.CodeAnalysis;
// using TaleWorlds.CampaignSystem.ViewModelCollection;
// using TaleWorlds.Core.ViewModelCollection;
// using TaleWorlds.Library;
// using UIExtenderLib;
// using UIExtenderLib.ViewModel;
// using YAPO.Global;
//
// namespace YAPO.Mixins
// {
//     [ViewModelMixin, SuppressMessage("ReSharper", "UnusedMember.Global")]
//     public class PartyVmStaticTextProperties : BaseViewModelMixin<PartyVM>
//     {
//         public PartyVmStaticTextProperties(PartyVM viewModel) : base(viewModel)
//         {
//             Console.Out.WriteLine("test");
//         }
//
//         #region Text - Other Sort Option Buttons
//
//         [DataSourceProperty]
//         public HintViewModel OtherSortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);
//
//         #endregion
//
//         #region Text - Dropdowns
//
//         [DataSourceProperty]
//         public HintViewModel SortByHintText => new HintViewModel(Strings.SORT_BY_HINT_TEXT);
//
//         [DataSourceProperty]
//         public HintViewModel ThenByHintText => new HintViewModel(Strings.THEN_BY_HINT_TEXT);
//
//         #endregion
//
//         #region Text - Party Sort Buttons
//
//         [DataSourceProperty]
//         public string SortPartyAscendingText => Strings.SORT_TEXT_ASCENDING;
//
//         [DataSourceProperty]
//         public HintViewModel SortPartyAscendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_ASCENDING);
//
//         [DataSourceProperty]
//         public string SortPartyDescendingText => Strings.SORT_TEXT_DESCENDING;
//
//         [DataSourceProperty]
//         public HintViewModel SortPartyDescendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_DESCENDING);
//
//         #endregion
//
//         #region Text - Other Sort Buttons
//
//         [DataSourceProperty]
//         public string SortOtherAscendingText => Strings.SORT_TEXT_ASCENDING;
//
//         [DataSourceProperty]
//         public HintViewModel SortOtherAscendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_ASCENDING);
//
//         [DataSourceProperty]
//         public string SortOtherDescendingText => Strings.SORT_TEXT_DESCENDING;
//
//         [DataSourceProperty]
//         public HintViewModel SortOtherDescendingHintText => new HintViewModel(Strings.SORT_HINT_TEXT_DESCENDING);
//
//         #endregion
//
//         #region Text - Party Sort Option Buttons
//
//         [DataSourceProperty]
//         public HintViewModel PartySortOrderOppositeDisabledHintText => new HintViewModel(Strings.SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED);
//
//         [DataSourceProperty]
//         public HintViewModel UpgradableOnTopDisabledHintText => new HintViewModel(Strings.UPGRADABLE_ON_TOP_HINT_TEXT_DISABLED);
//
//         #endregion
//
//         #region Text - Action Buttons
//
//         [DataSourceProperty]
//         public HintViewModel ActionUpgradeHintText => new HintViewModel(Strings.UPGRADE_HINT_TEXT);
//
//         [DataSourceProperty]
//         public HintViewModel ActionUpgradeDisabledHintText => new HintViewModel(Strings.UPGRADE_HINT_TEXT_DISABLED);
//
//         [DataSourceProperty]
//         public HintViewModel ActionRecruitHintText => new HintViewModel(Strings.RECRUIT_HINT_TEXT);
//
//         [DataSourceProperty]
//         public HintViewModel ActionRecruitDisabledHintText => new HintViewModel(Strings.RECRUIT_HINT_TEXT_DISABLED);
//
//         #endregion
//     }
// }
