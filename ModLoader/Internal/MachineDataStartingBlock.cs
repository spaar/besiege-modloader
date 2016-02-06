using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class MachineDataStartingBlock : MonoBehaviour
  {
    public void Awake()
    {
      var prefab = new GameObject("Mod Loader Saving Object");
      prefab.AddComponent<MachineDataBlock>();
      // TODO: Maybe figure out something better than overwriting the old spring
      prefab.AddComponent<MachineTrackerMyId>().myId = (int)Prefab.SpringOld;
      //prefab.transform.parent = ModLoader.Instance.transform;
      MachineObjectTracker.Instance.AllPrefabs[(int)Prefab.SpringOld] = prefab;
      AddPiece.Instance.blockTypes[(int)BlockType.SpringOld] = prefab.transform;
      MachineConstructor.Instance.ReadBlockBehaviours();
      Machine.Active().AddBlock(Vector3.zero, Quaternion.identity,
        (int)Prefab.SpringOld);

      prefab.SetActive(false);
    }
  }
}
