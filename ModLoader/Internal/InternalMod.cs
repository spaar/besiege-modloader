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
      catch (Exception)
      { /* Printed to console, ignore */ }
      IsActive = true;
    }

    public void Deactivate()
    {
      if (!IsActive) return;

      try
      {
        Mod.OnUnload();
      }
      catch (Exception)
      { /* Printed to console, ignore */ }
      IsActive = false;
    }

    public void Enable()
    {
      if (IsEnabled) return;

      //ModLoader.Instance.EnableMod(Mod.Name);
      Activate();
      IsEnabled = true;
      Debug.Log("Activated and enabled " + Mod.DisplayName);
    }

    public void Disable()
    {
      if (!IsEnabled) return;

      //ModLoader.Instance.DisableMod(Mod.Name);

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
  }
}
