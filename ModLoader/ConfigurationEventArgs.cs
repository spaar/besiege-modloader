using System;

namespace spaar.ModLoader
{
  public class ConfigurationEventArgs : EventArgs
  {
    private string key;
    private string type;
    private string value;

    public ConfigurationEventArgs(string key, string type, string value)
    {
      this.key = key;
      this.type = type;
      this.value = value;
    }

    public string Key { get { return key; } }
    public string Type { get { return type; } }
    public string Value { get { return value; } }
  }
}
