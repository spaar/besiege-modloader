using System;
using System.Collections.Generic;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  internal class Keys : MonoBehaviour
  {
    private static Dictionary<string, Key> Keymap;

    private static Keys _instance;
    public static Keys K { get { return _instance; } }

    public static void LoadKeys()
    {
      Keymap = new Dictionary<string, Key>();

      Keymap["Console"] = new Key(Configuration.Get("consoleK1"),
        Configuration.Get("consoleK2"));
      Keymap["ObjectExplorer"] = new Key(Configuration.Get("objExpK1"),
        Configuration.Get("objExpK2"));

      _instance = new Keys();
    }

    /// <summary>
    /// Equivalent to <c>getKey(key).IsDown()</c>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool this[string key]
    {
      get
      {
        return getKey(key).IsDown();
      }
    }

    public static Key getKey(string keyName)
    {
      if (Keymap.ContainsKey(keyName))
      {
        return Keymap[keyName];
      }
      else
      {
        throw new ArgumentException("No such key: " + keyName);
      }
    }
  }
}
