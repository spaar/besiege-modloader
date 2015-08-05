using UnityEngine;

namespace spaar.ModLoader
{
  public class PrefabHolder
  {
    /// <summary>
    /// Name of the prefab.
    /// </summary>
    public string Name { get { return GameObject.name; } }

    /// <summary>
    /// The prefab game object.
    /// <para>Be careful when modifying this. It will modify the original,
    /// use Clone() if you want a copy.</para>
    /// </summary>
    public GameObject GameObject { get; private set; }

    /// <summary>
    /// The prefab game object's transform.
    /// <para>Be careful when modifying this. It will modify the original,
    /// use Clone() if you want a copy.</para>
    /// </summary>
    public Transform Transform { get; private set; }

    public PrefabHolder(GameObject prefab)
    {
      GameObject = prefab;
      Transform = prefab.transform;
    }

    /// <summary>
    /// Returns a new instance of the block. This clone is safe to modify.
    /// </summary>
    /// <returns>New instance of block.</returns>
    public GameObject Clone()
    {
      return Clone(Transform.name);
    }

    /// <summary>
    /// Returns a new instance of the block. This clone is safe to modify.
    /// </summary>
    /// <param name="name">Name of new game object.</param>
    /// <returns></returns>
    public GameObject Clone(string name)
    {
      return Clone(name, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Returns a new instance of the block. This clone is safe to modify.
    /// </summary>
    /// <param name="name">Name of new game object.</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject Clone(string name, Vector3 position, Quaternion rotation)
    {
      return (GameObject)Object.Instantiate(GameObject, position, rotation);
    }
  }
}