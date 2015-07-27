namespace spaar.ModLoader.Internal
{
  public static class Activator
  {
    private static bool activated = false;

    public static void Activate()
    {
      if (!activated)
      {
        ModLoader.Initialize();
        activated = true;
      }
    }
  }
}
