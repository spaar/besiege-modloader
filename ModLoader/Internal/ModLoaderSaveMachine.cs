//using System;
//using System.IO;
//using UnityEngine;

//namespace spaar.ModLoader.Internal
//{
//  public class ModLoaderSaveMachine : MySaveMachine
//  {
//    private bool saving = false;

//    public void CopyFrom(MySaveMachine o)
//    {
//      prefabReferences = o.prefabReferences;
//      blockPositions = o.blockPositions;
//      blockRotations = o.blockRotations;
//      wheelFlipped = o.wheelFlipped;
//      braceStartPos = o.braceStartPos;
//      braceEndPos = o.braceEndPos;
//      parentPosition = o.parentPosition;
//      parentRotation = o.parentRotation;
//      myKeyMaps1 = o.myKeyMaps1;
//      myKeyMaps2 = o.myKeyMaps2;
//      extraFloat = o.extraFloat;
//      extraBoolean = o.extraBoolean;
//      machineObjTrackerCode = o.machineObjTrackerCode;
//    }

//    public override string WriteBooleanArray(bool[] arrayToUse)
//    {
//      saving = true;

//      // Abuse this method to add data to the end, simplest way to hook into saving
//      string result = base.WriteBooleanArray(arrayToUse);

//      // Gather custom data and append it, it will be written to the save file
//      // at the end by 'MySaveMachine.SaveAll'
//      result += "\n" + MachineData.GetSaveData();

//      return result;
//    }

//    public override void LoadAll(string namey)
//    {
//      base.LoadAll(namey);

//      saving = false;
//      StartCoroutine(DelayedLoading(namey, false));
//    }

//    public override void WorkshopLoadAll(string fullPath)
//    {
//      base.WorkshopLoadAll(fullPath);

//      saving = false;
//      StartCoroutine(DelayedLoading(fullPath, true));
//    }

//    private System.Collections.IEnumerator DelayedLoading(string namey,
//      bool fullPath)
//    {
//      // If the machine is saved, LoadAll will nevertheless be called before
//      // WriteBooleanArray will be. Because of this, when loading, we wait for
//      // 4 frames (after which WriteBooleanArray should have been called) and
//      // then check whether we're actually saving and not loading in order to
//      // avoid false calls to the load callbacks.
//      for (int i = 0; i < 4; i++)
//      {
//        yield return null;
//      }

//      if (saving == true) yield break;

//      try
//      {
//        var streamReader = new StreamReader(fullPath ?
//          namey : Application.dataPath + "/SavedMachines/" + namey + ".bsg");
//        var saveData = streamReader.ReadToEnd();
//        streamReader.Close();

//        MachineData.LoadData(saveData);
//      }
//      catch (Exception)
//      {
//        Debug.LogError("[MachineData] Error while calling load callbacks.");
//      }
//    }

//  }
//}
