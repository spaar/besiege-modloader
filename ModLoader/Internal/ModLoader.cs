using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  /// <summary>
  /// This work-around is required so that the mod loader can be correctly
  /// registered as a loaded 'mod'. That enabled the mod loader to itself
  /// register commands in the Commands class.
  /// </summary>
  public class LoaderMod : Mod
  {
    public string Name { get { return "modLoader"; } }
    public string DisplayName { get { return "spaar's Mod Loader"; } }
    public string Author { get { return "spaar"; } }
    public Version Version { get { return new Version(1, 0); } }
    public string BesiegeVersion { get { return ModLoader.BesiegeVersion; } }
    public bool CanBeUnloaded { get { return false; } }
    public void OnLoad() { }
    public void OnUnload() { }
  }

  public class ModLoader : SingleInstance<ModLoader>
  {
    // TODO: Extract version at runtime
    /// <summary>
    /// The currently running Besiege version.
    /// </summary>
    public static readonly string BesiegeVersion = "v0.10";

    public override string Name { get { return "spaar's Mod Loader"; } }

    private List<InternalMod> loadedMods;

    public List<InternalMod> LoadedMods
    {
      get { return new List<InternalMod>(loadedMods); }
    }

    private void Start()
    {
      loadedMods = new List<InternalMod>();
      loadedMods.Add(new InternalMod(new LoaderMod(),
        Assembly.GetExecutingAssembly()));

      Commands.init();

      // Create the console before loading the config so it can display
      // possible errors.
      var console = Tools.Console.Instance;

      Configuration.Initialize();
      Keys.LoadKeys();

      //gameObject.AddComponent<KeySettings>();
#if DEV_BUILD
      gameObject.AddComponent<ObjectExplorer>();
#endif

      // Enable the console interface since it can now ask the configuration
      // for the correct keys to use.
      console.EnableInterface();

      LoadMods();
      InitializeMods();
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

      Commands.RegisterCommand("listMods", (args, namedArgs) =>
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
      Configuration.SaveConfig();
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
