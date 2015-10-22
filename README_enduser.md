spaar's Mod Loader is currently the only Besiege mod loader out there and enables you to use many mods at once.

Version 1.0.0 was just released and is a very big change from the previous 0.x.x versions.
Most of the code was completely rewritten, the GUI has a new (better!) style and many new features were added.
Many thanks to everyone who made this possible, especially @VapidLinus for allowing me to use his code after he was unable to continue working on his own mod loader.

Features
===
- Loads all mods placed in Besiege_Data/Mods.
- An in-game console to show output from the game and from mods and to enter commands to interact with mods. (Open with Ctrl+K by default)
- An in-game GUI for enabling and disabling mods without having to install/uninstall them. (Open with Ctrl+M) by default)
- An in-game key remapper for all key combinations in the mod loader itself. (Open with Ctrl+J by default)
- A graphical installer for Windows

- Many APIs to make developing mods much easier and to make mods more compatible to each other.
- An in-game object explorer with many features such as showing the GameObject hierarchy and changing some values in Components. (Only in the Developer build, Open with Ctrl+O by default)

How to install
===
You can choose between two versions to download: The normal version if you just want to play Besiege with some mods and the developer version if you want to develop mods yourself.
Either way, the installation process is the same for both versions, see below for your respective platform.

Windows (Installer)
---
There is a graphical installer for Windows available. Just download ModLoader Installer.exe, save it anywhere on your computer and run it.
If the installer doesn't automatically detect it, use the Browse button to select your Besiege installation, select a version and press "Install Mod Loader".
The installation might take a while, depending on your internet connection. The program might appear to freeze during the installation but it's just working in the background.
To update the mod loader, you can just run the program again, select the new version and press Install again.

Linux & Windows (Manual Installation)
---
Locate your Besiege installation. If you installed Besiege via Steam, this will usually be in C:\Program Files (x86)\Steam\steamapps\common\Besiege.
I will from now on refer to this directory as simply Besiege in any paths.
Copy the file Assembly-UnityScript.dll into Besiege/Besiege_Data/Managed.
When asked, choose to replace the original file. You may also want to make a backup of the original file before doing this.
Then create the folder Besiege/Besiege_Data/Mods. Copy SpaarModLoader.dll and the Resources folder into it. You will also place any mods you install into this folder.

Mac OS X
---
Locate your Besiege installation (Besiege.app, right-click on Besiege and choose Show Package Content).
I will from now on refer to this directory as simply Besiege in any paths.
Copy the file Assembly-UnityScript.dll into Besiege/Contents/Data/Managed.
When asked, choose to replace the original file. You may also want to make a backup of the original file before doing this.
Then create the folder Besiege/Contents/Mods. Copy SpaarModLoader.dll and the Resources folder into it. You will also place any mods you install into this folder.

That's it, the mod loader should now be installed. If you have any problems or questions, just ask on the forum and I will do my best to help.

For Mod developers
===
There is a basic tutorial for creating mods [here](======INSERT LINK HERE========). To get started, take a look at the [Visual Studio template](http://forum.spiderlinggames.co.uk/forum/main-forum/besiege-early-access/modding/30194-new-visual-studio-template-spaar-s-mod-loader-1-x-x-mod-development).
The mod loader also has an [online documentation](http://spaar.github.io/besiege-modloader) for its various APIs.
Lastly, as a mod developer you can PM me your email address and I will add you to the [Besiege Modding Slack team](http://forum.spiderlinggames.co.uk/forum/main-forum/besiege-early-access/modding/19322-slack-team),
where you can ask me and many other modders for assistance.

License and Source code
===
The mod loader is an open-source project licensed under the MIT license.
Take a look at the [GitHub page](https://github.com/spaar/besiege-modloader) for details. Contributions in the form of pull-requests or issues are always welcome!


See the Changelog.md file for a list of changes each version.