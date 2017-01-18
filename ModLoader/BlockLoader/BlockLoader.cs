using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using spaar.ModLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public enum ColliderType { Box, Sphere, Capsule, Cylinder, Mesh, None }
public enum Opacity { Opaque, Translucent }
public enum Direction { X = 0, Y = 1, Z = 2 }
public enum ResourceType { Texture, Audio, Mesh, AssetBundle }

namespace TheGuysYouDespise
{
  public class BlockLoaderInfo
  {
    private static GameObject BlockLoaderGO, prefabChild, ghostChild, associatedPrefabChild;

    public static Transform GetIt()
    {
      return BlockLoaderGO.transform;
    }
    public static Transform GetBlockPrefabsParent()
    {
      return prefabChild.transform;
    }
    public static Transform GetBlockGhostParent()
    {
      return ghostChild.transform;
    }
    public static Transform PrefabsUsedForBlocksParent()
    {
      return associatedPrefabChild.transform;
    }

    public void LoadBlockLoader()
    {
      BlockLoaderGO = new GameObject("TGYD's BlockLoader");
      prefabChild = new GameObject("Block 'Prefabs'");
      ghostChild = new GameObject("Block Ghosts");
      associatedPrefabChild = new GameObject("'Prefabs' used for blocks");
      prefabChild.transform.parent = BlockLoaderGO.transform;
      ghostChild.transform.parent = BlockLoaderGO.transform;
      associatedPrefabChild.transform.parent = BlockLoaderGO.transform;
      prefabChild.SetActive(false);
      BlockLoaderGO.AddComponent<BlockLoader>();
      GameObject.DontDestroyOnLoad(BlockLoaderGO);
    }
  }

  public class BlockLoader : MonoBehaviour
  {
    public static Dictionary<string, Block> ModBlocks = new Dictionary<string, Block>();
    private BlockTabController blockTabController;
    private MachineObjectTracker machineObjectTracker;
    private Material colliderDisplayMaterial = new Material(Shader.Find("Transparent/Diffuse"));
    private Material addingPointDisplayMaterial = new Material(Shader.Find("Transparent/Diffuse"));
    private List<string> blockLoadingTimes = new List<string>();
    private Dictionary<Block, List<Coroutine>> loadingResources = new Dictionary<Block, List<Coroutine>>();
    private string datapath;

    public BlockLoader()
    {
      datapath = Application.dataPath;
      colliderDisplayMaterial.color = new Color(1f, 0.5f, 1f, 0.45f);
      addingPointDisplayMaterial.color = new Color(0.5f, 1f, 1f, 0.45f);
      ModConsole.AddMessage(LogType.Log, "[BlockLoader]: Ready. Blocks are loaded upon entering a zone.");
    }

    public static void AddModBlock(Block block)
    {
      if (ModBlocks.ContainsKey(block.name))
      {
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: Tried to load '" + block.name + "', but it is already loaded.", "BlockLoader automatically stopped loading process.");
        return;
      }
      if (block == null)
      {
        ModConsole.AddMessage(LogType.Warning, "[BlockLoader]: Block entry is null for unknown block for during registration.");
        return;
      }
      ModBlocks.Add(block.name, block);
    }

    public static void AddModGameObject(string objFileName, string textureName, string goName, List<ColliderComposite> compoundCollider,
                                 bool showCollider, float mass, Vector3 visualOffset, Type[] scripts)
    {
      ModConsole.AddMessage(LogType.Warning, "Game Object loader is a work in progress");
    }

    public static int GetBlockId(string blockName)
    {
      return ModBlocks[blockName].id;
    }

    private float stamp;

    private void Awake()
    {
      SceneManager.sceneLoaded += _OnLevelWasLoaded;
    }

    private void _OnLevelWasLoaded(Scene scene, LoadSceneMode mode)
    {
      if (Game.AddPiece)
      {
        Debug.Log(Camera.main.renderingPath);
        machineObjectTracker = FindObjectOfType<MachineObjectTracker>();
        blockLoadingTimes = new List<string>();
        stamp = Time.realtimeSinceStartup;
        blockTabController = FindObjectOfType<BlockTabController>();

        List<string> keys = new List<string>(ModBlocks.Keys);
        foreach (var key in keys)
        {
          if (ModBlocks[key] == null)
          {
            ModConsole.AddMessage(LogType.Warning, "[BlockLoader]: Block entry is null for key '" + key + "' during construction.");
            ModBlocks.Remove(key);
            continue;
          }
          StartCoroutine(NewBlockPrefab(ModBlocks[key]));
        }
        StartCoroutine(createModBlocksTab());
        UpdateIgnoreForLayers();
      }
    }

    //For placing a block to the scene
    private GameObject ModifiedAddBlock(BlockType block, Vector3 pos, Quaternion rot)
    {
      Transform transforms = ((GameObject)Instantiate(PrefabMaster.BlockPrefabs[(int)block].gameObject, pos, rot)).transform;
      transforms.GetComponent<Rigidbody>().solverIterationCount = Game.AddPiece.woodBlockSolverIterationCount;

      return transforms.gameObject;
    }

