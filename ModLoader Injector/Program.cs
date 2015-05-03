using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using spaar.injector;

namespace ModLoader_Injector
{
    class Program
    {
        static void Main(string[] args)
        {
            Injector injector = new Injector(args[0], args[1]);
            injector.InjectModLoader();
        }
    }
}
