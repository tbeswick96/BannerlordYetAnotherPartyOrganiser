# Mount &amp; Blade II: Bannerlord - Yet Another Party Organiser (YAPO)

## Features

### Sorting

Adds sorting functionality to party screen to enable sorting of troops

- Sorting modes:
  - Alphabetical (Name)
  - Type (Infantry, Ranged, Cavalry etc)
  - Group (Formation group)
  - Tier
  - Culture
  - Count
- Two sorting modes can be selected through two drop downs
  - Sorting is treated as a Sort By -> Then By
  - Button to set opposite sorting direction between the Sort By -> Then By Modes, to allow more granular sorting rules
- Button to place upgradable troops at the top of the list
- Buttons will disable themselves if functionality is not available (e.g. cannot set opposite sorting if no Then By mode is selected)

### Party Actions

- Upgrade upgradable single-path troops
- Recruit recruitable prisoners - sort order is respected, party limit is respected but can be overridden by holding CTRL
- Buttons will disable themselves if functionality is not available (e.g. no troops to upgrade)

### Hotkeys

- LCTRL + LSHIFT + A - Sort Ascending (Both Sides)
- LCTRL + LSHIFT + D - Sort Descending (Both Sides)
- U - Upgrade Troops
- R - Recruit Prisoners

## Planned Features

- Options (waiting for Mod Options lib from Community Patches)
  - Configurable group sort order

## Notes

- This mod makes use of the engine saving mechanic to store your sorting configuration. This allows your configuration to persist between game sessions.
- As a result, if you remove the mod, it's likely your save will not load. Therefore please, backup your saves before using this mod
- I might look into mitigating this in the future (potentially move to a config.xml file, however I was trying to avoid that)