    //For creating a block
    private IEnumerator NewBlockPrefab(Block block)
    {
      ModBlocks[block.name] = block;
      block.loading = true;
      float timeKeeper = Time.realtimeSinceStartup;

      if (block.id < 100)
      {
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: Tried to load " + block.name + " into an ID reserved for normal blocks.", "[BlockLoader]: Please only use IDs from 100 and above.");
        blockLoadingTimes.Add("[BlockLoader]: Didn't load '" + block.name + " due to attempted overwrite of standard block.");
        yield break;
      }
      else if (PrefabMaster.BlockPrefabs.ContainsKey(block.id))
      {
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: Tried to load " + block.name + " with the same block ID as " + PrefabMaster.BlockPrefabs[block.id].name + ".");
        blockLoadingTimes.Add("[BlockLoader]: Didn't load '" + block.name + " due to doubble ID entry at " + block.id + ".");
        yield break;
      }

      //Add needed resources
      loadingResources.Add(block, new List<Coroutine>());
      if (block.neededResources != null)
        foreach (NeededResource resource in block.neededResources)
        {
          switch (resource.resourceType)
          {
            case ResourceType.Audio:
              loadingResources[block].Add(StartCoroutine(CreateAudioRef(resource.resourcePath, resource)));
              break;
            case ResourceType.Texture:
              loadingResources[block].Add(StartCoroutine(CreateTextureRef(resource.resourcePath, resource)));
              break;
            case ResourceType.Mesh:
              AssetImporter.StartImport.Mesh(ref resource.mesh, resource.resourcePath);
              BlockScript.AddMeshResource(resource);
              break;
            case ResourceType.AssetBundle:
              loadingResources[block].Add(StartCoroutine(CreateAssetBundleRef(resource.resourcePath, resource)));
              break;
            default:
              break;
          }
        }

      // Use the Modified AddBlock Function
      block.gameObject = ModifiedAddBlock(BlockType.Spike, new Vector3(0f, -1000f, 0f), Quaternion.identity);
      //Set the name of the GameObject and block
      block.gameObject.name = block.name;
      if (BlockLoaderInfo.GetBlockPrefabsParent().FindChild(block.name))
        Destroy(BlockLoaderInfo.GetBlockPrefabsParent().FindChild(block.name).gameObject);

      block.gameObject.transform.parent = BlockLoaderInfo.GetBlockPrefabsParent();

      GameObject wood = null;
      ModHealthBar healthBar = null;
      Material woodMat = null;
      Texture destroyedTex = null;

      if (block.properties != null)
        SetupProperties(block, ref wood, ref healthBar, ref woodMat, ref destroyedTex);
      else
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: " + block.name + " has no properties defined.", "Remember to use something like '.Properties(new BlockProperties().SearchKeywords(new string[] { \"search term\", }).Burnable(3f).CanBeDamaged(3))'");

      if (block.objs != null)
        loadingResources[block].Add(StartCoroutine(SetupVisuals(block, healthBar, woodMat, destroyedTex)));
      else
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: " + block.name + " has no visuals defined through Objs().");

      if (block.compoundCollider != null)
        SetupCompoundCollider(block);
      else
        ModConsole.AddMessage(LogType.Error, "[BlockLoader]: " + block.name + " has no compound collider defined.");

      SetupGhost(block);

      SetupAddingPoints(block, wood.transform.FindChild("TriggerForJoint2").gameObject);

      Destroy(wood);

      //Set mass
      block.gameObject.GetComponent<Rigidbody>().mass = block.mass;
      block.gameObject.GetComponent<MachineTrackerMyId>().myId = block.id;

      yield return StartCoroutine(AddComponentsAndMore(block));
      BlockPrefab p = new BlockPrefab(block.id);
      p.name = block.name;
      p.ID = block.id;
      p.gameObject = block.gameObject;
      p.ghost = block.ghost;
      p.VisualController = block.gameObject.GetComponent<BlockVisualController>();
      p.VisualController.SkinCanBeChanged = block.objs.Count <= 1;
      p.blockVisControllers = new List<BlockVisualController>();
      p.SetGameObject(block.gameObject).SetNameFromGameObject();
      block.prefab = p;
      PrefabMaster.BlockPrefabs.Add(block.id, block.prefab);
      PrefabMaster.BlockGhosts.Add(block.ghost);
      GhostMaterialController g = block.prefab.ghost.GetComponent<GhostMaterialController>();
      g.originalMaterials = new Material[0];
      g.Awake();
      block.prefab.ConcludePrefab();
      block.loading = false;

      blockLoadingTimes.Add("[BlockLoader]: Loaded '" + block.name + "' as ID: " + block.id + " with mass " + block.mass);
      yield break;
    }

