using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponHapticFeedback : MonoBehaviour
{
    [SerializeField]
    private bool _isRightHand;

    [SerializeField]
    private ActionBasedController _handController;

    [SerializeField]
    private float _intensity = 0.2f;

    [SerializeField]
    private float _duration = 0.25f;

    [SerializeField]
    private InputActionReference _triggerAction;

    private XRPlayerComponents _xrPlayerComponents;

    public void EnableHapticFeedback()
    {
        _triggerAction.action.started += SendHapticImpulse;

        if (_xrPlayerComponents is null)
        {
            _xrPlayerComponents = LocalPlayerReference.Instance.LocalPlayer.GetComponent<XRPlayerComponents>();

            if (_isRightHand)
            {
                _handController = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<XRPlayerComponents>().RightHandInteractors.transform.Find("RayInteractor")
                .GetComponent<ActionBasedController>();
            }
            else
            {
                _handController = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<XRPlayerComponents>().LeftHandInteractors.transform.Find("RayInteractor")
                .GetComponent<ActionBasedController>();
            }
        }
    }

    public void DisableHapticFeedback()
    {
        _triggerAction.action.started -= SendHapticImpulse;
    }

    public void SendHapticImpulse(InputAction.CallbackContext context)
    {
        _handController.SendHapticImpulse(_intensity, _duration);
    }
}
