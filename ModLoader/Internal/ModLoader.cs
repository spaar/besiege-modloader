using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public override string VersionExtra { get { return "beta6"; } }
    public override string BesiegeVersion { get { return ModLoader.BesiegeVersion; } }
    public override bool Preload { get { return true; } }
    public override void OnLoad() { }
    public override void OnUnload() { }
  }

  public class ModLoader : SingleInstance<ModLoader>
  {
    /// <summary>
    /// The currently running Besiege version.
    /// </summary>
    public static readonly string BesiegeVersion = "v0.11";
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
      SettingsMenu.Initialize();
      MachineData.Initialize();

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
      var files = (new DirectoryInfo(Application.dataPath + "/Mods"))
        .GetFiles("*.dll");
      var loadingOutput = "";

      foreach (var fileInfo in files)
      {
        if (!fileInfo.Name.EndsWith(".no.dll", StringComparison.CurrentCulture)
          && fileInfo.Name != "SpaarModLoader.dll")
        {
          loadingOutput += "Trying to load " + fileInfo.Name + "\n";
          try
          {
            var assembly = Assembly.LoadFrom(fileInfo.FullName);
            var types = assembly.GetExportedTypes();
            var modTypes = new List<Type>();

            foreach (var type in types)
            {
              if (typeof(Mod).IsAssignableFrom(type)
                && Attribute.GetCustomAttribute(type, typeof(TemplateAttribute))
                  == null)
              {
                  modTypes.Add(type);
              }
            }

            if (modTypes.Count < 1)
            {
#if COMPAT
              loadingOutput += fileInfo.Name + " contains"
                + " no implementation of Mod. Trying fallback system.\n";

              // Fallback to old attribute-way of loading, this will be removed
              // in the future.

              bool fallbackWorked = false;
              foreach (var type in types)
              {
                var attrib = Attribute.GetCustomAttribute(type, typeof(spaar.Mod)) as spaar.Mod;
                if (attrib != null)
                {
                  gameObject.AddComponent(type);
                  attrib.assembly = assembly;
                  ModCompatWrapper wrapper = new ModCompatWrapper();
                  wrapper.SetCompatInfo(attrib.author, attrib.Name(),
                    attrib.version);
                  loadingOutput += "Loaded " + attrib.Name()
                    + " (" + attrib.version + ") by " + attrib.author + "\n";
                  loadingOutput += "This mod was loade using a compatibility "
                    + "wrapper for the old system.\nPlease upgrade the mod.\n";
                  loadedMods.Add(new InternalMod(wrapper, assembly));
                  fallbackWorked = true;
                }
              }
              if (!fallbackWorked)
              {
                loadingOutput += "Fallback system failed too. Skipping.\n";
              }
              continue;
#else
              loadingOutput += fileInfo.Name
                + " contains no implementation of Mod. Not loading it."\n";
              continue;
#endif
            }
            else if (modTypes.Count > 1)
            {
              loadingOutput += fileInfo.Name + " contains"
                + " more than one implementation of Mod. Not loading it.\n";
              continue;
            }

            var mod = (Mod)System.Activator.CreateInstance(modTypes[0]);

            if (modStatus.ContainsKey(mod.Name) && !modStatus[mod.Name])
            {
              loadingOutput += "Not activating mod " + mod.DisplayName
                + ": Is disabled\n";
              continue;
            }

            loadedMods.Add(new InternalMod(mod, assembly));
            loadingOutput += "\t" + mod.ToString() + " was loaded!\n";
          }
          catch (Exception exception)
          {
            Debug.Log("Could not load " + fileInfo.Name + ":");
            Debug.LogException(exception);
          }
        }
      }

      ModConsole.AddMessage(LogType.Log, "Loaded mods", loadingOutput);
    }

    private void RegisterModManagementCommands()
    {
      Commands.RegisterCommand("listMods", (args, nArgs) =>
      {
        var result = "Loaded mods:";
        foreach (var mod in loadedMods)
        {
          var name = args.Length > 0 ? mod.Mod.Name : mod.Mod.DisplayName;
          var version = mod.Mod.VersionExtra == "" ? mod.Mod.Version.ToString()
            : mod.Mod.Version.ToString() + "-" + mod.Mod.VersionExtra;
          result += "\n" + name + " (" + version + ") by " + mod.Mod.Author;
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
      // Initialize mods marked as Preload first
      var preloadMods = loadedMods.Where(m => m.Mod.Preload);
      foreach (var mod in preloadMods)
      {
        if (mod.Mod.BesiegeVersion != BesiegeVersion)
        {
          Debug.LogWarning(mod.Mod.DisplayName
            + " is not targeted at the current Besiege version."
            + " Unexpected behaviour may occur.");
        }

        try
        {
          mod.Activate();
        }
        catch (Exception)
        {
          // Ignore the exception, it was printed to the console, modder's
          // responsibility
        }
      }

      foreach (var mod in loadedMods)
      {
        if (mod.Mod.Preload)
        {
          // Preload mods were already initialized
          continue;
        }

        if (mod.Mod.BesiegeVersion != BesiegeVersion)
        {
          Debug.LogWarning(mod.Mod.DisplayName
            + " is not targeted at the current Besiege version."
            + " Unexpected behaviour may occur.");
        }

        try
        {
          mod.Activate();
        }
        catch (Exception)
        {
          // Ignore the exception, it was printed to the console, modder's
          // responsibility
        }
      }
    }

    private void OnApplicationQuit()
    {
      foreach (var mod in loadedMods)
      {
        try
        {
          mod.Deactivate();
        }
        catch (Exception)
        {
          // Ignore the exception, it was printed to the console, modder's
          // responsibility
        }
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
