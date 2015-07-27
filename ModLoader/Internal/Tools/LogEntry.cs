using System;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class LogEntry
  {
    public readonly LogType type;
    public readonly string log;
    public readonly string trace;

    public bool IsExpanded { get; set; }

    public LogEntry(LogType type, string log, string trace)
    {
      this.type = type;
      this.log = log;
      this.trace = trace.TrimEnd(Environment.NewLine.ToCharArray());

      IsExpanded = false;
    }

    public Color Color
    {
      get
      {
        switch (type)
        {
          case LogType.Log: return Elements.Colors.LogNormal;
          case LogType.Assert: return Elements.Colors.LogAssert;
          case LogType.Error: return Elements.Colors.LogError;
          case LogType.Exception: return Elements.Colors.LogException;
          case LogType.Warning: return Elements.Colors.LogWarning;

          default: throw new InvalidOperationException("Invalid log type!");
        }
      }
    }

    public override string ToString()
    {
      var str = "[" + type.ToString() + "] " + log;
      if (trace != "")
      {
        str += "\n" + trace;
      }
      return str;
    }
  }
}
