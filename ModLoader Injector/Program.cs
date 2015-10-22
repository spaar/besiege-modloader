using System;
using Mono.Cecil;

namespace spaar.ModLoader.Injector
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Assembly-UnityScript.dll location:");
      string pathUnityScript = Console.ReadLine();

      Console.WriteLine("Output path:");
      string pathOutput = Console.ReadLine();

      Console.WriteLine("Using Assembly-UnityScript.dll at " + pathUnityScript
        + " and writing to " + pathOutput);

      AssemblyDefinition aUnityScript
        = AssemblyDefinition.ReadAssembly(pathUnityScript);

      Injector.Inject(aUnityScript, pathOutput);

      Console.WriteLine("Done.");
      Console.Read();
    }
  }
}
