using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Block
{
    public BlockPrefab prefab = new BlockPrefab(9999);
    public bool loading = false;

    public int id { get; set; }
    public GameObject gameObject { get; set; }
    public GameObject ghost { get; set; }
    public string name { get; set; }
    public float mass { get; set; }
    public bool showCollider { get; set; }
    public List<Obj> objs { get; set; }
    public Icon icon { get; set; }
    public BlockProperties properties { get; set; }
    public List<ColliderComposite> compoundCollider { get; set; }
    public Type[] components { get; set; }
    public List<NeededResource> neededResources { get; set; }
    public List<AddingPoint> addingPoints { get; set; }

    public bool ignoreIntersectionForBase = false;
    public FireController fireController;
    public List<GameObject> displayColliders = new List<GameObject>();

    public Block()
    {

    }
    public Block ID(int id)
    {
        this.id = id;
        return this;
    }
    public Block Obj(List<Obj> objs)
    {
        this.objs = objs;
        return this;
    }
    public Block BlockName(string blockName)
    {
        this.name = blockName;
        return this;
    }
    public Block Mass(float mass)
    {
        this.mass = mass;
        return this;
    }
    public Block ShowCollider(bool showCollider)
    {
        this.showCollider = showCollider;
        return this;
    }
    public Block IconOffset(Icon icon)
    {
        this.icon = icon;
        return this;
    }
    public Block Properties(BlockProperties blockProp)
    {
        this.properties = blockProp;
        return this;
    }
    public Block CompoundCollider(List<ColliderComposite> compoundCollider)
    {
        this.compoundCollider = compoundCollider;
        return this;
    }
    public Block Components(Type[] components)
    {
        this.components = components;
        return this;
    }
    public Block NeededResources(List<NeededResource> neededResources)
    {
        this.neededResources = neededResources;
        return this;
    }
    public Block AddingPoints(List<AddingPoint> addingPoints)
    {
        this.addingPoints = addingPoints;
        return this;
    }
    public Block IgnoreIntersectionForBase()
    {
        this.ignoreIntersectionForBase = true;
        return this;
    }

    public Block(Block block)
    {
        //TO DO: FINISH:
        this.ghost = block.ghost;
        this.id = block.id;
        this.name = block.name;
        this.mass = block.mass;
        this.showCollider = block.showCollider;
        //new objs
        this.objs = block.objs;
        //new icon
        this.icon = block.icon;
        //new properties
        this.properties = block.properties;
        //new colliders
        this.compoundCollider = block.compoundCollider;
        //new components
        this.components = block.components;
        //new needed resources
        this.neededResources = block.neededResources;
        //new adding points
        this.addingPoints = block.addingPoints;
        this.ignoreIntersectionForBase = block.ignoreIntersectionForBase;
        this.fireController = block.fireController;
        this.displayColliders = block.displayColliders;
    }

    public Block(int requestedBlockID, string blockName, List<Obj> objs, Icon icon, BlockProperties properties,
                 List<ColliderComposite> compoundCollider, List<NeededResource> neededResources, bool showCollider,
                 float mass, Type[] scripts, List<AddingPoint> addingPoints)
    {
        this.id = requestedBlockID;
        this.objs = objs;
        this.name = blockName;
        this.icon = icon;
        this.properties = properties;
        this.compoundCollider = compoundCollider;
        this.neededResources = neededResources;
        this.showCollider = showCollider;
        this.mass = mass;
        this.components = scripts;
        this.addingPoints = addingPoints;
    }
}

public class Icon
{
    public float size;
    public Vector3 scale;
    public Vector3 offset;
    public Vector3 rotation;

    public Icon(float size, Vector3 offset, Vector3 rotation)
    {
        this.size = size;
        this.offset = offset;
        this.rotation = rotation;
    }

    public Icon(Vector3 scale, Vector3 offset, Vector3 rotation)
    {
        this.scale = scale;
        this.offset = offset;
        this.rotation = rotation;
    }
}

public class BlockProperties
{
    public damageType damageType;
    public bool canFlip;
    public bool canBeDamaged = false;
    public bool burnable = false;
    public float health;
    public float burningDuration;
    public string[] keywords;

    public BlockProperties()
    {
        this.damageType = damageType.Blunt;
        this.canFlip = false;
    }


    public BlockProperties Burnable(float burningDuration)
    {
        this.burnable = true;
        this.burningDuration = burningDuration;
        return this;
    }

    public BlockProperties CanBeDamaged(float maxHealth)
    {
        this.canBeDamaged = true;
        this.health = maxHealth;
        return this;
    }

    public BlockProperties DamageType(damageType type)
    {
        this.damageType = type;
        return this;
    }

    public BlockProperties SearchKeywords(string[] keywords)
    {
        this.keywords = keywords;
        return this;
    }
}

public class VisualOffset
{
    public Vector3 size;
    public Vector3 position;
    public Vector3 rotation;

    public VisualOffset(Vector3 size, Vector3 position, Vector3 rotation)
    {
        this.size = size;
        this.position = position;
        this.rotation = rotation;
    }
}

