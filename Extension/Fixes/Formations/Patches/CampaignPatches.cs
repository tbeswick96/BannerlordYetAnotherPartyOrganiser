using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

// ReSharper disable InconsistentNaming
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

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

            public static MethodBase TargetMethod() => TryToAccessTargetMethod();

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
                FixedFormationsBehaviour.INSTANCE.FormationClasses =
                    FixedFormationsBehaviour.INSTANCE.FormationClasses ??
                    new Dictionary<BasicCharacterObject, FormationClass>();
                FixedFormationsBehaviour.INSTANCE.GameStarted = true;
            }
        }
    }
}
