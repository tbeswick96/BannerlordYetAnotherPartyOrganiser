using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using YAPO.Services;

// ReSharper disable InconsistentNaming
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Patches
{
    public class PartyVmPatches
    {
        [HarmonyPatch(typeof(PartyVM), "RefreshPartyInformation")]
        public static class PartyVMPopulatePartyListLabelCallsite
        {
            public static void Postfix(PartyVM __instance)
            {
                FieldInfo partyScreenLogicField =
                    __instance.GetType()
                              .BaseType?.GetField("_partyScreenLogic", BindingFlags.NonPublic | BindingFlags.Instance);
                PartyScreenLogic partyScreenLogic = (PartyScreenLogic) partyScreenLogicField?.GetValue(__instance);

                if (partyScreenLogic == null) return;

                __instance.OtherPartyTroopsLbl =
                    PartyHeaderCountHelper.PopulatePartyListLabel(__instance.OtherPartyTroops,
                                                                  partyScreenLogic.LeftPartySizeLimit);
                __instance.OtherPartyPrisonersLbl =
                    PartyHeaderCountHelper.PopulatePartyListLabel(__instance.OtherPartyPrisoners);
                __instance.MainPartyTroopsLbl =
                    PartyHeaderCountHelper.PopulatePartyListLabel(__instance.MainPartyTroops,
                                                                  partyScreenLogic.RightOwnerParty.PartySizeLimit);

                int limit = 0;
                if (partyScreenLogic.RightOwnerParty?.Leader != null)
                {
                    limit = partyScreenLogic.RightOwnerParty.PrisonerSizeLimit;
                }

                __instance.MainPartyPrisonersLbl =
                    PartyHeaderCountHelper.PopulatePartyListLabel(__instance.MainPartyPrisoners, limit);
            }
        }
    }
}
