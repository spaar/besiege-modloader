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
    public override string VersionExtra { get { return ""; } }
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
    public static readonly Version ModLoaderVersion = new Version(1, 0, 0);

    public override string Name { get { return "spaar's Mod Loader"; } }

    private List<InternalMod> loadedMods;

    public List<InternalMod> LoadedMods
    {
      get { return new List<InternalMod>(loadedMods); }
    }

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
      OptionsMenu.Initialize();
      MachineData.Initialize();
      Tools.ModToggle.Initialize();
      Tools.Keymapper.Initialize();

#if DEV_BUILD
      Tools.ObjectExplorer.Initialize();
#endif

      // Enable the console interface since it can now ask the configuration
      // for the correct keys to use.
      console.EnableInterface();

      LoadMods();

      RegisterModManagementCommands();

      InitializeMods();

      UpdateChecker.Initialize();
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

        var mod = loadedMods.Find(m => m.Mod.Name == args[0]);
        if (mod != null)
        {
          mod.Enable();
          return "";
        }
        else
        {
          return "Can't find mod " + mod.Mod.Name + ", not enabling it.";
        }
      });

      Commands.RegisterCommand("disableMod", (args, nArgs) =>
      {
        const string usage = "Usage: disableMod <mod>";
        if (args.Length != 1)
        {
          return usage;
        }

        var mod = loadedMods.Find(m => m.Mod.Name == args[0]);
        if (mod != null)
        {
          mod.Disable();
          return "Disabled mod " + args[0];
        }
        else
        {
          return "There is currently no mod named " + args[0]
            + " loaded. Did you spell the name correctly? Remember to use "
            + "internal names.";
        }
      });
    }

    private void InitializeMods()
    {
      string output = "";

      // Initialize mods marked as Preload first
      var preloadMods = loadedMods.Where(m => m.Mod.Preload);
      foreach (var mod in preloadMods)
      {
        mod.IsEnabled = Configuration.GetBool("modStatus:" + mod.Mod.Name, true);

        if (!mod.IsEnabled)
        {
          output += "Not activating " + mod.Mod.DisplayName + "\n";
          continue;
        }

        if (mod.Mod.BesiegeVersion != BesiegeVersion)
        {
          Debug.LogWarning(mod.Mod.DisplayName
            + " is not targeted at the current Besiege version."
            + " Unexpected behaviour may occur.");
        }

        mod.Activate();

        output += "Activated " + mod.Mod.ToString() + "\n";
      }

      foreach (var mod in loadedMods)
      {
        if (mod.Mod.Preload)
        {
          // Preload mods were already initialized
          continue;
        }

        mod.IsEnabled = Configuration.GetBool("modStatus:" + mod.Mod.Name, true);

        if (!mod.IsEnabled)
        {
          output += "Not activating " + mod.Mod.DisplayName + "\n";
          continue;
        }

        if (mod.Mod.BesiegeVersion != BesiegeVersion)
        {
          Debug.LogWarning(mod.Mod.DisplayName
            + " is not targeted at the current Besiege version."
            + " Unexpected behaviour may occur.");
        }

        mod.Activate();

        output += "Activated " + mod.Mod.ToString() + "\n";
      }

      ModConsole.AddMessage(LogType.Log, "Activated mods", output);
    }

    private void OnApplicationQuit()
    {
      foreach (var mod in loadedMods)
      {
        mod.Deactivate();
        Configuration.SetBool("modStatus:" + mod.Mod.Name, mod.IsEnabled);
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
