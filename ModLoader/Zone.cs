using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spaar.ModLoader
{
  public enum Island
  {
    Ipsilon,
    Tolbrynd
  }

  public class Zone
  {
    public int Index { get; private set; }
    public string Name { get; private set; }
    public Island Island { get; private set; }

    public Zone(int index, string name, Island island)
    {
      Index = index;
      Name = name;
      Island = island;
    }

    public override string ToString()
    {
      return string.Format("Zone {0}: {1} on {2}",
        Index, Name, Island);
    }
  }
}
