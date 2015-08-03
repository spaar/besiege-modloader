using System;
using System.Collections.Generic;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class HierarchyPanel : MonoBehaviour
  {
    private const string SEARCH_FIELD_NAME = "search_field";
    private const string SEARCH_FIELD_DEFAULT = "Search";

    private readonly HashSet<HierarchyEntry> inspectorEntries
      = new HashSet<HierarchyEntry>();
    private Vector2 hierarchyScroll = Vector2.zero;

    private string searchFieldText = SEARCH_FIELD_DEFAULT;
    private bool isSearching = false;

    private void Start()
    {
      RefreshGameObjectList();
    }

    private void OnLevelWasLoaded(int level)
    {
      RefreshGameObjectList();
    }

    public void Display()
    {
      GUILayout.BeginVertical();

      #region Buttons
      GUILayout.BeginHorizontal(
        GUILayout.Width(Elements.Settings.HierarchyPanelWidth));

      // Search field
      // Set the name of the  search field so we can later detect if it's in focus
      GUI.SetNextControlName(SEARCH_FIELD_NAME);
      const int SEARCH_FIELD_WIDTH = 160;
      var oldSearchText = searchFieldText;
      searchFieldText = GUILayout.TextField(oldSearchText,
        Elements.InputFields.ThinNoTopBotMargin,
        GUILayout.Width(SEARCH_FIELD_WIDTH));
      if (oldSearchText != searchFieldText)
      {
        RefreshGameObjectList();
      }

      // Expand/collapse all entries button
      bool allCollapsed = AreAllCollapsed(inspectorEntries);
      const int BUTTON_COLLAPSE_WIDTH = 90;
      if (GUILayout.Button(allCollapsed ? "Expand All" : "Collapse All",
        Elements.Buttons.ThinNoTopBotMargin, GUILayout.Width(BUTTON_COLLAPSE_WIDTH)))
      {
        if (allCollapsed)
        {
          ExpandAll(inspectorEntries);
        }
        else
        {
          CollapseAll(inspectorEntries);
        }
      }

      // Refresh list button
      if (GUILayout.Button("Refresh", Elements.Buttons.ThinNoTopBotMargin))
      {
        RefreshGameObjectList();
      }

      // Only during repaint, to avoid errors
      if (Event.current.type == EventType.Repaint)
      {
        // If the current focused control is the search textfield
        if (GUI.GetNameOfFocusedControl() == SEARCH_FIELD_NAME)
        {
          // Clear the default value
          if (searchFieldText == SEARCH_FIELD_DEFAULT)
          {
            isSearching = true;
            searchFieldText = "";
          }
        }
        else
        {
          // If searchfield is not in focus and it is empty, restore default text
          if (searchFieldText.Length < 1)
          {
            isSearching = false;
            searchFieldText = SEARCH_FIELD_DEFAULT;
          }
        }
      }

      GUILayout.EndHorizontal();
      #endregion

      hierarchyScroll = GUILayout.BeginScrollView(hierarchyScroll, false, true,
        GUILayout.Width(Elements.Settings.HierarchyPanelWidth));

      DoShowEntries(inspectorEntries);

      GUILayout.EndScrollView();

      GUILayout.EndVertical();
    }

    private bool AreAllCollapsed(IEnumerable<HierarchyEntry> entries)
    {
      foreach (var entry in entries)
      {
        if (entry.IsExpanded)
        {
          return false;
        }
        if (entry.HasChildren)
        {
          if (!AreAllCollapsed(entry.Children))
            return false;
        }
      }

      return true;
    }

    private void ExpandAll(IEnumerable<HierarchyEntry> entries)
    {
      foreach (var entry in entries)
      {
        entry.IsExpanded = true;
        if (entry.HasChildren)
        {
          ExpandAll(entry.Children);
        }
      }
    }

    private void CollapseAll(IEnumerable<HierarchyEntry> entries)
    {
      foreach (var entry in entries)
      {
        entry.IsExpanded = false;
        if (entry.HasChildren)
        {
          CollapseAll(entry.Children);
        }
      }
    }

    private void DoShowEntries(IEnumerable<HierarchyEntry> entries,
      int iterationDepth = 0)
    {
      foreach (var entry in entries)
      {
        if (entry.Transform == null)
        {
          // The object has been deleted
          entry.Children.Remove(entry);
          continue;
        }

        GUILayout.BeginHorizontal();
        Elements.Tools.Indent(iterationDepth);

        if (Elements.Tools.DoCollapseArrow(entry.IsExpanded && entry.HasChildren,
          entry.HasChildren))
        {
          entry.IsExpanded = !entry.IsExpanded;
        }

        if (GUILayout.Button(entry.Transform.name,
          Elements.Buttons.LogEntryLabel))
        {
          ObjectExplorer.Instance.InspectorPanel.SelectedGameObject
            = entry.Transform.gameObject;
        }

        GUILayout.EndHorizontal();

        if (entry.IsExpanded)
        {
          DoShowEntries(entry.Children, iterationDepth + 1);
        }
      }
    }

    public void RefreshGameObjectList()
    {
      inspectorEntries.Clear();
      foreach (var transform in FindObjectsOfType<Transform>())
      {
        if (transform.parent == null)
        {
          var entry = new HierarchyEntry(transform);
          inspectorEntries.Add(entry);
        }
      }
      if (isSearching)
      {
        foreach (var entry in new HashSet<HierarchyEntry>(inspectorEntries))
        {
          if (!EntryOrChildrenContain(entry, searchFieldText))
          {
            inspectorEntries.Remove(entry);
          }
        }
      }
    }

    private bool EntryOrChildrenContain(HierarchyEntry entry, string text)
    {
      if (entry.Transform.name.Contains(text)) return true;
      foreach (var child in entry.Children)
      {
        if (EntryOrChildrenContain(child, text)) return true;
      }
      return false;
    }
  }
}
