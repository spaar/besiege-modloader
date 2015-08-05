using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Provides convenient access to various parts of the game that are often
  /// needed by mods.
  /// </summary>
  public class Game
  {

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
          _addPiece = UnityEngine.Object.FindObjectOfType<AddPiece>();
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
          _mot = UnityEngine.Object.FindObjectOfType<MachineObjectTracker>();
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

  }
}
