using System;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class ModLoaderHudInputControl : HudInputControl
  {
    public bool hideHud;

    public void CopyFrom(HudInputControl o)
    {
      this.toDisable = o.toDisable;
    }

    public override void Update()
    {
      if (Input.GetKeyDown(KeyCode.Tab))
      {
        ToggleHud();
      }
    }

    public void ToggleHud()
    {
      hideHud = !hideHud;
      foreach (var behaviour in toDisable)
      {
        behaviour.enabled = !hideHud;
      }
    }
  }
}
