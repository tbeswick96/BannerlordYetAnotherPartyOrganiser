using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using TaleWorlds.Engine;
using YAPO.Configuration.Models;
using Path = System.IO.Path;

namespace YAPO.Configuration {
    public static class SorterConfigurationJsonService {
        private static readonly SemaphoreSlim SEMAPHORE = new SemaphoreSlim(1);
        private static string FilePath => $"{Utilities.GetConfigsPath()}YetAnotherPartyOrganiser/Config.json";

        public static SorterConfigurationContainer Load() {
            try {
                SEMAPHORE.Wait();
                if (!File.Exists(FilePath)) {
                    return new SorterConfigurationContainer {ConfigurationSaves = new List<SorterConfigurationSave>()};
                }

                string fileContents = File.ReadAllText(FilePath);
                SorterConfigurationContainer configurationContainer = JsonConvert.DeserializeObject<SorterConfigurationContainer>(fileContents);
                return configurationContainer;
            } catch (Exception exception) {
                Global.Helpers.ShowError("Failed to load sorter configuration file for YetAnotherPartyOrganiser. Configurations have not been loaded and will get overwritten on next game save", "JsonFileService Load exception", exception);
            } finally {
                SEMAPHORE.Release();
            }

            return new SorterConfigurationContainer();
        }

        public static void Save(SorterConfigurationContainer configurationContainer) {
            try {
                SEMAPHORE.Wait();
                string configurationContainerString = JsonConvert.SerializeObject(configurationContainer, Formatting.Indented);
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? throw new DirectoryNotFoundException());
                File.WriteAllText(FilePath, configurationContainerString);
            } catch (Exception exception) {
                Global.Helpers.ShowError("Failed to save sorter configuration file for YetAnotherPartyOrganiser. Configurations have not been saved and won't load correctly on next game load", "JsonFileService Save exception", exception);
            } finally {
                SEMAPHORE.Release();
            }
        }
    }
}
