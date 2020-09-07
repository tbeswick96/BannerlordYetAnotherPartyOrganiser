using System.Collections.Generic;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;
using MCM.Abstractions.Settings.Base.Global;
using YAPO.Global;
using YAPO.MultipathUpgrade.Enum;
using YAPO.MultipathUpgrade.Model;

namespace YAPO {
    public class YapoSettings : AttributeGlobalSettings<YapoSettings> {
        public override string Id => $"{Strings.MODULE_FOLDER_NAME}Settings";
        public override string DisplayName => $"{Strings.MODULE_NAME} Settings";
        public override string FolderName => Strings.MODULE_FOLDER_NAME;
        public override string Format => "json";

        #region Sorting

        [SettingPropertyBool(Strings.SETTINGS_ENABLE_AUTO_SORT_NAME, Order = 1, RequireRestart = false, HintText = Strings.SETTINGS_ENABLE_AUTO_SORT_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_SORTING_GROUP_NAME)]
        public bool IsAutoSortEnabled { get; set; } = true;

        #endregion

        #region Upgrade

        [SettingPropertyInteger(Strings.SETTINGS_BUFFER_DAILY_COST_NAME, 0, 10, Order = 1, RequireRestart = false, HintText = Strings.SETTINGS_BUFFER_DAILY_COST_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public int DaysToPayDailyCostsBuffer { get; set; } = 3;

        [SettingPropertyBool(Strings.SETTINGS_SPLIT_UPGRADES_NAME, Order = 2, RequireRestart = false, HintText = Strings.SETTINGS_SPLIT_UPGRADES_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public bool SplitUpgrades { get; set; }

        [SettingPropertyBool(Strings.SETTINGS_PLAYER_DECISION_NAME, Order = 3, RequireRestart = false, HintText = Strings.SETTINGS_PLAYER_DECISION_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public bool PlayerDecision { get; set; }

        [SettingPropertyBool(Strings.SETTINGS_PREFER_SHIELD_NAME, Order = 4, RequireRestart = false, HintText = Strings.SETTINGS_PREFER_SHIELD_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public bool PreferShield { get; set; } = true;

        [SettingPropertyDropdown(Strings.SETTINGS_RANGED_PREFERENCE_NAME, Order = 5, RequireRestart = false, HintText = Strings.SETTINGS_RANGED_PREFERENCE_TOOLTIP), SettingPropertyGroup(Strings.SETTINGS_UPGRADE_GROUP_NAME)]
        public DropdownDefault<RangedPreferenceObject> RangedPreference { get; set; } = new DropdownDefault<RangedPreferenceObject>(new[] {
                                                                                                                                        new RangedPreferenceObject(MultipathUpgrade.Enum.RangedPreference.NONE, "No Preference"),
                                                                                                                                        new RangedPreferenceObject(MultipathUpgrade.Enum.RangedPreference.BOWS, "Prefer Bows"),
                                                                                                                                        new RangedPreferenceObject(MultipathUpgrade.Enum.RangedPreference.CROSSBOWS, "Prefer Crossbows")
                                                                                                                                    },
                                                                                                                                    0);

        public readonly List<PreferredTroopsByCulture> PreferredTroopsByCulture = new List<PreferredTroopsByCulture> {
            new PreferredTroopsByCulture {CultureIdentifier = "Khuzait", TroopClasses = new List<CharacterClassType> {CharacterClassType.HORSE_ARCHER, CharacterClassType.CAVALRY}},
            new PreferredTroopsByCulture {CultureIdentifier = "Battania", TroopClasses = new List<CharacterClassType> {CharacterClassType.INFANTRY}},
            new PreferredTroopsByCulture {CultureIdentifier = "Aserai", TroopClasses = new List<CharacterClassType> {CharacterClassType.INFANTRY, CharacterClassType.HORSE_ARCHER, CharacterClassType.RANGED}},
            new PreferredTroopsByCulture {CultureIdentifier = "Sturgia", TroopClasses = new List<CharacterClassType> {CharacterClassType.INFANTRY}},
            new PreferredTroopsByCulture {CultureIdentifier = "Vlandia", TroopClasses = new List<CharacterClassType> {CharacterClassType.CAVALRY, CharacterClassType.INFANTRY, CharacterClassType.RANGED}},
            new PreferredTroopsByCulture {CultureIdentifier = "Empire", TroopClasses = new List<CharacterClassType> {CharacterClassType.INFANTRY, CharacterClassType.RANGED}}
        };

        #endregion
    }
}
