using System;
using UnityEngine;

namespace spaar.ModLoader.UI
{
  // This class was taken from Vapid's ModLoader with permissions.
  // All credit goes to VapidLinus.
  public class Tools
  {
    public bool DoCollapseArrow(bool isExpanded, bool enabled,
      params GUILayoutOption[] options)
    {
      return DoCollapseArrow(isExpanded, enabled,
        Elements.Settings.LogEntrySize, Elements.Settings.LogEntrySize, options);
    }

    public bool DoCollapseArrow(bool isExpanded, params GUILayoutOption[] options)
    {
      return DoCollapseArrow(isExpanded, true, options);
    }

    public bool DoCollapseArrow(bool isExpanded, bool enabled,
      float width, float height, params GUILayoutOption[] options)
    {
      int length = options.Length;

      Array.Resize(ref options, options.Length + 2);
      options[length] = GUILayout.Width(width);
      options[length + 1] = GUILayout.Height(height);

      var style = enabled
        ? (isExpanded ? Elements.Buttons.ArrowExpanded
                      : Elements.Buttons.ArrowCollapsed)
        : (isExpanded ? Elements.Buttons.ArrowDarkExpanded
                      : Elements.Buttons.ArrowDarkCollapsed);

      return GUILayout.Button("", style, options) && enabled;
    }

    /// <summary>
    /// Uses GUILayout.Space() to indent the next elements.
    /// <para>Uses the amount specified with Elements.Settings.TreeEntryIndention.</para>
    /// <para>Recommended to be used inside a horizontal GUI layout.</para>
    /// </summary>
    /// <param name="depth">The indention depth. 0 is no indentetion, 2 is double.</param>
    public void Indent(int depth = 1)
    {
      GUILayout.Space(Elements.Settings.TreeEntryIndention * depth);
    }
  }
}
