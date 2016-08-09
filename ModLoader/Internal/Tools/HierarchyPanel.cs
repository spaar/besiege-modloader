using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

#if DEV_BUILD
namespace spaar.ModLoader.Internal.Tools
{
  public class HierarchyPanel : MonoBehaviour
  {
    private const string SEARCH_FIELD_NAME = "search_field";
    private const string SEARCH_FIELD_DEFAULT = "Search";

    private HashSet<HierarchyEntry> inspectorEntries
      = new HashSet<HierarchyEntry>();
    private HashSet<HierarchyEntry> searchFilteredEntries
      = new HashSet<HierarchyEntry>();
    private Vector2 hierarchyScroll = Vector2.zero;

    private string searchFieldText = SEARCH_FIELD_DEFAULT;
    private bool isSearching = false;

    private bool shouldUpdate = false;

    private void Start()
    {
      RefreshGameObjectList();

      shouldUpdate = Configuration.GetBool("objExpAutoUpdate", false);
      StartCoroutine(AutoUpdate());

      Configuration.OnConfigurationChange += OnConfigurationChange;
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnConfigurationChange(object s, ConfigurationEventArgs e)
    {
      var old = shouldUpdate;
      shouldUpdate = Configuration.GetBool("objExpAutoUpdate", false);
      if (!old)
      {
        StartCoroutine(AutoUpdate());
      }
    }

    private System.Collections.IEnumerator AutoUpdate()
    {
      while (shouldUpdate)
      {
        yield return new WaitForSeconds(0.5f);
        if (ObjectExplorer.Instance.IsVisible)
        {
          RefreshGameObjectList();
        }
      }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
        RefreshSearchList();
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
            RefreshSearchList();
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
          continue;
        }
        if (isSearching && !searchFilteredEntries.Contains(entry))
        {
          // The search filtered this object out
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
      var newEntries = new HashSet<HierarchyEntry>();
      foreach (var transform in FindObjectsOfType<Transform>())
      {
        if (transform.parent == null)
        {
          var entry = new HierarchyEntry(transform);
          newEntries.Add(entry);
        }
      }
      CopyIsExpanded(inspectorEntries, newEntries);
      inspectorEntries = newEntries;
      if (isSearching)
      {
        RefreshSearchList();
      }
    }

    private void CopyIsExpanded(HashSet<HierarchyEntry> src,
      HashSet<HierarchyEntry> dest)
    {
      foreach (var entry in src)
      {
        HierarchyEntry newEntry = null;
        if ((newEntry = dest.FirstOrDefault(e => e.Transform == entry.Transform))
          != null)
        {
          newEntry.IsExpanded = entry.IsExpanded;
          CopyIsExpanded(entry.Children, newEntry.Children);
        }
      }
    }

    private void RefreshSearchList()
    {
      searchFilteredEntries.Clear();
      foreach (var entry in inspectorEntries)
      {
        searchFilteredEntries.UnionWith(Flatten(entry));
      }
      searchFilteredEntries.RemoveWhere(
        e => !EntryOrChildrenContain(e, searchFieldText));
    }

    private HashSet<HierarchyEntry> Flatten(HierarchyEntry root)
    {
      var flattened = new HashSet<HierarchyEntry>() { root };
      var children = root.Children;
      if (children != null)
      {
        foreach (var child in children)
        {
          flattened.UnionWith(Flatten(child));
        }
      }
      return flattened;
    }

    private bool EntryOrChildrenContain(HierarchyEntry entry, string text)
    {
      string toSearch = text.ToLower();
      if (entry.Transform == null) return false;
      if (entry.Transform.name.ToLower().Contains(toSearch)) return true;
      foreach (var child in entry.Children)
      {
        if (EntryOrChildrenContain(child, text)) return true;
      }
      return false;
    }
  }
}
#endif