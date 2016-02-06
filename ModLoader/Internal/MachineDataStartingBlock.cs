using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class MachineDataStartingBlock : MonoBehaviour
  {
    public void Awake()
    {
      if (MachineData.MachineDataBlockPrefab == null)
      {
        var prefab = new GameObject("Mod Loader Saving Object");
        prefab.AddComponent<MachineDataBlock>();
        // TODO: Maybe figure out something better than overwriting the old spring
        prefab.AddComponent<MachineTrackerMyId>().myId = (int)Prefab.SpringOld;
        var rb = prefab.AddComponent<Rigidbody>();
        rb.mass = 1;
        rb.isKinematic = true;
        var col = prefab.AddComponent<BoxCollider>();
        col.size = new Vector3(0.1f, 0.1f, 0.1f);
        col.isTrigger = true;
        col.gameObject.layer = 19;
        prefab.AddComponent<BlockHealthBar>().health = 0;
        //prefab.transform.parent = ModLoader.Instance.transform;
        MachineObjectTracker.Instance.AllPrefabs[(int)Prefab.SpringOld] = prefab;
        AddPiece.Instance.blockTypes[(int)BlockType.SpringOld] = prefab.transform;
        MachineConstructor.Instance.ReadBlockBehaviours();
        prefab.SetActive(false);

        MachineData.MachineDataBlockPrefab = prefab;
      }
      if (!AddPiece.isSimulating)
      {
        Machine.Active().AddBlock(Vector3.zero, Quaternion.identity,
          (int)Prefab.SpringOld);
      }
    }
  }
}
