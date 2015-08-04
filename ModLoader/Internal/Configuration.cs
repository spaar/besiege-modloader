using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  /// <summary>
  /// Stores configuration data for the mod loader, currently only keybindings.
  /// </summary>
  public static class Configuration
  {
    /// <summary>
    /// Path to the mod loader configuration file
    /// </summary>
    public static readonly string CONFIG_FILE_NAME = Application.dataPath + "/Mods/Config/ModLoader.xml";

    private static SerializableDictionary<string, string> configItems;

    private static SerializableDictionary<string, string> defaultConfig
      = new SerializableDictionary<string, string>()
      {
        {"consoleK1", "LeftControl" },
        {"consoleK2", "K" },
        {"objExpK1", "LeftControl" },
        {"objExpK2", "O" },
        {"enableUpdateChecker", "true" }
      };

    public static string Get(string key)
    {
      return configItems[key];
    }

    public static void Initialize()
    {
      LoadOrCreateDefault();

      Commands.RegisterCommand("setConfigValue", (args, nArgs) =>
      {
        bool updated = false;
        foreach (var arg in nArgs)
        {
          if (configItems.ContainsKey(arg.Key))
          {
            configItems[arg.Key] = arg.Value;
            updated = true;
          }
          else
          {
            Debug.LogError("No such configuration key: " + arg.Key);
          }
        }
        Keys.LoadKeys();
        return updated ? "Successfully updated configuration."
                       : "Did not update configuration.";
      });
    }

    /// <summary>
    /// Save the configuration.
    /// </summary>
    public static void SaveConfig()
    {
      var serializer = new XmlSerializer(configItems.GetType());
      var writer = File.Open(CONFIG_FILE_NAME, FileMode.Create);
      serializer.Serialize(writer, configItems);
      writer.Close();
    }

    private static void LoadConfig()
    {
      StreamReader reader = null;
      try
      {
        XmlSerializer xs = new XmlSerializer(typeof(SerializableDictionary<string,string>));
        xs.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);
        reader = File.OpenText(CONFIG_FILE_NAME);
        configItems = (SerializableDictionary<string, string>)xs.Deserialize(reader);
        reader.Close();
      }
      catch (Exception e)
      {
        Debug.LogError("Loading of configuration failed! Details:");
        Debug.LogError(e.Message);
        Debug.LogWarning("A default configuration will be created.");
        if (reader != null)
          reader.Close();

        File.Delete(CONFIG_FILE_NAME);
        LoadOrCreateDefault();
      }

      foreach (var key in defaultConfig.Keys)
      {
        if (!configItems.ContainsKey(key))
        {
          Debug.LogWarning("Configuration does not contain key " + key + "."
                         + "\nUsing the default value.");
          configItems[key] = defaultConfig[key];
        }
      }
    }

    private static void LoadOrCreateDefault()
    {
      if (Directory.Exists(Application.dataPath + "/Mods/Config")
        && File.Exists(CONFIG_FILE_NAME))
      {
        LoadConfig();
      }
      else
      {
        configItems = defaultConfig;
        Directory.CreateDirectory(Application.dataPath + "/Mods/Config");
        SaveConfig();
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
