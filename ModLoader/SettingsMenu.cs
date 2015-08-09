using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Callback delegate for the default toggle setting.
  /// </summary>
  /// <param name="active">Whether the toggle is active</param>
  public delegate void SettingsToggle(bool active);

  /// <summary>
  /// SettingsMenu contains methods for adding a setting (toggle button) to the
  /// settings drop-down of Besiege.
  /// </summary>
  public class SettingsMenu : SingleInstance<SettingsMenu>
  {
    public override string Name { get { return "spaar's Mod Loader: Settings Utility"; } }

    private struct SettingsButton
    {
      public string text;
      public SettingsToggle cb;
      public bool defaultValue;
      public int fontSize;
    }

    private static int numRegistered = 0;

    private static List<SettingsButton> toAdd = new List<SettingsButton>();

    /// <summary>
    /// Registers a new toggle button. It will be placed below all others
    /// that are currently registered.
    /// </summary>
    /// <param name="text">The text to display on the button</param>
    /// <param name="cb">Callback to call when the button is clicked</param>
    /// <param name="defaultValue">Starting state of the toggle</param>
    /// <param name="fontSize">Font size of the text on the button</param>
    public static void RegisterSettingsButton(string text, SettingsToggle cb,
      bool defaultValue = false, int fontSize = 0)
    {
      var button = new SettingsButton()
      {
        text = text,
        cb = cb,
        defaultValue = defaultValue,
        fontSize = fontSize
      };
      toAdd.Add(button);

      if (Game.AddPiece != null)
        RegisterSettingsButtonInternal(button);
    }

    private void Start()
    {
      Internal.ModLoader.MakeModule(this);
    }

    private void OnLevelWasLoaded(int level)
    {
      if (Game.AddPiece != null)
      {
        numRegistered = 0;
        foreach (var button in toAdd)
        {
          RegisterSettingsButtonInternal(button);
        }
      }
    }

    private static void RegisterSettingsButtonInternal(SettingsButton button)
    {
      var settingsObjects = GameObject.Find("Settings").transform
        .FindChild("SettingsObjects");
      var bottomDefaultSetting = settingsObjects.FindChild("ScreenshotIcon");
      const float SettingSize = 0.362f;
      var settingPos = bottomDefaultSetting.position;
      settingPos.y -= 0.575f + numRegistered * SettingSize;

      var fxaa = settingsObjects.FindChild("FXAA");

      var newSetting = UnityEngine.Object.Instantiate(fxaa, settingPos,
        fxaa.rotation) as Transform;
      newSetting.parent = settingsObjects;

      var newSettingButton = newSetting.FindChild("AA BUTTON");
      var newSettingText = newSetting.FindChild("AA text");
      newSetting.gameObject.name = button.text;
      newSettingButton.gameObject.name = button.text + " button";
      newSettingText.gameObject.name = button.text + " text";
      var textMesh = newSettingText.gameObject.GetComponent<TextMesh>();
      textMesh.text = button.text;
      textMesh.fontSize = button.fontSize;

      var settingsComponent
        = newSettingButton.gameObject.AddComponent<SettingsComponent>();

      var fxaaToggle = newSettingButton.gameObject.GetComponent<ToggleAA>();
      settingsComponent.darkMaterial = fxaaToggle.darkMaterial;
      settingsComponent.redMaterial = fxaaToggle.redMaterial;
      UnityEngine.Object.Destroy(fxaaToggle);

      settingsComponent.SetCallback(button.cb);
      settingsComponent.SetOn(button.defaultValue);

      numRegistered++;
    }
  }
}
