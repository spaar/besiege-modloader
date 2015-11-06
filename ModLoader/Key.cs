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
      return (((Modifier == KeyCode.None && Trigger != KeyCode.None)
          || Input.GetKey(Modifier))
        && ((Trigger == KeyCode.None && Modifier != KeyCode.None)
          || Input.GetKeyDown(Trigger)));
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
