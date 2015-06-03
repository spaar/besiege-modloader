using System;
using System.Reflection;

namespace spaar
{
    /// <summary>
    /// Mod is used to mark classes to be loaded as mods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Mod : Attribute
    {
        private string name;
        /// <summary>
        /// Version of the mod, does not have to conform to any specific format.
        /// </summary>
        public string version;
        /// <summary>
        /// Author of the mod
        /// </summary>
        public string author;

        internal Assembly assembly;

        /// <summary>
        /// Initalizes a Mod instance with a default version of 1.0 and no author.
        /// </summary>
        /// <param name="name">Name of the mod</param>
        public Mod(string name)
        {
            this.name = name;
            version = "1.0";
            author = "";
        }

        /// <summary>
        /// Get the name of this Mod.
        /// </summary>
        /// <returns>The name</returns>
        public string Name()
        {
            return name;
        }
    }
}