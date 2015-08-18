using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader
{

  /// <summary>
  /// Handler delegate for Game.OnSimulationToggle
  /// </summary>
  /// <param name="simulating">Whether the game is simulating</param>
  public delegate void OnSimulationToggle(bool simulating);

  /// <summary>
  /// Handler delegate for Game.OnLevelWon
  /// </summary>
  public delegate void OnLevelWon();

  /// <summary>
  /// Handler delegate for Game.OnKeymapperOpen
  /// </summary>
  public delegate void OnKeymapperOpen();

  /// <summary>
  /// Provides convenient access to various parts of the game that are often
  /// needed by mods.
  /// </summary>
  public class Game : SingleInstance<Game>
  {

    public override string Name { get { return "spaar's Mod Loader: Game State"; } }

    private static AddPiece _addPiece;
    /// <summary>
    /// Reference to the AddPiece instance of the current scene.
    /// Null if there is no AddPiece in the current scene.
    /// </summary>
    public static AddPiece AddPiece
    {
      get
      {
        if (_addPiece == null)
          _addPiece = FindObjectOfType<AddPiece>();
        return _addPiece;
      }
    }

    private static MachineObjectTracker _mot;
    /// <summary>
    /// Reference to the MachineObjectTracker instance of the current scene.
    /// Null if there is no MachineObjectTracker in the current scene.
    /// </summary>
    public static MachineObjectTracker MachineObjectTracker
    {
      get
      {
        if (_mot == null)
          _mot = FindObjectOfType<MachineObjectTracker>();
        return _mot;
      }
    }

    private static BlockInfoController _boi;
    /// <summary>
    /// Reference to the BlockInfoController instance of the current scene.
    /// Null if there is no BlockInfoController in the current scene.
    /// </summary>
    public static BlockInfoController BlockInfoController
    {
      get
      {
        if (_boi == null)
          _boi = FindObjectOfType<BlockInfoController>();
        return _boi;
      }
    }

    /// <summary>
    /// Whether the game is currently simulating.
    /// </summary>
    public static bool IsSimulating
    {
      get
      {
        return AddPiece.isSimulating;
      }
    }

    /// <summary>
    /// This event is fired whenever the simulation started or stopped.
    /// </summary>
    public static event OnSimulationToggle OnSimulationToggle;

    /// <summary>
    /// This event is fired whenever the user completes a level.
    /// </summary>
    public static event OnLevelWon OnLevelWon;

    /// <summary>
    /// This event is fired whenever the keymapper is opened.
    /// </summary>
    public static event OnKeymapperOpen OnKeymapperOpen;

    private static Zone[] zones = {
      new Zone(1, "Southern Cottage", Island.Ipsilon),
      new Zone(2, "Southern Mill", Island.Ipsilon),
      new Zone(3, "Old Howl Battlefield", Island.Ipsilon),
      new Zone(4, "Perimeter Wall", Island.Ipsilon),
      new Zone(5, "The Queen's Fodder", Island.Ipsilon),
      new Zone(6, "Old Mining Site", Island.Ipsilon),
      new Zone(7, "Standing Stone", Island.Ipsilon),
      new Zone(8, "Thinside Fort", Island.Ipsilon),
      new Zone(9, "Midlands Encampment", Island.Ipsilon),
      new Zone(10, "Lyre Peak", Island.Ipsilon),
      new Zone(11, "Highland Tower", Island.Ipsilon),
      new Zone(12, "Pine Lumber Site", Island.Ipsilon),
      new Zone(13, "Solomon's Flock", Island.Ipsilon),
      new Zone(14, "Marksman's Pass", Island.Ipsilon),
      new Zone(15, "Wynnfrith's Keep", Island.Ipsilon),

      new Zone(16, "The Duke's Plea", Island.Tolbrynd),
      new Zone(17, "Southern Shrine", Island.Tolbrynd),
      new Zone(18, "Scouts of Tolbrynd", Island.Tolbrynd),
      new Zone(19, "The Duke's Prototypes", Island.Tolbrynd),
      new Zone(20, "The Duke's Dear Freighers", Island.Tolbrynd),
      new Zone(21, "Grand Crystal", Island.Tolbrynd),
      new Zone(22, "Farmer Gascoigne", Island.Tolbrynd),
      new Zone(23, "Village of Diom", Island.Tolbrynd),
      new Zone(24, "Midland Patrol", Island.Tolbrynd),
      new Zone(25, "Valley of the Wind", Island.Tolbrynd),
    };

    /// <summary>
    /// Gets a Zone object representing the specified zone.
    /// </summary>
    /// <param name="index">Index of the zone</param>
    /// <returns>Zone object for the specified zone.</returns>
    public static Zone GetZone(int index)
    {
      return zones[index];
    }

    /// <summary>
    /// Gets a Zone object representing the current zone.
    /// Returns null if the current level is not a zone.
    /// </summary>
    /// <returns>Current Zone or null</returns>
    public static Zone GetCurrentZone()
    {
      int index;
      if (int.TryParse(Application.loadedLevelName, out index))
      {
        return GetZone(index - 1);
      }
      else
      {
        return null;
      }
    }

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);
    }


    private bool hasNotifiedWinCondition = false;
    private bool hasNotifiedKeymapperOpen = false;
    private void Update()
    {
      if (IsSimulating)
      {
        if (WinCondition.hasWon && !hasNotifiedWinCondition)
        {
          var handler = OnLevelWon;
          if (handler != null) handler();
          hasNotifiedWinCondition = true;
        }
        else if (!WinCondition.hasWon && hasNotifiedWinCondition)
        {
          hasNotifiedWinCondition = false;
        }
      }

      if (OnKeymapperOpen != null)
      {
        if (BlockInfoController != null
          && BlockInfoController.menuHolder.gameObject.activeSelf
          && !hasNotifiedKeymapperOpen)
        {
          var handler = OnKeymapperOpen;
          if (handler != null) handler();
          hasNotifiedKeymapperOpen = true;
        }
        else if (BlockInfoController != null
          && !BlockInfoController.menuHolder.gameObject.activeSelf
          && hasNotifiedKeymapperOpen)
        {
          hasNotifiedKeymapperOpen = false;
        }
      }
    }

    private void OnLevelWasLoaded(int level)
    {
      var addPiece = AddPiece;
      if (addPiece == null) return;

      addPiece.sendSimulateMessage.Add(transform);
    }

    private void OnSimulate()
    {
      var handler = OnSimulationToggle;
      if (handler != null) handler(true);
    }

    private void OnStopSimulate()
    {
      var handler = OnSimulationToggle;
      if (handler != null) handler(false);
    }

  }
}
