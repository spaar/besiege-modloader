﻿using System;
using System.Reflection;

namespace spaar
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Mod : Attribute
    {
        private string name;
        public string version;
        public string author;

        internal Assembly assembly;

        public Mod(string name)
        {
            this.name = name;
            version = "1.0";
            author = "";
        }

        public string Name()
        {
            return name;
        }
    }
}