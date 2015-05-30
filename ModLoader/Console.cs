using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if DEV_BUILD
using System.IO;
#endif

namespace spaar
{
    public class Console : MonoBehaviour
    {
        
        /// <summary>
        /// Delegate to use with registering commands.
        /// </summary>
        /// <param name="args">The arguments passed on the command line</param>
        /// <returns>Result of the command, will be printed to the console, if not null or empty string</returns>
        public delegate string CommandCallback(string[] args);

        // Simple struct for representing all information needed about a command
        private struct Command
        {
            public Mod mod;
            public CommandCallback callback;
        }

#if DEV_BUILD
        // In a developer build, all console messages are also written to Mods/Debug/ConsoleOutput.txt to assist in debuggin.
        // This is especially useful because the format in output_log.txt is less than ideal for this use-case.
        private TextWriter tw;
#endif

        private List<string> logMessages;
        private readonly int maxLogMessages = 200;

        private Rect windowRect;
        private Vector2 scrollPosition;
        private string commandText = "";

        private char[] newLine = { '\n', '\r' };

        private bool visible = false;
        private bool interfaceEnabled;
        private Dictionary<LogType, bool> messageFilter;

        // Map commands the user can enter to all Command's registered with that name. Mapping to multiple Commands
        // is necessary so that multiple mods can register the same command
        private static Dictionary<string, List<Command>> commands = new Dictionary<string, List<Command>>();
        private static Dictionary<Mod, string> helpMessages = new Dictionary<Mod, string>();

        public Console()
        {
            interfaceEnabled = false;
        }

        void OnEnable()
        {
            Application.RegisterLogCallback(HandleLog);
            logMessages = new List<string>(maxLogMessages);
            windowRect = new Rect(50f, 50f, 600f, 600f);

            initMessageFiltering();
            initHelp();
        }

        private void initMessageFiltering()
        {
            messageFilter = new Dictionary<LogType, bool>();
            messageFilter.Add(LogType.Assert, true);
            messageFilter.Add(LogType.Error, true);
            messageFilter.Add(LogType.Exception, true);
            messageFilter.Add(LogType.Log, true);
            messageFilter.Add(LogType.Warning, true);

            RegisterCommand("setMessageFilter", (string[] args) =>
            {
                foreach (var arg in args)
                {
                    bool val = !arg.StartsWith("!");
                    string key = arg;
                    if (!val) key = arg.Substring(1);
                    try
                    {
                        var type = (LogType)Enum.Parse(typeof(LogType), key);
                        Debug.Log("Setting " + type + " to " + val);
                        messageFilter[type] = val;
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogError("Not a valid filter setting: " + arg);
                    }
                }
                return "Successfully updated console message filter.";
            });
        }

        private void initHelp()
        {
            Console.RegisterCommand("help", (string[] args) =>
            {
                if (args.Length == 0)
                {
                    return @"List of built-in commands: 
setMessageFilter - Filter console messages by type
listMods - List all loaded mods, along with their author and version
version - Prints the current version
help <modname> - Prints help information about the specified mod, if available
help - Prints this help message";
                }
                
                if (ModLoader.LoadedMods.Exists(m => m.Name() == args[0]))
                {
                    return helpMessages[ModLoader.LoadedMods.Find(m => m.Name() == args[0])];
                }
                else
                {
                    return "No help for " + args[0] + " could be found.";
                }
            });
        }

        void OnDisable()
        {
            Application.RegisterLogCallback(null);
#if DEV_BUILD
            if (tw != null)
                tw.Close();
#endif
        }

        /// <summary>
        /// Enables the interface which is disabled by default after creating the Console.
        /// </summary>
        public void EnableInterface()
        {
            interfaceEnabled = true;
        }

        void Update()
        {
            if (interfaceEnabled && Input.GetKey(Keys.getKey("Console").Modifier) && Input.GetKeyDown(Keys.getKey("Console").Trigger))
            {
                visible = !visible;
            }
        }

