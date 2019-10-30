Deprecated
==========
This mod loader is deprecated. Besiege now contains an official mod loader out of the box that has more features and is easier to use.

**If you are using a current version of Besiege, this repository is not useful for you!**

The last supported version of Besiege is v0.45a, which is before the release of multiplayer. If you want to continue using an old Besiege version, this repository and all downloads are still available, but there won't be any more bug fixes or feature improvements.

[![](https://img.shields.io/github/release/spaar/besiege-modloader.svg?maxAge=2592000&style=flat-square)]()
[![](https://img.shields.io/github/downloads/spaar/besiege-modloader/latest/total.svg?maxAge=2592000&style=flat-square)]()

Project Structure
=================
The mod loader currently consists of three projects:
- ModLoader: The core mod loader, outputs SpaarModLoader.dll
- ModLoader Injector: Used to create a patched Assembly-UnityScript.dll file that loads the mod loader
- ModLoader Installer: The Windows GUI installer
- DebugHelper: The debug helper program, allowing reloading of mods without restarting the game

Building
========
A patched version of Assembly-UnityScript.dll, which is required for building is provided, as Besiege's modding conditions allow distribution of this file.
In order for Visual Studio to properly load the references and for the post-build step of installing the mod loader and the Start action to work, you will
need to set the BESIEGE_LOCATION environment variable to the path to your Besiege installation, including a \ at the end.

Futher information
==================
Check the README_enduser.md file for information about installing the mod loader and a list of features.

License
=======
The mod loader is licensed under the MIT license.
It is using the Mono.Cecil and Harmony libraries, which are licensed under a similiar license.
The full license text for the mod loader and each library can be found in the LICENSE file.
