﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace spaar.ModLoader.Injector
{
  public static class Injector
  {

    public static void Inject(AssemblyDefinition game, string outputPath)
    {
      TypeDefinition planetRotate = game.MainModule.GetType("", "PlanetRotateMouse");

      if (planetRotate == null)
      {
        throw new Exception("PlanetRotateMouse not found");
      }

      MethodDefinition planetStart = planetRotate.Methods.FirstOrDefault(
        method => method.Name == "Start");

      if (planetStart == null)
      {
        throw new Exception("PlanetRotateMouse.Start not found");
      }

      var p = planetStart.Body.GetILProcessor();
      var i = p.Body.Instructions;
      // Assembly.LoadFrom(Application.dataPath + "/Mods/SpaarModLoader.dll")
      i.Insert(0, p.Create(OpCodes.Call,
        Util.ImportMethod<Application>(game, "get_dataPath")));
      i.Insert(1, p.Create(OpCodes.Ldstr, "/Mods/SpaarModLoader.dll"));
      i.Insert(2, p.Create(OpCodes.Call,
        Util.ImportMethod<string>(game, "Concat", typeof(string), typeof(string))));
      i.Insert(3, p.Create(OpCodes.Call,
        Util.ImportMethod<Assembly>(game, "LoadFrom", typeof(string))));
      // .GetType("spaar.ModLoader.Internal.Activator()
      i.Insert(4, p.Create(OpCodes.Ldstr, "spaar.ModLoader.Internal.Activator"));
      i.Insert(5, p.Create(OpCodes.Callvirt,
        Util.ImportMethod<Assembly>(game, "GetType", typeof(string))));
      // .GetMethod("Activate")
      i.Insert(6, p.Create(OpCodes.Ldstr, "Activate"));
      i.Insert(7, p.Create(OpCodes.Callvirt,
        Util.ImportMethod<Type>(game, "GetMethod", typeof(string))));
      // .Invoke(null, null);
      i.Insert(8, p.Create(OpCodes.Ldnull));
      i.Insert(9, p.Create(OpCodes.Ldnull));
      i.Insert(10, p.Create(OpCodes.Callvirt,
        Util.ImportMethod<MethodBase>(game, "Invoke", typeof(object), typeof(object[]))));
      i.Insert(11, p.Create(OpCodes.Pop));

      game.Write(outputPath);
    }

  }

  public static class Util
  {
    public static MethodReference ImportMethod<T>(AssemblyDefinition assembly, string name)
    {
      return assembly.MainModule.Import(typeof(T).GetMethod(name, Type.EmptyTypes));
    }

    public static MethodReference ImportMethod<T>(AssemblyDefinition assembly, string name, params Type[] types)
    {
      return assembly.MainModule.Import(typeof(T).GetMethod(name, types));
    }

    public static MethodReference ImportMethod(AssemblyDefinition assembly, string type, string method, params Type[] types)
    {
      TypeReference reference = assembly.MainModule.Types.First(t => t.Name == type);
      return assembly.MainModule.Import(reference.Resolve().Methods.First(m => m.Name == method));
    }
  }
}
