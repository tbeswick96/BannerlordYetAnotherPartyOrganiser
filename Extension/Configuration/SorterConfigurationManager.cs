using System;
using System.Linq;
using ModLib.Debugging;
using YAPO.Configuration.Models;
using YAPO.Global;

namespace YAPO.Configuration
{
    public class SorterConfigurationManager
    {
        private static SorterConfigurationManager instance;

        private SorterConfigurationContainer _configurationContainer;

        public static SorterConfigurationManager Instance => instance ?? (instance = new SorterConfigurationManager());

        public void LoadConfigurations()
        {
            _configurationContainer = SorterConfigurationJsonService.Load();
        }

        public void SaveConfigurations()
        {
            try
            {
                SorterConfigurationSave configurationSave =
                    _configurationContainer.ConfigurationSaves.First(x => x.SaveName == States.CurrentSaveName);
                configurationSave.LastSaved = DateTime.Now;
            }
            catch (Exception exception)
            {
                ModDebug.ShowError($"YetAnotherPartyOrganiser failed to find sorter configuration for the current save {States.CurrentSaveName}. This shouldn't happen, please report this error by creating an issue on our github page",
                                   "SorterConfigurationManager SaveConfigurations error",
                                   exception);
            }

            RemoveOldSaveConfigurations();

            SorterConfigurationJsonService.Save(_configurationContainer);
        }

        private void RemoveOldSaveConfigurations()
        {
            _configurationContainer.ConfigurationSaves = _configurationContainer
                                                        .ConfigurationSaves.OrderByDescending(x => x.LastSaved)
                                                        .Take(50)
                                                        .ToList();
        }

        public (SorterConfiguration, SorterConfiguration) GetConfiguration(string saveName)
        {
            SorterConfigurationSave configurationSave =
                _configurationContainer.ConfigurationSaves.FirstOrDefault(x => x.SaveName == saveName);
            if (configurationSave != null) return (configurationSave.Party, configurationSave.Other);

            configurationSave = new SorterConfigurationSave
            {
                SaveName = saveName, Party = new SorterConfiguration(), Other = new SorterConfiguration()
            };
            _configurationContainer.ConfigurationSaves.Add(configurationSave);
            return (configurationSave.Party, configurationSave.Other);
        }
    }
}
