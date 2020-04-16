using System.Xml.Serialization;
using ModLib;
using ModLib.Attributes;
using YAPO.Global;

namespace YAPO
{
    public class Settings : SettingsBase
    {
        public const string InstanceId = Strings.MODULE_FOLDER_NAME + "Settings";

        public override string ModName { get; } = Strings.MODULE_NAME;
        public override string ModuleFolderName { get; } = Strings.MODULE_FOLDER_NAME;

        [XmlElement]
        public override string ID { get; set; } = InstanceId;

        public static Settings Instance => (Settings) SettingsDatabase.GetSettings(InstanceId);

        #region GeneralSettings

        [XmlElement, SettingProperty(Strings.SETTINGS_ENABLE_AUTO_SORT_NAME, Strings.SETTINGS_ENABLE_AUTO_SORT_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_GENERAL_GROUP_NAME)]
        public bool AutoSortEnabled { get; set; } = true;

        #endregion
    }
}
