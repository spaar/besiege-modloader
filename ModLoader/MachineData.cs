using System;
using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  public class MachineData : SingleInstance<MachineData>
  {
    public override string Name { get { return "spaar's Mod Loader: Machine Data"; } }

    public delegate string SaveCallback(string title);
    public delegate void LoadCallback(string title, string value);

    private struct Metadata
    {
      public SaveCallback saveCb;
      public LoadCallback loadCb;
    }

    private static Dictionary<string, Metadata> metadata
      = new Dictionary<string, Metadata>();

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

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.J))
        Debug.Log(FindObjectOfType<MySaveMachine>());
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
