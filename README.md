# Mount &amp; Blade II: Bannerlord - Yet Another Party Organiser (YAPO)

[Nexus Link](https://www.nexusmods.com/mountandblade2bannerlord/mods/539)

### Another party organisation mod to add to your ever expanding collection!

**IMPORTANT** If you used a version prior to 1.5.0, `ModLib` is no longer a dependency. Make sure you delete the YAPO module folder before installing the new version.

**IMPORTANT** If you used a version prior to 1.5.0, there are new dependencies. See `Dependencies` below.

---

## Dependencies

| Dependency | Required | Notes |
|------------|:---------:|-------|
| [Harmony](https://www.nexusmods.com/mountandblade2bannerlord/mods/2006) | ✓ | Load first |
| [Butterlib](https://www.nexusmods.com/mountandblade2bannerlord/mods/2018) | ✓ | Load second |
| [UIExtender](https://www.nexusmods.com/mountandblade2bannerlord/mods/2102) | ✓ | Load third |
| [MCMv4 (Beta)](https://www.nexusmods.com/mountandblade2bannerlord/mods/612) | ✓ | Load fourth. Load before Native |

Make sure that the dependent mods are enabled and listed above this mod within the launcher.

## Features

### Sorting

- Separate Main Party and Other Party sorting
- Sorting modes:
  - Alphabetical (Name)
  - Type (Infantry, Ranged, Cavalry etc)
  - Group (Formation group)
  - Tier
  - Culture
  - Count
- Two sorting modes can be selected through two drop downs
  - Sorting is treated as Sort By -> Then By
  - Button to set opposite sorting direction between the Sort By -> Then By Modes, to allow more granular sorting rules
- Customisable sorting rules
  - Type sorting mode can be customised for each party and for each sort step. Select Type and press the * next to the dropdown
- Button to place upgradable troops at the top of the list
- Buttons will disable themselves if functionality is not available (e.g. cannot set opposite sorting if no Then By mode is selected)
- Sorting configuration is saved to your save file

### Party Actions

- Upgrade all single-path and most multi-path upgradable troops based on the culture's strength
- Recruit recruitable prisoners - sort order is respected, party limit is respected but can be overridden by holding CTRL
- Buttons will disable themselves if functionality is not available (e.g. no troops to upgrade)

### Hotkeys

- LCTRL + LSHIFT + A - Sort Ascending (Both Sides)
- LCTRL + LSHIFT + D - Sort Descending (Both Sides)
- LCTRL + {Move troops arrow} - Moves entire stack of troops
- U - Upgrade Troops
- R - Recruit Prisoners

### Options

Options can be found via the main menu option:
`Mod Options -> Yet Another Party Organiser`

| Option | Default | Description |
|--------|---------|-------------|
| Enable Auto Sorting | On | Sorts troops and prisoners upon opening the party screen |
| Prefer ranged with specific weapons | 0 | Prefer specific ranged weapons when upgrading to ranged troop classes. (0 for no preference, 1 for Bows, 2 for crossbows) |
| Keep enough gold for X days of troop wages | 3 | Reserves gold to pay daily costs for X amount of days |
| Split upgrades if decision can't be made | false | Splits upgrade paths when no decision can be made _NOTE: decisions will be made based on culture strengths_ |
| Player decides on split upgrade path | false | Lets player decide on every upgrade when two upgrade options are available |

### Planned Features

- Anything that fits within the scope of party organisation that anyone may suggest! Feel free to create feature requests!
- Configurable group sort orders (`Type` done)
- Configurable upgrade rules
- Handling of all multi-path upgradable troops by ratio or by party composition
- Configurable main party prisoner and other party troops/prisoner (after battle) recruitment rules

## Versions

We maintain separate versions for Bannerlord Stable and Beta branches. The beta-compatible branch is clearly labelled with BETA in the files section.

We will aim to do the majority of development on the beta-compatible version, so new features will most likely hit that version before they hit stable.

For bugs and crashes, we will aim to make fixes for these available on both versions, depending on their nature.

Please make sure you identify which version you are using when reporting any issues on the github repository.

## Known Issues

- None

## Contributing

This mod is open source, so please feel free to explore it. It isn't perfect but we are working on it.

If you want to help don't hesitate to get into contact and have a look at our [contribution guide](https://github.com/tbeswick96/BannerlordYetAnotherPartyOrganiser/wiki/Contributing).
