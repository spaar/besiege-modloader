using System;
using UnityEngine;

namespace spaar.ModLoader
{
  public class Key
  {
    public KeyCode Modifier { get; set; }
    public KeyCode Trigger { get; set; }

    public Key(string modifier, string trigger)
    {
      Modifier = (KeyCode)Enum.Parse(typeof(KeyCode), modifier);
      Trigger = (KeyCode)Enum.Parse(typeof(KeyCode), trigger);
    }

    public Key(KeyCode modifier, KeyCode trigger)
    {
      Modifier = modifier;
      Trigger = trigger;
    }

    public bool Pressed()
    {
      var pressed = (((Modifier == KeyCode.None && Trigger != KeyCode.None)
          || Input.GetKey(Modifier))
        && ((Trigger == KeyCode.None && Modifier != KeyCode.None)
          || Input.GetKeyDown(Trigger)));

      if (pressed && (Trigger == KeyCode.Tab))
      {
        // Prevent turning off the HUD when using Tab as keybinding
        if (Game.AddPiece != null)
          Game.AddPiece.hudCam.enabled = !Game.AddPiece.hudCam.enabled;
      }

      return pressed;
    }

    public bool IsDown()
    {
      return (((Modifier == KeyCode.None && Trigger != KeyCode.None)
          || Input.GetKey(Modifier))
        && ((Trigger == KeyCode.None && Modifier != KeyCode.None)
          || Input.GetKey(Trigger)));
    }
  }
}
