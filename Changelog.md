1.6.2
=====
- Update for Besiege v0.42 Hotfix B compatibility

1.6.1
=====
- Update for Besiege v0.42 Hotfix A compatibility
- [BUGFIX] Fix the block loader throwing an ArgumentException under certain circumstances.

1.6.0
=====
- Update for Besiege v0.42 compatibility
- [API] Allow mods to keep references to OptionsButtons and SettingsButtons after they were created. Thanks @Lench for this.

1.5.4
=====
- Update for Besiege v0.4 compatibility
- [BUGFIX] Numpad keys can now be used in the key mapper

1.5.3
=====
- Update for Besiege v0.35 compatibility

1.5.2
=====
- Update for Besiege v0.32 Hotfix C compatibility
- [BUGFIX] Fix pin blocks always displaying interception warning
- [BUGFIX] Include Old Sandbox and Barren Expanse in zone handling API
- [API] Add MirrorVisuals and MirrorColliders method to BlockScript

1.5.1
=====
- Fix missing block loader resources

1.5.0
=====
- Update for Besiege v0.32 Hotfix B compatibility
- [Feature] Integrate the block loader by TheGuysYouDespise into the mod loader

1.4.4
=====
- Update for Besiege v0.32 Patch 1 compatibility

1.4.3
=====
- [BUGFIX] Fix update checker sometimes crashing due to a stack overflow

1.4.2
=====
- Update for Besiege v0.32 compatibility
- [BUGFIX] Fix pressing enter not working on Macs

1.4.1
=====
- Update for Besiege v0.3 Patch 1 compatibility

1.4.0
=====
- Update for Besiege v0.3 compatibility
- [API] Add ModConsole.ForceWriteToDisk() method
- [Enhancement] Add F1 through F15 as possibly keys in the keymapper


1.3.3
=====
- Update for Besiege v0.27 compatibility
- [Enhancement] The way the MachineData API works, changed fundamentally.
   This should remove any and all bugs relating to the "red muscle" block.
   It requires shipping a modified version of Assembly-CSharp-firstpass.dll
   however, which is only a temporary measure.

1.3.2
=====
- [BUGFIX] Fix various bugs related to the MachineData API (these are also the red springs bugs)
- [BUGFIX] Fix block chooser background being over settings toggles registered by mods
- [BUGFIX] Prevent tab toggling the HUD when being used as keybinding again

1.3.1
=====
- Update for Besiege v0.25 Patch 1 compatibility
- [BUGFIX] Fix the translate machine tool being completely broken

1.3.0
=====
- Update for Besiege v0.25 compatibility
- [BUGFIX] Fix wrong line-endings being written in MachineData
- [BUGFIX] Remove left-over debug output
- [BUGFIX, Developer Build] Fix SingleInstance and possibly other classes not working with the debug helper
Note that due to the new saving system in Besiege, MachineData behaves a bit differently now. The callbacks may be called more often than they used to be.
Any data saved using MachineData will likely get lost when converting a save file from the old format to the new one unfortunately.

1.2.0
=====
- Update for Besiege v0.23 compatibility
- [Enhancement] Add ability to map mouse buttons in the keymapper
- [Enhancement] Add ability to map number keys in the keymapper
- [Enhancement] Add "Enable All" and "Disable All" buttons to mod toggle
- [Enhancement] Make it impossible to move GUI windows completely off-screen
- [BUGFIX] Prevent toggling of HUD when Tab is used in a keybinding
- [API] Add OnBlockPlaced and OnBlockRemoved events to Game
- [Feature] Add 'loadMod' command
- [Feature, Developer Build] Add Debug Server, which allows connecting to the in-game console via a TCP server on port 5000
- [Feature, Developer Build] Add beta version of Debug Helper

1.1.0
=====
- [Feature, API] Add keybinding API so mods can register keys to be mapped in the Keybinding window (Ctrl+J)
- [Enhancement, Documentation] Updated Mac installation instructions
- [Enhancement, API] Improve Zone API so it can be used to 1) load levels and 2) check what levels are completed.
- [BUGFIX] Fix the update checker's enabled status being the opposite of what it should be.
           This means the update checker is now enabled by default.
- [BUGFIX] Remove leftover debug output
- [BUGFIX] Hopefully fix a bug on Mac that made pressing enter in the console not be recognized.
- [BUGFIX, API] Fix bug in SingleInstance that may cause .Instance to point to a destroyed object instead of creating a new one.

