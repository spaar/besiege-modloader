namespace spaar.ModLoader
{
  /// <summary>
  /// A collection of useful utility functions.
  /// </summary>
  public static class Util
  {
    private static int currentWindowID = int.MaxValue;

    /// <summary>
    /// Returns a window id that is guaranteed to not conflict with another id
    /// received from this method.
    /// Use this instead of declaring constant ids!
    /// </summary>
    /// <remarks>
    /// Do not use this method as an argument to GUI.Window directly!
    /// Call this method once and store and re-use the id!
    /// </remarks>
    public static int GetWindowID()
    {
      return currentWindowID--;
    }

  }
}
