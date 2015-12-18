using System;
using System.Collections.Generic;
using spaar.ModLoader.UI;
using UnityEngine;

#if DEV_BUILD
using System.IO;
#endif

namespace spaar.ModLoader.Internal.Tools
{
  /// <summary>
  /// The in-game console, showing log output and enabling users to enter commands.
  /// </summary>
  /// <remarks>
  /// The console also writes all log output to the file Mods/Debug/ConsoleOuput.txt for modders.
  /// The main advantage of this is that it is a lot easier to read than output_log.txt and contains less unrelated messages.
  /// </remarks>
  public class Console : SingleInstance<Console>
  {
    public override string Name { get { return "spaar's Mod Loader: Console"; } }

    private List<LogEntry> entries;

    private Rect windowRect;
    private int windowID = Util.GetWindowID();
    private Vector2 scrollPosition;

    private string commandText = "";
    private string nonCompletedText = "";
    private bool doingCompletion = false;
    private int historyIndex = 0;

    private bool visible = false;
    private Key key;
    private bool interfaceEnabled;
    private Dictionary<LogType, bool> messageFilter;

    private int maxMessages = 250;

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
      ModLoader.MakeModule(this);

      Application.logMessageReceived += HandleLog;
      entries = new List<LogEntry>();
      windowRect = new Rect(50f, 50f, Elements.Settings.ConsoleSize.x,
        Elements.Settings.ConsoleSize.y);

      initMessageFiltering();
      registerClear();

      key = Keybindings.AddKeybinding("Console",
        new Key(KeyCode.LeftControl, KeyCode.K));
    }

    private void initMessageFiltering()
    {
      messageFilter = new Dictionary<LogType, bool>()
      {
        { LogType.Assert, true },
        { LogType.Error, true },
        { LogType.Exception, true },
        { LogType.Log, true },
        { LogType.Warning, true }
      };

      Commands.RegisterCommand("setMessageFilter", (args, namedArgs) =>
      {
        foreach (var arg in args)
        {
          bool val = !arg.StartsWith("!", StringComparison.Ordinal);
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
      }, "Update the filter settings for console messages. "
         + "Every argument must be in the form 'type' or '!type'. "
         + "The first form will activate the specified type. "
         + "The second one will deactive it. "
         + "Valid values for type are Assert, Error, Exception, Log and Warning.");
    }

    private void registerClear()
    {
      Commands.RegisterCommand("clear", (args, namedArgs) =>
      {
        entries.Clear();
        return "";
      }, "Clears the console");
    }

    private void OnDisable()
    {
      Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Enables the interface which is disabled by default after creating the Console.
    /// </summary>
    public void EnableInterface()
    {
      interfaceEnabled = true;

      // Load configuration values here as well
      maxMessages = Configuration.GetInt("maxConsoleMessages", 250);
      Configuration.OnConfigurationChange += OnConfigurationChange;
    }

    private void OnConfigurationChange(object s, ConfigurationEventArgs e)
    {
      maxMessages = Configuration.GetInt("maxConsoleMessages", 250);
    }

    private void Update()
    {
      if (interfaceEnabled && key.Pressed())
      {
        visible = !visible;
      }
    }

    private void OnGUI()
    {
      GUI.skin = ModGUI.Skin;

      if (visible)
      {
        windowRect = GUI.Window(windowID, windowRect, OnWindow, "Console");
        windowRect = Util.PreventOffScreenWindow(windowRect);
      }
    }

    // In case of auto-completion, cursor has to be moved the first time
    // _after_ the text-box was refocused, it doesn't work in the same frame.
    bool moveCursorNextFrame = false;
    bool skippedFrame = false;
    private void OnWindow(int windowId)
    {
      float lineHeight = GUI.skin.box.lineHeight;

      scrollPosition = GUILayout.BeginScrollView(scrollPosition);

      foreach (var entry in entries)
      {
        if (messageFilter[entry.type])
          DoEntry(entry);
      }

      GUILayout.EndScrollView();

      // Here comes the input box, along with command handling to a certain extent
      // and auto-completion.
      // TODO: Rework this section, it's rather more complex and messy than it
      // should be.

      bool refocus = false;
      if (Event.current.type == EventType.KeyUp)
      {
        if (Event.current.keyCode == KeyCode.UpArrow)
        {
          if (historyIndex > 0)
          {
            historyIndex--;
          }
          commandText = Commands.History[historyIndex];
          moveCursorNextFrame = true;
          Commands.AutoCompleteReset();
        }
        else if (Event.current.keyCode == KeyCode.DownArrow)
        {
          if (historyIndex < Commands.History.Count - 1)
          {
            historyIndex++;
            commandText = Commands.History[historyIndex];
          }
          else if (historyIndex == Commands.History.Count - 1)
          {
            historyIndex++;
            commandText = "";
          }
          moveCursorNextFrame = true;
          Commands.AutoCompleteReset();
        }
        else if (Event.current.keyCode == KeyCode.Tab)
        {
          if (!doingCompletion)
          {
            nonCompletedText = commandText;
            doingCompletion = true;
          }
          commandText = Commands.AutoCompleteNext(nonCompletedText);
          moveCursorNextFrame = true;
          refocus = true;
        }
        else
        {
          doingCompletion = false;
          Commands.AutoCompleteReset();
        }
      }

      GUI.SetNextControlName("ConsoleInput");
      string input = GUILayout.TextField(commandText, 100, GUI.skin.textField);
      if (moveCursorNextFrame)
      {
        if (!skippedFrame)
        {
          skippedFrame = true;
        }
        else
        {
          var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor),
            GUIUtility.keyboardControl);
          editor.pos = commandText.Length + 1;
          editor.selectPos = commandText.Length + 1;
          moveCursorNextFrame = false;
          skippedFrame = false;
          GUI.FocusControl("ConsoleInput");
        }
      }

      if (refocus)
      {
        GUI.FocusControl("ConsoleInput");
      }

      if (input.IndexOfAny(new char[] { '\n', '\r' }) != -1)
      {
        commandText = "";
        Commands.HandleCommand(input.Replace("\n", "").Replace("\r", ""));
        historyIndex++;
        moveCursorNextFrame = true;
      }
      else
      {
        commandText = input;
      }

      GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
    }

