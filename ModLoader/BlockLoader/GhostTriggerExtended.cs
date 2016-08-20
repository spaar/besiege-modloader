using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostTriggerExtended : GhostTrigger
{
  new public int touchingCount = 0;
  new public GhostMaterialController materialCode;
  public bool ignoreBase = false;
  public new int[] HUDLayers = new int[] { 9, 13, 19, 21, 23 };

  public override void Update()
  {
    if (!isTouching && touchingCount > 0)
      Check();

    if (materialCode)
    {
      if (!StatMaster.disabledBlockIntersectionWarning && (touchingCount > 0 || AddPiece.Instance.outOfBounds))
      {
        materialCode.SetRed();
        if (Input.GetButtonDown("Fire1"))
        {
          IntersectWarning.WarningFromWorldPos(transform.position);
        }
      }
      else
      {
        materialCode.SetNormal();
      }
    }
  }

  public override void OnDisable()
  {
    touchingCount = 0;

    Check();
  }

  public override void OnTriggerEnter(Collider other)
  {
    if (!ColliderIsIgnored(other))
      touchingCount += 1;

    Check();
  }

  public override void OnTriggerStay(Collider other)
  {
    if (touchingCount > 0)
    {
      return;
    }
    if (!ColliderIsIgnored(other))
    {
      touchingCount += 1;
      Check();
    }
  }

  public override void OnTriggerExit(Collider other)
  {
    if (!ColliderIsIgnored(other))
      touchingCount -= 1;

    Check();
  }

  public override bool ColliderIsIgnored(Collider col)
  {/*
        if (col == null)
            return true;
        if (col.tag == "DoubleBlock")
            return true;
        if (col.gameObject.layer == overlayBlockLayer)
            return true;
        if (col.gameObject.name == "Adding Point" && col instanceof BoxCollider && (col as BoxCollider).size.y != .0f)
            return true;
        for (int i = 0; i < HUDLayers.length; i++)
        {
            if (col.gameObject.layer == HUDLayers[i])
                return true;
        }
        for (int j = 0; j < layersToIgnore.Count; j++)
        {
            if (col.gameObject.layer == layersToIgnore[j])
                return true;
        }
        return false;*/

    if (!col)
      return true;
    if (transform.parent)
      if (col.attachedRigidbody == transform.parent.GetComponent<Rigidbody>())
        return true;
    if (ignoreBase)
      if (col.attachedRigidbody?.gameObject == AddPiece.Instance?.mouseHit.rigidbody?.gameObject)
        return true;
    if (col.transform.name == "occluder" || col.transform.name == "Occluder")
      return true;
    if (col.tag == "DoubleBlock")
      return true;
    if (col.gameObject.layer == overlayBlockLayer)
      return true;
    if (col.gameObject.name == "Adding Point" && col is BoxCollider && (col as BoxCollider).size.y != .0f)
      return true;
    for (int i = 0; i < HUDLayers.Length; i++)
    {
      if (col.gameObject.layer == HUDLayers[i])
        return true;
    }
    for (int j = 0; j < layersToIgnore.Count; j++)
    {
      if (col.gameObject.layer == layersToIgnore[j])
        return true;
    }
    return false;
  }

  public override void Check()
  {

    if (touchingCount > 0)
    {
      isTouching = true;
    }
    else
    {
      isTouching = false;
    }
  }
}