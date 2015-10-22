using System;
using System.Reflection;
using UnityEngine;

namespace spaar
{
  /// <summary>
  /// Old Mod attribute, included temporarily for backwards-compatibility to
  /// ease porting mods.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class Mod : Attribute
  {
    private string name;
    /// <summary>
    /// Version of the mod, does not have to conform to any specific format.
    /// </summary>
    public string version;
    /// <summary>
    /// Author of the mod
    /// </summary>
    public string author;

    internal Assembly assembly;

    /// <summary>
    /// Initalizes a Mod instance with a default version of 1.0 and no author.
    /// </summary>
    /// <param name="name">Name of the mod</param>
    public Mod(string name)
    {
      this.name = name;
      version = "1.0";
      author = "";
    }

    /// <summary>
    /// Get the name of this Mod.
    /// </summary>
    /// <returns>The name</returns>
    public string Name()
    {
      return name;
    }
  }
}

namespace spaar.ModLoader.Internal
{
  public class ModCompatWrapper : Mod
  {
    private string compatAuthor, compatName;
    private Version compatVersion;

    public override string Author { get { return compatAuthor; } }
    public override string Name { get { return compatName; } }
    public override string DisplayName { get { return compatName; } }
    public override Version Version { get { return compatVersion; } }
    public override void OnLoad()
    {
    }
    public override void OnUnload()
    {
    }

    public void SetCompatInfo(string author, string name, string version)
    {
      compatAuthor = author;
      compatName = name;
      try
      {
        compatVersion = new Version(version);
      }
      catch (Exception)
      {
        Debug.Log("Can't convert version to Version object, using 1.0");
        compatVersion = new Version(1, 0);
      }
    }
  }
}