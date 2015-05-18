using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaar
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Mod : Attribute
    {
        private string name;
        public string version;
        public string author;

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