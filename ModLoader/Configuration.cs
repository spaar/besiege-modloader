using System.IO;
using UnityEngine;

namespace spaar
{
    public class Configuration
    {
        public static readonly string CONFIG_FILE_NAME = Application.dataPath + "/Mods/Config/ModLoader.xml";

        public string ConsoleK1 = "LeftControl";
        public string ConsoleK2 = "K";
        public string OEK1 = "LeftControl";
        public string OEK2 = "O";
        public string SettingsK1 = "LeftControl";
        public string SettingsK2 = "L";

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
            if (Directory.Exists(Application.dataPath + "/Mods/Config") && File.Exists(fileName))
            {
                return LoadConfig(fileName);
            }
            else
            {
                Configuration config = new Configuration();
                Directory.CreateDirectory(Application.dataPath + "/Mods/Config");
                SaveConfig(fileName, config);
                return config;
            }
        }
    }
}
