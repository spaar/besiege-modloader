using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace spaar
{
    /// <summary>
    /// Stores configuration data for the mod loader, currently only keybindings.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Path to the mod loader configuration file
        /// </summary>
        public static readonly string CONFIG_FILE_NAME = Application.dataPath + "/Mods/Config/ModLoader.xml";

        /// <summary>Modifer key for the console</summary>
        public string consoleK1 = "LeftControl";
        /// <summary>Trigger key for the console</summary>
        public string consoleK2 = "K";
        /// <summary>Modifer key for the object explore</summary>
        public string objExpK1 = "LeftControl";
        /// <summary>Trigger key for the object explorer</summary>
        public string objExpK2 = "O";
        /// <summary>Modifer key for the settings window</summary>
        public string settingsK1 = "LeftControl";
        /// <summary>Trigger key for the settings window</summary>
        public string settingsK2 = "L";

        /// <summary>
        /// Save the specified configuration to the specified path, using XML serialization.
        /// </summary>
        /// <param name="fileName">Path to configuration file</param>
        /// <param name="c">Configuration to save</param>
        public static void SaveConfig(string fileName, Configuration c)
        {
            XmlSerializer xs = new XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(fileName);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Load a configuration from the specified path, using XML serialization.
        /// </summary>
        /// <remarks>
        /// If no valid configuration can be loaded from the file, an error message is printed and a default one created and returned instead.
        /// If the file does not contain one or more values, the defaults for those are used instead.
        /// </remarks>
        /// <param name="fileName">Path to configuration file</param>
        /// <returns>The loaded configuration</returns>
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

        /// <summary>
        /// Load the specified configuration if one can be found at the specified location, otherwise create a default one there.
        /// </summary>
        /// <param name="fileName">Path to the configuration file</param>
        /// <returns>The loaded or created configuration</returns>
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
