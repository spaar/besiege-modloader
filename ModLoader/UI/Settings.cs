using UnityEngine;

namespace spaar.ModLoader.UI
{
  // This class was taken from Vapid's ModLoader with permissions.
  // All credit goes to VapidLinus.
  public class Settings
  {
    public RectOffset DefaultMargin { get; set; }
    public RectOffset DefaultPadding { get; set; }
    public RectOffset LowMargin { get; set; }
    public RectOffset LowPadding { get; set; }

    public float InspectorPanelWidth { get; set; }
    public float HierarchyPanelWidth { get; set; }

    public Vector2 ConsoleSize { get; set; }

    public float LogEntrySize { get; set; }
    public float TreeEntryIndention { get; set; }

    internal Settings()
    {
      DefaultMargin = new RectOffset(8, 8, 8, 8);
      DefaultPadding = new RectOffset(8, 8, 6, 6);

      LowMargin = new RectOffset(4, 4, 4, 4);
      LowPadding = new RectOffset(4, 4, 2, 2);

      HierarchyPanelWidth = 350;
      InspectorPanelWidth = 450;

      ConsoleSize = new Vector2(550, 600);

      LogEntrySize = 16;
      TreeEntryIndention = 20;
    }
  }
}