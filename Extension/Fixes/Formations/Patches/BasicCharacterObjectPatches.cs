using HarmonyLib;
using TaleWorlds.Core;

namespace YAPO.Fixes.Formations.Patches
{
    public class BasicCharacterObjectPatches
    {
        [HarmonyPatch(typeof(BasicCharacterObject), "get_DefaultFormationGroup")]
        public static class GetDefaultFormationGroupPatch
        {
            public static bool Prepare()
            {
                return YapoSettings.Instance.IsFormationPersistenceFixEnabled;
            }

            public static bool Prefix(BasicCharacterObject __instance, ref int __result)
            {
                if (!FixedFormationsBehaviour.Instance.GameStarted
                    || FixedFormationsBehaviour.Instance.FormationClasses == null
                    || !FixedFormationsBehaviour.Instance.FormationClasses.ContainsKey(__instance))
                {
                    return true;
                }

                __result = (int) FixedFormationsBehaviour.Instance.FormationClasses[__instance];
                return false;
            }
        }

        [HarmonyPatch(typeof(BasicCharacterObject), "get_CurrentFormationClass")]
        public static class GetCurrentFormationClassPatch
        {
            public static bool Prepare()
            {
                return YapoSettings.Instance.IsFormationPersistenceFixEnabled;
            }

            public static bool Prefix(BasicCharacterObject __instance, ref FormationClass __result)
            {
                if (!FixedFormationsBehaviour.Instance.GameStarted
                    || FixedFormationsBehaviour.Instance.FormationClasses == null
                    || !FixedFormationsBehaviour.Instance.FormationClasses.ContainsKey(__instance))
                {
                    return true;
                }

                __result = FixedFormationsBehaviour.Instance.FormationClasses[__instance];
                return false;
            }
        }

        [HarmonyPatch(typeof(BasicCharacterObject), "set_CurrentFormationClass", typeof(FormationClass))]
        public static class SetCurrentFormationClassPatch
        {
            public static bool Prepare()
            {
                return YapoSettings.Instance.IsFormationPersistenceFixEnabled;
            }

            public static bool Prefix(BasicCharacterObject __instance, FormationClass value)
            {
                if (!FixedFormationsBehaviour.Instance.GameStarted
                    || FixedFormationsBehaviour.Instance.FormationClasses == null)
                {
                    return true;
                }

                FixedFormationsBehaviour.Instance.FormationClasses[__instance] = value;
                return false;
            }
        }
    }
}
