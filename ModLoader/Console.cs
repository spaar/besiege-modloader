using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#if DEV_BUILD
using System.IO;
#endif

namespace spaar
{
    /// <summary>
    /// The in-game console, showing log output and enabling users to enter commands.
    /// </summary>
    /// <remarks>
    /// The console also writes all log output to the file Mods/Debug/ConsoleOuput.txt for modders.
    /// The main advantage of this is that it is a lot easier to read than output_log.txt and contains less unrelated messages.
    /// </remarks>
    public class Console : MonoBehaviour
    {
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
        private string lastCommand = "";

        private char[] newLine = { '\n', '\r' };

        private bool visible = false;
        private bool interfaceEnabled;
        private Dictionary<LogType, bool> messageFilter;

        /// <summary>
        /// Initializes a new console, with a disabled interface.
        /// </summary>
        /// <seealso cref="EnableInterface"/>
        public Console()
        {
            interfaceEnabled = false;
        }

        private void OnEnable()
        {
            Application.RegisterLogCallback(HandleLog);
            logMessages = new List<string>(maxLogMessages);
            windowRect = new Rect(50f, 50f, 600f, 600f);

            initMessageFiltering();
            registerClear();
        }

        private void initMessageFiltering()
        {
            messageFilter = new Dictionary<LogType, bool>();
            messageFilter.Add(LogType.Assert, true);
            messageFilter.Add(LogType.Error, true);
            messageFilter.Add(LogType.Exception, true);
            messageFilter.Add(LogType.Log, true);
            messageFilter.Add(LogType.Warning, true);

            Commands.RegisterCommand("setMessageFilter", (string[] args, IDictionary<string, string> namedArgs) =>
            {
                foreach (var arg in args)
                {
                    bool val = !arg.StartsWith("!");
                    string key = arg;
                    if (!val) key = arg.Substring(1);
                    try
                    {
                        var type = (LogType)Enum.Parse(typeof(LogType), key);
                        messageFilter[type] = val;
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogError("Not a valid filter setting: " + arg);
                    }
                }
                return "Successfully updated console message filter.";
            }, "Update the filter settings for console messages. Every argument must be in the form 'type' or '!type'. " + 
               "The first form will activate the specified type. The second one will deactive it. " +
               "Vaild values for type are Assert, Error, Exception, Log and Warning.");
        }

        private void registerClear()
        {
            Commands.RegisterCommand("clear", (args, namedArgs) =>
            {
                logMessages = new List<string>(maxLogMessages);
                return "Cleared.";
            }, "Clears the console");
        }

        private void OnDisable()
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

        private void Update()
        {
            if (interfaceEnabled && Input.GetKey(Keys.getKey("Console").Modifier) && Input.GetKeyDown(Keys.getKey("Console").Trigger))
            {
                visible = !visible;
            }
        }

        private void OnGUI()
        {
            if (visible)
            {
                windowRect = GUI.Window(-1001, windowRect, OnWindow, "Console");
            }
        }

        private void OnWindow(int windowId)
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

            bool moveCursor = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
            {
                commandText = lastCommand;
                moveCursor = true;
            }

            string input = GUILayout.TextField(commandText, 100, GUI.skin.textField);

            if (moveCursor)
            {
                TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                editor.pos = commandText.Length + 1;
                editor.selectPos = commandText.Length + 1;
            }
            if (input.IndexOfAny(newLine) != -1)
            {
                commandText = "";
                lastCommand = input.Replace("\n", "").Replace("\r", "");
                Commands.HandleCommand(this, input.Replace("\n", "").Replace("\r", ""));
            }
            else
            {
                commandText = input;
            }

            GUILayout.EndArea();


            GUI.DragWindow();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
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

            AddLogMessage(logMessage, messageFilter[type]);
        }

        /// <summary>
        /// Add a new message to the console. The message will only be printed if printToConsole is true,
        /// but it will always be written to the output file, if in a developer build.
        /// </summary>
        /// <param name="logMessage">The message to add</param>
        /// <param name="printToConsole">Whether to show the message in the console</param>
        internal void AddLogMessage(string logMessage, bool printToConsole = true)
        {
            if (printToConsole)
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
            }

#if DEV_BUILD
            if (tw == null){
                if(!Directory.Exists(Application.dataPath + "/Mods/Debug")){
                    Directory.CreateDirectory(Application.dataPath + "/Mods/Debug");
                }
                tw = new StreamWriter(Application.dataPath + "/Mods/Debug/ConsoleOutput.txt");
            }
            var lines = logMessage.Split('\n');
            foreach (var line in lines)
            {
                tw.WriteLine(line);
            }
#endif

            scrollPosition.y = Mathf.Infinity;
        }
    }
}
