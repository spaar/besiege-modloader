using System;
using System.Collections.Generic;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.ModLoader.Internal.Tools
{
  public class ModToggle : SingleInstance<ModToggle>
  {
    public override string Name { get; } = "spaar's Mod Loader: Mod Toggle";

    private bool visible = false;
    private int windowID = Util.GetWindowID();
    private Rect windowRect = new Rect(400, 300, 400, 550);
    private Vector2 scrollPosition = new Vector2(0f, 0f);

    private string errorMessage = "";
    private bool displayError = false;
    private int errorID = Util.GetWindowID();
    private Rect errorRect = new Rect(Screen.width/2 - 200, Screen.height/2 - 150,
      400, 300);
    private Vector2 errorScrollPos = new Vector2(0f, 0f);

    private void Start()
    {
      ModLoader.MakeModule(this);
    }

    private void Update()
    {
      if (Keys.K["ModToggle"])
      {
        visible = !visible;
      }
    }

    private void OnGUI()
    {
      if (!visible) return;

      GUI.skin = ModGUI.Skin;

      windowRect = GUI.Window(windowID, windowRect, DoWindow, "Mod Toggle");

      if (displayError)
      {
        errorRect = GUI.Window(errorID, errorRect, DoErrorWindow, "Error");
      }
    }

    private void DoWindow(int id)
    {
      scrollPosition = GUILayout.BeginScrollView(scrollPosition);

      foreach (var mod in ModLoader.Instance.LoadedMods)
      {
        DisplayMod(mod);
      }

      GUILayout.EndScrollView();

      GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
    }

    private void DisplayMod(InternalMod mod)
    {
      var toggleSize = new GUILayoutOption[]
      {
        GUILayout.Width(16f),
        GUILayout.Height(16f)
      };

      GUILayout.BeginHorizontal();

      var newValue = GUILayout.Toggle(mod.IsActive, "", toggleSize);
      if (newValue != mod.IsActive)
      {
        if (newValue || (!mod.IsEnabled && mod.IsActive))
        {
          mod.Enable();

          if (!mod.IsActive)
          {
            // Display error
            errorMessage = "An error occured while enabling "
              + mod.Mod.DisplayName + ".\nThere may be further information "
              + " available in the console.";
            displayError = true;
          }
        }
        else
        {
          mod.Disable();

          if (mod.IsActive)
          {
            // Display error/message
            if (mod.Mod.CanBeUnloaded)
            {
              errorMessage = "An error occured while disabling "
                + mod.Mod.DisplayName + "\nThere may be further information "
                + " available in the console.";
              displayError = true;
            }
            else
            {
              errorMessage = "Did not deactivate " + mod.Mod.DisplayName + ".\n"
                + "It cannot be deactivated at runtime.\n"
                + "It was marked as deactivated and will not be activated\n"
                + "the next time the game is started.";
              displayError = true;
            }
          }
        }
      }

      var modString = mod.Mod.ToString();
      if (!mod.IsEnabled)
      {
        modString += " <i>(disabled)</i>";
      }

      GUILayout.Label(modString);
      GUILayout.EndHorizontal();

      GUILayout.Space(10f);
    }

    private void DoErrorWindow(int id)
    {
      errorScrollPos = GUILayout.BeginScrollView(errorScrollPos);

      GUILayout.TextArea(errorMessage);

      GUILayout.EndScrollView();

      if (GUILayout.Button("Close"))
      {
        displayError = false;
      }

      GUI.DragWindow(new Rect(0, 0, windowRect.width, GUI.skin.window.padding.top));
    }
  }
}
