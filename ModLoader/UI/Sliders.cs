using UnityEngine;

namespace spaar.ModLoader.UI
{
  public class Sliders
  {
    public GUIStyle Horizontal { get; set; }
    public GUIStyle Vertical { get; set; }

    public GUIStyle ThumbHorizontal { get; set; }
    public GUIStyle ThumbVertical { get; set; }

    internal Sliders()
    {
      Horizontal = new GUIStyle(GUI.skin.horizontalSlider)
      {
        normal = { background = Elements.LoadImage("blue-normal.png") },
      };
      Vertical = new GUIStyle(GUI.skin.verticalSlider)
      {
        normal = { background = Elements.LoadImage("blue-normal.png") },
      };
      ThumbHorizontal = new GUIStyle(GUI.skin.horizontalSliderThumb)
      {
        normal = { background = Elements.LoadImage("slider-thumb.png") },
        hover = { background = Elements.LoadImage("slider-thumb-hover.png") },
        active = { background = Elements.LoadImage("slider-thumb-active.png") }
      };
      ThumbVertical = new GUIStyle(GUI.skin.verticalSliderThumb)
      {
        normal = { background = Elements.LoadImage("slider-thumb.png") },
        hover = { background = Elements.LoadImage("slider-thumb-hover.png") },
        active = { background = Elements.LoadImage("slider-thumb-active.png") }
      };
    }
  }
}
