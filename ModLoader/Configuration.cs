using System.IO;
using UnityEngine;

namespace spaar
{
    public class Configuration
    {
        public static readonly string CONFIG_FILE_NAME = Application.dataPath + "/Mods/Config/ModLoader.xml";

        public string ConsoleK1, ConsoleK2, OEK1, OEK2, SettingsK1, SettingsK2;

        public static void SaveConfig(string fileName, Configuration c)
        {
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(fileName);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }
        public static Configuration LoadConfig(string fileName)
        {
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
            StreamReader reader = File.OpenText(fileName);
            Configuration c = (Configuration)xs.Deserialize(reader);
            reader.Close();
            return c;
        }

        public static Configuration LoadOrCreateDefault(string fileName)
        {
            if (File.Exists(fileName))
            {
                return LoadConfig(fileName);
            }
            else
            {
                Configuration config = new Configuration();
                config.ConsoleK1 = "LeftControl";
                config.ConsoleK2 = "K";
                config.OEK1 = "LeftControl";
                config.OEK2 = "O";
                config.SettingsK1 = "LeftControl";
                config.SettingsK2 = "L";
                SaveConfig(fileName, config);
                return config;
            }
        }
    }
}
