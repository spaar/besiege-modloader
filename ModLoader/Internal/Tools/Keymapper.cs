using System;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class Keymapper : SingleInstance<Keymapper>
  {
    public override string Name { get; } = "spaar's Mod Loader: Keymapper";

    private bool visible = false;
    private int windowID = Util.GetWindowID();
    private Key key;
    private Rect windowRect = new Rect(500, 300, 412, 600);
    private Vector2 scrollPos = new Vector2(0f, 0f);

    private Key currentKeyToMap = null;
    private bool currentlyModifier = false;

    private GUIStyle textStyle;
    private GUIStyle sectionTitleStyle;
    private GUILayoutOption buttonWidth = GUILayout.Width(110f);

    private void Start()
    {
      ModLoader.MakeModule(this);

      key = Keybindings.AddKeybinding("Keymaper",
        new Key(KeyCode.LeftControl, KeyCode.J));
    }

    private void Update()
    {
      if (key.IsDown())
      {
        visible = !visible;
      }

      if (visible && currentKeyToMap != null)
      {
        if (Input.inputString.Length > 0)
        {
          if (currentlyModifier)
          {
            currentKeyToMap.Modifier = (KeyCode)Enum.Parse(typeof(KeyCode),
              (Input.inputString[0] + "").ToUpper());
          }
          else
          {
            currentKeyToMap.Trigger = (KeyCode)Enum.Parse(typeof(KeyCode),
              (Input.inputString[0] + "").ToUpper());
          }
        }

        KeyCode keyCode = KeyCode.None;
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
          keyCode = KeyCode.LeftControl;
        }
        else if (Input.GetKeyDown(KeyCode.RightControl))
        {
          keyCode = KeyCode.RightControl;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
          keyCode = KeyCode.LeftShift;
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
          keyCode = KeyCode.RightShift;
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
          keyCode = KeyCode.LeftAlt;
        }
        else if (Input.GetKeyDown(KeyCode.RightAlt))
        {
          keyCode = KeyCode.RightAlt;
        }

        if (keyCode != KeyCode.None)
        {
          if (currentlyModifier)
            currentKeyToMap.Modifier = keyCode;
          else
            currentKeyToMap.Trigger = keyCode;
        }
      }
    }

    private void OnGUI()
    {
      if (!visible) return;

      GUI.skin = ModGUI.Skin;
      textStyle = new GUIStyle(Elements.Labels.Title)
      {
        margin = { top = 10 },
        fontSize = 15
      };
      sectionTitleStyle = new GUIStyle(Elements.Labels.Title)
      {
        margin = { top = 10 },
        fontSize = 20,
        richText = true
      };

      windowRect = GUI.Window(windowID, windowRect, DoWindow, "Keymapper");
    }

    private void DoWindow(int id)
    {
      var keybindings = Keybindings.GetAllKeybindings();

      scrollPos = GUILayout.BeginScrollView(scrollPos);

      foreach (var modPair in keybindings)
      {
        DoSectionTitle(modPair.Key);
        foreach (var bindingPair in keybindings[modPair.Key])
        {
          DoKeybinding(modPair.Key, bindingPair.Key, bindingPair.Value);
        }
      }

      GUILayout.EndScrollView();

      if (Event.current.type == EventType.Repaint)
      {
        if (GUI.tooltip == "")
        {
          currentKeyToMap = null;
        }
        else
        {
          var parts = GUI.tooltip.Split(':');
          if (parts.Length != 3)
          {
            currentKeyToMap = null;
          }
          else
          {
            currentKeyToMap = keybindings[parts[0]][parts[1]];
            if (parts[2] == "modifier")
            {
              currentlyModifier = true;
            }
            else
            {
              currentlyModifier = false;
            }
          }
        }
      }

      GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
    }

    private void DoKeybinding(string mod, string name, Key key)
    {
      var tooltip = mod + ":" + name;

      GUILayout.BeginHorizontal();
      GUILayout.Label(name + ":", textStyle);
      GUILayout.FlexibleSpace();
      GUILayout.Button(new GUIContent(
        key.Modifier.ToString(), tooltip + ":modifier"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.Button(new GUIContent(
        key.Trigger.ToString(), tooltip + ":trigger"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.EndHorizontal();
    }

    private void DoSectionTitle(string mod)
    {
      var title = ModLoader.Instance.LoadedMods
        .Find(m => m.Mod.Name == mod).Mod.DisplayName;

      GUILayout.Label("<b>" + title + "</b>", sectionTitleStyle);
    }
  }
}
