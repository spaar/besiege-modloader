using System;
using System.Reflection;

namespace spaar.ModLoader
{
  /// <summary>
  /// Main entry point for mods.
  /// Should be implemented by exactly one class per mod.
  /// </summary>
  public interface Mod
  {
    /// <summary>
    /// Name of the mod.
    /// Should be all lowercase and be in camelCase.
    /// This name should not change once a mod was published.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Display name of the mod.
    /// Can contain any symbols and be changed freely.
    /// This is the name that's presented to users.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Author of the mod.
    /// If there are serveral author, seperate them with commas.
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Current version of the mod. Any versioning scheme can be used,
    /// as long as it can be expressed in a Version object.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// The version of Besiege the mod is targeted at.
    /// Versions follow the format used by the game itself, 'vMajor.Minor'.
    /// If this is not equal to the currently running game version,
    /// a warning will be printed, however the mod will still be loaded.
    /// </summary>
    string BesiegeVersion { get; }

    /// <summary>
    /// Whether the mod can be unloaded without a restart.
    /// Only set this to true if your mod undoes all its changes to the game
    /// in OnUnload().
    /// </summary>
    bool CanBeUnloaded { get; }

    /// <summary>
    /// Called when your mod is loaded.
    /// You can perform any initialization here, such as creating game objects
    /// to control your mod.
    /// </summary>
    void OnLoad();

    /// <summary>
    /// Called when your mod is unloaded.
    /// This occurs in two cases: The user unloads the mod or the game exits.
    /// Perform any clean-up work you need to here, such as saving
    /// configuration and reverting modifications to the game.
    /// </summary>
    void OnUnload();

  }
}