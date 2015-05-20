using System;

namespace spaar
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field)]
    internal class CommandAttribute : Attribute
    {
        private readonly string commandName;
        private readonly bool isConstructor;
        private readonly bool hasParams;
        private readonly string WhatToDo;
        private bool isMethod;
        private string modname;
        private string classname;
        private string MethodOrFieldname;

        /// <summary>
        ///     Attribute for creating console command. WhatToDo needs a key value "call/change", then the Modname with .dll, then the classname, then
        ///     the Method to call or field to change.
        ///     Separated by ","
        ///     The constructor has to be non-static, Methods can be static or non-static.
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="WhatToDo"></param>
        /// <param name="Constructor"></param>
        /// <param name="has the method or constructor params"></param>
        public CommandAttribute(string CommandName, string WhatToDo, bool constructor, bool hasParams)
        {
            commandName = CommandName;
            this.WhatToDo = WhatToDo;
            isConstructor = constructor;
            this.hasParams = hasParams;
            whatToDo();
        }

        public string CommandName()
        {
            return commandName;
        }

        public void whatToDo()
        {
            String[] key = WhatToDo.ToLower().Split(","[1]);
            if (key[0].Equals("call,"))
            {
                isMethod = true;
            }
            else if (key[0].Equals("change,"))
            {
                isMethod = false;
            }
            else
            {
                throw new ArgumentException("No such key value" + key[1].TrimEnd(new char[',']));
            }
            modname = key[1].TrimEnd(new char[',']);
            classname = key[2].TrimEnd(new char[',']);
            MethodOrFieldname = key[3].TrimEnd(new char[',']);
        }

        public bool Constructor()
        {
            return isConstructor;
        }

        public String Modname()
        {
            return modname;
        }

        public String Classname()
        {
            return classname;
        }

        public String MethodOrFieldName()
        {
            return MethodOrFieldname;
        }

        public bool Params()
        {
            return hasParams;
        }

    }
}