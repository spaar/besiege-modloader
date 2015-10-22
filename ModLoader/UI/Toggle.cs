using UnityEngine;

namespace spaar.ModLoader.UI
{
  public class Toggle
  {
    public GUIStyle Default { get; set; }

    internal Toggle()
    {
      Default = new GUIStyle()
      {
        normal = {
          background = Elements.LoadImage("toggle-normal.png"),
        },
        onNormal = {
          background = Elements.LoadImage("toggle-on-normal.png"),
        },
        hover = {
          background = Elements.LoadImage("toggle-hover.png"),
        },
        onHover = {
          background = Elements.LoadImage("toggle-on-hover.png"),
        },
        active = {
          background = Elements.LoadImage("toggle-active.png"),
        },
        onActive = {
          background = Elements.LoadImage("toggle-on-active.png"),
        },
        margin = { right = 10 }
      };
    }
  }
}
