using System;
using System.Collections;
using System.Linq;
using System.IO;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class ModLoaderSaveMachine : MySaveMachine
  {

    public override string WriteBooleanArray(bool[] arrayToUse)
    {
      // Abuse this method to add data to the end, simplest way to hook into saving
      string result = base.WriteBooleanArray(arrayToUse);

      // Gather custom data and append it, it will be written to the save file
      // at the end by 'MySaveMachine.SaveAll'
      result += "\n" + MachineData.GetSaveData();

      return result;
    }

    public override void LoadAll(string namey)
    {
      base.LoadAll(namey);

      try
      {
        var streamReader = new StreamReader(Application.dataPath
          + "/SavedMachines/" + namey + ".bsg");
        var saveData = streamReader.ReadToEnd();
        streamReader.Close();

        MachineData.LoadData(saveData);
      }
      catch (Exception)
      {

      }
    }

    public override void WorkshopLoadAll(string fullPath)
    {
      base.WorkshopLoadAll(fullPath);

      var streamReader = new StreamReader(fullPath);
      var saveData = streamReader.ReadToEnd();
      streamReader.Close();

      MachineData.LoadData(saveData);
    }

  }
}
