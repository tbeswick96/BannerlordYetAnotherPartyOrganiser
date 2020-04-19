using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using YAPO.Global;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Patches
{
    public class PartyVmPatches
    {
        [HarmonyPatch(typeof(PartyVM), "Update")]
        public static class PartyVMUpdateCallsite
        {
            public static void Postfix(PartyScreenLogic.PartyCommand command)
            {
                if (States.MassActionInProgress) return;

                switch (command.Code)
                {
                    case PartyScreenLogic.PartyCommandCode.TransferTroop:
                    case PartyScreenLogic.PartyCommandCode.TransferPartyLeaderTroop:
                    case PartyScreenLogic.PartyCommandCode.TransferTroopToLeaderSlot:
                        States.PartyVmMixin?.SortInPlace(SortSide.PARTY);
                        States.PartyVmMixin?.SortInPlace(SortSide.OTHER);
                        break;
                    case PartyScreenLogic.PartyCommandCode.UpgradeTroop:
                    case PartyScreenLogic.PartyCommandCode.RecruitTroop:
                        States.PartyVmMixin?.SortInPlace(SortSide.PARTY);
                        break;
                    case PartyScreenLogic.PartyCommandCode.ShiftTroop:
                    case PartyScreenLogic.PartyCommandCode.ExecuteTroop: break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        [HarmonyPatch(typeof(PartyVM), "ExecuteReset")]
        public static class PartyVMExecuteResetCallsite
        {
            public static void Postfix()
            {
                States.PartyVmMixin?.SortInPlace(SortSide.PARTY);
                States.PartyVmMixin?.SortInPlace(SortSide.OTHER);
            }
        }
    }
}
