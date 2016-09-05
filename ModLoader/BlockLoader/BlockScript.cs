using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using spaar.ModLoader;
using TheGuysYouDespise;

public class BlockScript : GenericBlock
{
  //_EmissMap("EmissMap", 2D) = "black" { }
  //_EmissCol("Emiss Color", Color) = (0,0,0,1)
  //_BloodMap("Blood Map", 2D) = "white" { }
  //_BloodAmount("Blood Amount", Float) = 0
  //_DamageMap("Damage Map", 2D) = "white" { }
  //_DamageAmount("Damage Amount", Float) = 0
  protected class BlockResource
  {
    public ResourceType resourceType;
    public AudioClip audioClip;
    public Texture texture;
    public Mesh mesh;
    public AssetBundle assetBundle;
    public string resourceName;

    public BlockResource(AudioClip audioClip, string name)
    {
      this.resourceType = ResourceType.Audio;
      this.audioClip = audioClip;
      this.resourceName = name;
    }

    public BlockResource(Texture texture, string name)
    {
      this.resourceType = ResourceType.Texture;
      this.texture = texture;
      this.resourceName = name;
    }

    public BlockResource(Mesh mesh, string name)
    {
      this.resourceType = ResourceType.Audio;
      this.mesh = mesh;
      this.resourceName = name;
    }

    public BlockResource(AssetBundle bundle, string name)
    {
      this.resourceType = ResourceType.Audio;
      this.assetBundle = bundle;
      this.resourceName = name;
    }
  }
  protected static Dictionary<string, BlockResource> resources = new Dictionary<string, BlockResource>();
  public static AudioClip FlipSound;
  public List<GameObject> Colliders = new List<GameObject>();
  public List<GameObject> Visuals = new List<GameObject>();
  public List<Vector3> visStartScales = new List<Vector3>();
  public List<Vector3> visStartPositions = new List<Vector3>();
  public List<Quaternion> visStartRotations = new List<Quaternion>();
  public List<Vector3> colliderStartPositions = new List<Vector3>();
  public List<Quaternion> colliderStartRotations = new List<Quaternion>();
  private FireController fireController;
  private static Texture damagedTexture, burnedTexture;
  private bool lastSimulateState = false;
  private bool lastFixedSimulateState = false;
  private bool lastBurningState = false;
  private bool burned = false;

  public static void AddAudioResource(NeededResource res)
  {
    if (!resources.ContainsKey(res.resourceName))
      resources.Add(res.resourceName, new BlockResource(res.audioClip, res.resourceName));
  }
  public static void AddTextureResource(NeededResource res)
  {
    if (!resources.ContainsKey(res.resourceName))
      resources.Add(res.resourceName, new BlockResource(res.texture, res.resourceName));
  }
  public static void AddAssetBundleResource(NeededResource res)
  {
    if (!resources.ContainsKey(res.resourceName))
      resources.Add(res.resourceName, new BlockResource(res.assetBundle, res.resourceName));
  }
  public static void AddMeshResource(NeededResource res)
  {
    if (!resources.ContainsKey(res.resourceName))
      resources.Add(res.resourceName, new BlockResource(res.mesh, res.resourceName));
  }

  public static Texture GetDamageTexture()
  {
    return damagedTexture;
  }
  public static void SetDamageTexture(Texture texture)
  {
    damagedTexture = texture;
  }

  public static Texture GetBurnedTexture()
  {
    return burnedTexture;
  }
  public static void SetBurnedTexture(Texture texture)
  {
    burnedTexture = texture;
  }

  public static void SetDamageAmount(Transform t, float amount)
  {
    t.FindChild("Vis")?.GetComponent<Renderer>().material.SetFloat("_DamageAmount", 1f);
  }

  protected Rigidbody rigidbody;
  protected Renderer visualRenderer;

  protected bool IsBurning()
  {
    if (fireController)
      return fireController.onFire;
    else
      return false;
  }
  protected bool HasBurnedOut()
  {
    return burned;
  }
  //is frozen
  //is heated
  //is wet
  protected bool Destroyed()
  {
    if (transform.GetComponent<ModHealthBar>())
      if (transform.GetComponent<ModHealthBar>().ModHealth <= 0)
        return true;
    return false;
  }

  protected void Awake()
  {
    if (Colliders.Count > 0 && Visuals.Count > 0)
    {
      SafeAwake();
    }
  }

  protected void Start()
  {
    RefreshComponents();
    if (transform.parent != BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (transform.FindChild("FireTrigger"))
        fireController = transform.FindChild("FireTrigger").GetComponent<FireController>();
      MonoStart();
      if (transform.parent == Machine.Active().BuildingMachine)
      {
        BlockPlaced();
      }
    }
  }

  private void RefreshComponents()
  {
    rigidbody = transform.GetComponent<Rigidbody>();
    //visualRenderer = transform.FindChild("Vis")?.GetComponent<Renderer>();
  }

  protected void Update()
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        if (!lastSimulateState)
        {
          //if (visualRenderer)
          //    visualRenderer.material.SetTexture("_DamageMap", GetDamageTexture());
          OnSimulateStart();
        }

