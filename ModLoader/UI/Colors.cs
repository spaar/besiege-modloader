using UnityEngine;

namespace spaar.ModLoader.UI
{
  // This class was taken from Vapid's ModLoader with permission.
  // All credit for that code goes to VapidLinus.
  // It was adapted slightly for this project.
  /// <summary>
  /// Various colors used by the mod loader GUI.
  /// </summary>
  public class Colors
  {
#pragma warning disable CS1591 // Self-explanatory names
    public Color DefaultText { get; set; }
    public Color LowlightText { get; set; }

    public Color TypeText { get; set; }

    public Color LogNormal { get; set; }
    public Color LogWarning { get; set; }
    public Color LogError { get; set; }
    public Color LogException { get; set; }
    public Color LogAssert { get; set; }
#pragma warning restore CS1591

    internal Colors()
    {
      DefaultText = Color.white;
      LowlightText = ColorUtil.FromRGB255(200, 200, 200);

      TypeText = ColorUtil.FromRGB255(78, 201, 176);

      LogNormal = DefaultText;
      LogWarning = ColorUtil.FromRGB255(240, 76, 23);
      LogError = ColorUtil.FromRGB255(238, 78, 16);
      LogException = LogWarning;
      LogAssert = ColorUtil.FromRGB255(191, 88, 203);
    }
  }

  /// <summary>
  /// Utility class for color-related things.
  /// </summary>
  public static class ColorUtil
  {
    /// <summary>
    /// Create a Color object from an RGB value where each component is expressed
    /// as a float in the range 0-255.
    /// </summary>
    /// <param name="r">Red component</param>
    /// <param name="g">Green component</param>
    /// <param name="b">Blue component</param>
    /// <returns>Constructed color object</returns>
    public static Color FromRGB255(float r, float g, float b)
    {
      return new Color(r / 255f, g / 255f, b / 255f);
    }
  }
}