using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UIHapticFeedback : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController _rightHandController;

    [SerializeField]
    private float _intensity = 0.2f;

    [SerializeField]
    private float _duration = 0.25f;

    public void SendButtonClickFeedback()
    {
        if (_rightHandController is null)
        {
            _rightHandController = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<XRPlayerComponents>().RightHandInteractors.transform.Find("RayInteractor")
                .GetComponent<ActionBasedController>();
        }
        _rightHandController.SendHapticImpulse(_intensity, _duration);
    }
}
