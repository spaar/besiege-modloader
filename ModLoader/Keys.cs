using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace spaar
{
    internal class Keys : MonoBehaviour
    {
        private static Dictionary<string, Key> Keymap;


        public static void LoadKeys()
        {
            Configuration c = ModLoader.Configuration;
            Keymap = new Dictionary<string, Key>();

            Keymap["Console"] = new Key(c.ConsoleK1, c.ConsoleK2);
            Keymap["ObjectExplorer"] = new Key(c.OEK2, c.OEK2);
            Keymap["Settings"] = new Key(c.SettingsK1, c.SettingsK2);
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

    internal class Key
    {
        public KeyCode Modifier { get; private set; }
        public KeyCode Trigger { get; private set; }

        public Key(string modifier, string key)
        {
            Modifier = (KeyCode)Enum.Parse(typeof(KeyCode), modifier);
            Trigger = (KeyCode)Enum.Parse(typeof(KeyCode), key);
        }
    }
}
