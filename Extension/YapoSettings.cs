using System.Collections.Generic;
using System.Xml.Serialization;
using ModLib.Definitions;
using ModLib.Definitions.Attributes;
using YAPO.Global;
using YAPO.MultipathUpgrade.Enum;
using YAPO.MultipathUpgrade.Model;

namespace YAPO
{
    public class YapoSettings : SettingsBase
    {
        private const string INSTANCE_ID = Strings.MODULE_FOLDER_NAME + "Settings";

        public override string ModName { get; } = Strings.MODULE_NAME;
        public override string ModuleFolderName { get; } = Strings.MODULE_FOLDER_NAME;

        [XmlElement]
        public override string ID { get; set; } = INSTANCE_ID;

        public static YapoSettings Instance => (YapoSettings) SettingsDatabase.GetSettings<YapoSettings>();

        #region GeneralSettings

        [XmlElement, SettingProperty(Strings.SETTINGS_ENABLE_AUTO_SORT_NAME, Strings.SETTINGS_ENABLE_AUTO_SORT_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_GENERAL_GROUP_NAME)]
        public bool IsAutoSortEnabled { get; set; } = true;

        #endregion

        #region Fixes

        [XmlElement,
         SettingProperty(Strings.SETTINGS_ENABLE_FORMATION_PERSISTENCE_NAME,
                         Strings.SETTINGS_ENABLE_FORMATION_PERSISTENCE_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_FIXES_GROUP_NAME)]
        public bool IsFormationPersistenceFixEnabled { get; set; } = true;

        #endregion

        #region UpgradeSettings

        [XmlElement,
         SettingProperty(Strings.SETTINGS_RANGED_PREFERENCE_NAME, 0, 2, Strings.SETTINGS_RANGED_PREFERENCE_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public int RangedPreference { get; set; }

        [XmlElement, SettingProperty(Strings.SETTINGS_PREFER_SHIELD_NAME, Strings.SETTINGS_PREFER_SHIELD_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public bool PreferShield { get; set; } = true;

        [XmlElement,
         SettingProperty(Strings.SETTINGS_BUFFER_DAILY_COST_NAME, 0, 10, Strings.SETTINGS_BUFFER_DAILY_COST_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public int DaysToPayDailyCostsBuffer { get; set; } = 3;

        [XmlElement, SettingProperty(Strings.SETTINGS_SPLIT_UPGRADES_NAME, Strings.SETTINGS_SPLIT_UPGRADES_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public bool SplitUpgrades { get; set; }

        [XmlElement, SettingProperty(Strings.SETTINGS_PLAYER_DECISION_NAME, Strings.SETTINGS_PLAYER_DECISION_TOOLTIP),
         SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public bool PlayerDecision { get; set; }

        [XmlElement]
        public List<PreferredTroopsByCulture> PreferredTroopsByCulture = new List<PreferredTroopsByCulture>
        {
            new PreferredTroopsByCulture
            {
                CultureIdentifier = "Khuzait",
                TroopClasses = new List<CharacterClassType>
                {
                    CharacterClassType.HORSE_ARCHER, CharacterClassType.CAVALRY
                }
            },
            new PreferredTroopsByCulture
            {
                CultureIdentifier = "Battania",
                TroopClasses = new List<CharacterClassType> {CharacterClassType.INFANTRY}
            },
            new PreferredTroopsByCulture
            {
                CultureIdentifier = "Aserai",
                TroopClasses = new List<CharacterClassType>
                {
                    CharacterClassType.INFANTRY, CharacterClassType.HORSE_ARCHER, CharacterClassType.RANGED
                }
            },
            new PreferredTroopsByCulture
            {
                CultureIdentifier = "Sturgia",
                TroopClasses = new List<CharacterClassType> {CharacterClassType.INFANTRY}
            },
            new PreferredTroopsByCulture
            {
                CultureIdentifier = "Vlandia",
                TroopClasses = new List<CharacterClassType>
                {
                    CharacterClassType.CAVALRY, CharacterClassType.INFANTRY, CharacterClassType.RANGED
                }
            },
            new PreferredTroopsByCulture
            {
                CultureIdentifier = "Empire",
                TroopClasses =
                    new List<CharacterClassType> {CharacterClassType.INFANTRY, CharacterClassType.RANGED}
            }
        };

        #endregion
    }
}