public class Obj
{
    public GameObject gameObject;
    public string objName;
    public string textureName;
    public Texture2D texture;
    public VisualOffset offset;
    public Mesh importedMesh;
    public Opacity opacity = Opacity.Opaque;
    public Material material;
    public Material ghostMaterial;
    public Color color;

    public Obj(string fileName, VisualOffset offset)
    {
        this.objName = fileName;
        this.offset = offset;
    }

    public Obj(string fileName, string textureName, VisualOffset offset)
    {
        this.objName = fileName;
        this.textureName = textureName;
        this.offset = offset;
    }

    public Obj(string fileName, string textureName, VisualOffset offset, Opacity opacity)
    {
        this.objName = fileName;
        this.textureName = textureName;
        this.offset = offset;
        this.opacity = opacity;
    }

    public Obj(string fileName)
    {
        this.objName = fileName;
        this.offset = new VisualOffset(Vector3.one, Vector3.zero, Vector3.zero);
    }
}

public class ColliderComposite
{
    public GameObject gameObject;
    public ColliderType colliderType;
    public Vector3 size;
    public Vector3 offset;
    public Vector3 rotation;
    public float radius;
    public float height;
    public float thickness;
    public int direction;
    public string meshName;
    public Mesh colliderMesh;
    public int layer = 0;
    public bool trigger = false;
    public bool ignoreForGhost = false;

    public static ColliderComposite Box(Vector3 boxSize, Vector3 offset, Vector3 rotation)
    {
        return new ColliderComposite(boxSize, offset, rotation);
    }

    public static ColliderComposite Sphere(float sphereRadius, Vector3 offset, Vector3 rotation)
    {
        return new ColliderComposite(sphereRadius, offset, rotation);
    }

    public static ColliderComposite Capsule(float capsuleRadius, float height, Direction direction, Vector3 offset, Vector3 rotation)
    {
        return new ColliderComposite(capsuleRadius, height, (int)direction, offset, rotation);
    }

    public static ColliderComposite Mesh(string meshObjName, Vector3 size, Vector3 offset, Vector3 rotation)
    {
        return new ColliderComposite(meshObjName, size, offset, rotation);
    }

    public ColliderComposite()
    {
    }

    public ColliderComposite(Vector3 boxSize, Vector3 offset, Vector3 rotation)
    {
        this.colliderType = ColliderType.Box;
        this.size = boxSize;
        this.offset = offset;
        this.rotation = rotation;
    }

    public ColliderComposite(float sphereRadius, Vector3 offset, Vector3 rotation)
    {
        this.colliderType = ColliderType.Sphere;
        this.radius = sphereRadius;
        this.offset = offset;
        this.rotation = rotation;
    }

    public ColliderComposite(float capsuleRadius, float height, int direction, Vector3 offset, Vector3 rotation)
    {
        this.colliderType = ColliderType.Capsule;
        this.radius = capsuleRadius;
        this.height = height;
        this.direction = direction;
        this.offset = offset;
        this.rotation = rotation;
    }

    public ColliderComposite(string meshObjName, Vector3 size, Vector3 offset, Vector3 rotation)
    {
        this.colliderType = ColliderType.Mesh;
        this.meshName = meshObjName;
        this.size = size;
        this.offset = offset;
        this.rotation = rotation;
    }


    public ColliderComposite Trigger()
    {
        this.trigger = true;
        return this;
    }

    public ColliderComposite Layer(int layer)
    {
        this.layer = layer;
        return this;
    }

    public ColliderComposite IgnoreForGhost()
    {
        ignoreForGhost = true;
        return this;
    }
}

public class NeededResource
{
    public ResourceType resourceType;
    public AudioClip audioClip;
    public Texture texture;
    public Mesh mesh;
    public AssetBundle assetBundle;
    public string resourceName;
    public string resourcePath;

    public NeededResource(ResourceType resourceType, string name)
    {
        this.resourceType = resourceType;
        this.resourceName = name;
        this.resourcePath = GetResourcePathOf(name);
    }

    private string GetResourcePathOf(string s)
    {
        return Application.dataPath + "/Mods/Blocks/Resources/" + s;
    }

}

public class AddingPoint
{
    public Vector3 position, rotation;
    public bool sticky;
    public float radius = 0.6f;

    public AddingPoint()
    {
    }

    public AddingPoint(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public AddingPoint(Vector3 position, Vector3 rotation, bool sticky)
    {
        this.sticky = sticky;
        this.position = position;
        this.rotation = rotation;
    }

    public AddingPoint SetStickyRadius(float r)
    {
        this.radius = r;
        return this;
    }

}

public class BasePoint : AddingPoint
{
    public bool motionable = false;
    public bool hasAddingPoint;
    public ConfigurableJointMotion xMotion, yMotion, zMotion;

    public BasePoint(bool hasAddingPoint, bool sticky)
    {
        this.hasAddingPoint = hasAddingPoint;
        this.sticky = sticky;
    }

    public BasePoint Motionable(bool x, bool y, bool z)
    {
        motionable = true;
        this.xMotion = (ConfigurableJointMotion)(2 * Convert.ToInt16(x));
        this.yMotion = (ConfigurableJointMotion)(2 * Convert.ToInt16(y));
        this.zMotion = (ConfigurableJointMotion)(2 * Convert.ToInt16(z));
        return this;
    }
}

