using System;
using System.Reflection;

namespace spaar.ModLoader
{
  /// <summary>
  /// Main entry point for mods.
  /// Should be implemented by exactly one class per mod.
  /// </summary>
  public abstract class Mod
  {
    /// <summary>
    /// Name of the mod.
    /// <para>
    /// Should be alphanumeric and all lowercase. If the name consists of
    /// multiple words, seperate them with hyphens, like 'my-mod'.
    /// </para>
    /// <para>
    /// This name should not change once a mod was published.
    /// It should also be unique among all published mod as it is used for
    /// identification by the mod loader.
    /// </para>
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Display name of the mod.
    /// <para>
    /// Can contain any symbols and be changed freely.
    /// This is the name that's presented to users.
    /// </para>
    /// </summary>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Author of the mod.
    /// If there are serveral author, seperate them with commas.
    /// </summary>
    public abstract string Author { get; }

    /// <summary>
    /// Current version of the mod. Any versioning scheme can be used,
    /// as long as it can be expressed in a Version object.
    /// </summary>
    public abstract Version Version { get; }

    /// <summary>
    /// The version of Besiege the mod is targeted at.
    /// <para>
    /// Versions follow the format used by the game itself, 'vMajor.Minor'.
    /// If this is not equal to the currently running game version,
    /// a warning will be printed, however the mod will still be loaded.
    /// </para>
    /// <para>
    /// Default is the version the running mod loader is targeted at.
    /// Please only use this default if you are reasonably sure that a Besiege
    /// update will not break your mod!
    /// </para>
    /// </summary>
    public virtual string BesiegeVersion { get { return Internal.ModLoader.BesiegeVersion; } }

    /// <summary>
    /// Whether the mod can be unloaded without a restart.
    /// Only set this to true if your mod undoes all its changes to the game
    /// in OnUnload().
    /// Defaults to false since many mods are not able to do this by default.
    /// </summary>
    public virtual bool CanBeUnloaded { get { return false; } }

    /// <summary>
    /// Called when your mod is loaded.
    /// You can perform any initialization here, such as creating game objects
    /// to control your mod.
    /// </summary>
    public abstract void OnLoad();

    /// <summary>
    /// Called when your mod is unloaded.
    /// This occurs in two cases: The user unloads the mod or the game exits.
    /// Perform any clean-up work you need to here, such as saving
    /// configuration and reverting modifications to the game.
    /// If your mod is capable of reverting its changes at runtime, also set
    /// CanBeUnloaded to true to indicate that the mod can be unloaded by the
    /// user at runtime.
    /// </summary>
    public abstract void OnUnload();
  }
}