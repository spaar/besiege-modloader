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

    public bool IsDown()
    {
      return Input.GetKey(Modifier) && Input.GetKeyDown(Trigger);
    }
  }
}
