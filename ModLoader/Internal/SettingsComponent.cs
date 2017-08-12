using System;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class SettingsComponent : MonoBehaviour
  {
    private SettingsToggle callback;

    public Material redMaterial;
    public Material darkMaterial;

    private bool isOn;

    public void SetCallback(SettingsToggle cb)
    {
      callback = cb;
    }

    public void SetOn(bool on)
    {
      isOn = on;
      try
      {
        GetComponent<Renderer>().material = isOn ? redMaterial : darkMaterial;
      }
      catch (NullReferenceException e)
      {
        // There are reports of an NRE occasionally happening in `GetComponent` here.
        // No idea how or why and not able to reproduce so far.
        // This should at least prevent this from breaking anything important.
        Debug.LogError("[SettingsComponent] Caught an NRE in SetOn!");
        Debug.LogException(e);
      }
    }

    private void OnMouseDown()
    {
      SetOn(!isOn);
      if (callback != null)
        callback(isOn);
    } 
  }
}
