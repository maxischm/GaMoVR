using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlBuildingPart : MonoBehaviour
{
    public delegate void BuildingPartGrab(Transform part);
    public static event BuildingPartGrab OnPartReleased;

    [Space, SerializeField]
    public bool _isGrabbed;

    public virtual void OnGrabBegin()
    {
        _isGrabbed = true;
    }

    public virtual void OnGrabEnd()
    {
        _isGrabbed = false;
    }

    public void InvokePartReleased(Transform releasedPart)
    {
        OnPartReleased?.Invoke(releasedPart);
    }
}
