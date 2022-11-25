using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlBuildingThruster : UmlBuildingPart
{
    public override void OnGrabEnd()
    {
        _isGrabbed = false;

        InvokePartReleased(transform);
    }
}