        OnSimulateUpdate();

        if (transform.FindChild("FireTrigger"))
        {
          if (!IsBurning() && lastBurningState)
            burned = true;
          else if (IsBurning() && !lastBurningState)
            StartedBurning();

          lastBurningState = IsBurning();
        }

        lastSimulateState = true;
      }
      else
      {
        BuildingUpdate();
        lastSimulateState = false;
      }
      MonoUpdate();
    }
  }

  protected void FixedUpdate()
  {
    if (transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        if (!lastFixedSimulateState)
        {
          OnSimulateFixedStart();
        }

        OnSimulateFixedUpdate();

        lastFixedSimulateState = true;
      }
      else
      {
        lastFixedSimulateState = false;
      }
      MonoFixedUpdate();
    }
  }

  protected void OnDisable()
  {
    if (!rigidbody || !rigidbody.isKinematic)
      if (transform.parent || transform.root != BlockLoaderInfo.GetBlockPrefabsParent() || transform.root != Machine.Active().BuildingMachine)
        if (transform.root.name != "AUTO SAVER" || transform.root.name == Machine.Active().SimulationMachine.name)
          OnSimulateExit();
  }

  protected void OnCollisionEnter(Collision collision)
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        OnSimulateCollisionEnter(collision);
      }
    }
  }
  protected void OnCollisionStay(Collision collision)
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        OnSimulateCollisionStay(collision);
      }
    }
  }
  protected void OnCollisionExit(Collision collision)
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        OnSimulateCollisionExit(collision);
      }
    }
  }
  protected void OnTriggerEnter(Collider other)
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        OnSimulateTriggerEnter(other);
      }
    }
  }
  protected void OnTriggerStay(Collider other)
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        OnSimulateTriggerStay(other);
      }
    }
  }
  protected void OnTriggerExit(Collider other)
  {
    if (transform.parent && transform.parent != TheGuysYouDespise.BlockLoaderInfo.GetBlockPrefabsParent())
    {
      if (StatMaster.isSimulating)
      {
        OnSimulateTriggerExit(other);
      }
    }
  }

  public virtual void SafeAwake() { }
  protected virtual void MonoStart() { }
  public virtual void OnPrefabCreation() { }
  protected virtual void BlockPlaced() { }

  protected virtual void BuildingUpdate() { }
  protected virtual void MonoUpdate() { }
  protected virtual void MonoFixedUpdate() { }

  protected virtual void OnSimulateStart() { }
  protected virtual void OnSimulateFixedStart() { }
  protected virtual void StartedBurning() { }

  protected virtual void OnSimulateUpdate() { }
  protected virtual void OnSimulateFixedUpdate() { }

  protected virtual void OnSimulateExit() { }

  protected virtual void OnSimulateCollisionEnter(Collision collision) { }
  protected virtual void OnSimulateCollisionStay(Collision collision) { }
  protected virtual void OnSimulateCollisionExit(Collision collision) { }
  protected virtual void OnSimulateTriggerEnter(Collider other) { }
  protected virtual void OnSimulateTriggerStay(Collider other) { }
  protected virtual void OnSimulateTriggerExit(Collider other) { }

  protected void PlayFlipSound()
  {
    AudioSource audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    audioSource.clip = FlipSound;
    audioSource.volume = 0.32f;
    audioSource.pitch = 2f;
    audioSource.Play();
  }

  public void MirrorVisuals(Axes axis, bool mirrored)
  {
    if (StatMaster.isSimulating)
      return;
    if (visStartScales.Count != Visuals.Count)
    {
      for (int i = 0; i < Visuals.Count; i++)
      {
        visStartScales.Add(Visuals[i].transform.localScale);
        visStartPositions.Add(Visuals[i].transform.localPosition);
        visStartRotations.Add(Visuals[i].transform.rotation);
      }
    }
    Vector3 mirrorVector = axis == Axes.x ? Vector3.right : axis == Axes.y ? Vector3.up : Vector3.forward;
    mirrorVector *= mirrored ? -1 : 1;
    for (int i = 0; i < Visuals.Count; i++)
    {
      Visuals[i].transform.localScale = new Vector3(visStartScales[i].x * mirrorVector.x, visStartScales[i].y * mirrorVector.y, visStartScales[i].z * mirrorVector.z);
      GameObject go = new GameObject("VisMirrorHelper");
      if (Visuals[i] != null)
      {
        go.transform.parent = Visuals[i].transform.parent;
        go.transform.position = Visuals[i].transform.position;
        go.transform.rotation = Visuals[i].transform.rotation;
        Visuals[i].transform.parent = go.transform;

        switch (axis)
        {
          case Axes.x:
            go.transform.localPosition = new Vector3((mirrored ? -1f : 1f) * visStartPositions[i].x, go.transform.localPosition.y, go.transform.localPosition.z);
            go.transform.rotation = new Quaternion(visStartRotations[i].x,
                                                   (mirrored ? -1f : 1f) * visStartRotations[i].y,
                                                   (mirrored ? -1f : 1f) * visStartRotations[i].z,
                                                   visStartRotations[i].w);
            break;
          case Axes.y:
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, (mirrored ? -1f : 1f) * visStartPositions[i].y, go.transform.localPosition.z);
            go.transform.rotation = new Quaternion((mirrored ? -1f : 1f) * visStartRotations[i].x,
                                                   visStartRotations[i].y,
                                                   (mirrored ? -1f : 1f) * visStartRotations[i].z,
                                                   visStartRotations[i].w);
            if (go.transform.rotation != visStartRotations[i])
              Visuals[i].transform.localEulerAngles += Vector3.forward * 180f;
            break;
          case Axes.z:
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, (mirrored ? -1f : 1f) * visStartPositions[i].z);

            go.transform.rotation = new Quaternion((mirrored ? -1f : 1f) * visStartRotations[i].x,
                                                   (mirrored ? -1f : 1f) * visStartRotations[i].y,
                                                   visStartRotations[i].z,
                                                   visStartRotations[i].w);
            if (go.transform.rotation != visStartRotations[i])
              Visuals[i].transform.localEulerAngles += Vector3.right * 180f + Vector3.forward * 180f;
            break;
        }
        Visuals[i].transform.parent = go.transform.parent;
      }
      Destroy(go);
    }
  }

  public void MirrorColliders(Axes axis, bool mirrored)
  {
    if (StatMaster.isSimulating)
      return;
    if (colliderStartPositions.Count != Colliders.Count)
    {
      for (int i = 0; i < Colliders.Count; i++)
      {
        colliderStartPositions.Add(Colliders[i].transform.localPosition);
        colliderStartRotations.Add(Colliders[i].transform.rotation);
      }
    }
    for (int i = 0; i < Colliders.Count; i++)
    {
      GameObject go = new GameObject("ColMirrorHelper");
      if (Colliders[i] != null)
      {
        go.transform.parent = Colliders[i].transform.parent;
        go.transform.position = Colliders[i].transform.position;
        go.transform.rotation = Colliders[i].transform.rotation;
        Colliders[i].transform.parent = go.transform;

        switch (axis)
        {
          case Axes.x:
            go.transform.localPosition = new Vector3((mirrored ? -1f : 1f) * colliderStartPositions[i].x, go.transform.localPosition.y, go.transform.localPosition.z);
            go.transform.rotation = new Quaternion(colliderStartRotations[i].x,
                                                   (mirrored ? -1f : 1f) * colliderStartRotations[i].y,
                                                   (mirrored ? -1f : 1f) * colliderStartRotations[i].z,
                                                   colliderStartRotations[i].w);
            break;
          case Axes.y:
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, (mirrored ? -1f : 1f) * colliderStartPositions[i].y, go.transform.localPosition.z);
            go.transform.rotation = new Quaternion((mirrored ? -1f : 1f) * colliderStartRotations[i].x,
                                                   colliderStartRotations[i].y,
                                                   (mirrored ? -1f : 1f) * colliderStartRotations[i].z,
                                                   colliderStartRotations[i].w);
            if (go.transform.rotation != colliderStartRotations[i])
              Colliders[i].transform.localEulerAngles += Vector3.forward * 180f;
            break;
          case Axes.z:
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, (mirrored ? -1f : 1f) * colliderStartPositions[i].z);

            go.transform.rotation = new Quaternion((mirrored ? -1f : 1f) * colliderStartRotations[i].x,
                                                   (mirrored ? -1f : 1f) * colliderStartRotations[i].y,
                                                   colliderStartRotations[i].z,
                                                   colliderStartRotations[i].w);
            if (go.transform.rotation != colliderStartRotations[i])
              Colliders[i].transform.localEulerAngles += Vector3.right * 180f + Vector3.forward * 180f;
            break;
        }
        Colliders[i].transform.parent = go.transform.parent;
      }
      Destroy(go);
    }
  }

  public void MirrorVisual(GameObject visual)
  {
    if (!StatMaster.isSimulating)
    {
      visual.transform.localScale -= Vector3.right * visual.transform.localScale.x * 2f;
    }
  }

  public void MirrorCollider(GameObject collider)
  {
    if (!StatMaster.isSimulating)
    {
      GameObject go = new GameObject("ColMirrorHelper");
      go.transform.parent = collider.transform.parent;
      if (collider != null)
      {
        go.transform.position = collider.transform.position;
        go.transform.rotation = collider.transform.rotation;
        collider.transform.parent = go.transform;

        go.transform.localPosition = new Vector3(-go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z);
        go.transform.rotation = new Quaternion(go.transform.rotation.x,
                                              -go.transform.rotation.y,
                                              -go.transform.rotation.z,
                                               go.transform.rotation.w);
        collider.transform.parent = go.transform.parent;
      }
      Destroy(go);
    }
  }

  protected string GetResourcePathOf(string name)
  {
    return Application.dataPath + "/Mods/Blocks/Resources/" + name;
  }

  protected LayerMask NewLayerMask(int[] layers, bool IgnoreTheseLayers = false)
  {
    LayerMask a = 0;
    for (int l = 0; l < layers.Length; l++)
      a += 1 << layers[l];
    if (IgnoreTheseLayers) a = ~a;
    return a;
  }
}