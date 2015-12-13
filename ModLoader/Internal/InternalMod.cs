using System;
using System.Reflection;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  /// <summary>
  /// Wrapper around Mod that contains additonal information that is necessary
  /// for some features of the mod loader.
  /// </summary>
  public class InternalMod
  {
    public Mod Mod { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsEnabled { get; internal set; }

    public string AssemblyName { get; private set; }

    public InternalMod(Mod m, Assembly a)
    {
      Mod = m;
      AssemblyName = a.FullName;
      IsActive = false;
    }

    public void Activate()
    {
      if (IsActive) return;

      try
      {
        Mod.OnLoad();
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
      IsActive = true;
    }

    public void Deactivate()
    {
      if (!IsActive) return;

      try
      {
        Mod.OnUnload();
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
      IsActive = false;
    }

    public void Enable()
    {
      if (IsEnabled) return;

      Activate();
      IsEnabled = true;
      Debug.Log("Activated and enabled " + Mod.DisplayName);
    }

    public void Disable()
    {
      if (!IsEnabled) return;

      IsEnabled = false;
      Debug.Log("Disabled " + Mod.DisplayName);

      if (Mod.CanBeUnloaded)
      {
        Deactivate();
        Debug.Log("Deactivated " + Mod.DisplayName);
      }
      else
      {
        Debug.Log("Not deactivating it, it won't be loaded next time.");
      }
    }

    public void SetOverrideName(string overrideName)
    {
      Mod = new OverrideMod(Mod, overrideName);
    }

    private class OverrideMod : Mod
    {
      private string overrideName;
      private Mod mod;

      public OverrideMod(Mod originalMod, string overrideName)
      {
        this.overrideName = overrideName;
        mod = originalMod;
      }

      public override string Name { get { return overrideName; } }
      public override string DisplayName { get { return mod.DisplayName; } }
      public override string Author { get { return mod.Author; } }
      public override Version Version { get { return mod.Version; } }
      public override string BesiegeVersion { get { return mod.BesiegeVersion; } }
      public override string VersionExtra { get { return mod.VersionExtra; } }
      public override bool CanBeUnloaded { get { return mod.CanBeUnloaded; } }
      public override bool Preload { get { return mod.Preload; } }
      public override void OnLoad()
      {
        mod.OnLoad();
      }
      public override void OnUnload()
      {
        mod.OnUnload();
      }
    }
  }
}
