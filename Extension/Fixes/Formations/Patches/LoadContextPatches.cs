using System.Reflection;
using HarmonyLib;
using TaleWorlds.SaveSystem;

namespace YAPO.Fixes.Formations.Patches
{
    public class LoadContextPatches
    {
        [HarmonyPatch]
        public static class LoadDataPatch
        {
            public static bool Prepare()
            {
                return YapoSettings.Instance.IsFormationPersistenceFixEnabled;
            }

            public static MethodBase TargetMethod()
            {
                return AccessTools.Method(AccessTools.TypeByName("TaleWorlds.SaveSystem.Load.LoadContext"),
                    "Load", new[] {typeof(LoadData)});
            }

            public static void Prefix()
            {
                FixedFormationsBehaviour.Instance.GameStarted = false;
            }
        }
    }
}
