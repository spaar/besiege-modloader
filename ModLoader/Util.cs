using UnityEngine;

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
    /// <returns>The generated window id</returns>
    public static int GetWindowID()
    {
      return currentWindowID--;
    }

    internal static Rect PreventOffScreenWindow(Rect windowRect)
    {
      if (windowRect.x < (-windowRect.width + 50))
        windowRect.x = -windowRect.width + 50;

      if (windowRect.x > (Screen.width -50))
        windowRect.x = Screen.width - 50;

      if (windowRect.y < 0)
        windowRect.y = 0;

      if (windowRect.y > (Screen.height - 50))
        windowRect.y = Screen.height - 50;

      return windowRect;
    }

  }
}
