using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using SimpleJSON;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Utility class to store configuration values for your mod.
  /// This is a basic Key-Value store and supports only strings as keys
  /// and strings, ints, floats, doubles and bools as values.
  /// Configuration is stored in <![CDATA[Mods/Config/<modname>.json]]>.
  /// </summary>
  public static class Configuration
  {

    private struct Value
    {
      public string type;
      public string value;

      public static bool operator ==(Value v1, Value v2)
      {
        return v1.Equals(v2);
      }

      public static bool operator !=(Value v1, Value v2)
      {
        return !v1.Equals(v2);
      }

      public override bool Equals(object obj)
      {
        if (!(obj is Value))
          return false;
        var other = (Value)obj;
        return value == other.value && type == other.type;
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }
    }

    private static Dictionary<string, Dictionary<string, Value>> configs
      = new Dictionary<string, Dictionary<string, Value>>();

    #region Getters for all supported types

    /// <summary>
    /// Gets a string configuration value stored by your mod.
    /// Returns defaultVal if the specified key does not exist for your mod or
    /// the value stored at the specified key is not a string.
    /// </summary>
    /// <param name="key">Key of your value</param>
    /// <param name="defaultVal">Default value</param>
    public static string GetString(string key, string defaultVal)
    {
      var val = GetValue(Assembly.GetCallingAssembly(), key);

      if (val == default(Value) || val.type != "string")
      {
        return defaultVal;
      }

      return val.value;
    }
    /// <summary>
    /// Gets an integer configuration value stored by your mod.
    /// Returns defaultVal if the specified key does not exist for your mod or
    /// the value stored at the specified key is not an integer.
    /// </summary>
    /// <param name="key">Key of your value</param>
    /// <param name="defaultVal">Default value</param>
    public static int GetInt(string key, int defaultVal)
    {
      var val = GetValue(Assembly.GetCallingAssembly(), key);

      if (val == default(Value) || val.type != "int")
      {
        return defaultVal;
      }

      return int.Parse(val.value);
    }
    /// <summary>
    /// Gets a float configuration value stored by your mod.
    /// Returns defaultVal if the specified key does not exist for your mod or
    /// the value stored at the specified key is not a float.
    /// </summary>
    /// <param name="key">Key of your value</param>
    /// <param name="defaultVal">Default value</param>
    public static float GetFloat(string key, float defaultVal)
    {
      var val = GetValue(Assembly.GetCallingAssembly(), key);

      if (val == default(Value) || val.type != "float")
      {
        return defaultVal;
      }

      return float.Parse(val.value);
    }
    /// <summary>
    /// Gets a double configuration value stored by your mod.
    /// Returns defaultVal if the specified key does not exist for your mod or
    /// the value stored at the specified key is not a double.
    /// </summary>
    /// <param name="key">Key of your value</param>
    /// <param name="defaultVal">Default value</param>
    public static double GetDouble(string key, double defaultVal)
    {
      var val = GetValue(Assembly.GetCallingAssembly(), key);

      if (val == default(Value) || val.type != "double")
      {
        return defaultVal;
      }

      return double.Parse(val.value);
    }
    /// <summary>
    /// Gets a bool configuration value stored by your mod.
    /// Returns defaultVal if the specified key does not exist for your mod or
    /// the value stored at the specified key is not a bool.
    /// </summary>
    /// <param name="key">Key of your value</param>
    /// <param name="defaultVal">Default value</param>
    public static bool GetBool(string key, bool defaultVal)
    {
      var val = GetValue(Assembly.GetCallingAssembly(), key);

      if (val == default(Value) || val.type != "bool")
      {
        return defaultVal;
      }

      return bool.Parse(val.value);
    }

    #endregion

    #region Setters for all supported types

    /// <summary>
    /// Sets a string in the configuration of your mod.
    /// </summary>
    /// <param name="key">Key of the value to set</param>
    /// <param name="value">Value to set</param>
    public static void SetString(string key, string value)
    {
      var val = new Value()
      {
        type = "string",
        value = value
      };
      SetValue(Assembly.GetCallingAssembly(), key, val);
    }
    /// <summary>
    /// Sets an integer in the configuration of your mod.
    /// </summary>
    /// <param name="key">Key of the value to set</param>
    /// <param name="value">Value to set</param>
    public static void SetInt(string key, int value)
    {
      var val = new Value()
      {
        type = "int",
        value = value.ToString()
      };
      SetValue(Assembly.GetCallingAssembly(), key, val);
    }
    /// <summary>
    /// Sets a float in the configuration of your mod.
    /// </summary>
    /// <param name="key">Key of the value to set</param>
    /// <param name="value">Value to set</param>
    public static void SetFloat(string key, float value)
    {
      var val = new Value()
      {
        type = "float",
        value = value.ToString()
      };
      SetValue(Assembly.GetCallingAssembly(), key, val);
    }
    /// <summary>
    /// Sets a double in the configuration of your mod.
    /// </summary>
    /// <param name="key">Key of the value to set</param>
    /// <param name="value">Value to set</param>
    public static void SetDouble(string key, double value)
    {
      var val = new Value()
      {
        type = "double",
        value = value.ToString()
      };
      SetValue(Assembly.GetCallingAssembly(), key, val);
    }
    /// <summary>
    /// Sets a bool in the configuration of your mod.
    /// </summary>
    /// <param name="key">Key of the value to set</param>
    /// <param name="value">Value to set</param>
    public static void SetBool(string key, bool value)
    {
      var val = new Value()
      {
        type = "bool",
        value = value.ToString()
      };
      SetValue(Assembly.GetCallingAssembly(), key, val);
    }

    #endregion

    private static Value GetValue(Assembly callingAsm, string key)
    {
      var mod = GetModFromAssembly(callingAsm);
      if (mod == null)
      {
        throw new InvalidOperationException("Cannot get configuration value: "
          + "Failed to determine mod");
      }

      var modName = mod.Mod.Name;

      if (!configs.ContainsKey(modName) || !configs[modName].ContainsKey(key))
      {
        return default(Value);
      }

      return configs[modName][key];
    }

    private static void SetValue(Assembly callingAsm, string key, Value value)
    {
      var mod = GetModFromAssembly(callingAsm);
      if (mod == null)
      {
        throw new InvalidOperationException("Cannot set configuration value: "
          + "Failed to determine mod");
      }

      var modName = mod.Mod.Name;

      if (!configs.ContainsKey(modName))
      {
        configs[modName] = new Dictionary<string, Value>();
      }

      configs[modName][key] = value;
    }

    internal static void Load()
    {
      var files = new DirectoryInfo(Application.dataPath + "/Mods/Config/")
        .GetFiles("*.json");
      foreach (var file in files)
      {
        LoadFile(file.Name);
      }
    }

    private static void LoadFile(string fileName)
    {
      string path = Application.dataPath + "/Mods/Config/" + fileName;
      string modName = fileName.Replace(".json", "");

      string fileContents = "";
      using (var reader = new StreamReader(path))
      {
        fileContents = reader.ReadToEnd();
      }

      var root = JSON.Parse(fileContents);

      configs[modName] = new Dictionary<string, Value>();
      var config = configs[modName];
      
      foreach (var elem in root.Children)
      {
        var key = elem["key"].Value;
        var value = new Value()
        {
          type = elem["value"]["type"].Value,
          value = elem["value"]["value"].Value
        };
        config[key] = value;
      }
    }

    /// <summary>
    /// Saves the configuration of your mod to disk.
    /// </summary>
    public static void Save()
    {
      Save(Assembly.GetCallingAssembly());
    }

    private static void Save(Assembly asm)
    {
      var mod = GetModFromAssembly(asm);
      if (mod == null)
      {
        throw new InvalidOperationException("Cannot save configuration: "
          + "Failed to determine mod");
      }

      var modName = mod.Mod.Name;

      if (!configs.ContainsKey(modName))
      {
        configs[modName] = new Dictionary<string, Value>();
      }

      var config = configs[modName];
      var keys = config.Keys.ToArray();
      var root = new JSONArray();
      for (int i = 0; i < keys.Length; i++)
      {
        var obj = new JSONClass();
        obj["key"] = keys[i];
        obj["value"]["type"] = config[keys[i]].type;
        obj["value"]["value"] = config[keys[i]].value.ToString();
        root[i] = obj;
      }

      var json = root.ToJSON(0);
      var path = Application.dataPath + "/Mods/Config/" + modName + ".json";

      Directory.CreateDirectory(Application.dataPath + "/Mods/Config/");

      using (var writer = new StreamWriter(File.Open(path, FileMode.Create)))
      {
        writer.Write(json);
      }
    }

    private static InternalMod GetModFromAssembly(Assembly asm)
    {
      var mod = Internal.ModLoader.Instance.LoadedMods.
        FirstOrDefault(m => m.AssemblyName == asm.FullName);

      if (mod == default(InternalMod))
        return null;
      else
        return mod;
    }
  }
}