    private void DoEntry(LogEntry entry)
    {
      GUILayout.BeginHorizontal();

      if (entry.trace == null || entry.trace == "")
      {
        Elements.Tools.DoCollapseArrow(false, false);
      }
      else
      {
        if (Elements.Tools.DoCollapseArrow(entry.IsExpanded))
        {
          entry.IsExpanded = !entry.IsExpanded;
        }
      }

      var style = new GUIStyle(Elements.Labels.LogEntry)
      {
        normal = { textColor = entry.Color }
      };

      var lineCount = entry.log.Split('\n').Length;

      GUILayout.TextField(entry.log, style,
        GUILayout.Height(Elements.Settings.LogEntrySize * lineCount));

      GUILayout.EndHorizontal();

      // If the entry is expanded, show the stack trace
      if (entry.IsExpanded)
      {
        GUILayout.BeginHorizontal();

        var traceStyle = GUI.skin.label;
        traceStyle.normal.textColor = new Color(.9f, .9f, .9f);

        // Move trace text to the right to match expansion arrows // TODO: Magic value
        GUILayout.Space(Elements.Settings.LogEntrySize + 8);
        GUILayout.TextArea(entry.trace, traceStyle);

        GUILayout.EndHorizontal();
      }
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
      entries.Add(new LogEntry(type, logString, stackTrace));
      if (entries.Count > maxMessages)
      {
        entries.RemoveAt(0);
      }
      scrollPosition.y = float.PositiveInfinity; // Auto-scrolling

#if DEV_BUILD
      DebugServer.Instance.SendLogEntry(logString + "\n" + stackTrace);
#endif
    }

#if DEV_BUILD
    private void OnApplicationQuit()
    {
      if (!Directory.Exists(Application.dataPath + "/Mods/Debug"))
      {
        Directory.CreateDirectory(Application.dataPath + "/Mods/Debug");
      }
      var tw = new StreamWriter(
        Application.dataPath + "/Mods/Debug/ConsoleOutput.txt");

      var logText = "";
      foreach (var entry in entries)
      {
        logText += entry.ToString() + "\n";
      }

      tw.Write(logText);
      tw.Close();
    }
#endif
  }
}
