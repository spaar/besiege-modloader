using System;
using UnityEngine;
using System.Collections;

public class BlockButtonControlExtended : BlockButtonControl
{
  public new MyBlockInfo blockInfo;
  public new bool initialized = false;

  public override void Initialize()
  {
    if (initialized)
      return;

    BlockBehaviour block = PrefabMaster.GetBlock(myIndex);
    if (block != null)
    {
      blockInfo = block.GetComponent<MyBlockInfo>();
    }

    initialized = true;
  }

  public virtual void ReInitialize()
  {
    initialized = false;
    Initialize();
  }

  public override void OnMouseDown()
  {
    Set();
  }

  public override void Set()
  {
    AddPiece.Instance.SetBlockType(myIndex);
    AddPiece.usingCopiedBlock = false;
    blockMenuControllerCode.CheckIfActive(true);

    if (StatMaster.isSearching)
    {
      BlockTabController tabController = blockMenuControllerCode.TabController;
      int tabIndex = tabController.GetTabIndex(myIndex);
      tabController.OpenTab(tabIndex);
    }
    StatMaster.ChangeSelectedBlock(myIndex);
  }

  public override void Activate()
  {
    bg.enabled = true;
    visBoxCode.Set(myIndex);
  }

  public override void DeActivate()
  {
    bg.enabled = false;
  }

  public override bool MatchesFilter(string filter)
  {
    Initialize();

    if (blockInfo == null)
    {
      return false;
    }

    string blockName = blockInfo.blockName.ToLower();

    if (filter.Length > 1 && blockName.Contains(filter))
    {
      return true;
    }
    else if (blockName.StartsWith(filter, StringComparison.Ordinal))
    {
      return true;
    }
    else
    {
      string[] filterSplit = filter.Split(' ');
      string[] blockSplit = blockName.Split(' ');
      string[] keywords = blockInfo.nameKeywords;

      foreach (string f in filterSplit)
      {
        bool exists = false;
        foreach (string b in blockSplit)
        {
          if (b.Contains(f))
          {
            exists = true;
            break;
          }
        }
        if (!exists && keywords != null && f.Length >= 2)
        {
          foreach (string keyword in keywords)
          {
            if (keyword.ToLower().StartsWith(f, StringComparison.Ordinal))
            {
              exists = true;
              break;
            }
          }
        }
        if (!exists)
        {
          return false;
        }
      }
      return true;
    }
  }
}