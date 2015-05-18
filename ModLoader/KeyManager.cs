using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace spaar
{
    public class KeyConfig
    {
        public string CK1;
        public string CK2;
        public string OK1;
        public string OK2;
        public string SK1;
        public string SK2;
    }

    internal class Key
    {
        public Key(KeyCode modifier, KeyCode key)
        {
            Modifier = modifier;
            Trigger = key;
        }

        public KeyCode Modifier { get; private set; }
        public KeyCode Trigger { get; private set; }
    }

    internal static class KeyGetter
    {
        private static Dictionary<string, Key> KeyMap = new Dictionary<string, Key>();

        /// <summary>
        ///     Gets the Keys by its name.
        ///     Used for Console, OE, and Settings
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns>Dictionary, KeyMap</returns>
        public static Key getKey(string KeyName)
        {
            if (KeyMap.ContainsKey(KeyName))
            {
                return KeyMap[KeyName];
            }
            throw new ArgumentException("No such Key" + KeyName);
        }

        /// <summary>
        ///     Called by Settings, saves changed keys to Dictionary
        /// </summary>
        public static void saveKeys()
        {
            var keys = GameObject.Find("MODLOADERLORD").GetComponent<KeySettings>().keyCode;
            KeyMap["ConsoleK"] = new Key(keys[0], keys[1]);
            KeyMap["OEK"] = new Key(keys[2], keys[3]);
            KeyMap["SettingsK"] = new Key(keys[4], keys[5]);
        }

        /// <summary>
        ///     Saves keys to Dictionary.
        ///     Used by KeyManager.LoadKeys
        /// </summary>
        public static void saveKeys(string Keyname, KeyCode KeyCodeModifier, KeyCode KeyCodeTrigger)
        {
            KeyMap[Keyname] = new Key(KeyCodeModifier, KeyCodeTrigger);
        }
    }

    internal class KeyManager
    {
        /// <summary>
        ///     Saves the keys to the file
        /// </summary>
        public static void saveKeys()
        {
            var kc = new KeyConfig();
            kc.CK1 = KeyGetter.getKey("ConsoleK").Modifier.ToString();
            kc.CK2 = KeyGetter.getKey("ConsoleK").Trigger.ToString();
            kc.OK1 = KeyGetter.getKey("OEK").Modifier.ToString();
            kc.OK2 = KeyGetter.getKey("OEK").Trigger.ToString();
            kc.SK1 = KeyGetter.getKey("SettingsK").Modifier.ToString();
            kc.SK2 = KeyGetter.getKey("SettingsK").Trigger.ToString();
            SaveKeys(Application.dataPath + "/Mods/Config/Keys.txt", kc);
        }

        /// <summary>
        ///     Loads the keys from the file.
        ///     If no file, Default Keys
        /// </summary>
        public static void LoadKeys()
        {
            var kc = loadKeys();
            KeyGetter.saveKeys("ConsoleK", (KeyCode) Enum.Parse(typeof (KeyCode), kc.CK1),
                (KeyCode) Enum.Parse(typeof (KeyCode), kc.CK2));
            KeyGetter.saveKeys("OEK", (KeyCode) Enum.Parse(typeof (KeyCode), kc.OK1),
                (KeyCode) Enum.Parse(typeof (KeyCode), kc.OK2));
            KeyGetter.saveKeys("SettingsK", (KeyCode) Enum.Parse(typeof (KeyCode), kc.SK1),
                (KeyCode) Enum.Parse(typeof (KeyCode), kc.SK2));
        }

        /// <summary>
        ///     Checks the keys to load from file before parsing to KeyConfig
        /// </summary>
        /// <returns>KeyConfig</returns>
        private static KeyConfig loadKeys()
        {
            if (!Directory.Exists(Application.dataPath + "/Mods/Config") ||
                !File.Exists(Application.dataPath + "/Mods/Config/Keys.txt"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Mods/Config");
                var k = new KeyConfig();
                k.CK1 = "LeftControl";
                k.CK2 = "K";
                k.OK1 = "LeftControl";
                k.OK2 = "O";
                k.SK1 = "LeftControl";
                k.SK2 = "L";
                return k;
            }
            return LoadKeys(string.Concat(Application.dataPath, "/Mods/Config/Keys.txt"));
        }

        private static void SaveKeys(string fileName, KeyConfig kc)
        {
            var xs = new XmlSerializer(kc.GetType());
            var writer = File.CreateText(fileName);
            xs.Serialize(writer, kc);
            writer.Flush();
            writer.Close();
        }

        private static KeyConfig LoadKeys(string fileName)
        {
            var xs =
                new XmlSerializer(typeof (KeyConfig));
            var reader = File.OpenText(fileName);
            var kc = (KeyConfig) xs.Deserialize(reader);
            reader.Close();
            return kc;
        }
    }
}
