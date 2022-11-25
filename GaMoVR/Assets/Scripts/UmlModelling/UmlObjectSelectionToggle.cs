using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class UmlObjectSelectionToggle : MonoBehaviour
{
    [SerializeField] private InputActionReference activationAction;

    private XRPlayerComponents playerComponents;

    [SerializeField]
    private GameObject umlRayInteractorRight;

    public void OnEnable()
    {
        activationAction.action.started += UIActionInvoked;
    }

    public void OnDisable()
    {
        activationAction.action.started -= UIActionInvoked;
    }

    private void UIActionInvoked(InputAction.CallbackContext obj) // FIXME: name likely to be not-fixed to this; adjust to fit use case
    {
        // Cache player reference if not yet done
        if (playerComponents == null)
        {
            playerComponents = LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>();
        }
        // deactivate teleport (access via InteractorStateHolder)
        playerComponents.RightHandInteractors.TeleportInteractorState = !playerComponents.RightHandInteractors.TeleportInteractorState;
        playerComponents.RightHandInteractors.RayInteractorState = !playerComponents.RightHandInteractors.RayInteractorState;

        // activate LineRenderer & XRInteractorLineVisual for selection
        umlRayInteractorRight.GetComponent<LineRenderer>().enabled = !umlRayInteractorRight.GetComponent<LineRenderer>().enabled;
        // umlRayInteractorRight.GetComponent<XRInteractorLineVisual>().enabled =
        //     !umlRayInteractorRight.GetComponent<XRInteractorLineVisual>().enabled;
        umlRayInteractorRight.GetComponent<XRInteractorLineVisual>().enabled =
            !umlRayInteractorRight.GetComponent<XRInteractorLineVisual>().enabled;
    }
}
