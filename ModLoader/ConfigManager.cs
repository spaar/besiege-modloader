using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace spaar
{
    /// <summary>
    ///     This class will manage the config.
    ///     GUI is stored into separate classes
    /// </summary>
    public static class ConfigManager
    {
        private static readonly Dictionary<string, Key> keys = new Dictionary<string, Key>();
        private static Config c;

        /// <summary>
        ///     Loads the config or creates default config
        /// </summary>
        /// <param name="File name"></param>
        public static void LoadOrCreateConfig()
        {
            if (Directory.Exists(Application.dataPath + "/Mods/Config") &&
                File.Exists(Application.dataPath + "/Mods/Config/Config.xml"))
            {
                //add other default values here
                Directory.CreateDirectory(Application.dataPath + "/Mods/Config");
                c = new Config();
                c.CK1 = "LeftControl";
                c.CK2 = "K";
                c.OEK1 = "LeftControl";
                c.OEK2 = "O";
                c.SK1 = "LeftControl";
                c.SK2 = "L";
                SaveConfig(c.CONFIG_FILE_NAME, c);
            }
            else
            {
                c = LoadConfig(c.CONFIG_FILE_NAME);
            }
            //add methods to load other config parts here
            loadKeysIntoDictionary();
        }

        //loads Keys into the dictionary
        private static void loadKeysIntoDictionary()
        {
            keys["ConsoleK"] = new Key(c.CK1, c.CK2);
            keys["OEK"] = new Key(c.OEK1, c.OEK2);
            keys["SettingsK"] = new Key(c.SK1, c.SK2);
        }

        //Save keys that are changed through KeySettings
        public static void SaveChangedKeys()
        {
            var keyCodes = GameObject.Find("MODLOADERLORD").GetComponent<KeySettings>().keyCode;
            c.CK1 = keyCodes[0].ToString();
            c.CK2 = keyCodes[1].ToString();
            c.OEK1 = keyCodes[2].ToString();
            c.OEK2 = keyCodes[3].ToString();
            c.SK1 = keyCodes[4].ToString();
            c.SK2 = keyCodes[5].ToString();
            SaveConfig(c.CONFIG_FILE_NAME, c);
            loadKeysIntoDictionary();
        }

        //getKey
        public static Key GetKey(string KeyName)
        {
            if (keys.ContainsKey(KeyName))
            {
                return keys[KeyName];
            }
            throw new ArgumentException("No such Key" + KeyName);
        }

        //Serialization
        public static void SaveConfig(string fileName, Config c)
        {
            var xs = new XmlSerializer(c.GetType());
            var writer = File.CreateText(fileName);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }

        public static Config LoadConfig(string fileName)
        {
            var xs = new XmlSerializer(typeof (Config));
            var reader = File.OpenText(fileName);
            var c = (Config) xs.Deserialize(reader);
            reader.Close();
            return c;
        }
    }
}