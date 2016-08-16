using System;
using System.Collections.Generic;
using spaar.ModLoader;
using TheGuysYouDespise;
using UnityEngine;

namespace Blocks
{
  [Template]
  public abstract class BlockMod : Mod
  {

    public override void OnLoad()
    {
    }

    public override void OnUnload()
    {
    }

    protected void LoadBlock(Block block)
    {
      if (block.objs == null)
      {
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: Tried to load '" + block.name + "', but it's outdated.", "BlockLoader automatically stopped loading process.");
        return;
      }
      BlockLoader.AddModBlock(block);
    }

    protected void LoadFancyBlock(Block block)
    {
      LoadBlock(block);
    }
  }
}
