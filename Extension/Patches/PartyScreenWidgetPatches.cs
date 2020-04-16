using HarmonyLib;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;
using YAPO.Global;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Patches
{
    public class PartyScreenWidgetPatches
    {
        [HarmonyPatch(typeof(PartyScreenWidget), MethodType.Constructor, typeof(UIContext))]
        public static class PartyScreenWidgetConstructorCallsite
        {
            public static void Postfix(PartyScreenWidget __instance)
            {
                States.PartyScreenWidget = __instance;
            }
        }

        [HarmonyPatch(typeof(PartyScreenWidget), "OnConnectedToRoot")]
        public static class PartyScreenWidgetOnConnectedToRootCallsite
        {
            public static void Postfix()
            {
                States.PartyVmMixin?.FirstRefresh();
            }
        }
    }
}
