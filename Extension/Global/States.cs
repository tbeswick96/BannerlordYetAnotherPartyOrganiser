using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;
using YAPO.Configuration.Models;

namespace YAPO.Global
{
    public static class States
    {
        public static bool HotkeyControl = false;
        public static PartyVmMixin PartyVmMixin = null;
        public static PartyScreenWidget PartyScreenWidget = null;
        
        public static string CurrentSaveName = "";
        public static SorterConfiguration PartySorterConfiguration = null;
        public static SorterConfiguration OtherSorterConfiguration = null;
    }
}
