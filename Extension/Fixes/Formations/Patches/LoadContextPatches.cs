using System.Reflection;
using HarmonyLib;
using TaleWorlds.SaveSystem;

// ReSharper disable InconsistentNaming
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Fixes.Formations.Patches
{
    public class LoadContextPatches
    {
        [HarmonyPatch]
        public static class LoadDataPatch
        {
            public static bool Prepare() => YapoSettings.Instance.IsFormationPersistenceFixEnabled;

            public static MethodBase TargetMethod() =>
                AccessTools.Method(AccessTools.TypeByName("TaleWorlds.SaveSystem.Load.LoadContext"),
                                   "Load",
                                   new[] {typeof(LoadData), typeof(bool)});

            public static void Prefix()
            {
                FixedFormationsBehaviour.INSTANCE.GameStarted = false;
            }
        }
    }
}
