using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class MachineDataBlock : GenericBlock
  {
    private static bool prefabCreated = false;

    private bool toRemove = false;

    private void Update()
    {
      transform.position = Machine.Active().Blocks[0].transform.position;
      transform.rotation = Machine.Active().Blocks[0].transform.rotation;
    }

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
        StartCoroutine(RemoveSelf());
      }
    }

    private System.Collections.IEnumerator RemoveSelf()
    {
      yield return null;
      Machine.Active().RemoveBlock(this);
    }

    public override void OnLoad(BlockXDataHolder stream)
    {
      if (toRemove)
      {
        return;
      }
      if (AddPiece.isSimulating) return;

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
        return;
      }
      if (AddPiece.isSimulating) return;

      stream.Write("modLoader-machineData", MachineData.GetSaveData());
    }

  }
}