        void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(-1001, windowRect, OnWindow, "Console");
            }
        }

        void OnWindow(int windowId)
        {
            float lineHeight = GUI.skin.box.lineHeight;

            GUILayout.BeginArea(new Rect(5f, lineHeight + 5f, windowRect.width - 10f, windowRect.height - 30f));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            string logText = "";
            foreach (var s in logMessages)
            {
                logText += s + "\n";
            }
            GUILayout.TextArea(logText);
            GUILayout.EndScrollView();

            string input = GUILayout.TextField(commandText, 100, GUI.skin.textField);

            if (input.IndexOfAny(newLine) != -1)
            {
                HandleCommand(input.Replace("\n", "").Replace("\r", ""));
            }
            else
            {
                commandText = input;
            }

            GUILayout.EndArea();


            GUI.DragWindow();
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!messageFilter[type])
                return;

            var typeString = "[";
            switch (type)
            {
                case LogType.Assert:
                    typeString += "Assert";
                    break;
                case LogType.Error:
                    typeString += "Error";
                    break;
                case LogType.Exception:
                    typeString += "Exception";
                    break;
                case LogType.Log:
                    typeString += "Log";
                    break;
                case LogType.Warning:
                    typeString += "Warning";
                    break;
            }
            typeString += "] ";

            var logMessage = "";
            if (type == LogType.Exception)
            {
                logMessage = typeString + logString + "\n" + stackTrace;
            }
            else
            {
                logMessage = typeString + logString;
            }

            AddLogMessage(logMessage);
        }

        private void AddLogMessage(string logMessage)
        {
            if (logMessages.Count < maxLogMessages)
            {
                logMessages.Add(logMessage);
            }
            else
            {
                logMessages.RemoveAt(0);
                logMessages.Add(logMessage);
            }

#if DEV_BUILD
            if (tw == null)
                tw = new StreamWriter(Application.dataPath + "/Mods/Debug/ConsoleOutput.txt");
            var lines = logMessage.Split('\n');
            foreach (var line in lines)
            {
                tw.WriteLine(line);
            }
#endif

            scrollPosition.y = Mathf.Infinity;
        }

        /// <summary>
        /// Register a console command. The passed callback will be called when the user enters
        /// the command in the console.
        /// </summary>
        /// <param name="command">The command to register</param>
        /// <param name="callback">The callback to be called when the command is entered</param>
        /// <returns>True if registration succeeded, false otherwise</returns>
        public static bool RegisterCommand(string command, CommandCallback callback)
        {
            Command com = new Command();
            com.callback = callback;
            var callingAssembly = Assembly.GetCallingAssembly();
            com.mod = ModLoader.LoadedMods.Find((Mod mod) =>
            {
                return mod.assembly.Equals(callingAssembly);
            });
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

        private void HandleCommand(string input)
        {
            commandText = "";
            var result = "";

            // Input parsing
            var parts = input.Split(' ');
            var command = "";

            if (parts[0].Contains(":"))
                command = parts[0].Split(':')[1];
            else
                command = parts[0];

            // TODO: improve argument parsing
            // possibly some kind of named paramters?
            var args = new List<string>();
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("\""))
                {
                    var currentArg = parts[i].Substring(1);
                    i++;
                    while (!parts[i].EndsWith("\""))
                    {
                        currentArg += " " + parts[i];
                        i++;
                    }
                    currentArg += " " + parts[i].Substring(0, parts[i].Length - 1);
                    args.Add(currentArg);
                }
                else
                {
                    args.Add(parts[i]);
                }
            }
            
            AddLogMessage("[Command] >" + input);

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
                        result = commands[command].Find((Command c) => { return c.mod.Name() == modname; }).callback(args.ToArray());
                    }
                }
                else
                {
                    result = commands[command][0].callback(args.ToArray());
                }
            }
            else
            {
                result = "No such command: " + command;
            }

            if (result != null && result != "")
                AddLogMessage("[Command] " + result);
        }
    }
}
