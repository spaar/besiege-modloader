using System.Collections.Generic;
using spaar.ModLoader.Internal.Tools;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Provides methods for interacting with the in-game console.
  /// </summary>
  public static class ModConsole
  {
    /// <summary>
    /// Add a message to the console.
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="text">Main message text</param>
    /// <param name="collapsedText">Collapsed text</param>
    public static void AddMessage(LogType type, string text,
      string collapsedText = "")
    {
      Console.Instance.HandleLog(text, collapsedText, type);
    }

    /// <summary>
    /// Add a message to the console.
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="text">Main message text</param>
    /// <param name="collapsedText">
    /// Collapsed text. Each entry in the list will be on its own line.
    /// </param>
    public static void AddMessage(LogType type, string text,
      List<string> collapsedText)
    {
      var collapsedString = "";
      foreach (var s in collapsedText)
      {
        collapsedString += s + "\n";
      }
      collapsedString.Trim();

      AddMessage(type, text, collapsedString);
    }

    /// <summary>
    /// Forces the serialization of the Mods/Debug/ConsoleOutput.txt file in
    /// the developer build. Does nothing in the normal build.
    /// </summary>
    public static void ForceWriteToDisk()
    {
#if DEV_BUILD
      Console.Instance.WriteMessagesToDisk();
#endif
    }
  }
}