    #region Composite functions for creating a new block prefab
    private void SetupProperties(Block block, ref GameObject wood, ref ModHealthBar healthBar, ref Material woodMat, ref Texture destroyedTex)
    {
      //Set block Info
      MyBlockInfo blockInfo = block.gameObject.GetComponent<MyBlockInfo>();
      blockInfo.blockName = block.name.ToUpper();
      blockInfo.nameKeywords = block.properties.keywords;
      block.gameObject.GetComponent<BlockDamageType>().myDamageType = block.properties.damageType;

      wood = ModifiedAddBlock(BlockType.SingleWoodenBlock, new Vector3(0f, -1005f, 0f), Quaternion.identity);
      if (block.properties.burnable)
      {
        //Add burnable
        FireTag fireTag = block.gameObject.AddComponent<FireTag>();
        Transform fireTrigger = wood.transform.FindChild("FireTrigger");
        Transform fireSystem = wood.transform.FindChild("Fire");
        fireSystem.parent = block.gameObject.transform;
        fireTrigger.parent = block.gameObject.transform;
        fireSystem.localPosition = Vector3.forward * 1.5f;
        fireTrigger.localPosition = Vector3.forward * 1.5f;
        block.fireController = fireTrigger.GetComponent<FireController>();
        fireTag.fireControllerCode = block.fireController;
        block.fireController.fireTagCode = fireTag;
        block.fireController.fireParticles = fireSystem.GetComponent<ParticleSystem>();
        block.fireController.myJoint = block.gameObject.GetComponent<ConfigurableJoint>();
        block.fireController.onFireDuration = block.properties.burningDuration;

      }
      woodMat = new Material(wood.transform.FindChild("Vis").GetComponent<MeshRenderer>().material);
      destroyedTex = wood.transform.FindChild("Vis").GetComponent<MeshRenderer>().material.GetTexture("_DamageMap");
      Texture burnedTex = wood.transform.FindChild("Vis").GetComponent<MeshRenderer>().material.GetTexture("_EmissMap");
      BlockScript.SetDamageTexture(destroyedTex);
      BlockScript.SetBurnedTexture(burnedTex);

      if (block.properties.canBeDamaged)
      {
        healthBar = block.gameObject.AddComponent<ModHealthBar>();
        healthBar.health = block.properties.health;
        healthBar.myJoint = block.gameObject.GetComponent<ConfigurableJoint>();
      }

      if (BlockScript.FlipSound)
        return;
      GameObject wheel = ModifiedAddBlock(BlockType.Wheel, new Vector3(0f, -1005f, 0f), Quaternion.identity);
      BlockScript.FlipSound = wheel.GetComponent<AudioSource>().clip;
      DestroyImmediate(wheel);

    }
    private IEnumerator SetupVisuals(Block block, ModHealthBar healthBar, Material woodMat, Texture destroyedTex)
    {
      //Get Visual Child
      GameObject vis = block.gameObject.transform.FindChild("Vis").gameObject;
      //Reset scale
      block.gameObject.transform.localScale = Vector3.one;
      vis.transform.localPosition = Vector3.zero;
      vis.transform.localScale = Vector3.one;
      vis.transform.rotation = Quaternion.identity;
      DestroyImmediate(vis.GetComponent<MeshFilter>());
      DestroyImmediate(vis.GetComponent<Renderer>());
      vis.layer = 0;
      BlockVisualController blockVisController = block.gameObject.GetComponent<BlockVisualController>();
      List<MeshRenderer> rs = new List<MeshRenderer>();
      int index = 0;
      foreach (var obj in block.objs)
      {
        obj.gameObject = new GameObject("Vis", new Type[] { typeof(MeshFilter), typeof(MeshRenderer) });
        obj.gameObject.transform.parent = vis.transform;
        obj.gameObject.transform.localPosition = block.objs[0].offset.position;
        obj.gameObject.transform.localScale = (block.objs[0].offset.size == Vector3.zero) ? Vector3.one : block.objs[0].offset.size;
        obj.gameObject.transform.localEulerAngles = block.objs[0].offset.rotation + Vector3.right * 90f;
        obj.gameObject.layer = 25;
        if (obj.importedMesh == null)
        {
          AssetImporter.LoadingObjects.Add(new AssetImporter.LoadingObject());
          obj.importedMesh = AssetImporter.LoadingObjects.Last().mesh;
          yield return StartCoroutine(AssetImporter.ImportMesh(AssetImporter.LoadingObjects.Last(), datapath + "/Mods/Blocks/Obj/" + obj.objName, false));
        }
        obj.gameObject.GetComponent<MeshFilter>().mesh = obj.importedMesh;

        NormalMaterial:
        if (obj.opacity == Opacity.Opaque)
        {
          obj.material = new Material(woodMat);
          obj.material.SetTexture("_DamageMap", destroyedTex);
        }
        else if (block.properties.canBeDamaged)
        {
          ModConsole.AddMessage(LogType.Error, "[BlockLoader]: Tried to make " + block.name + " transparent, but it can be damaged, damaged blocks can only be opaque.");
          obj.opacity = Opacity.Opaque;
          goto NormalMaterial;
        }
        else if (block.properties.burnable)
        {
          ModConsole.AddMessage(LogType.Error, "[BlockLoader]: Tried to make " + block.name + " transparent, but it is burnable, burnable blocks can only be opaque.");
          obj.opacity = Opacity.Opaque;
          goto NormalMaterial;
        }
        else
        {
          obj.material = new Material(Shader.Find("Custom/TranspDiffuseRim"));
          obj.material.SetFloat("_RimPower", woodMat.GetFloat("_RimPower"));
          obj.material.SetColor("_RimColor", woodMat.GetColor("_RimColor"));
        }
        MeshRenderer render = obj.gameObject.GetComponent<MeshRenderer>();
        render.material = obj.material;
        if (healthBar)
          healthBar.myRenderers.Add(obj.gameObject.GetComponent<Renderer>());

        rs.Add(render);

        //Set Texture for Mesh
        int myTextIndex = index;
        if (obj.textureName != null && obj.textureName != "")
          SetTexture(obj.texture, obj.gameObject, datapath + "/Mods/Blocks/Textures/" + block.objs[myTextIndex].textureName);
        else
        {
          TryAgain:
          if (myTextIndex > 0)
          {
            myTextIndex--;
            if (block.objs[myTextIndex].textureName != null && block.objs[myTextIndex].textureName != "")
            {
              obj.texture = block.objs[myTextIndex].texture;
              obj.material.mainTexture = block.objs[myTextIndex].texture;
              render.material.mainTexture = block.objs[myTextIndex].texture;
            }
            else goto TryAgain;
          }
          else
          {
            obj.texture = null;
            obj.material.mainTexture = null;
            render.material.mainTexture = null;
          }
        }
        index++;
      }
      blockVisController.renderers = rs.ToArray();
      if (blockVisController.renderers.Length > 0)
        blockVisController.meshFiltery = blockVisController.renderers[0].GetComponent<MeshFilter>();
      yield break;
    }
    private void SetupCompoundCollider(Block block)
    {
      //Get Collider Child
      Destroy(block.gameObject.transform.FindChild("Collider").gameObject);

      MyBounds myBounds = block.gameObject.GetComponent<MyBounds>();
      myBounds.childColliders.Clear();
      block.displayColliders.Clear();

      for (int i = 0; i < block.compoundCollider.Count; i++)
      {
        ColliderComposite collider = block.compoundCollider[i];
        //Add wanted collider
        switch (collider.colliderType)
        {
          default:
          case ColliderType.None: //Blocks need a collider, so None is not possible
          case ColliderType.Box:
            {
              BoxCollider colliderComponent;

              #region AutoSize Collider
              if (collider.size == Vector3.zero)
                collider.size = block.objs[0].importedMesh.bounds.size;
              #endregion

              #region add any additional colliders to the "prefab"
              //create new child with purpose of being a collider
              collider.gameObject = new GameObject("BoxCollider");
              //set parent to the current block
              collider.gameObject.transform.parent = block.gameObject.transform;
              collider.gameObject.layer = collider.layer;

              //set offset and rotation of this child
              collider.gameObject.transform.localPosition = collider.offset;
              //colliderChild.transform.localEulerAngles = tempEuler;
              collider.gameObject.transform.localEulerAngles = collider.rotation;
              //Add BoxCollider component and size it
              colliderComponent = collider.gameObject.AddComponent<BoxCollider>();
              colliderComponent.size = collider.size;
              #endregion

              #region handle collider displaying
              if (block.showCollider)
              {
                Transform DisplayCollider = block.gameObject.transform.FindChild("DisplayCollider") ?? new GameObject("DisplayCollider").transform;
                DisplayCollider.parent = block.gameObject.transform;
                DisplayCollider.localPosition = Vector3.zero;
                //Show visual box for the collider
                GameObject colliderMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                colliderMesh.name = "BoxCollider";
                DestroyImmediate(colliderMesh.GetComponent<BoxCollider>());
                colliderMesh.transform.parent = DisplayCollider;
                colliderMesh.transform.localPosition = colliderComponent.transform.localPosition;
                colliderMesh.transform.localEulerAngles = collider.rotation;
                colliderMesh.transform.localScale = collider.size;
                colliderMesh.GetComponent<MeshRenderer>().material = colliderDisplayMaterial;
                block.displayColliders.Add(colliderMesh);
              }
              #endregion

              colliderComponent.isTrigger = collider.trigger;
              myBounds.childColliders.Add(colliderComponent);
            }
            break;
          case ColliderType.Sphere:
            {
              SphereCollider colliderComponent;

              #region AutoSize Collider
              if (collider.radius == 0f)
              {
                Vector3 meshBounds = block.objs[0].importedMesh.bounds.size;
                SortedList sortedBounds = new SortedList { { meshBounds.x, "x" }, { meshBounds.y, "y" }, { meshBounds.z, "y" } };

                collider.radius = (float)sortedBounds.GetKey(2) / 2f;
              }
              #endregion

              #region add any additional colliders to the "prefab"
              //create new child with purpose of being a collider
              collider.gameObject = new GameObject("SphereCollider");
              //set parent to the current block
              collider.gameObject.transform.parent = block.gameObject.transform;
              collider.gameObject.layer = collider.layer;

              //set offset and rotation of this child
              collider.gameObject.transform.localPosition = collider.offset;
              collider.gameObject.transform.localEulerAngles = collider.rotation;
              //Add SphereCollider component and set radius
              colliderComponent = collider.gameObject.AddComponent<SphereCollider>();
              colliderComponent.radius = collider.radius;
              #endregion

              #region handle collider displaying
              if (block.showCollider)
              {
                Transform DisplayCollider = block.gameObject.transform.FindChild("DisplayCollider") ?? new GameObject("DisplayCollider").transform;
                DisplayCollider.parent = block.gameObject.transform;
                DisplayCollider.localPosition = Vector3.zero;
                //Show visual sphere for the collider
                GameObject colliderMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                DestroyImmediate(colliderMesh.GetComponent<SphereCollider>());
                colliderMesh.transform.parent = DisplayCollider;
                colliderMesh.transform.localPosition = collider.offset;
                colliderMesh.transform.localEulerAngles = collider.rotation;
                colliderMesh.transform.localScale = Vector3.one * collider.radius * 2f;
                colliderMesh.GetComponent<MeshRenderer>().material = colliderDisplayMaterial;
                block.displayColliders.Add(colliderMesh);
              }
              #endregion

              colliderComponent.isTrigger = collider.trigger;
              myBounds.childColliders.Add(colliderComponent);
            }
            break;
          case ColliderType.Capsule:
            {
              CapsuleCollider colliderComponent;

              #region AutoSize Collider
              //largest value define height and direction, while second largest define radius
              if (collider.radius == 0f)
              {
                Vector3 meshBounds = block.objs[0].importedMesh.bounds.size;
                SortedList sortedBounds = new SortedList { { meshBounds.x, 0 }, { meshBounds.y, 1 }, { meshBounds.z, 2 } };

                collider.height = (float)sortedBounds.GetKey(2);
                collider.radius = (float)sortedBounds.GetKey(1) / 2f;
                collider.direction = (int)sortedBounds[collider.height];
              }
              #endregion

              #region add any additional colliders to the "prefab"
              //create new child with purpose of being a collider
              collider.gameObject = new GameObject("CapsuleCollider");
              //set parent to the current block
              collider.gameObject.transform.parent = block.gameObject.transform;
              collider.gameObject.layer = collider.layer;

              //set offset and rotation of this child
              collider.gameObject.transform.localPosition = collider.offset;
              collider.gameObject.transform.localEulerAngles = collider.rotation;
              //Add SphereCollider component and set radius
              colliderComponent = collider.gameObject.AddComponent<CapsuleCollider>();
              colliderComponent.direction = collider.direction;
              colliderComponent.height = collider.height;
              colliderComponent.radius = collider.radius;
              #endregion

              #region handle collider displaying
              if (block.showCollider)
              {
                Transform DisplayCollider = block.gameObject.transform.FindChild("DisplayCollider") ?? new GameObject("DisplayCollider").transform;
                DisplayCollider.parent = block.gameObject.transform;
                DisplayCollider.localPosition = Vector3.zero;
                //set sixe for bounding box arround the capsule collider
                Vector3 boundinBox;
                if (collider.direction == 0)
                  boundinBox = new Vector3(collider.height, collider.radius * 2f, collider.radius * 2f);
                else if (collider.direction == 1)
                  boundinBox = new Vector3(collider.radius * 2f, collider.height, collider.radius * 2f);
                else
                  boundinBox = new Vector3(collider.radius * 2f, collider.radius * 2f, collider.height);

                //Show visual box for the collider
                GameObject colliderMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(colliderMesh.GetComponent<BoxCollider>());
                colliderMesh.transform.parent = DisplayCollider;
                colliderMesh.transform.localPosition = colliderComponent.transform.localPosition;
                colliderMesh.transform.localEulerAngles = collider.rotation;
                Vector3 meshBounds = boundinBox;
                colliderMesh.transform.localScale = meshBounds;
                colliderMesh.GetComponent<MeshRenderer>().material = colliderDisplayMaterial;
                //Debug.Log("r: " + colliderComponent.radius + ", h: " + colliderComponent.height + ", dir: " + colliderComponent.direction);
                block.displayColliders.Add(colliderMesh);
              }
              #endregion

              colliderComponent.isTrigger = collider.trigger;
              myBounds.childColliders.Add(colliderComponent);
            }
            break;
          case ColliderType.Mesh:
            {
              string path = datapath + "/Mods/Blocks/Obj/" + collider.meshName;
              if (collider.colliderMesh == null)
                AssetImporter.StartImport.Mesh(ref collider.colliderMesh, path);

              MeshCollider colliderComponent;
              //TODO: Add offset, rotation and scale?
              #region add any additional colliders to the "prefab"
              collider.gameObject = new GameObject("MeshCollider");
              //set parent to the current block
              collider.gameObject.transform.parent = block.gameObject.transform;
              collider.gameObject.layer = collider.layer;
              //set offset and rotation of this child
              collider.gameObject.transform.localScale = collider.size;
              collider.gameObject.transform.localPosition = collider.offset;
              collider.gameObject.transform.localEulerAngles = collider.rotation;
              colliderComponent = collider.gameObject.AddComponent<MeshCollider>();
              colliderComponent.sharedMesh = collider.colliderMesh;
              #endregion
              colliderComponent.convex = true;

              #region handle collider displaying
              if (block.showCollider)
              {
                GameObject colDisplayer = new GameObject("ColDisplayer");
                colDisplayer.transform.parent = block.gameObject.transform;
                colDisplayer.transform.localPosition = collider.offset;
                colDisplayer.transform.localEulerAngles = collider.rotation;
                colDisplayer.transform.localScale = collider.size;
                MeshFilter colFilter = colDisplayer.AddComponent<MeshFilter>();
                MeshRenderer colRenderer = colDisplayer.AddComponent<MeshRenderer>();
                colFilter.mesh = collider.colliderMesh;
                colRenderer.material = colliderDisplayMaterial;
                block.displayColliders.Add(colDisplayer);
              }
              #endregion

              colliderComponent.isTrigger = collider.trigger;
              myBounds.childColliders.Add(colliderComponent);
            }
            break;
        }
      }
    }
    private void SetupGhost(Block block)
    {
      GameObject ghost = (GameObject)Instantiate(PrefabMaster.BlockPrefabs[20].ghost.gameObject);
      ghost.name = block.name + " Ghost";
      ghost.transform.parent = BlockLoaderInfo.GetBlockGhostParent();
      ghost.transform.localScale = Vector3.one;
      ghost.GetComponent<MachineTrackerMyId>().myId = block.id;
      ReplaceGhostTrigger(ghost.GetComponent<GhostTrigger>(), block.ignoreIntersectionForBase);
      GhostMaterialController gmc = ghost.GetComponent<GhostMaterialController>();
      DestroyImmediate(ghost.transform.FindChild("Collider").gameObject);
      DestroyImmediate(ghost.transform.FindChild("Vis").gameObject);

      MeshRenderer ghostRen;
      GameObject ghostCol, colDisp;
      gmc.renderers = new Renderer[block.objs.Count];
      Material ghostMat = new Material(Shader.Find("Custom/TranspDiffuseRim"));
      if (block.objs != null)
        for (int i = 0; i < block.objs.Count; i++)
        {
          ghostRen = Instantiate(block.objs[i].gameObject).GetComponent<MeshRenderer>();

          ghostMat.mainTexture = ghostRen.material.mainTexture;
          ghostMat.color = new Color(ghostRen.material.color.r - 0.2f, ghostRen.material.color.g - 0.2f, ghostRen.material.color.b - 0.2f, 0.61f);
          ghostMat.SetFloat("_RimPower", ghostRen.material.GetFloat("_RimPower") + 2.2f);
          ghostRen.material = ghostMat;
          ghostRen.transform.parent = ghost.transform;
          ghostRen.transform.localPosition = block.objs[i].gameObject.transform.localPosition;
          ghostRen.transform.localRotation = block.objs[i].gameObject.transform.localRotation;
          ghostRen.transform.localScale = block.objs[i].gameObject.transform.localScale;
          gmc.renderers[i] = ghostRen.GetComponent<Renderer>();
        }
      if (block.compoundCollider != null)
        for (int i = 0; i < block.compoundCollider.Count; i++)
        {
          if (!block.compoundCollider[i].ignoreForGhost)
          {
            ghostCol = (GameObject)Instantiate(block.compoundCollider[i].gameObject);
            ghostCol.transform.parent = ghost.transform;
            ghostCol.layer = 2;
            ghostCol.transform.localPosition = block.compoundCollider[i].gameObject.transform.localPosition;
            ghostCol.transform.localRotation = block.compoundCollider[i].gameObject.transform.localRotation;
            ghostCol.transform.localScale = block.compoundCollider[i].gameObject.transform.localScale - Vector3.one * 0.05f;
            ghostCol.GetComponent<Collider>().isTrigger = true;
          }
        }
      if (block.displayColliders != null)
        for (int i = 0; i < block.displayColliders.Count; i++)
        {
          if (!block.compoundCollider[i].ignoreForGhost)
          {
            colDisp = (GameObject)Instantiate(block.displayColliders[i]);
            colDisp.transform.parent = ghost.transform;
            colDisp.layer = 2;
            colDisp.transform.localPosition = block.displayColliders[i].transform.localPosition;
            colDisp.transform.localRotation = block.displayColliders[i].transform.localRotation;
            colDisp.transform.localScale = block.displayColliders[i].transform.localScale;
          }
        }
      block.ghost = ghost.gameObject;
      block.gameObject.GetComponent<MyBlockInfo>().ghost = block.ghost;

    }
    private void SetupAddingPoints(Block block, GameObject secondaryJointTrigger)
    {
      //Add points where other blocks can connect to
      GameObject triggerForJointTemplate = secondaryJointTrigger;
      GameObject primaryStickyJoint = block.gameObject.transform.FindChild("TriggerForJoint").gameObject;
      primaryStickyJoint.name = "StickyJointTrigger";
      bool firstBase = true;
      if (block.addingPoints != null)
        foreach (AddingPoint point in block.addingPoints)
        {
          GameObject addingPoint;
          if (point is BasePoint && firstBase)
          {
            firstBase = false;
            BasePoint basePoint = ((BasePoint)point);
            if (basePoint.hasAddingPoint)
            {
              addingPoint = new GameObject("Adding Point");
              addingPoint.transform.parent = block.gameObject.transform;
              addingPoint.transform.localPosition = new Vector3(0f, 0f, 0.5f);
              addingPoint.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
              BoxCollider apcol = addingPoint.AddComponent<BoxCollider>();
              apcol.isTrigger = true;
              apcol.center = new Vector3(0f, -0.58f, 0f);
              apcol.size = new Vector3(0.6f, 0.0f, 0.6f);
              addingPoint.layer = 12;

              #region handle addingPoint displaying
              if (block.showCollider)
              {
                GameObject addingPointMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                addingPointMesh.name = "Adding Point Displayer";
                Destroy(addingPointMesh.GetComponent<BoxCollider>());
                addingPointMesh.transform.parent = addingPoint.transform;
                addingPointMesh.transform.localPosition = apcol.center;
                addingPointMesh.transform.localEulerAngles = Vector3.zero;
                addingPointMesh.transform.localScale = new Vector3(0.6f, 0.0f, 0.6f);
                addingPointMesh.GetComponent<MeshRenderer>().material = addingPointDisplayMaterial;
              }
              #endregion
            }
            if (basePoint.sticky)
            {
              #region handle stickyPoint displaying
              if (block.showCollider)
              {
                GameObject stickyPointMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stickyPointMesh.name = "Sticky Joint Displayer";
                Destroy(stickyPointMesh.GetComponent<SphereCollider>());
                stickyPointMesh.transform.parent = primaryStickyJoint.transform;
                stickyPointMesh.transform.localPosition = new Vector3(0f, 0f, 0f);
                stickyPointMesh.transform.localScale = new Vector3(1f, 1f, 1f);
                stickyPointMesh.GetComponent<MeshRenderer>().material = addingPointDisplayMaterial;
              }
              #endregion
              primaryStickyJoint.transform.localScale = new Vector3(point.radius, point.radius, point.radius);
            }
            else
            {
              Destroy(block.gameObject.GetComponent<ConfigurableJoint>());
              Destroy(primaryStickyJoint);
              continue;
            }
            if (basePoint.motionable)
            {
              MechanicalJointTag mechTag = block.gameObject.GetComponent<MechanicalJointTag>() ?? block.gameObject.AddComponent<MechanicalJointTag>();
              MechanicalJointTag mechTag2 = primaryStickyJoint.GetComponent<MechanicalJointTag>() ?? primaryStickyJoint.AddComponent<MechanicalJointTag>();
              block.gameObject.GetComponent<ConfigurableJoint>().angularXMotion = basePoint.xMotion;
              block.gameObject.GetComponent<ConfigurableJoint>().angularYMotion = basePoint.yMotion;
              block.gameObject.GetComponent<ConfigurableJoint>().angularZMotion = basePoint.zMotion;
            }
            continue;
          }
          addingPoint = new GameObject("Adding Point");
          addingPoint.transform.parent = block.gameObject.transform;
          addingPoint.transform.localPosition = point.position;
          addingPoint.transform.localEulerAngles = point.rotation;
          BoxCollider addingPointCol = addingPoint.AddComponent<BoxCollider>();
          addingPointCol.isTrigger = true;
          addingPointCol.center = new Vector3(0f, -0.51f, 0f);
          addingPointCol.size = new Vector3(0.6f, 0.0f, 0.6f);
          addingPoint.layer = 12;

          if (point.sticky)
          {
            GameObject temp = new GameObject("Temp");
            temp.transform.parent = addingPoint.transform;
            temp.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            GameObject stickyJoint = (GameObject)Instantiate(triggerForJointTemplate, temp.transform.position, Quaternion.identity);
            stickyJoint.name = "StickyJointTrigger";
            stickyJoint.GetComponent<SphereCollider>().radius = point.radius;
            stickyJoint.transform.parent = block.gameObject.transform;
            stickyJoint.GetComponent<TriggerSetJoint2>().myParent = block.gameObject.transform;
            stickyJoint.GetComponent<TriggerSetJoint2>().myParentRigidbody = block.gameObject.GetComponent<Rigidbody>();

            #region handle stickyPoint displaying
            if (block.showCollider)
            {
              GameObject stickyPointMesh = GameObject.CreatePrimitive(PrimitiveType.Sphere);
              stickyPointMesh.name = "Sticky Joint Displayer";
              Destroy(stickyPointMesh.GetComponent<SphereCollider>());
              stickyPointMesh.transform.position = temp.transform.position;
              stickyPointMesh.transform.parent = addingPoint.transform;
              stickyPointMesh.transform.localScale = new Vector3(stickyJoint.GetComponent<SphereCollider>().radius, stickyJoint.GetComponent<SphereCollider>().radius, stickyJoint.GetComponent<SphereCollider>().radius);
              stickyPointMesh.GetComponent<MeshRenderer>().material = addingPointDisplayMaterial;
            }
            #endregion
            Destroy(temp);
          }

          #region handle addingPoint displaying
          if (block.showCollider)
          {
            GameObject addingPointMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            addingPointMesh.name = "Adding Point Displayer";
            Destroy(addingPointMesh.GetComponent<BoxCollider>());
            addingPointMesh.transform.parent = addingPoint.transform;
            addingPointMesh.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            addingPointMesh.transform.localEulerAngles = Vector3.zero;
            addingPointMesh.transform.localScale = new Vector3(0.6f, 0.0f, 0.6f);
            addingPointMesh.GetComponent<MeshRenderer>().material = addingPointDisplayMaterial;
          }
          #endregion
        }
    }
    #endregion

