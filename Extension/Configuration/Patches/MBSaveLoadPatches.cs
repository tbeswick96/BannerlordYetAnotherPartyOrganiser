using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.Core;
using YAPO.Global;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace YAPO.Configuration.Patches
{
    public class MBSaveLoadPatches
    {
        [HarmonyPatch(typeof(MBSaveLoad), "LoadSaveGameData")]
        public static class MBSaveLoadLoadSaveGameDataPatch
        {
            public static void Postfix(string saveName)
            {
                States.CurrentSaveName = saveName;
                Task unused = Task.Run(LoadAsync);
            }

            private static void LoadAsync()
            {
                SorterConfigurationManager.Instance.LoadConfigurations();
                (States.PartySorterConfiguration, States.OtherSorterConfiguration) =
                    SorterConfigurationManager.Instance.GetConfiguration(States.CurrentSaveName);
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "SaveGame")]
        public static class MBSaveLoadSaveGamePatch
        {
            public static void Postfix(string saveName)
            {
                // Removing ".tmp" from the savename is necessary as MBB starts saving the file as <name>.tmp,
                // and only removes the ".tmp" when it finalises the save file
                States.CurrentSaveName = saveName.Replace(".tmp", "");
                Task unused = Task.Run(SaveAsync);
            }

            private static void SaveAsync()
            {
                SorterConfigurationManager.Instance.SaveConfigurations();
            }
        }
    }
}
