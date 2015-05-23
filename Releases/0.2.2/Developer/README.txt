Project Structure
=================
The mod loader currently consists of three projects:
- ModLoader: The core mod loader, outputs SpaarModLoader.dll
- InternalModLoader: Currently not used in any releases, contains the code for the internal mod loader to be injected into Assembly-UnityScript.dll
- ModLoader Injector: Currently not used in any releases, the plan is to make this able to inject the internal mod loader into any Assembly-UnityScript.dll, so that the mod loader can be used in conjunction with one stand-alone mod.

The file Patchset.txt contains all the modifications to Assembly-UnityScript.dll that have been made.

Building
========
A patched version of Assembly-UnityScript.dll, which is required for building is provided, as Besiege's modding conditions allow distribution of this file.
However, UnityEngine.dll is also required for building. As I'm not sure about its license terms, it is not included. Before building, you will need to copy it out of Besiege_Data/Managed and place it in the root folder of your project once.
Visual Studio will also try to install the mod loader after building, which makes developing much faster as it eliminates the step of copying the final dll around. For this to work, the environment variable %BESIEGE_LOCATION% needs to be set to the root of the Besiege installation you want to use (The directory with Besiege.exe in it, also include the backslash at the end). If you do not do this, the build will throw an error.

How to install
==============

Windows & Linux
---------------
Locate your Besiege installation. If you installed Besiege via Steam, this will usually be in C:\Program Files (x86)\Steam\steamapps\common\Besiege. I will from now on refer to this directory as simply Besiege in any paths.
Copy the file Assembly-UnityScript.dll into Besiege/Besiege_Data/Managed. When asked, choose to replace the original file. You may also want to make a backup of the original file before doing this.
Then create the folder Besiege/Besiege_Data/Mods. Copy SpaarModLoader.dll into it. You will also place any mods you install into this folder.

Mac OS X
--------
Locate your Besiege installation (Besiege.app, right-click on Besiege and choose Show Package Content). I will from now on refer to this directory as simply Besiege in any paths.
Copy the file Assembly-UnityScript.dll into Besiege/Contents/Data/Managed. When asked, choose to replace the original file. You may also want to make a backup of the original file before doing this.
Then create the folder Besiege/Contents/Mods. Copy SpaarModLoader.dll into it. You will also place any mods you install into this folder.

That's it, the mod loader should now be installed. If you have any problems or questions, just ask on the forum and I will do my best to help.