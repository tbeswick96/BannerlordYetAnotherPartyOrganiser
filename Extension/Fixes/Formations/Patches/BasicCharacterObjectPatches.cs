using HarmonyLib;
using TaleWorlds.Core;

// ReSharper disable InconsistentNaming
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Fixes.Formations.Patches
{
    public class BasicCharacterObjectPatches
    {
        [HarmonyPatch(typeof(BasicCharacterObject), "get_DefaultFormationGroup")]
        public static class GetDefaultFormationGroupPatch
        {
            public static bool Prepare() => YapoSettings.Instance.IsFormationPersistenceFixEnabled;

            public static bool Prefix(BasicCharacterObject __instance, ref int __result)
            {
                if (!FixedFormationsBehaviour.INSTANCE.GameStarted
                    || FixedFormationsBehaviour.INSTANCE.FormationClasses == null
                    || !FixedFormationsBehaviour.INSTANCE.FormationClasses.ContainsKey(__instance))
                {
                    return true;
                }

                __result = (int) FixedFormationsBehaviour.INSTANCE.FormationClasses[__instance];
                return false;
            }
        }

        [HarmonyPatch(typeof(BasicCharacterObject), "get_CurrentFormationClass")]
        public static class GetCurrentFormationClassPatch
        {
            public static bool Prepare() => YapoSettings.Instance.IsFormationPersistenceFixEnabled;

            public static bool Prefix(BasicCharacterObject __instance, ref FormationClass __result)
            {
                if (!FixedFormationsBehaviour.INSTANCE.GameStarted
                    || FixedFormationsBehaviour.INSTANCE.FormationClasses == null
                    || !FixedFormationsBehaviour.INSTANCE.FormationClasses.ContainsKey(__instance))
                {
                    return true;
                }

                __result = FixedFormationsBehaviour.INSTANCE.FormationClasses[__instance];
                return false;
            }
        }

        [HarmonyPatch(typeof(BasicCharacterObject), "set_CurrentFormationClass", typeof(FormationClass))]
        public static class SetCurrentFormationClassPatch
        {
            public static bool Prepare() => YapoSettings.Instance.IsFormationPersistenceFixEnabled;

            public static bool Prefix(BasicCharacterObject __instance, FormationClass value)
            {
                if (!FixedFormationsBehaviour.INSTANCE.GameStarted
                    || FixedFormationsBehaviour.INSTANCE.FormationClasses == null)
                {
                    return true;
                }

                FixedFormationsBehaviour.INSTANCE.FormationClasses[__instance] = value;
                return false;
            }
        }
    }
}
