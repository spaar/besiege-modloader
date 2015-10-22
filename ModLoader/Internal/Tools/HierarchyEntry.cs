using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class HierarchyEntry
  {
    public readonly Transform Transform;
    public HashSet<HierarchyEntry> Children;

    public bool HasChildren { get { return Children.Count > 0; } }
    public bool IsExpanded { get; set; }

    public HierarchyEntry(Transform transform)
    {
      Transform = transform;
      Children = new HashSet<HierarchyEntry>();

      UpdateChildrenList();
    }

    private void UpdateChildrenList()
    {
      Children.Clear();

      foreach (Transform child in Transform)
      {
        Children.Add(new HierarchyEntry(child));
      }
    }

    public override int GetHashCode()
    {
      return Transform.GetHashCode();
    }
  }
}
