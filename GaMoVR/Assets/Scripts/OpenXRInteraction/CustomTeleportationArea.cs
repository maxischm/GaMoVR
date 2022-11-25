using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomTeleportationArea : TeleportationArea
{
    [SerializeField]
    private ActionBasedControllerManager _rightHandControllerManager;

    [SerializeField]
    private ActionBasedControllerManager _leftHandControllerManager;

    public override void AttachCustomReticle(IXRInteractor interactor)
    {
        if (_rightHandControllerManager is null)
        {
            _rightHandControllerManager = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<XRPlayerComponents>().RightHandInteractors.gameObject.GetComponent<ActionBasedControllerManager>();
        }
        if (_leftHandControllerManager is null)
        {
            _leftHandControllerManager = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<XRPlayerComponents>().LeftHandInteractors.gameObject.GetComponent<ActionBasedControllerManager>();
        }
        if (_rightHandControllerManager.teleportState.enabled || _leftHandControllerManager.teleportState.enabled)
        {
            base.AttachCustomReticle(interactor);
        }
    }
}
