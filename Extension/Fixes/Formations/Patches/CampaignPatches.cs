using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace YAPO.Fixes.Formations.Patches
{
    public class CampaignPatches
    {
        [HarmonyPatch(typeof(Campaign), "OnSessionInitialized", typeof(CampaignGameStarter))]
        public static class OnSessionInitializedPatch
        {
            public static void Postfix()
            {
                FixedFormationsBehaviour.Instance.FormationClasses = FixedFormationsBehaviour.Instance.FormationClasses ?? new Dictionary<BasicCharacterObject, FormationClass>();
                FixedFormationsBehaviour.Instance.GameStarted = true;
            }
        }
    }
}
