using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using YAPO.Global;

namespace YAPO.Patches
{
    public class PartyCharacterVmPatches
    {
        [HarmonyPatch(typeof(PartyCharacterVM), "ApplyTransfer")]
        public class ApplyTransferPatch
        {
            public static void Prefix(PartyCharacterVM __instance, ref int transferAmount)
            {
                if (States.HotkeyControl) transferAmount = __instance.Troop.Number;
            }
        }
    }
}
