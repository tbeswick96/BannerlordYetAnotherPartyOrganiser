using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace YAPO.Fixes.Formations.Patches
{
    public class CampaignPatches
    {
        [HarmonyPatch]
        public static class CampaignStartPatch
        {
            public static bool Prepare()
            {
                if (YapoSettings.Instance.IsFormationPersistenceFixEnabled)
                {
                    return TryToAccessTargetMethod() != null;
                }

                return false;
            }

            public static MethodBase TargetMethod()
            {
                return TryToAccessTargetMethod();
            }

            private static MethodBase TryToAccessTargetMethod()
            {
                Type[] parameters = {typeof(CampaignGameStarter)};
                MethodBase method = AccessTools.Method(typeof(Campaign), "OnSessionStart", parameters);

                if (method == null)
                {
                    method = AccessTools.Method(typeof(Campaign), "OnSessionInitialized", parameters);
                }

                return method;
            }

            public static void Postfix()
            {
                FixedFormationsBehaviour.Instance.FormationClasses =
                    FixedFormationsBehaviour.Instance.FormationClasses ??
                    new Dictionary<BasicCharacterObject, FormationClass>();
                FixedFormationsBehaviour.Instance.GameStarted = true;
            }
        }
    }
}
