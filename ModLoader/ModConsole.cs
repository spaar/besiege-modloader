using System.Collections.Generic;
using spaar.ModLoader.Internal.Tools;
using UnityEngine;

namespace spaar.ModLoader
{
  public static class ModConsole
  {
    public static void AddMessage(LogType type, string text,
      string collapsedText = "")
    {
      Console.Instance.HandleLog(text, collapsedText, type);
    }

    public static void AddMessage(LogType type, string text,
      List<string> collapsedText)
    {
      var collapsedString = "";
      foreach (var s in collapsedText)
      {
        collapsedString += collapsedText + "\n";
      }
      collapsedString.Trim();

      AddMessage(type, text, collapsedString);
    }
  }
}