    private void SetTexture(GameObject vis, string path)
    {
      vis.GetComponent<MeshRenderer>().material.mainTexture = AssetImporter.LoadTexture(path);
    }
    private void SetTexture(Texture a, GameObject vis, string path)
    {
      a = AssetImporter.LoadTexture(path);
      vis.GetComponent<MeshRenderer>().material.mainTexture = a;
      vis.GetComponent<MeshRenderer>().material.color = Color.white;
    }

    #region adding resources for blocks
    private IEnumerator CreateAudioRef(string path, NeededResource resource)
    {
      WWW www = new WWW("file:///" + path);
      yield return www;
      resource.audioClip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
      BlockScript.AddAudioResource(resource);
      www.Dispose();
      yield break;
    }
    private IEnumerator CreateTextureRef(string path, NeededResource resource)
    {
      WWW www = new WWW("file:///" + path);
      yield return www;
      resource.texture = www.texture;
      BlockScript.AddTextureResource(resource);
      www.Dispose();
      yield break;
    }
    private IEnumerator CreateAssetBundleRef(string path, NeededResource resource)
    {
      WWW www = new WWW("file:///" + path);
      yield return www;
      if (resource.assetBundle == null)
        resource.assetBundle = www.assetBundle;
      BlockScript.AddAssetBundleResource(resource);
      www.Dispose();
      yield break;
    }
    #endregion

