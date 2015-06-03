using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace spaar
{        
    /// <summary>
    /// Delegate to use with registering commands.
    /// </summary>
    /// <param name="args">The unnamed arguments passed on the command line, in the order they appear</param>
    /// <param name="namedArgs">The named arguments passed on the command line</param>
    /// <returns>Result of the command, will be printed to the console, if not null or empty string</returns>
    public delegate string CommandCallback(string[] args, IDictionary<string, string> namedArgs);

    /// <summary>
    /// Commands is used for all things regarding commands. Mainly, you can register your own commands and help messages here.
    /// </summary>
    /// <remarks>
    /// Commands can be entered in the console by the user. The command can be entered by itself if it is only registered once,
    /// if multiple mods registered the same command they are differentiated by entering <![CDATA[<modname>:<command>]]>.
    /// Commands can take two types of arguments: named arguments and unnamed arguments. Unnamed arguments are simply values entered
    /// on the console after the command, seperated by spaces. Named arguments follow this format: <c>--name value</c>.
    /// </remarks>
    /// <example>
    /// <code>
    ///   Commands.RegisterCommand("myCommand", myCallback);
    /// </code>
    /// Registers a new command called myCommand. This can then be used like this:
    /// <code>
    ///   myCommand arg1 "second unnamed arg" --namedArg1 value1 --namedArg2 "second value"
    /// </code>
    /// If called as above, <c>args</c> passed to the callback will contain <c>["arg1", "second unnamed arg"]</c> and namedArgs
    /// will contain the following mappings: <c>"namedArg1" -> "value1", "namedArg2" -> "second value"</c>.
    /// </example>
    public static class Commands
    {
        // Simple struct for representing all information needed about a command
        private struct Command
        {
            public Mod mod;
            public CommandCallback callback;
            public string helpMessage;
        }
        
        // Map commands the user can enter to all Command's registered with that name. Mapping to multiple Commands
        // is necessary so that multiple mods can register the same command
        private static Dictionary<string, List<Command>> commands = new Dictionary<string, List<Command>>();
        private static Dictionary<Mod, string> helpMessages = new Dictionary<Mod, string>();

        /// <summary>
        /// Registers the general built-in commands.
        /// Specifically help and version.
        /// </summary>
        internal static void init()
        {
            initHelp();
            RegisterCommand("version", (args, namedArgs) => { return "spaar's Mod Loader version 0.2.2, Besiege v0.09"; });
            RegisterCommand("clear", (args, namedArgs) => { return GameObject.Find("MODLOADERLORD").GetComponent<Console>().clearConsole(args, namedArgs); }, "Clears the console");
        }

        /// <summary>
        /// Registers the built-in help command.
        /// </summary>
        private static void initHelp()
        {
            RegisterCommand("help", (string[] args, IDictionary<string, string> namedArgs) =>
            {
                if (args.Length == 0)
                {
                    return @"List of built-in commands: 
setMessageFilter - Filter console messages by type
listMods - List all loaded mods, along with their author and version
version - Prints the current version
help <modname> - Prints help information about the specified mod, if available
help <modname> <command> or help <modname>:<command> - Prints help information about the specified command, if available
help - Prints this help message
clear - Clears the Console";
                }
                else if (args.Length == 1 && !args[0].Contains(":"))
                {

                    if (ModLoader.LoadedMods.Exists(m => m.Name() == args[0]))
                    {
                        var mod = ModLoader.LoadedMods.Find(m => m.Name() == args[0]);
                        if (helpMessages.ContainsKey(mod))
                        {
                            return helpMessages[mod];
                        }
                        else
                        {
                            return "No help for " + args[0] + " could be found.";
                        }
                    }
                    else
                    {
                        return "No mod named " + args[0] + " could be found.";
                    }
                }
                else if (args.Length == 2 || args[0].Contains(":"))
                {
                    var modName = "";
                    var commandName = "";
                    if (args[0].Contains(":"))
                    {
                        modName = args[0].Split(':')[0];
                        commandName = args[0].Split(':')[1];
                    }
                    else
                    {
                        modName = args[0];
                        commandName = args[1];
                    }
                    if (!commands.ContainsKey(commandName))
                    {
                        return "No such command: " + commandName;
                    }
                    var coms = commands[commandName];
                    if (!coms.Exists(com => com.mod.Name() == modName))
                    {
                        return "No command " + commandName + " in mod " + modName;
                    }
                    var helpMessage = coms.Find(com => com.mod.Name() == modName).helpMessage;
                    if (helpMessage == "")
                    {
                        return "No help registered for " + modName + ":" + commandName;
                    }
                    return helpMessage;
                }
                return "Usage: 'help' or 'help <mod>' or 'help <mod> <command>' or 'help <mod>:<command>'";
            });
        }

        /// <summary>
        /// Register a console command. The passed callback will be called when the user enters the command in the console.
        /// </summary>
        /// <param name="command">The command to register</param>
        /// <param name="callback">The callback to be called when the command is entered</param>
        /// <param name="helpText">Help text for this command. Used for <c><![CDATA[help <mod> <command>]]></c></param>
        /// <returns>True if registration succeeded, false otherwise</returns>
        public static bool RegisterCommand(string command, CommandCallback callback, string helpText = "")
        {
            Command com = new Command();
            com.callback = callback;
            com.helpMessage = helpText;
            var callingAssembly = Assembly.GetCallingAssembly();
            com.mod = ModLoader.LoadedMods.Find(mod => mod.assembly.Equals(callingAssembly));
            if (com.mod == null)
            {
                Debug.LogError("Could not identify mod trying to register command " + command + "!");
                return false;
            }
            if (commands.ContainsKey(command))
            {
                commands[command].Add(com);
            }
            else
            {
                List<Command> newList = new List<Command>();
                newList.Add(com);
                commands.Add(command, newList);
            }
            return true;
        }

        /// <summary>
        /// Register a help message for your mod to be displayed with the 'help' command.
        /// This should give a general overview, to give specific information about a command,
        /// use the <c>helpText</c> parameter of <c>RegisterCommand</c>.
        /// </summary>
        /// <param name="message">The help message</param>
        public static void RegisterHelpMessage(string message)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var mod = ModLoader.LoadedMods.Find((Mod m) =>
            {
                return m.assembly.Equals(callingAssembly);
            });

            helpMessages[mod] = message;
        }

        /// <summary>
        /// Parse the next argument from parts, starting at i. This respects quotes, so that arguments can contain spaces.
        /// </summary>
        /// <param name="parts">The whole argument string, split by spaces</param>
        /// <param name="i">The index where the argument to get starts</param>
        /// <param name="newI">The first index after the argument</param>
        /// <returns>The parsed argument</returns>
        private static string GetArgument(string[] parts, int i, out int newI)
        {
            if (parts[i].StartsWith("\""))
            {
                var currentArg = parts[i].Substring(1);
                if (parts[i].EndsWith("\""))
                {
                    currentArg = currentArg.Substring(0, currentArg.Length - 1);
                }
                else
                {
                    i++;
                    while (!parts[i].EndsWith("\""))
                    {
                        currentArg += " " + parts[i];
                        i++;
                    }
                    currentArg += " " + parts[i].Substring(0, parts[i].Length - 1);
                }
                newI = i;
                return currentArg;
            }
            else
            {
                newI = i;
                return parts[i];
            }  
        }

        /// <summary>
        /// Handle an entered command. Arguments are parsed and then the callback corresponding to the command is called.
        /// </summary>
        /// <param name="console">Console to log the command and the return value to</param>
        /// <param name="input">Complete command line</param>
        internal static void HandleCommand(Console console, string input)
        {
            var result = "";

            // Input parsing
            var parts = input.Split(' ');
            var command = "";

            if (parts[0].Contains(":"))
                command = parts[0].Split(':')[1];
            else
                command = parts[0];

            var args = new List<string>();
            var namedArgs = new Dictionary<string, string>();
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("--"))
                {
                    // Named arg
                    var name = parts[i].Substring(2);
                    var value = "";
                    i++;
                    value = GetArgument(parts, i, out i);
                    namedArgs.Add(name, value);
                }
                else
                {
                    // Unnamed arg
                    args.Add(GetArgument(parts, i, out i));
                }
            }
            
            console.AddLogMessage("[Command] >" + input);

            if (commands.ContainsKey(command))
            {
                if (commands[command].Count > 1)
                {
                    if (!parts[0].Contains(":"))
                    {
                        result = "Error: Multiple mods have registered " + command + ", use <modname>:" + command + " to specify which one to use.\n"
                            + "Mods that provide command " + command + ": ";
                        foreach (var c in commands[command])
                            result += "\n" + c.mod.Name();
                    }
                    else
                    {
                        var modname = parts[0].Split(':')[0];
                        result = commands[command].Find(c => c.mod.Name().Equals(modname, StringComparison.CurrentCultureIgnoreCase)).callback(args.ToArray(), namedArgs);
                    }
                }
                else
                {
                    result = commands[command][0].callback(args.ToArray(), namedArgs);
                }
            }
            else
            {
                result = "No such command: " + command;
            }

            if (result != null && result != "")
                console.AddLogMessage("[Command] " + result);
        }
    }
}
