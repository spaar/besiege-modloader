namespace spaar.ModLoader.Internal
{
  /// <summary>
  /// Activates the mod loader the first time Active is called.
  /// This is the entry point for the mod loader that's called by the injected
  /// code in Assembly-UnityScript.dll.
  /// </summary>
  public static class Activator
  {
    private static bool activated = false;

    /// <summary>
    /// Activate the mod loader.
    /// Activation is only preformed the first time this is called.
    /// </summary>
    public static void Activate()
    {
      if (!activated)
      {
        ModLoader.Initialize();
        activated = true;
      }
    }
  }
}