    private IEnumerator AddComponentsAndMore(Block block)
    {
      foreach (var coroutine in loadingResources[block])
        yield return coroutine;
      loadingResources.Remove(block);

      bool firstBlockScript = true;
      foreach (Type component in block.components)
      {
        Component comp = block.gameObject.AddComponent(component);

        if (comp is GenericBlock)
        {
          if (firstBlockScript)
          {
            DestroyImmediate(block.gameObject.GetComponent<GenericBlock>());
            firstBlockScript = false;
          }
          if (comp is BlockScript)
          {
            foreach (var col in block.compoundCollider)
              ((BlockScript)comp).Colliders.Add(col.gameObject);
            foreach (var obj in block.objs)
              ((BlockScript)comp).Visuals.Add(obj.gameObject);
            ((BlockScript)comp).OnPrefabCreation();
            ((BlockScript)comp).SafeAwake();
          }
        }
      }
      yield break;
    }

    private IEnumerator createModBlocksTab()
    {
      foreach (var block in ModBlocks.Values)
        if (block.loading)
          yield return null;
      //Make the background
      GameObject BGtemplate = GameObject.Find("TAB BUTTONS").transform.FindChild("BG").gameObject;
      GameObject BG = (GameObject)Instantiate(BGtemplate, BGtemplate.transform.position, BGtemplate.transform.rotation);
      BG.transform.position = new Vector3(BG.transform.position.x - 4.77f + (((ModBlocks.Count - 1) / 16) - 1) * 0.79f,
                                          BG.transform.position.y + 0.75f,
                                          BG.transform.position.z);
      BG.transform.parent = GameObject.Find("TAB BUTTONS").transform;

      int maxNumberOfTabs = 7;
      int tMax = ((ModBlocks.Count - 1) / 16 > maxNumberOfTabs) ? maxNumberOfTabs : (ModBlocks.Count - 1) / 16;
      for (int t = 0; t <= tMax; t++)
      {
        //Make the tab
        GameObject tabButTemp = FindObjectOfType<BlockTabController>().buttons[0].gameObject;
        GameObject tabButton = (GameObject)Instantiate(tabButTemp, tabButTemp.transform.position, tabButTemp.transform.rotation);
        tabButton.name = "MODDED BLOCKS";
        tabButton.transform.position = new Vector3(tabButton.transform.position.x - 0.79f + (t * 0.79f),
                                                   tabButton.transform.position.y + 0.75f,
                                                   tabButton.transform.position.z);
        tabButton.GetComponent<BlockTabButton>().myIndex = 8 + t;
        tabButton.transform.parent = GameObject.Find("TAB BUTTONS").transform;

        //Set Tooltip to be visable
        foreach (Transform child in tabButton.transform.FindChild("Tooltip"))
        {
          Renderer childRenderer = child.GetComponent<Renderer>();
          Color colour = childRenderer.material.GetColor("_TintColor");
          colour.a = 1.0f;
          childRenderer.material.SetColor("_TintColor", colour);

          TextMesh textMesh = child.GetComponent<TextMesh>();
          if (textMesh != null)
          {
            colour = textMesh.color;
            colour.a = 1.0f;
            textMesh.color = colour;
          }
        }
        tabButton.transform.FindChild("Tooltip").FindChild("TooltipText").GetComponent<TextMesh>().text = "MODDED BLOCKS";

        if (t == 0)
          SetTexture(tabButton.transform.FindChild("Icon").gameObject, datapath + "/Mods/Blocks/BLR/ModBlocksIco64.gyd");
        else
          SetTexture(tabButton.transform.FindChild("Icon").gameObject, datapath + "/Mods/Blocks/BLR/ModBlocksExtendIco64_" + t + ".gyd");

        List<BlockTabButton> blockTabButtonList = new List<BlockTabButton>(FindObjectOfType<BlockTabController>().buttons);
        blockTabButtonList.Add(tabButton.GetComponent<BlockTabButton>());
        FindObjectOfType<BlockTabController>().buttons = blockTabButtonList.ToArray();

        //Make the thing the tab opens
        Transform BLOCKS = GameObject.Find("AlignBottomLeft").transform.FindChild("BLOCK BUTTONS/t_BLOCKS");
        GameObject blockButtons = (GameObject)Instantiate(BLOCKS.gameObject, BLOCKS.position, BLOCKS.rotation);
        blockButtons.transform.parent = GameObject.Find("AlignBottomLeft").transform.FindChild("BLOCK BUTTONS").transform;
        blockButtons.name = "MODDED " + t;

        blockButtons.GetComponent<BlockMenuControl>().buttons = new BlockButtonControl[0];

        for (int i = 0; i < blockButtons.transform.childCount; i++)
        {
          Destroy(blockButtons.transform.GetChild(i).gameObject);
        }

        //Register Tab
        List<Transform> blockTabList = new List<Transform>(blockTabController.tabs);
        blockTabList.Add(blockButtons.transform);
        blockTabController.tabs = blockTabList.ToArray();

        //For all entries in modBlocks
        int tabIndex = t * 16;
        int tabCount = (ModBlocks.Count - tabIndex > 16) ? tabIndex + 16 : ModBlocks.Count;
        if (t == 0 && tabCount == 0)
        {
          ModConsole.AddMessage(LogType.Warning, "[BlockLoader]: No block mods seem to have been loaded correctly.");
          GameObject templateButton;
          Transform aLeftmostButton = GameObject.Find("AlignBottomLeft").transform.FindChild("BLOCK BUTTONS/t_BLOCKS/SingleWood");
          templateButton = aLeftmostButton.gameObject;
          GameObject button = (GameObject)Instantiate(templateButton,
                                                      new Vector3(aLeftmostButton.position.x, aLeftmostButton.position.y, aLeftmostButton.position.z),
                                                      templateButton.transform.rotation);
          button.transform.parent = blockButtons.transform;
          button.name = "NO MOD BLOCKS LOADED";
          Transform nameGO = button.transform.FindChild("Tooltip/TooltipText");
          nameGO.name = "NAME";
          nameGO.GetComponent<TextMesh>().text = ("BLOCKLOADER WORKING\nBUT NO BLOCKS FOUND");

          Destroy(button.transform.FindChild("IconPivot/Icon").gameObject);

          DestroyImmediate(button.GetComponent<BlockButtonControl>());
          DestroyImmediate(button.GetComponent<ScaleOnMouseOver>());
        }
        else
        {
          for (int i = tabIndex; i < tabCount; i++)
          {
            #region Create a button
            Block current = ModBlocks.ElementAt(i).Value;
            int blockID = current.id;
            string blockName = current.name;

            if (blockName == "ID FAILURE")
              break;

            //Add a button
            GameObject templateButton;
            Transform aLeftmostButton = GameObject.Find("AlignBottomLeft").transform.FindChild("BLOCK BUTTONS/t_BLOCKS/SingleWood");
            templateButton = aLeftmostButton.gameObject;
            GameObject button = (GameObject)Instantiate(templateButton,
                                                        new Vector3(aLeftmostButton.position.x, aLeftmostButton.position.y, aLeftmostButton.position.z) + (Vector3.right * (i - (t * 16)) * 0.76f),
                                                        templateButton.transform.rotation);
            button.transform.parent = blockButtons.transform;
            button.name = blockName + " Button";
            #endregion
            #region find tooltip children
            Transform nameGO = button.transform.FindChild("Tooltip/TooltipText");
            nameGO.name = "NAME";
            #endregion

            nameGO.GetComponent<TextMesh>().text = blockName.ToUpper();

            StartCoroutine(SetupButtonIcon(button, current));

            //Set controller code and block index for the button
            BlockButtonControlExtended bbce = button.AddComponent<BlockButtonControlExtended>();
            BlockButtonControl bbc = button.GetComponent<BlockButtonControl>();

            bbce.bg = bbc.bg;
            bbce.visBoxCode = bbc.visBoxCode;
            bbce.blockMenuControllerCode = blockButtons.GetComponent<BlockMenuControl>();
            bbce.myIndex = blockID;
            DestroyImmediate(bbc);
            bbce.ReInitialize();

            //Add button to buttons
            List<BlockButtonControl> buttonsList = new List<BlockButtonControl>(blockButtons.GetComponent<BlockMenuControl>().buttons);
            buttonsList.Add(bbce);
            blockButtons.GetComponent<BlockMenuControl>().buttons = buttonsList.ToArray();
            bbce.StartDisregardInactive();
          }

          List<BlockMenuControl> menulist = new List<BlockMenuControl>(BlockMenuControl.Menus);
          menulist.Add(blockButtons.GetComponent<BlockMenuControl>());
          BlockMenuControl.Menus = menulist.ToArray();
        }
      }
      ModConsole.AddMessage(LogType.Log, "[BlockLoader]: Done loading " + ModBlocks.Count + " blocks in " + (Time.realtimeSinceStartup - stamp) + " sec", blockLoadingTimes);
    }

