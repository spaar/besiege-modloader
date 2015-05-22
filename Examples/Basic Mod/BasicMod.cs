using System;
using UnityEngine;

/* 
 * This is a very basic skeleton for a mod, it doesn't actually do anything
 * For more complete examples that show of some useful functions, take a look at the other projects.
*/

namespace Examples
{
    // Some meta data about your mod (required for it to be loaded)
    [spaar.Mod("Example Mod", version="1.0", author="spaar")]
    public class BasicMod : MonoBehaviour
    {
        /*
         * For general documentation about these methods and how you can manipulate the game in them,
         * see http://docs.unity3d.com/ScriptReference/index.html.
         * For things that are specific to the game and not in Unity, currently the best resource is
         * opening Assembly-UnityScript.dll in any .NET decompiler (e.g. JustDecompile).
         * There are also some blogs in the Besiege forums that give a little assistance about the basics,
         * see http://forum.spiderlinggames.co.uk/blogs.
         * You can always ask other modders for assistance as well, we try to be generally helpful.
        */

        public void Start()
        {
            // This will get called when your mod is loaded
        }

        public void Update()
        {
            // This is called every frame
        }

        public void OnGUI()
        {
            // Create your GUI here if you have one
            // (see http://docs.unity3d.com/ScriptReference/GUILayout.html)
        }
    }
}
