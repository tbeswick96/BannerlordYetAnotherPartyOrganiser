using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;

namespace YAPO.UI
{
    public static class PartyScreenWidgetExtensions
    {
        public static void RefreshWidgetStates(this PartyScreenWidget partyScreenWidget, SortMode currentPartyThenByMode)
        {
            List<Widget> allChildren = partyScreenWidget.AllChildren.ToList();
            Widget partySortOrderOppositeButton = allChildren.FirstOrDefault(x => x.Id == "PartySortOrderOppositeButton");
            partySortOrderOppositeButton?.SetState(currentPartyThenByMode == SortMode.NONE ? "Disabled" : "Default");
        }
    }
}
