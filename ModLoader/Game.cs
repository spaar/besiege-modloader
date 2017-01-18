using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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

  public delegate void OnBlockPlaced(Transform block);

  public delegate void OnBlockRemoved();

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

    /// <summary>
    /// Whether the game is currently simulating.
    /// </summary>
    public static bool IsSimulating
    {
      get
      {
        return StatMaster.isSimulating;
      }
    }

    private static bool[] _completedLevels;

    /// <summary>
    /// Array specifying whether each level is completed or not.
    /// </summary>
    public static bool[] CompletedLevels
    {
      get
      {
        if (_completedLevels == null)
        {
          LoadCompletedLevels();
        }

        return (bool[])_completedLevels.Clone();
      }
    }

    private static void LoadCompletedLevels()
    {
      using (var reader = new StreamReader(
        Application.dataPath + "/CompletedLevels.txt"))
      {
        var text = reader.ReadToEnd().Trim();
        var items = text.Split('|');
        _completedLevels = new bool[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
          _completedLevels[i] = items[i] == "1";
        }
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

    public static event OnBlockPlaced OnBlockPlaced;

    public static event OnBlockRemoved OnBlockRemoved;

    private static Zone barrenExpanse = new Zone(-1, 5, "Barren Expanse",
      Island.Sandbox);
    private static Zone oldSandbox = new Zone(-2, 6, "Old Sandbox",
      Island.Sandbox);
    private static Zone[] zones = {
      new Zone(0, 7, "Sandbox", Island.Sandbox),

      new Zone(1, 8, "Southern Cottage", Island.Ipsilon),
      new Zone(2, 9, "Mill", Island.Ipsilon),
      new Zone(3, 10, "Old Howl Battlefield", Island.Ipsilon),
      new Zone(4, 11, "Perimeter Wall", Island.Ipsilon),
      new Zone(5, 12, "The Queen's Fodder", Island.Ipsilon),
      new Zone(6, 13, "Old Mining Site", Island.Ipsilon),
      new Zone(7, 14, "Standing Stone", Island.Ipsilon),
      new Zone(8, 15, "Thinside Fort", Island.Ipsilon),
      new Zone(9, 16, "Midlands Encampment", Island.Ipsilon),
      new Zone(10, 17, "Lyre Peak", Island.Ipsilon),
      new Zone(11, 18, "Highland Tower", Island.Ipsilon),
      new Zone(12, 19, "Pine Lumber Site", Island.Ipsilon),
      new Zone(13, 20, "Solomon's Flock", Island.Ipsilon),
      new Zone(14, 21, "Marksman's Pass", Island.Ipsilon),
      new Zone(15, 22, "Wynnfrith's Keep", Island.Ipsilon),

      new Zone(16, 23, "The Duke's Plea", Island.Tolbrynd),
      new Zone(17, 24, "Southern Shrine", Island.Tolbrynd),
      new Zone(18, 25, "Scouts of Tolbrynd", Island.Tolbrynd),
      new Zone(19, 26, "The Duke's Prototypes", Island.Tolbrynd),
      new Zone(20, 27, "The Duke's Dear Freighers", Island.Tolbrynd),
      new Zone(21, 28, "Grand Crystal", Island.Tolbrynd),
      new Zone(22, 29, "Farmer Gascoigne", Island.Tolbrynd),
      new Zone(23, 30, "Village of Diom", Island.Tolbrynd),
      new Zone(24, 31, "Midland Patrol", Island.Tolbrynd),
      new Zone(25, 32, "Valley of the Wind", Island.Tolbrynd),
      new Zone(26, 33, "Odd Contraption", Island.Tolbrynd),
      new Zone(27, 34, "Diom Well", Island.Tolbrynd),
      new Zone(28, 35, "Surrounded", Island.Tolbrynd),
      new Zone(29, 36, "Sacred Flame", Island.Tolbrynd),
      new Zone(30, 37, "Argus' Grounds", Island.Tolbrynd),
      new Zone(31, 38, "The Duke's Knowledge", Island.Tolbrynd),
      new Zone(32, 39, "The Venerated Heart", Island.Tolbrynd),
      new Zone(33, 40, "Shattered Field", Island.Tolbrynd),
      new Zone(34, 41, "Aras' Refuge", Island.Tolbrynd),

      new Zone(35, 42, "The Frozen Path", Island.Valfross),
      new Zone(36, 43, "The Awakening Bells", Island.Valfross),
      new Zone(37, 44, "Peculiar Clearing", Island.Valfross),
      new Zone(38, 45, "The Martyr Knights", Island.Valfross),
    };

    /// <summary>
    /// Gets a Zone object representing the specified zone.
    /// The Sandbox is Zone 0, Barren Expanse is Zone -1
    /// and the old sandbox is -2,
    /// all other zones are numbered as shown in the level select screens.
    /// </summary>
    /// <param name="index">Index of the zone</param>
    /// <returns>Zone object for the specified zone.</returns>
    public static Zone GetZone(int index)
    {
      if (index == -2) return oldSandbox;
      if (index == -1) return barrenExpanse;
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
      if (int.TryParse(SceneManager.GetActiveScene().name, out index))
      {
        return GetZone(index);
      }
      else if (SceneManager.GetActiveScene().name == "SANDBOX")
      {
        return GetZone(0);
      }
      else if (SceneManager.GetActiveScene().name == "BARREN EXPANSE")
      {
        return GetZone(-1);
      }
      else if (SceneManager.GetActiveScene().name == "SANDBOX OLD")
      {
        return GetZone(-2);
      }
      else
      {
        return null;
      }
    }

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);

      SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private bool hasNotifiedWinCondition = false;
    private bool hasNotifiedKeymapperOpen = false;
    private int machineChildCount = 0;
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
        if (BlockMapper.CurrentInstance != null
          && !hasNotifiedKeymapperOpen)
        {
          var handler = OnKeymapperOpen;
          if (handler != null) handler();
          hasNotifiedKeymapperOpen = true;
        }
        else if (BlockMapper.CurrentInstance == null
          && hasNotifiedKeymapperOpen)
        {
          hasNotifiedKeymapperOpen = false;
        }
      }

      if (!IsSimulating && (OnBlockPlaced != null || OnBlockRemoved != null)
        && AddPiece != null && Machine.Active() != null)
      {
        var currentCount = Machine.Active().BuildingBlocks.Count;
        if (machineChildCount == 0)
        {
          machineChildCount = currentCount;
        }
        else
        {
          if (machineChildCount > currentCount)
          {
            if (OnBlockRemoved != null) OnBlockRemoved();
          }
          else if (machineChildCount < currentCount)
          {
            if (OnBlockPlaced != null)
            {
              for (int i = machineChildCount; i < currentCount; i++)
              {
                OnBlockPlaced(Machine.Active().BuildingBlocks[i].transform);
              }
            }
          }
          machineChildCount = currentCount;
        }
      }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      var addPiece = AddPiece;
      if (addPiece == null) return;

      addPiece.sendSimulateMessage.Add(transform);

      Debug.Log("Loaded scene: " + scene.name + " ("
        + Application.loadedLevelName + ") with index " + scene.buildIndex
        + " (" + Application.loadedLevel + ")");

      var global = GameObject.Find("GLOBAL");
      if (global != null)
      {
        var hudInputControl = global.GetComponent<HudInputControl>();
        var modLoaderControl = global
          .AddComponent<Internal.ModLoaderHudInputControl>();
        modLoaderControl.CopyFrom(hudInputControl);
        DestroyImmediate(hudInputControl);
      }
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
