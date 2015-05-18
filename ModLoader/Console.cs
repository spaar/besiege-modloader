using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if DEV_BUILD
using System.IO;
#endif

namespace spaar
{
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

        private bool visible = false;

        void OnEnable()
        {
            Application.RegisterLogCallback(HandleLog);
            logMessages = new List<string>(maxLogMessages);
            windowRect = new Rect(50f, 50f, 600f, 600f);
        }

        void OnDisable()
        {
            Application.RegisterLogCallback(null);
#if DEV_BUILD
            if (tw != null)
                tw.Close();
#endif
        }

        void Update()
        {
            if (Input.GetKey(KeyGetter.getKey("ConsoleK").Modifier) &&
                Input.GetKeyDown(KeyGetter.getKey("ConsoleK").Trigger))
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

            GUILayout.BeginArea(new Rect(5f, lineHeight + 5f, windowRect.width - 10f, windowRect.height - 50f));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            string logText = "";
            foreach (var s in logMessages)
            {
                logText += s + "\n";
            }
            GUILayout.TextArea(logText);
            GUILayout.EndScrollView();

            GUILayout.TextField("Not yet implemented");

            GUILayout.EndArea();


            GUI.DragWindow();
        }

        void HandleLog(string logString, string stackTrace, LogType type)
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

    }
}
