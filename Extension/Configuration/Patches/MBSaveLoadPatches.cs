using HarmonyLib;
using TaleWorlds.Core;
using YAPO.Global;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Configuration.Patches {
    public class MBSaveLoadPatches {
        [HarmonyPatch(typeof(MBSaveLoad), "LoadSaveGameData")]
        public static class MBSaveLoadLoadSaveGameDataPatch {
            public static void Postfix(string saveName) {
                States.LoadedSaveName = saveName;
                SorterConfigurationManager.Instance.LoadConfigurations();
                (States.PartySorterConfiguration, States.OtherSorterConfiguration) = SorterConfigurationManager.Instance.GetConfiguration(States.LoadedSaveName);
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "SaveGame")]
        public static class MBSaveLoadSaveGamePatch {
            public static void Postfix(string saveName) {
                // Removing ".tmp" from the savename is necessary as MBB starts saving the file as <name>.tmp, and only removes the ".tmp" when it finalises the save file
                States.NewSaveName = saveName.Replace(".tmp", "");
                SorterConfigurationManager.Instance.SaveConfigurations();
            }
        }
    }
}
