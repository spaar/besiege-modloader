using UnityEngine;

namespace spaar.ModLoader.UI
{
  // This class was taken from Vapid's ModLoader with permissions.
  // All credit goes to VapidLinus.
  public class InputFields
  {
    public GUIStyle Default { get; set; }
    public GUIStyle Alternate { get; set; }
    public GUIStyle AlternateThin { get; set; }
    public GUIStyle ThinNoTopBotMargin { get; set; }
    public GUIStyle ComponentField { get; set; }

    internal InputFields()
    {
      Default = new GUIStyle
      {
        normal = { background = Elements.LoadImage("background-dark.png"), textColor = Elements.Colors.DefaultText },
        font = GUI.skin.font,
        alignment = TextAnchor.UpperLeft,
        clipping = TextClipping.Clip,
        border = new RectOffset(4, 4, 4, 4),
        padding = Elements.Settings.DefaultPadding,
        margin = Elements.Settings.DefaultMargin,
        fontSize = 14
      };

      Alternate = new GUIStyle(Default)
      {
        normal = { background = Elements.LoadImage("blue-normal.png"), textColor = Elements.Colors.DefaultText }
      };

      var margin = Elements.Settings.LowMargin;
      margin.left = 0;
      ComponentField = new GUIStyle(Alternate)
      {
        margin = margin,
        padding = Elements.Settings.LowPadding,
        fontSize = 13
      };

      ThinNoTopBotMargin = new GUIStyle(Default)
      {
        margin = Elements.Buttons.ThinNoTopBotMargin.margin,
        padding = Elements.Buttons.ThinNoTopBotMargin.padding
      };
    }
  }
}