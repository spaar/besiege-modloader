using System;
using System.Collections.Generic;
using System.Reflection;

namespace spaar
{
    public static class Commands
    {
        public static Dictionary<String, CommandClass> commands = new Dictionary<string, CommandClass>();

        /// <summary>
        /// New Command for method
        /// </summary>
        /// <param name="Commandname"></param>
        /// <param name="Modname"></param>
        /// <param name="Classname"></param>
        /// <param name="MethodName"></param>
        public static void addCommand(string Commandname, string Modname, string Classname, string MethodName, bool hasParams)
        {
            commands[Commandname] = new CommandClass(Commandname, Modname, Classname, MethodName, hasParams);
        }
        /// <summary>
        /// New command for Field
        /// </summary>
        /// <param name="Commandname"></param>
        /// <param name="Modname"></param>
        /// <param name="Classname"></param>
        /// <param name="Fieldname"></param>
        /// <param name="null"></param>
        public static void addCommand(String Commandname, String Modname, String Classname, String Fieldname,
            String JustWriteNull)
        {
            commands[Commandname] = new CommandClass(Commandname, Modname, Classname, Fieldname, null);
        }
        /// <summary>
        /// New command for constructor
        /// </summary>
        /// <param name="Commandname"></param>
        /// <param name="Modname"></param>
        /// <param name="Classname"></param>
        public static void addCommand(string Commandname, string Modname, string Classname, bool hasParams)
        {
            commands[Commandname] = new CommandClass(Commandname, Modname, Classname, hasParams);
        }

        public static void runCommand(String Commandname, String input, String param)
        {
            if (commands.ContainsKey(Commandname))
            {
                CommandClass cc = commands[Commandname];
                Assembly ass = Assembly.LoadFrom(cc.Modname);
                Type t = ass.GetType(cc.Classname);
                if (cc.isConstructor)
                {
                    ConstructorInfo[] ci = t.GetConstructors();
                    ci.Initialize();
                }
                else if (cc.isMethod)
                {
                    MethodBase mb = t.GetMethod(cc.MethodName);
                    if (cc.hasParams)
                    {
                        ParameterInfo[] pi = mb.GetParameters();

                    }
                }
            }
        }

    }

    public class CommandClass
    {
        public string Classname;
        public string commandname;
        public string Fieldname;
        public bool isConstructor;
        public bool isMethod;
        public string MethodName;
        public string Modname;
        public bool hasParams;

        /// <summary>
        ///     New Command with Method
        /// </summary>
        public CommandClass(string Commandname, string Modname, string Classname, string MethodName, bool hasParams)
        {
            commandname = Commandname;
            this.Modname = Modname;
            this.Classname = Classname;
            isConstructor = false;
            isMethod = true;
            this.MethodName = MethodName;
            this.hasParams = hasParams;
        }

        /// <summary>
        ///     New command with field
        /// </summary>
        /// <param name="Commandname"></param>
        /// <param name="Modname"></param>
        /// <param name="Classname"></param>
        /// <param name="Fieldname"></param>
        /// <param name="null"></param>
        public CommandClass(string Commandname, string Modname, string Classname, string Fieldname,
            string JustWriteNull)
        {
            commandname = Commandname;
            this.Modname = Modname;
            this.Classname = Classname;
            isConstructor = false;
            isMethod = false;
            this.Fieldname = Fieldname;
        }

        /// <summary>
        ///     New command with constructor
        /// </summary>
        /// <param name="Commandname"></param>
        /// <param name="Modname"></param>
        /// <param name="Classname"></param>
        public CommandClass(string Commandname, string Modname, string Classname, bool hasParams)
        {
            commandname = Commandname;
            this.Modname = Modname;
            this.Classname = Classname;
            isConstructor = true;
            this.hasParams = hasParams;
        }
    }
}