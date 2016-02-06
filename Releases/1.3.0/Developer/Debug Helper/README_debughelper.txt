How to use the debug helper
===========================

The debug helper in essence allows easy reloading of mods during runtime.
This is primarily useful when developing or debugging mods, which would ordinarily require restarting Besiege a lot.

The debug helper works by first disabling the currently loaded version of your mod and then loading a new one at runtime.
This means that in order for the reloading to work properly, mods must have CanBeUnloaded set to true, and actually undo
all changes when unloaded so that the new version can be loaded into a clean environment.

The recommend way to set up a project for use of the debug helper is as follows:
	1. Change the Start action of your project to start the debug helper (Project Properties -> Debug -> Start action)
	2. Add two command line arguments with the corresponding text box (seperated by spaces):
		a) The path to the compiled mod DLL file.
		   You can also edit the .csproj (or .csproj.user) file with a text editor and insert $(TargetPath) as start argument instead of hardcoding the path in the text box.
		b) The name of your mod. This *must* be the same as the Name property of your mod.
	3. If you have not already done so (for example for the Visual Studio Besiege Mod template), set the BESIEGE_LOCATION environment variable to the path to your Besiege installation.
	   This allows the debug helper to automatically find Besiege.exe in order to start it.
	   
Note: It is not necessary to set up your project for use with the debug helper. You can also simply launch DebugHelper.exe yourself and supply the information yourself via the text boxes.
	  However setting your project up so this is automatically done by Visual Studio for you makes usage much easier as you don't need to enter the values every time you want to use the debug helper.
	  It's also possible to go with a hybrid solution where you enter some values yourself and others are set automatically. Your choice.
	  
How to use the debug helper to develop/debug your mods:
	(This assumes the project is set up as described above, adapt accordingly if you have not done so)
	1. When you want to start Besiege the first time, press Ctrl+F5 (or the "Start without Debugging" button) to start the debug helper.
	2. Press the Start button in the debug helper.
	   (The first time you do this, you may get a firewall popup. This is because the mod loader opens a TCP server to communicate with the debug helper.
	    You may choose any option that allows the mod loader to open a socket on port 5000 and the debug helper to connect to this.)
	3. Test your mod.
	4. Continue programming in VS without exiting Besiege.
	5. When you want to reload your mod, build it by pressing Ctrl+Shift+B (or by pressing the appropriate button), then go back to the debug helper window and press "Reload Mod".
	6. Your mod should first be deactivated and then the new version of your mod should be loaded. You can confirm this by looking at the in-game console output.
	7. Repeat steps 3-6 as many times as necessary.
	8. When you're done, first exit Besiege. Then go back to the debug helper and press the "Reset" button.

Note that every time you press "Reload Mod" a new version of your mod is copied to your Mods folder and loaded into the game. This means that disk&memory usage will go up every time too.
Due to this, it's a good idea to occasionally exit Besiege, press the "Reset" button (which will delete all the accumulated versions of your mod) and then press the "Start" button again.
This should be the first thing you do if you encounter any issues related to the debug helper too.