using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class Keymapper : SingleInstance<Keymapper>
  {
    public override string Name { get; } = "spaar's Mod Loader: Keymapper";

    private bool visible = false;
    private int windowID = Util.GetWindowID();
    private Rect windowRect = new Rect(500, 300, 400, 207);

    private string currentKeyToMap = "";

    private void Start()
    {
      ModLoader.MakeModule(this);
    }

    private void Update()
    {
      if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.J))
      {
        visible = !visible;
      }

      if (visible && currentKeyToMap != "")
      {
        if (Input.inputString.Length > 0)
        {
          Configuration.SetString(currentKeyToMap,
            (Input.inputString[0] + "").ToUpper());
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
          Configuration.SetString(currentKeyToMap, "LeftControl");
        }
        else if (Input.GetKeyDown(KeyCode.RightControl))
        {
          Configuration.SetString(currentKeyToMap, "RightControl");
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
          Configuration.SetString(currentKeyToMap, "LeftShift");
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
          Configuration.SetString(currentKeyToMap, "RightShift");
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
          Configuration.SetString(currentKeyToMap, "LeftAlt");
        }
        else if (Input.GetKeyDown(KeyCode.RightAlt))
        {
          Configuration.SetString(currentKeyToMap, "RightAlt");
        }
      }
    }

    private void OnGUI()
    {
      if (!visible) return;

      GUI.skin = ModGUI.Skin;

      windowRect = GUI.Window(windowID, windowRect, DoWindow, "Keymapper");
    }

    private void DoWindow(int id)
    {
      var textStyle = new GUIStyle(Elements.Labels.Title)
      {
        margin = { top = 10 },
        fontSize = 15
      };
      var buttonWidth = GUILayout.Width(110f);

      GUILayout.BeginHorizontal();
      GUILayout.Label("Console:", textStyle);
      GUILayout.FlexibleSpace();
      GUILayout.Button(new GUIContent(
          Keys.getKey("Console").Modifier.ToString(), "consoleK1"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.Button(new GUIContent(
        Keys.getKey("Console").Trigger.ToString(), "consoleK2"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.EndHorizontal();

#if DEV_BUILD
      GUILayout.BeginHorizontal();
      GUILayout.Label("Object Explorer:", textStyle);
      GUILayout.FlexibleSpace();
      GUILayout.Button(new GUIContent(
        Keys.getKey("ObjectExplorer").Modifier.ToString(), "objExpK1"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.Button(new GUIContent(
        Keys.getKey("ObjectExplorer").Trigger.ToString(), "objExpK2"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.EndHorizontal();
#endif

      GUILayout.BeginHorizontal();
      GUILayout.Label("Mod Toggle:", textStyle);
      GUILayout.FlexibleSpace();
      GUILayout.Button(new GUIContent(
        Keys.getKey("ModToggle").Modifier.ToString(), "modToggleK1"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.Button(new GUIContent(
        Keys.getKey("ModToggle").Trigger.ToString(), "modToggleK2"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Keymapper:", textStyle);
      GUILayout.FlexibleSpace();
      GUILayout.Button(new GUIContent(
        Keys.getKey("Keymapper").Modifier.ToString(), "keymapK1"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.Button(new GUIContent(
        Keys.getKey("Keymapper").Trigger.ToString(), "keymapK2"),
        Elements.Buttons.Red, buttonWidth);
      GUILayout.EndHorizontal();

      if (Event.current.type == EventType.Repaint) 
        currentKeyToMap = GUI.tooltip;

      GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
    }
  }
}
