using System;
using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Allows mods to add data to machine save files.
  /// </summary>
  /// <remarks>
  /// Save files are organized in the following format:
  /// <code>
  /// SECTION 1
  /// value of section 1
  /// SECTION 2
  /// value of section 2
  /// ....
  /// </code>
  /// There are multiple sections predefined by the game. This class allows
  /// you to add new ones.
  /// </remarks>
  public class MachineData : SingleInstance<MachineData>
  {
    public override string Name { get { return "spaar's Mod Loader: Machine Data"; } }

    /// <summary>
    /// Called when the machine is saved to retrieve the value to be saved.
    /// </summary>
    /// <param name="title">Title of the section to be written</param>
    /// <returns>Value to be saved</returns>
    public delegate string SaveCallback(string title);
    /// <summary>
    /// Called when a machine is loaded with the value that was loaded from
    /// the save file.
    /// </summary>
    /// <param name="title">Title of the section that was loaded</param>
    /// <param name="value">Value that was loaded</param>
    public delegate void LoadCallback(string title, string value);

    private struct Metadata
    {
      public SaveCallback saveCb;
      public LoadCallback loadCb;
    }

    private static Dictionary<string, Metadata> metadata
      = new Dictionary<string, Metadata>();


    /// <summary>
    /// Add a section to the save files with the specified callbacks for
    /// retrieving values to be saved and notifying the mod of loaded values.
    /// </summary>
    /// <param name="title">Title of the section</param>
    /// <param name="saveCb">Callback for saving</param>
    /// <param name="loadCb">Callback for loading</param>
    public static void Add(string title,
      SaveCallback saveCb, LoadCallback loadCb)
    {
      metadata.Add(title, new Metadata()
      {
        saveCb = saveCb,
        loadCb = loadCb,
      });
    }

    internal static string GetSaveData()
    {
      string result = "";

      foreach (var md in metadata)
      {
        result += md.Key + "\n";
        result += md.Value.saveCb(md.Key) + "\n";
      }

      return result;
    }

    internal static void LoadData(string saveData)
    {
      var lines = saveData.Split(Environment.NewLine.ToCharArray());
      for (int i = 0; i < lines.Length; i++)
      {
        if (metadata.ContainsKey(lines[i]))
        {
          metadata[lines[i]].loadCb(lines[i], lines[i + 1]);
          i++;
        }
      }
    }

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);
    }

    private void OnLevelWasLoaded(int level)
    {
      if (Game.AddPiece)
      {
        //Not a menu level

        var saveMachineGO = FindObjectOfType<MySaveMachine>().gameObject;
        DestroyImmediate(saveMachineGO.GetComponent<MySaveMachine>());
        var newSaveMachine = saveMachineGO.AddComponent<ModLoaderSaveMachine>();
        newSaveMachine.machineObjTrackerCode = Game.MachineObjectTracker;
        Game.MachineObjectTracker.mySaveCode = newSaveMachine;
        FindObjectOfType<SaveAndDestroyOnClick>().mySaveCode = newSaveMachine;
      }
    }

  }
}
