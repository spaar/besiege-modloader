using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  /// <summary>
  /// This work-around is required so that the mod loader can be correctly
  /// registered as a loaded 'mod'. That enables the mod loader to e.g.
  /// register commands in the Commands class or use the Configuration class.
  /// </summary>
  public class LoaderMod : Mod
  {
    public override string Name { get { return "modLoader"; } }
    public override string DisplayName { get { return "spaar's Mod Loader"; } }
    public override string Author { get { return "spaar"; } }
    public override Version Version { get { return ModLoader.ModLoaderVersion; } }
    public override void OnLoad() { }
    public override void OnUnload() { }
  }

  public class ModLoader : SingleInstance<ModLoader>
  {
    // TODO: Extract version at runtime
    /// <summary>
    /// The currently running Besiege version.
    /// </summary>
    public static readonly string BesiegeVersion = "v0.10";
    public static readonly Version ModLoaderVersion = new Version(1, 0);

    public override string Name { get { return "spaar's Mod Loader"; } }

    private List<InternalMod> loadedMods;

    public List<InternalMod> LoadedMods
    {
      get { return new List<InternalMod>(loadedMods); }
    }

    private Dictionary<string, bool> modStatus;

    private void Start()
    {
      DontDestroyOnLoad(this);

      loadedMods = new List<InternalMod>();
      loadedMods.Add(new InternalMod(new LoaderMod(),
        Assembly.GetExecutingAssembly()));

      Commands.Initialize();

      // Create the console before loading the config so it can display
      // possible errors.
      var console = Tools.Console.Instance;

      Configuration.Load();
      Keys.Initialize();
      Game.Initialize();

#if DEV_BUILD
      Tools.ObjectExplorer.Initialize();
#endif

      // Enable the console interface since it can now ask the configuration
      // for the correct keys to use.
      console.EnableInterface();

      LoadModStatus();

      LoadMods();

      RegisterModManagementCommands();

      InitializeMods();

      UpdateChecker.Initialize();
    }

    private void LoadModStatus()
    {
      modStatus = new Dictionary<string, bool>();

      var keys = Configuration.GetKeys();
      foreach (var key in keys)
      {
        if (key.StartsWith("modStatus:", StringComparison.CurrentCulture))
        {
          var mod = key.Replace("modStatus:", "");
          var modEnabled = Configuration.GetBool(key, true);
          modStatus[mod] = modEnabled;
        }
      }
    }

    private void LoadMods()
    {
      FileInfo[] files = (new DirectoryInfo(Application.dataPath + "/Mods"))
        .GetFiles("*.dll");
      foreach (FileInfo fileInfo in files)
      {
        if (!fileInfo.Name.EndsWith(".no.dll", StringComparison.CurrentCulture)
          && fileInfo.Name != "SpaarModLoader.dll")
        {
          Debug.Log("Trying to load " + fileInfo.Name);
          try
          {
            var assembly = Assembly.LoadFrom(fileInfo.FullName);
            var types = assembly.GetExportedTypes();
            var modTypes = new List<Type>();

            foreach (var type in types)
            {
              if (typeof(Mod).IsAssignableFrom(type))
                modTypes.Add(type);
            }

            if (modTypes.Count < 1)
            {
              Debug.LogError(fileInfo.Name
                + " contains no implementation of Mod. Not loading it.");
              continue;
            }
            else if (modTypes.Count > 1)
            {
              Debug.LogError(fileInfo.Name
                + " contains more than one implementation of Mod. Not loading it.");
              continue;
            }

            var mod = (Mod)System.Activator.CreateInstance(modTypes[0]);

            if (modStatus.ContainsKey(mod.Name) && !modStatus[mod.Name])
            {
              Debug.Log("Not activating mod " + mod.DisplayName + ": Is disabled");
              continue;
            }

            loadedMods.Add(new InternalMod(mod, assembly));
            Debug.Log(mod.DisplayName + " was loaded!");
          }
          catch (Exception exception)
          {
            Debug.Log("Could not load " + fileInfo.Name + ":");
            Debug.LogException(exception);
          }
        }
      }
    }

    private void RegisterModManagementCommands()
    {
      Commands.RegisterCommand("listMods", (args, nArgs) =>
      {
        var result = "Loaded mods:";
        foreach (var mod in loadedMods)
        {
          string name = args.Length > 0 ? mod.Mod.Name : mod.Mod.DisplayName;
          result += "\n" + name + " (" + mod.Mod.Version + ") by " + mod.Mod.Author;
        }
        return result;
      }, "Print a list of all mods. If 'internalNames' is passed,"
       + " internal names will be shown instead of display names.");

      Commands.RegisterCommand("enableMod", (args, nArgs) =>
      {
        const string usage = "Usage: enableMod <mod>";
        if (args.Length != 1)
        {
          return usage;
        }

        EnableMod(args[0]);

        return "Enabled mod " + args[0];
      });

      Commands.RegisterCommand("disableMod", (args, nArgs) =>
      {
        const string usage = "Usage: disableMod <mod>";
        if (args.Length != 1)
        {
          return usage;
        }

        DisableMod(args[0]);

        if (loadedMods.FindIndex(m => m.Mod.Name == args[0]) == -1)
        {
          Debug.LogWarning("There is currently no mod named " + args[0]
            + " loaded. Did you spell the name correctly? Remember to use "
            + "internal names.");
        }

        return "Disabled mod " + args[0];
      });
    }

    public void EnableMod(string modName)
    {
      modStatus[modName] = true;
    }

    public void DisableMod(string modName)
    {
      modStatus[modName] = false;

      var mod = loadedMods.Find(m => m.Mod.Name == modName);
      if (mod != null)
      {
        if (mod.Mod.CanBeUnloaded)
        {
          Debug.Log("Unloading " + mod.Mod.DisplayName);
          mod.Mod.OnUnload();
        }
        else
        {
          Debug.Log("Not unloading " + mod.Mod.DisplayName
            + ", it cannot be unloaded at runtime.");
        }
      }
      else
      {
        Debug.Log("Not unloading " + modName + ", can't find it.");
      }
    }

    private void InitializeMods()
    {
      foreach (var mod in loadedMods)
      {
        if (mod.Mod.BesiegeVersion != BesiegeVersion)
        {
          Debug.LogWarning(mod.Mod.DisplayName
            + " is not targeted at the current Besiege version."
            + " Unexpected behaviour may occur.");
        }

        mod.Activate();
      }
    }

    private void OnApplicationQuit()
    {
      foreach (var mod in loadedMods)
      {
        mod.Deactivate();
      }

      foreach (var pair in modStatus)
      {
        Configuration.SetBool("modStatus:" + pair.Key, pair.Value);
      }

      Configuration.Save();
    }

    /// <summary>
    /// Parents the components's game object to the mod loader and sets it to
    /// not be destroyed on load.
    /// </summary>
    /// <param name="comp"></param>
    public static void MakeModule(Component comp)
    {
      DontDestroyOnLoad(comp);
      comp.transform.parent = Instance.transform;
    }

  }
}
