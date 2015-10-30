namespace spaar.ModLoader
{
  /// <summary>
  /// Represents an in-game island. The sandbox is counted as a seperate island.
  /// </summary>
  public enum Island
  {
    Sandbox,
    Ipsilon,
    Tolbrynd
  }

  /// <summary>
  /// Represents a zone, which is an in-game level.
  /// </summary>
  public class Zone
  {
    public int Index { get; private set; }
    public int LevelIndex { get; private set; }
    public string Name { get; private set; }
    public Island Island { get; private set; }

    public bool IsCompleted {
      get
      {
        if (Island == Island.Sandbox) return false;

        return Game.CompletedLevels[Index - 1];
      }
    }

    public Zone(int index, int levelIndex, string name, Island island)
    {
      Index = index;
      LevelIndex = levelIndex;
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
