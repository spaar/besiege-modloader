using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

namespace spaar
{
    public class InternalModLoader
    {
        public static bool loadedModLoader;

        public InternalModLoader()
        {
        }

        public void LoadModLoader(GameObject go)
        {
            if (!InternalModLoader.loadedModLoader)
            {
                Assembly assembly = Assembly.LoadFrom(string.Concat(Application.dataPath, "/Mods/SpaarModLoader.dll"));
                go.AddComponent(assembly.GetType("spaar.ModLoader"));
                InternalModLoader.loadedModLoader = true;
            }
        }
    }
}