    private IEnumerator SetupButtonIcon(GameObject button, Block current)
    {
      Transform buttonMeshChild = button.transform.FindChild("IconPivot/Icon");
      buttonMeshChild.GetComponent<MeshFilter>().mesh = current.prefab.DefaultSkin.mesh;
      buttonMeshChild.GetComponent<MeshRenderer>().material = current.prefab.DefaultSkin.material;
      buttonMeshChild.GetComponent<MeshRenderer>().material.color = Color.white;

      buttonMeshChild.localPosition += current.icon.offset;
      buttonMeshChild.localEulerAngles = current.icon.rotation;
      //buttonMeshChild.parent.localEulerAngles = current.icon.rotation;
      if (current.icon.scale == Vector3.zero)
        buttonMeshChild.localScale *= 1.75f * current.icon.size;
      else
      {
        buttonMeshChild.localScale = current.icon.scale * 0.25f;
      }
      //buttonMeshChild.parent.localScale = Vector3.one;
      //buttonVisChild.name = blockName;
      for (int j = 1; j < current.objs.Count; j++)
      {
        Transform child = (Transform)Instantiate(buttonMeshChild);
        child.parent = buttonMeshChild.transform;
        child.localPosition = current.objs[j].offset.position;
        child.localEulerAngles = current.objs[j].offset.rotation;
        child.localScale = current.objs[j].offset.size;
        child.GetComponent<MeshFilter>().mesh = current.objs[j].importedMesh;
        child.gameObject.GetComponent<MeshRenderer>().material.mainTexture = current.objs[j].material.mainTexture;
        child.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;

      }
      yield break;
    }

