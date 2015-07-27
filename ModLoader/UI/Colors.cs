using UnityEngine;

namespace spaar.ModLoader.UI
{
  // This class was taken from Vapid's ModLoader with permissions.
  // All credit for that code goes to VapidLinus.
  // It was adapted slightly for this project.
  public class Colors
  {
    public Color DefaultText { get; set; }
    public Color LowlightText { get; set; }

    public Color TypeText { get; set; }

    public Color LogNormal { get; set; }
    public Color LogWarning { get; set; }
    public Color LogError { get; set; }
    public Color LogException { get; set; }
    public Color LogAssert { get; set; }

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

  public static class ColorUtil
  {
    public static Color FromRGB255(float r, float g, float b)
    {
      return new Color(r / 255f, g / 255f, b / 255f);
    }
  }
}