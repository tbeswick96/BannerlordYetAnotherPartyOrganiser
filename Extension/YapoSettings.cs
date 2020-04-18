using System.Xml.Serialization;
using ModLib;
using ModLib.Attributes;
using YAPO.Global;

namespace YAPO
{
    public class YapoSettings : SettingsBase
    {
        public const string InstanceId = Strings.MODULE_FOLDER_NAME + "Settings";

        public override string ModName { get; } = Strings.MODULE_NAME;
        public override string ModuleFolderName { get; } = Strings.MODULE_FOLDER_NAME;

        [XmlElement]
        public override string ID { get; set; } = InstanceId;

        public static YapoSettings Instance => (YapoSettings) SettingsDatabase.GetSettings(InstanceId);

        #region GeneralSettings

        [XmlElement,
         SettingProperty(Strings.SETTINGS_ENABLE_AUTO_SORT_NAME, Strings.SETTINGS_ENABLE_AUTO_SORT_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_GENERAL_GROUP_NAME)]
        public bool IsAutoSortEnabled { get; set; } = true;

        #endregion

        #region Fixes

        [XmlElement,
         SettingProperty(
             Strings.SETTINGS_ENABLE_FORMATION_PERSISTENCE_NAME,
             Strings.SETTINGS_ENABLE_FORMATION_PERSISTENCE_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_FIXES_GROUP_NAME)]
        public bool IsFormationPersistenceFixEnabled { get; set; } = true;

        #endregion
    }
}
