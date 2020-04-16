# Mount &amp; Blade II: Bannerlord - Yet Another Party Organiser (YAPO)

[Nexus Link](https://www.nexusmods.com/mountandblade2bannerlord/mods/539)

### Another party organisation mod to add to your ever expanding collection!

_**Disclaimer**: Yes I know a lot of these mods have been released in the space of a few days. All us mod devs had the same ideas and have implemented similar solutions in different ways. Over the course of the next few weeks it's likely some of these mods will either lose support from their dev(s), or be merged into one feature-complete mod that everyone can use. Other mod devs see note at bottom about this._

**Important - Save Game Compatibility**

- This mod makes use of the engine saving mechanic to store your sorting configuration.
- This allows your sorting configuration to persist between game sessions.
- As a result, if you remove this mod having saved your game with this mod loaded, it's likely your save will not load.
- Therefore please, BACKUP YOUR SAVES before installing this mod (or any mod, for that matter)
- I might look into mitigating this in the future if it turns out to be a problem (e.g. instead use a config.xml file, however I was trying to avoid that)

## Required dependencies

The following mods are required for this mod to work. Make sure they are listed above this mod within the launcher for it to function correctly.

- [UIExtenderLib](https://www.nexusmods.com/mountandblade2bannerlord/mods/323)
- [ModLib](https://www.nexusmods.com/mountandblade2bannerlord/mods/592)

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
- Button to place upgradable troops at the top of the list
- Buttons will disable themselves if functionality is not available (e.g. cannot set opposite sorting if no Then By mode is selected)
- Sorting configuration is saved to your save file
 
### Party Actions

- Upgrade single-path upgradable troops
- Recruit recruitable prisoners - sort order is respected, party limit is respected but can be overridden by holding CTRL
- Buttons will disable themselves if functionality is not available (e.g. no troops to upgrade)

### Hotkeys

- LCTRL + LSHIFT + A - Sort Ascending (Both Sides)
- LCTRL + LSHIFT + D - Sort Descending (Both Sides)
- U - Upgrade Troops
- R - Recruit Prisoners

### Planned Features

- Anything that fits within the scope of party organisation that anyone may suggest! Feel free to request features in the posts section!
- Options (waiting for Mod Options lib from Community Patches)
- Configurable group sort order (sort cavalry after archers for example)
- Configurable upgrade rules
- Handling of multi-path upgradable troops
- Configurable main party prisoner and other party troops/prisoner (after battle) recruitment rules

---

_Notes to other mod devs who have made similar mods_

As mentioned at the top, it's likely all our mods will be merged into a single (or hopefully at least only 2 or 3 at most) feature-complete mod. Personally I encourage this as long as it is/they are open source. This mod is open source, please feel free to explore my implementations. They're not perfect of course, however I discourage simple copy and pasting code into your own mod. It'd be better for us all to collaborate. I'm happy to get the ball rolling on this if there's a few of us up for it!
If you wish to, feel free to contact me regarding any of this, or if you want to rip my mod from my cold dead hands and take it over as your own.

## Contributing

If you want to contribute to this project please read the following to make it easier for all of us.

### Installation

- Fork the repository
- Clone the forked repository
- Navigate to the repository on disk
- Navigate to the `Extension` folder
- Open the `YetAnotherPartyOrganiser.props` file with a text editor
- Modify the `<GameFolder>` tag so it actually points to your installation of Bannerlord
