using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using YAPO.Services;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Patches {
    public class PartyVmPatches {
        [HarmonyPatch(typeof(PartyVM), "RefreshPartyInformation")]
        public static class PartyVMPopulatePartyListLabelCallsite {
            public static void Postfix(PartyVM __instance) {
                if (__instance.PartyScreenLogic == null) return;

                __instance.OtherPartyTroopsLbl = PartyHeaderCountHelper.PopulatePartyListLabel(__instance.OtherPartyTroops, __instance.PartyScreenLogic.LeftPartySizeLimit);
                __instance.OtherPartyPrisonersLbl = PartyHeaderCountHelper.PopulatePartyListLabel(__instance.OtherPartyPrisoners);
                __instance.MainPartyTroopsLbl = PartyHeaderCountHelper.PopulatePartyListLabel(__instance.MainPartyTroops, __instance.PartyScreenLogic.RightOwnerParty.PartySizeLimit);

                int limit = 0;
                if (__instance.PartyScreenLogic.RightOwnerParty?.Leader != null) {
                    limit = __instance.PartyScreenLogic.RightOwnerParty.PrisonerSizeLimit;
                }

                __instance.MainPartyPrisonersLbl = PartyHeaderCountHelper.PopulatePartyListLabel(__instance.MainPartyPrisoners, limit);
            }
        }
    }
}
