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

        [HarmonyPatch(typeof(PartyCharacterVM), "UpdateTransferHint")]
        public class UpdateTransferHintPatch
        {
            public static void Postfix(PartyCharacterVM __instance)
            {
                __instance.TransferHint.HintText += Strings.TRANSFER_HINT_TEXT_APPENDIX;
            }
        }
    }
}
