using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class MachineDataBlock : BraceCode
  {
    private static bool prefabCreated = false;

    public bool toRemove = false;

    private Rigidbody rg;
    private bool isSimulationClone = false;
    private Transform otherBlock;

    private void Update()
    {
      if (rg == null) rg = GetComponent<Rigidbody>();
      rg.isKinematic = true;

      if (isSimulationClone && AddPiece.isSimulating)
      {
        // The camera sometimes focuses on this block after the machine was
        // binned. Might be because it becomes Blocks[0].
        // This doesn't prevent that, but it makes it unnoticable by moving
        // the block to another block in the machine.
        if (otherBlock == null)
        {
          foreach (Transform block in Machine.Active().SimulationMachine)
          {
            if (block != transform)
            {
              otherBlock = block;
              break;
            }
          }
        }
        transform.position = otherBlock.position;
      }
    }

    private void Awake()
    {
      if (!prefabCreated)
      {
        prefabCreated = true;
        return;
      }

      transform.Translate(0, -1000, 0);

      if (AddPiece.isSimulating)
      {
        isSimulationClone = true;
        // See comment in Update; this call is necessary because otherwise the
        // camera moves downwards before the block can be moved.
        Update();
        // Don't delete simulation clones to avoid NREs in
        // EnemyAISimple.CheckTargetBlock
        return;
      }

      Transform other;
      if ((other = Machine.Active().BuildingMachine
        .FindChild("Mod Loader Saving Object")) != null)
      {
        var otherBlock = other.GetComponent<MachineDataBlock>();
        otherBlock.toRemove = true;
        otherBlock.StartCoroutine(otherBlock.RemoveSelf());
      }
      else
      {
        Transform springOld;
        while (springOld = Machine.Active().BuildingMachine.FindChild("SpringOld"))
        {
          Machine.Active().RemoveBlock(springOld);
        }
      }
    }

    public System.Collections.IEnumerator RemoveSelf()
    {
      yield return null;
      Machine.Active().RemoveBlock(this);
    }

    public override void LateUpdate()
    {
    }

    public override void Start()
    {
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
