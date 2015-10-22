using System;

namespace spaar.ModLoader
{
  /// <summary>
  /// EventArgs for the OnConfigurationChange event.
  /// </summary>
  public class ConfigurationEventArgs : EventArgs
  {
    private string key;
    private string type;
    private string value;

    /// <summary>
    /// Initializes a new instance of the ConfigurationEventArgs class.
    /// As a mod author, you should not need to call this yourself.
    /// </summary>
    /// <param name="key">Key that was changed</param>
    /// <param name="type">Type of new value</param>
    /// <param name="value">New value</param>
    public ConfigurationEventArgs(string key, string type, string value)
    {
      this.key = key;
      this.type = type;
      this.value = value;
    }

    /// <summary>
    /// The key that was modified.
    /// </summary>
    public string Key { get { return key; } }
    /// <summary>
    /// Type of the value that was set.
    /// </summary>
    public string Type { get { return type; } }
    /// <summary>
    /// Value that was set.
    /// </summary>
    public string Value { get { return value; } }
  }
}
