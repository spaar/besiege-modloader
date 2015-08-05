using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader
{
  public class BlockPrefabs : SingleInstance<BlockPrefabs>
  {
    public override string Name { get { return "spaar's Mod Loader: Block Prefabs"; } }

    #region Block prefabs
    public static PrefabHolder StartingBlock { get; private set; }
    public static PrefabHolder SmallWoodBlock { get; private set; }
    public static PrefabHolder Wheel { get; private set; }
    public static PrefabHolder Blade { get; private set; }
    public static PrefabHolder ExplosiveDecoupler { get; private set; }
    public static PrefabHolder HingeCube { get; private set; }
    public static PrefabHolder MetalBall { get; private set; }
    public static PrefabHolder Brace { get; private set; }
    public static PrefabHolder SpringOld { get; private set; }
    public static PrefabHolder SpringCode { get; private set; }
    public static PrefabHolder WoodPanel2joints { get; private set; }
    public static PrefabHolder Canon { get; private set; }
    public static PrefabHolder Chain { get; private set; }
    public static PrefabHolder SteeringPiece { get; private set; }
    public static PrefabHolder FlyingSpiral { get; private set; }
    public static PrefabHolder SingleWoodBlock { get; private set; }
    public static PrefabHolder Suspension { get; private set; }
    public static PrefabHolder SawDisc { get; private set; }
    public static PrefabHolder Piston { get; private set; }
    public static PrefabHolder HingeRound { get; private set; }
    public static PrefabHolder Spike { get; private set; }
    public static PrefabHolder Flamethrower { get; private set; }
    public static PrefabHolder SpinningBlock { get; private set; }
    public static PrefabHolder BombBlock { get; private set; }
    public static PrefabHolder MetalArmourPlateSmall { get; private set; }
    public static PrefabHolder DaVinciWing { get; private set; }
    public static PrefabHolder Propellor { get; private set; }
    public static PrefabHolder Grabber { get; private set; }
    public static PrefabHolder SteeringHinge { get; private set; }
    public static PrefabHolder MetalArmourPlateRound { get; private set; }
    public static PrefabHolder BombHolder { get; private set; }
    public static PrefabHolder FlameBallBlock { get; private set; }
    public static PrefabHolder MetalArmourPlate2 { get; private set; }
    public static PrefabHolder Plow { get; private set; }
    public static PrefabHolder WingPanel { get; private set; }
    public static PrefabHolder Ballast { get; private set; }
    public static PrefabHolder Boulder { get; private set; }
    public static PrefabHolder HalfPipe { get; private set; }
    public static PrefabHolder CogMediumFree { get; private set; }
    public static PrefabHolder CogMediumPowered { get; private set; }
    public static PrefabHolder WheelFree { get; private set; }
    public static PrefabHolder WoodenPole2 { get; private set; }
    public static PrefabHolder Slider { get; private set; }
    public static PrefabHolder BalloonSmall { get; private set; }
    public static PrefabHolder BallAndSocket { get; private set; }
    public static PrefabHolder RopeSimple { get; private set; }
    public static PrefabHolder LargeWheel { get; private set; }
    public static PrefabHolder SmallFlameTorch { get; private set; }
    public static PrefabHolder Drill { get; private set; }
    public static PrefabHolder GripPad { get; private set; }
    public static PrefabHolder SmallWheel { get; private set; }
    public static PrefabHolder CogLargeFree { get; private set; }
    public static PrefabHolder SmallPropellor { get; private set; }
    public static PrefabHolder ShrapnelCannon { get; private set; }
    #endregion

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);
    }

    private void OnLevelWasLoaded(int level)
    {
      // Attempt to load prefabs everytime a level is loaded
      LoadPrefabs();
    }

    private void LoadPrefabs()
    {
      // Don't load prefabs if there's no machine tracker in the scene
      var tracker = Game.MachineObjectTracker;
      if (tracker == null) return;

      var prefabs = new HashSet<GameObject>(tracker.AllPrefabs);

      #region Assign prefabs
      StartingBlock = CreatePrefabHolder(prefabs, "StartingBlock");
      SmallWoodBlock = CreatePrefabHolder(prefabs, "SmallWoodBlock");
      Wheel = CreatePrefabHolder(prefabs, "Wheel");
      Blade = CreatePrefabHolder(prefabs, "Blade");
      ExplosiveDecoupler = CreatePrefabHolder(prefabs, "ExplosiveDecoupler");
      HingeCube = CreatePrefabHolder(prefabs, "HingeCube");
      MetalBall = CreatePrefabHolder(prefabs, "MetalBall");
      Brace = CreatePrefabHolder(prefabs, "Brace");
      SpringOld = CreatePrefabHolder(prefabs, "SpringOld");
      SpringCode = CreatePrefabHolder(prefabs, "SpringCode");
      WoodPanel2joints = CreatePrefabHolder(prefabs, "WoodPanel2joints");
      Canon = CreatePrefabHolder(prefabs, "Canon");
      Chain = CreatePrefabHolder(prefabs, "Chain");
      SteeringPiece = CreatePrefabHolder(prefabs, "SteeringPiece");
      FlyingSpiral = CreatePrefabHolder(prefabs, "FlyingSpiral");
      SingleWoodBlock = CreatePrefabHolder(prefabs, "SingleWoodBlock");
      Suspension = CreatePrefabHolder(prefabs, "Suspension");
      SawDisc = CreatePrefabHolder(prefabs, "SawDisc");
      Piston = CreatePrefabHolder(prefabs, "Piston");
      HingeRound = CreatePrefabHolder(prefabs, "HingeRound");
      Spike = CreatePrefabHolder(prefabs, "Spike");
      Flamethrower = CreatePrefabHolder(prefabs, "Flamethrower");
      SpinningBlock = CreatePrefabHolder(prefabs, "SpinningBlock");
      BombBlock = CreatePrefabHolder(prefabs, "BombBlock");
      MetalArmourPlateSmall = CreatePrefabHolder(prefabs, "MetalArmourPlateSmall");
      DaVinciWing = CreatePrefabHolder(prefabs, "DaVinciWing");
      Propellor = CreatePrefabHolder(prefabs, "Propellor");
      Grabber = CreatePrefabHolder(prefabs, "Grabber");
      SteeringHinge = CreatePrefabHolder(prefabs, "SteeringHinge");
      MetalArmourPlateRound = CreatePrefabHolder(prefabs, "MetalArmourPlateRound");
      BombHolder = CreatePrefabHolder(prefabs, "BombHolder");
      FlameBallBlock = CreatePrefabHolder(prefabs, "FlameBallBlock");
      MetalArmourPlate2 = CreatePrefabHolder(prefabs, "MetalArmourPlate2");
      Plow = CreatePrefabHolder(prefabs, "Plow");
      WingPanel = CreatePrefabHolder(prefabs, "WingPanel");
      Ballast = CreatePrefabHolder(prefabs, "Ballast");
      Boulder = CreatePrefabHolder(prefabs, "Boulder");
      HalfPipe = CreatePrefabHolder(prefabs, "HalfPipe");
      CogMediumFree = CreatePrefabHolder(prefabs, "CogMediumFree");
      CogMediumPowered = CreatePrefabHolder(prefabs, "CogMediumPowered");
      WheelFree = CreatePrefabHolder(prefabs, "WheelFree");
      WoodenPole2 = CreatePrefabHolder(prefabs, "WoodenPole2");
      Slider = CreatePrefabHolder(prefabs, "Slider");
      BalloonSmall = CreatePrefabHolder(prefabs, "BalloonSmall");
      BallAndSocket = CreatePrefabHolder(prefabs, "BallAndSocket");
      RopeSimple = CreatePrefabHolder(prefabs, "RopeSimple");
      LargeWheel = CreatePrefabHolder(prefabs, "LargeWheel");
      SmallFlameTorch = CreatePrefabHolder(prefabs, "SmallFlameTorch");
      Drill = CreatePrefabHolder(prefabs, "Drill");
      GripPad = CreatePrefabHolder(prefabs, "GripPad");
      SmallWheel = CreatePrefabHolder(prefabs, "SmallWheel");
      CogLargeFree = CreatePrefabHolder(prefabs, "CogLargeFree");
      SmallPropellor = CreatePrefabHolder(prefabs, "SmallPropellor");
      ShrapnelCannon = CreatePrefabHolder(prefabs, "ShrapnelCannon");
      #endregion
    }

    private static PrefabHolder CreatePrefabHolder(ICollection<GameObject> search, string name)
    {
      // Find prefab in list.
      GameObject found = null;
      foreach (var gameObject in search)
      {
        if (gameObject.name == name)
        {
          found = gameObject;
          break;
        }
      }


      // If not found in list, return null.
      if (found == null) return null;

      search.Remove(found); // Remove found from search list to make then next search quicker.
      return new PrefabHolder(found); // Wrap in PrefabHolder
    }
  }
}
