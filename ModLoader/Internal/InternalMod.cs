using System.Reflection;

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

      Mod.OnLoad();
      IsActive = true;
    }

    public void Deactivate()
    {
      if (!IsActive) return;

      Mod.OnUnload();
      IsActive = false;
    }
  }
}
