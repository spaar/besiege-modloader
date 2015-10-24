using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class OptionsComponent : MonoBehaviour
  {
    private OptionsToggle callback;

    public Material redMaterial;
    public Material darkMaterial;

    private bool isOn;

    public void SetCallback(OptionsToggle cb)
    {
      callback = cb;
    }

    public void SetOn(bool on)
    {
      isOn = on;
      if (isOn)
      {
        GetComponent<Renderer>().material = redMaterial;
      }
      else
      {
        GetComponent<Renderer>().material = darkMaterial;
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
