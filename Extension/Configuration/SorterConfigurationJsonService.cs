using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TaleWorlds.Engine;
using YAPO.Configuration.Models;
using Path = System.IO.Path;

namespace YAPO.Configuration {
    public static class SorterConfigurationJsonService {
        private const string CONFIG_FILE = "YetAnotherPartyOrganiser/Config.json";
        private static string FilePath => $"{Utilities.GetConfigsPath()}{CONFIG_FILE}";

        public static SorterConfigurationContainer Load() {
            try {
                if (!File.Exists(FilePath)) {
                    return new SorterConfigurationContainer {ConfigurationSaves = new List<SorterConfigurationSave>()};
                }

                string fileContents = File.ReadAllText(FilePath);
                SorterConfigurationContainer configurationContainer = JsonConvert.DeserializeObject<SorterConfigurationContainer>(fileContents);
                return configurationContainer;
            } catch (Exception exception) {
                Global.Helpers.ShowError("Failed to load sorter configuration file for YetAnotherPartyOrganiser. Configurations have not been loaded and will get overwritten on next game save", "JsonFileService Load exception", exception);
            }

            return new SorterConfigurationContainer();
        }

        public static void Save(SorterConfigurationContainer configurationContainer) {
            try {
                string configurationContainerString = JsonConvert.SerializeObject(configurationContainer, Formatting.Indented);
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? throw new DirectoryNotFoundException());
                File.WriteAllText(FilePath, configurationContainerString);
            } catch (Exception exception) {
                Global.Helpers.ShowError("Failed to save sorter configuration file for YetAnotherPartyOrganiser. Configurations have not been saved and won't load correctly on next game load", "JsonFileService Save exception", exception);
            }
        }
    }
}
