using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace spaar.ModLoader.Internal
{
  public class ScrollSettingsMenu : MonoBehaviour
  {
    public Transform settingsObjects;
    public bool scrollingEnabled = false;

    private Camera cam;
    private MouseOrbit mo;
    private Vector3 topLeft, bottomRight;

    private float moOriginalSensitivity;
    private bool storedOriginalSensitivity = false;

    private void Start()
    {
      cam = GameObject.Find("HUD Cam").GetComponent<Camera>();
      mo = Camera.main.GetComponent<MouseOrbit>();
      CalcBounds();
    }

    public void CalcBounds()
    {
      topLeft = transform.position
        + Vector3.left * transform.localScale.x / 2
        + Vector3.up * transform.localScale.y / 2;
      bottomRight = transform.position
        + Vector3.right * transform.localScale.x / 2
        + Vector3.down * transform.localScale.y / 2;
    }

    private void Update()
    {
      var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
      if (mousePos.x >= topLeft.x && mousePos.x <= bottomRight.x
        && mousePos.y <= topLeft.y && mousePos.y >= bottomRight.y
        && scrollingEnabled)
      {
        settingsObjects.Translate(Vector3.down * Input.GetAxis("Mouse ScrollWheel"));

        if (settingsObjects.position.y < -14.6f)
        {
          var pos = settingsObjects.position;
          pos.y = -14.6f;
          settingsObjects.position = pos;
        }

        if (!storedOriginalSensitivity)
        {
          moOriginalSensitivity = mo.scrollSensitivityScaler;
          storedOriginalSensitivity = true;
        }
        mo.scrollSensitivityScaler = 0.0f;
      }
      else
      {
        if (storedOriginalSensitivity)
        {
          mo.scrollSensitivityScaler = moOriginalSensitivity;
          moOriginalSensitivity = 0.0f;
          storedOriginalSensitivity = false;
        }
      }
    }
  }

}