    //using a slightly tweaked ghost trigger for our blocks
    private void UpdateIgnoreForLayers()
    {
      foreach (BlockPrefab prefab in PrefabMaster.BlockPrefabs.Values)
      {
        GhostTrigger trigger = prefab.ghost.GetComponent<GhostTrigger>();
        if (trigger is GhostPinTrigger) continue;
        ReplaceGhostTrigger(trigger);
      }
    }
    private void ReplaceGhostTrigger(GhostTrigger gt, bool ignoreBase = false)
    {
      if (gt)
      {
        GhostTriggerExtended gte = gt.gameObject.AddComponent<GhostTriggerExtended>();
        gte.materialCode = gt.materialCode;
        gte.layersToIgnore = gt.layersToIgnore;
        gte.HUDLayers = new int[] { 9, 13, 19, 21, 23 };
        gte.ignoreBase = ignoreBase;
        Destroy(gt);
      }
    }

    private bool CheckToAddLayerForGhost(int layer, GhostTriggerExtended gte)
    {
      if (layer == -1)
        return false;
      for (int i = 0; i < gte.layersToIgnore.Count; i++)
      {
        if (layer == gte.layersToIgnore[i])
          return false;
      }
      return true;
    }

    private void SetToolTipBinding(Transform key1, GameObject f, Material mat, string keyString)
    {
      if (key1.gameObject.GetComponent<MeshFilter>())
        DestroyImmediate(key1.gameObject.GetComponent<MeshFilter>());
      TextMesh key1TextMesh = key1.gameObject.AddComponent<TextMesh>();
      key1.gameObject.GetComponent<MeshRenderer>().material = new Material(mat);
      key1TextMesh.text = keyString.ToUpper();
      key1TextMesh.font = f.GetComponent<TextMesh>().font;
      key1TextMesh.fontStyle = f.GetComponent<TextMesh>().fontStyle;
      key1TextMesh.fontSize = 40;
      key1.localScale *= 0.25f;
      key1.localEulerAngles = Vector3.zero;
    }
  }
}