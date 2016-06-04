using System.Collections.Generic;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Callback delegate for an option toggle.
  /// </summary>
  /// <param name="active">Whether the toggle is active</param>
  public delegate void OptionsToggle(bool active);

  /// <summary>
  /// OptionsMenu contains methods for adding an option (toggle button) to the
  /// options menu in the main menu of Besiege.
  /// </summary>
  public class OptionsMenu : SingleInstance<OptionsMenu>
  {
    public override string Name { get; } = "spaar's Mod Loader: Options Utility";

    private struct OptionsButton
    {
      public string text;
      public OptionsToggle cb;
      public bool defaultValue;
      public int fontSize;
    }

    private static int numRegistered = 0;

    private static List<OptionsButton> toAdd = new List<OptionsButton>();

    /// <summary>
    /// Registers a new toggle button. It will be placed below all others that
    /// are currently registered.
    /// </summary>
    /// <param name="text">The text to display on the button</param>
    /// <param name="cb">Callback to call when the button is clicked</param>
    /// <param name="defaultValue">Starting state of the toggle</param>
    /// <param name="fontSize">Font size of the text on the button</param>
    public static void RegisterOptionsToggle(string text, OptionsToggle cb,
      bool defaultValue = false, int fontSize = 0)
    {
      var button = new OptionsButton()
      {
        text = text,
        cb = cb,
        defaultValue = defaultValue,
        fontSize = fontSize
      };
      toAdd.Add(button);

      if (Application.loadedLevel == 2)
        RegisterOptionsToggleInternal(button);
    }

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);
    }

    private void OnLevelWasLoaded(int level)
    {
      if (Application.loadedLevel == 1) // Main Menu
      {
        numRegistered = 0;
        foreach (var button in toAdd)
        {
          RegisterOptionsToggleInternal(button);
        }
      }
    }

    private static Transform optionsList;
    private static Transform windowedText;
    private static Transform windowedTickBox;

    private static void RegisterOptionsToggleInternal(OptionsButton button)
    {
      if (optionsList == null)
      {
        GameObject.Find("O P T I O N S")
          .GetComponent<EnableObjOnClick>().OnMouseUp();
        optionsList = GameObject.Find("OPTIONS LIST").transform;
      }

      if (numRegistered == 0)
      {
        // Move Windowed setting up to make room
        windowedText = optionsList.FindChild("WINDOWED");
        windowedText.Translate(0f, 1.3f, 0f);
        windowedTickBox = optionsList.FindChild("WINDOWED TICK BOX");
        windowedTickBox.Translate(0f, 1.3f, 0f);
      }

      var textPos = windowedText.position
        + Vector3.down * 0.45f * (numRegistered + 1);
      var textScale = windowedText.localScale;
      var boxPos = windowedTickBox.position
        + Vector3.down * 0.45f * (numRegistered + 1);
      var boxScale = windowedTickBox.localScale;

      var newText = (Transform)Instantiate(windowedText, textPos,
        Quaternion.identity);
      var newBox = (Transform)Instantiate(windowedTickBox, boxPos,
        Quaternion.identity);

      newText.parent = optionsList;
      newBox.parent = optionsList;

      newText.name = button.text + " (Modded)";
      newBox.name = button.text + "TickBox (Modded)";

      newText.localScale = textScale;
      newBox.localScale = boxScale;

      newText.GetComponent<TextMesh>().text = button.text;
      if (button.fontSize != 0)
        newText.GetComponent<TextMesh>().fontSize = button.fontSize;

      var fullscreenController = newBox.GetComponent<FullScreenController>();
      var optionsComponent = newBox.gameObject.AddComponent<OptionsComponent>();

      optionsComponent.darkMaterial = fullscreenController.darkMaterial;
      optionsComponent.redMaterial = fullscreenController.redMaterial;
      Destroy(fullscreenController);

      optionsComponent.SetCallback(button.cb);
      optionsComponent.SetOn(button.defaultValue);

      numRegistered++;

      optionsList.FindChild("QUIT WINDOW ICON")
        .GetComponent<DisableObjOnClick>().OnMouseUp();
    }
  }
}
