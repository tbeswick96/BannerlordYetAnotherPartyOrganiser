﻿namespace YAPO.Global {
    // TODO: How to localise this?
    public static class Strings {
        public const string MODULE_NAME = "Yet Another Party Organiser";
        public const string MODULE_FOLDER_NAME = "YetAnotherPartyOrganiser";

        public const string MODULE_DATA_PARTY_COUNT_STRINGS = "ModuleData/PartyCountStrings.xml";

        public const string SETTINGS_SORTING_GROUP_NAME = "Sorting";
        public const string SETTINGS_ENABLE_AUTO_SORT_NAME = "Enable Auto Sorting";
        public const string SETTINGS_ENABLE_AUTO_SORT_TOOLTIP = "When enabled troops and prisoners are sorted upon opening the party screen.";

        public const string SETTINGS_UPGRADE_GROUP_NAME = "Upgrade";
        public const string SETTINGS_PREFER_SHIELD_NAME = "Prefer infantry with shields";
        public const string SETTINGS_PREFER_SHIELD_TOOLTIP = "Prefer infantry with shields when upgrading to infantry troop classes.";
        public const string SETTINGS_RANGED_PREFERENCE_NAME = "Prefer ranged with specific weapons";
        public const string SETTINGS_RANGED_PREFERENCE_TOOLTIP = "Prefer specific ranged weapons when upgrading to ranged troop classes. (0 for no preference, 1 for Bows, 2 for crossbows)";
        public const string SETTINGS_BUFFER_DAILY_COST_NAME = "Keep enough gold for X days of troop wages";
        public const string SETTINGS_BUFFER_DAILY_COST_TOOLTIP = "Reserves gold to pay daily costs for X amount of days.";
        public const string SETTINGS_SPLIT_UPGRADES_NAME = "Split upgrades if decision can't be made";
        public const string SETTINGS_SPLIT_UPGRADES_TOOLTIP = "Splits upgradepaths when no decision can be made. NOTE: decisions will be made based on culture strengths.";
        public const string SETTINGS_PLAYER_DECISION_NAME = "Player decides on split upgrade path";
        public const string SETTINGS_PLAYER_DECISION_TOOLTIP = "Lets player decide on every upgrade when two upgrade options are available.";

        public const string SORT_TEXT_ASCENDING = "/\\";
        public const string SORT_TEXT_DESCENDING = "\\/";
        public const string SORT_ORDER_OPPOSITE_TEXT_SAME = "SO";
        public const string SORT_ORDER_OPPOSITE_TEXT_OPPOSITE = "OO";
        public const string UPGRADABLE_ON_TOP_TEXT_ON = "UN";
        public const string UPGRADABLE_ON_TOP_TEXT_OFF = "UT";

        public const string SORT_HINT_TEXT_ASCENDING = "Sort ascending";
        public const string SORT_HINT_TEXT_DESCENDING = "Sort descending";
        public const string SORT_BY_HINT_TEXT = "Select sort mode to sort troops by";
        public const string THEN_BY_HINT_TEXT = "Select sort mode to further sort troops by after above sort mode";
        public const string SORT_ORDER_OPPOSITE_HINT_TEXT_SAME = "Set same sort order for Sort By and Then By";
        public const string SORT_ORDER_OPPOSITE_HINT_TEXT_OPPOSITE = "Set opposite sort order for Sort By and Then By";
        public const string SORT_ORDER_OPPOSITE_HINT_TEXT_DISABLED = "Opposite sort order toggle requires a Then By mode to be selected";
        public const string UPGRADABLE_ON_TOP_HINT_TEXT_ON = "Place upgradable troops normally";
        public const string UPGRADABLE_ON_TOP_HINT_TEXT_OFF = "Place upgradable troops on top";
        public const string UPGRADABLE_ON_TOP_HINT_TEXT_DISABLED = "Upgradable troops on top requires upgradable troops";
        public const string UPGRADE_HINT_TEXT = "Upgrade all toops with single upgrade path";
        public const string UPGRADE_HINT_TEXT_DISABLED = "Upgrade all toops requires upgradable troops";
        public const string RECRUIT_HINT_TEXT = "Recruit all recruitable prisoners (Hold control to override party size limit)";
        public const string RECRUIT_HINT_TEXT_DISABLED = "Recruit all recruitable prisoners requires recruitable prisoners";
        public const string TRANSFER_HINT_TEXT_APPENDIX = "\nHold [Left Ctrl] to do all";

        public const string TYPE_ORDER_HINT_TEXT_OPEN = "Customise Type order to determine sort order";
        public const string TYPE_ORDER_HINT_TEXT_CLOSE = "Close";
    }
}
