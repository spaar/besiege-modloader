using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;

namespace spaar.injector
{
    class Injector
    {
        private string asUnityScriptPath;
        private AssemblyDefinition asUnityScriptDef;

        private string asModLoaderPath;
        private AssemblyDefinition asModLoaderDef;

        public Injector(string asUnityScript, string asModLoader)
        {
            asUnityScriptPath = asUnityScript;
            asUnityScriptDef = AssemblyDefinition.ReadAssembly(asUnityScript);

            asModLoaderPath = asModLoader;
            asModLoaderDef = AssemblyDefinition.ReadAssembly(asModLoader);
        }

        public void InjectModLoader()
        {
            TypeDefinition internalModLoader = asModLoaderDef.MainModule.GetType("spaar.InternalModLoader");
            TypeDefinition internalModLoaderCopy = new TypeDefinition("spaar", "InternalModLoader", internalModLoader.Attributes, internalModLoader.BaseType);
            foreach (var field in internalModLoader.Fields)
            {
                FieldDefinition fieldCopy = new FieldDefinition(field.Name, field.Attributes, field.FieldType);
                internalModLoaderCopy.Fields.Add(fieldCopy);
            }
            foreach (var method in internalModLoader.Methods)
            {
                MethodDefinition methodCopy = new MethodDefinition(method.Name, method.Attributes, method.ReturnType);
                methodCopy.Body = method.Body;
                internalModLoaderCopy.Methods.Add(methodCopy);
            }
            asUnityScriptDef.MainModule.Types.Add(internalModLoaderCopy);

            asUnityScriptDef.MainModule.Import(typeof(object).GetConstructor(new Type[0]));
            asUnityScriptDef.Write(asUnityScriptPath + ".modloader");
        }
    }
}