1.0.1
=====
- Update for Besiege v0.2 compatibility

1.0.0
=====
Finally the full 1.0.0 release!
This barely changes anything from 1.0.0-beta9, but the whole project was brought to release-ready state.

1.0.0-beta9
===========
- [Feature] Add window to remap mod loader keys (Ctrl+J by default)
- [Feature] Add GUI for enabling&disabling mods (Ctrl+M by default)
- [Enhancement, Developer Build] Add Destroy button to object explorer
- [API] Add OptionsMenu API for adding options to the options screen in the main menu
- [Enhancement, API] Add toggles to the GUISkin for mods
- [Enhancement, API] Add new red button style for GUI
- [BUGFIX] Fix exceptions during mod loading and unloading not being shown

1.0.0-beta8
===========
- [Enhancement, Developer Build] Add Color and Quaternion editing support to object explorer
- [Enhancement, Developer Build] Make layer editable in object explorer
- [API] Add Configuration.RemoveKey function

1.0.0-beta7
===========
- [API] Add a template attribute

1.0.0-beta6
===========
- [Enhancement] Enable scrolling the settings menu if not all items fit on screen
- [Enhancement] Improve the output from mod loading
- [Feature, Developer Build] Add tag and layer display to object explorer
- [API] Add VersionExtra property to Mod
- [API] Using Configuration.KeyExists is now an error
- [BUGFIX] Remove debug output that was printed when L was pressed

1.0.0-beta5
===========
- [Enhancement] Make mod-registered settings fit better into new Settings layout in v0.11
- [BUGFIX] Fix bug that prevented some machines from being loaded

1.0.0-beta4
===========
- [BUGFIX] Fix critical bug that broke saving of new machines
This is a beta release, it's not finished yet. Some features may be broken, others may be buggy. Use on your own risk.

1.0.0-beta3
===========
- Update for Besiege v0.11 Compatibility
- [Feature] Enable rich text in the console
- [API] Add API to save data with machines
- [API] Add API for more flexibility when outputting to the console
- [API] Add API for getting information about Zones
- [BUGFIX, Developer Build] Enable the Mods/Debug/ConsoleOutput.txt logging file again
This is a beta release, it's not finished yet. Some features may be broken, others may be buggy. Use on your own risk.

1.0.0-beta2
===========
- [Feature] Add optional auto-updating to object explorer
- [Feature] Add maxConsoleMessage setting
- [Feature, Developer Build] Add "Focus" and "Select Focused" buttons to object explorer
- [API] Add option for certain mods to be loaded before all others
- [API] Add API to add buttons to the in-game settings
- [API] Rename Configuration.KeyExists to DoesKeyExist
- [BUGFIX] Create "Config" directory if it does not exist
This is a beta release, it's not finished yet. Some features may be broken, others may be buggy. Use on your own risk.

1.0.0-beta1
===========
- Major overhaul of the mod loader
- New GUI style
- New APIs for modders
- [Developer Build] Advanced Object Explorer
- Ability to enable&disable mods
- Unlimited command history in console
This is a beta release, it's not finished yet. Some features may be broken, others may be buggy. Use on your own risk.

0.3.3
=====
- Update for compatibility with Besiege v0.11

0.3.2
=====
- Update for compatibility with the Besiege v0.1 Hotfix

0.3.1
=====
- Update for Besiege v0.10
- Add auto-completion for console commands (activated by pressing tab)
- Make console commands case-insensitive
- Added 'list' command to list all commands
- Improved 'help' command
- [Developer Build] Small bug fixes

0.3
===
- Add a console command system
- Add console message filtering via console commands
- [Developer Build] Add tag and layer display to object explorer (only shown if not 'Untagged' or layer 0 respectively)

0.2.2
=====
- Update for Besiege v0.09

0.2.1
=====
- Make it possible for mods to have optional dependencies again
- Remove object explorer key settings from non-developer builds

0.2
=====
- Fixed console autoscrolling
- Introduce seperate developer and end-user builds
- Improve performance
- Add new, improved way of loading mods
- Remappable keys
- Add mod examples for developers
- Fixed various small bugs

0.1.3
=====
- Updated for Besiege v0.08
- Fixed console for large number of messages

0.1.2
=====
- Fixed bug where mods were loaded twice
- Added a way for mods to be notified when the simulation starts

0.1.1
=====
Added Mac support thanks to Mtschroll

0.1
===
Initial Release
