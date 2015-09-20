using System.Collections.Generic;
using spaar.ModLoader.Internal;
using UnityEngine;

namespace spaar.ModLoader
{
  /// <summary>
  /// Callback delegate for a toggle setting.
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

    private static Transform modSection;

    private static void RegisterSettingsButtonInternal(SettingsButton button)
    {
      var settingsObjects = GameObject.Find("Settings").transform
        .FindChild("SettingsObjects");
      var bottomDefaultSetting = settingsObjects.FindChild("GOD/PYRO");
      Vector3 SettingSize = new Vector3(0.748f, 0.375f);

      if (modSection == null)
      {
        // Create a MODS section
        var settings = settingsObjects.FindChild("SETTINGS");

        var modsPos = settings.position;
        modsPos.y = bottomDefaultSetting.position.y - 1.2f;

        modSection = (Transform)Instantiate(settings, modsPos,
          settings.rotation);
        modSection.parent = settingsObjects;
        modSection.name = "MOD SETTINGS";

        foreach (Transform child in modSection)
        {
          if (child.name == "GENERAL")
          {
            child.GetComponent<TextMesh>().text = "M O D S";
            child.name = "Title";
          }
          else
          {
            Destroy(child.gameObject);
          }
        }

        var bg = settingsObjects.FindChild("BG");
        var bgScale = bg.localScale;
        bgScale.y += 1.35f; // Adjust background to include mods section title
        bg.localScale = bgScale;
        bg.gameObject.AddComponent<BoxCollider>();
        var scrollCollider = new GameObject("Scrolling").transform;
        scrollCollider.parent = settingsObjects;
        scrollCollider.rotation = bg.rotation;
        scrollCollider.localScale = bg.localScale;
        scrollCollider.gameObject.layer = bg.gameObject.layer;
        var pos = bg.position;
        pos.z = 15.0f; // Put collider behind all settings items
        scrollCollider.position = pos;
        scrollCollider.gameObject.AddComponent<ScrollSettingsMenu>()
          .settingsObjects = settingsObjects;
      }
      var settingPos = bottomDefaultSetting.position;

      settingPos.x += (numRegistered % 2) * SettingSize.x;
      settingPos.y -= 1.25f + (numRegistered / 2) * SettingSize.y;

      var fxaa = settingsObjects.FindChild("SETTINGS/FXAA");

      var newSetting = (Transform)Instantiate(fxaa, settingPos, fxaa.rotation);
      newSetting.parent = modSection;

      if (numRegistered % 2 == 0)
      {
        // Expand background to include new toggle
        var background = settingsObjects.FindChild("BG");
        var backgroundScale = background.localScale;
        backgroundScale.y += SettingSize.y * 2;
        background.localScale = backgroundScale;

        // Expand the scrolling object to the same size
        var scrolling = settingsObjects.FindChild("Scrolling");
        scrolling.localScale = backgroundScale;
        scrolling.GetComponent<ScrollSettingsMenu>().CalcBounds();

        // Check whether the new element row is outside of the screen
        // and enable scrolling if it is
        var lowestPoint = newSetting.position - SettingSize;
        var cam = GameObject.Find("HUD Cam").GetComponent<Camera>();
        if (cam.WorldToViewportPoint(lowestPoint).y < 0.0f)
        {
          scrolling.GetComponent<ScrollSettingsMenu>().scrollingEnabled = true;
        }
      }

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
      Destroy(fxaaToggle);

      settingsComponent.SetCallback(button.cb);
      settingsComponent.SetOn(button.defaultValue);

      numRegistered++;
    }
  }
}
