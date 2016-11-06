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
    private Rect windowRect = new Rect(500, 300, 450, 600);
    private Vector2 scrollPos = new Vector2(0f, 0f);

    private Key currentKeyToMap = null;
    private bool currentlyModifier = false;

    private GUIStyle textStyle;
    private GUIStyle sectionTitleStyle;
    private GUILayoutOption buttonWidth = GUILayout.Width(110f);

    private static KeyCode[] SpecialKeys =
    {
      KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftShift,
      KeyCode.RightShift, KeyCode.LeftAlt, KeyCode.RightAlt,
      KeyCode.Backspace, KeyCode.Mouse0, KeyCode.Mouse1,
      KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4,
      KeyCode.Mouse5, KeyCode.Mouse6, KeyCode.Alpha0,
      KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
      KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
      KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
      KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4,
      KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8,
      KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12,
      KeyCode.F13, KeyCode.F14, KeyCode.F15, KeyCode.Keypad0,
      KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3,
      KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6,
      KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9
    };

    private void Start()
    {
      ModLoader.MakeModule(this);

      key = Keybindings.AddKeybinding("Keymaper",
        new Key(KeyCode.LeftControl, KeyCode.J));
    }

    private void Update()
    {
      if (key.Pressed())
      {
        visible = !visible;
      }

      if (visible && currentKeyToMap != null)
      {
        if (Input.inputString.Length > 0
          // Make sure inputString is not the BACKSPACE character (U+0008) since
          // that is a special case down below
          && !Input.inputString.Contains('\u0008' + ""))
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

        foreach (var key in SpecialKeys)
        {
          if (Input.GetKeyDown(key))
          {
            keyCode = key;
            break;
          }
        }

        if (keyCode != KeyCode.None)
        {
          if (currentlyModifier)
          {
            if (keyCode == KeyCode.Backspace)
              currentKeyToMap.Modifier = KeyCode.None;
            else
              currentKeyToMap.Modifier = keyCode;
          }
          else
          {
            if (keyCode == KeyCode.Backspace)
              currentKeyToMap.Trigger = KeyCode.None;
            else
              currentKeyToMap.Trigger = keyCode;
          }
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
      windowRect = Util.PreventOffScreenWindow(windowRect);
    }

    private void DoWindow(int id)
    {
      var keybindings = Keybindings.GetAllKeybindings();

      scrollPos = GUILayout.BeginScrollView(scrollPos);

      foreach (var modPair in keybindings)
      {
        if (DoSectionTitle(modPair.Key))
        {
          foreach (var bindingPair in keybindings[modPair.Key])
          {
            DoKeybinding(modPair.Key, bindingPair.Key, bindingPair.Value);
          }
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

    private bool DoSectionTitle(string mod)
    {
      var internalMod = ModLoader.Instance.LoadedMods
        .Find(m => m.Mod.Name == mod);

      if (internalMod == null) return false;

      var title = internalMod.Mod.DisplayName;

      GUILayout.Label("<b>" + title + "</b>", sectionTitleStyle);

      return true;
    }
  }
}
