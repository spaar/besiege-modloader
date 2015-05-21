using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace spaar
{
    public class Configuration
    {
        public static readonly string CONFIG_FILE_NAME = Application.dataPath + "/Mods/Config/ModLoader.xml";

        public string consoleK1 = "LeftControl";
        public string consoleK2 = "K";
        public string objExpK1 = "LeftControl";
        public string objExpK2 = "O";
        public string settingsK1 = "LeftControl";
        public string settingsK2 = "L";

        public static void SaveConfig(string fileName, Configuration c)
        {
            XmlSerializer xs = new XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(fileName);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }
        public static Configuration LoadConfig(string fileName)
        {
            StreamReader reader = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Configuration));
                xs.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);
                reader = File.OpenText(fileName);
                Configuration c = (Configuration)xs.Deserialize(reader);
                reader.Close();
                return c;
            }
            catch (Exception e)
            {
                Debug.LogError("Loading of configuration failed! Details:");
                Debug.LogError(e.Message);
                Debug.LogWarning("A default configuration will be created.");
                if (reader != null)
                    reader.Close();

                File.Delete(fileName);
                return LoadOrCreateDefault(fileName);
            }
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

        private static void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            Debug.LogWarning("Configuration: Unknown element: " + e.Element.Name);
            Debug.LogWarning("Is your configuration syntax invalid?");
            Debug.LogWarning("The default values will be used for all values that cannot be read.");
            Debug.LogWarning("You can genereate a new valid configuration by deleting the old one.");
        }
    }
}
