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
  /// Provides convenient access to various parts of the game that are often
  /// needed by mods.
  /// </summary>
  public class Game : SingleInstance<Game>
  {

    public override string Name { get { return "spaar's Mod Loader: Game State"; } }

    private AddPiece _addPiece;
    /// <summary>
    /// Reference to the AddPiece instance of the current scene.
    /// Null if there is no AddPiece in the current scene.
    /// </summary>
    public AddPiece AddPiece
    {
      get
      {
        if (_addPiece == null)
          _addPiece = FindObjectOfType<AddPiece>();
        return _addPiece;
      }
    }

    private MachineObjectTracker _mot;
    /// <summary>
    /// Reference to the MachineObjectTracker instance of the current scene.
    /// Null if there is no MachineObjectTracker in the current scene.
    /// </summary>
    public MachineObjectTracker MachineObjectTracker
    {
      get
      {
        if (_mot == null)
          _mot = FindObjectOfType<MachineObjectTracker>();
        return _mot;
      }
    }

    /// <summary>
    /// Whether the game is currently simulating.
    /// </summary>
    public bool IsSimulating
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

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);
    }

    private bool hasNotifiedWinCondition = false;
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
