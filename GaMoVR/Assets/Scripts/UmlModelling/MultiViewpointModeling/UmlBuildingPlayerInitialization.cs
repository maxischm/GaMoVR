using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlBuildingPlayerInitialization : MonoBehaviour
{
    private bool _isInitialized;

    // Update is called once per frame
    public void Update()
    {
        if (_isInitialized)
        {
            return;
        }

        if (LocalPlayerReference.Instance.LocalPlayer is not null)
        {
            var rightHand = LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>().RightHandBaseInteractor;
            // rightHand.gameObject.AddComponent<TransformPositionDriver>();
            // rightHand.gameObject.AddComponent<VelocityTracker>();

            var leftHand = LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>().LeftHandBaseInteractor;
            // leftHand.gameObject.AddComponent<TransformPositionDriver>();
            // leftHand.gameObject.AddComponent<VelocityTracker>();
        }

        _isInitialized = true;
    }
}
