using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class MachineDataBlock : GenericBlock
  {
    private static bool prefabCreated = false;

    private bool toRemove = false;

    private int id = Util.GetWindowID();

    private void Awake()
    {
      if (!prefabCreated)
      {
        prefabCreated = true;
        return;
      }

      if (Machine.Active().BuildingMachine
        .FindChild("Mod Loader Saving Object") != null)
      {
        toRemove = true;
      }
    }

    public override void OnLoad(BlockXDataHolder stream)
    {
      if (toRemove)
      {
        Machine.Active().RemoveBlock(this);
        return;
      }

      if (stream.HasKey("modLoader-machineData"))
      {
        MachineData.LoadData(stream.ReadString("modLoader-machineData"));
      }
      else
      {
        MachineData.LoadData("NOTFOUND");
      }
    }

    public override void OnSave(BlockXDataHolder stream)
    {
      if (toRemove)
      {
        Machine.Active().RemoveBlock(this);
        return;
      }

      stream.Write("modLoader-machineData", MachineData.GetSaveData());
    }

  }
}
