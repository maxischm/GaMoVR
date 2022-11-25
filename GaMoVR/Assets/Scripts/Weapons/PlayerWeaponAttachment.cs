using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeaponAttachment : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _meshRenderer;

    [SerializeField]
    private InputActionReference _rightHandGrabAction;

    [SerializeField]
    private InputActionReference _leftHandGrabAction;

    [SerializeField]
    private XRPlayerComponents _xrPlayerComponents;

    [SerializeField]
    private XRInteractionManager _interactionManager;

    public GameObject[] Weapons;

    private int containedHands;

    public void Start()
    {
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OpenXRRightHand"))
        {
            _rightHandGrabAction.action.started += GrabRightHandWeapon;
            containedHands++;
            _meshRenderer.enabled = true;
        }
        else if (other.CompareTag("OpenXRLeftHand"))
        {
            _leftHandGrabAction.action.started += GrabLeftHandWeapon;
            containedHands++;
            _meshRenderer.enabled = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OpenXRRightHand"))
        {
            _rightHandGrabAction.action.started -= GrabRightHandWeapon;
            containedHands--;
        }
        else if (other.CompareTag("OpenXRLeftHand"))
        {
            _leftHandGrabAction.action.started -= GrabLeftHandWeapon;
            containedHands--;
        }

        if (containedHands == 0)
        {
            _meshRenderer.enabled = false;
        }
    }

    public void GrabWeapon(int weaponIndex)
    {
        if (_interactionManager is null)
        {
            _interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        if (_xrPlayerComponents is null)
        {
            _xrPlayerComponents = LocalPlayerReference.Instance.LocalPlayer?.GetComponentInChildren<XRPlayerComponents>();
        }

        Weapons[weaponIndex].SetActive(true);

        if (weaponIndex == 0)
        {
            Weapons[weaponIndex].transform.position = _xrPlayerComponents.RightHandBaseInteractor.gameObject.transform.position;
            _interactionManager.SelectEnter(
                (IXRSelectInteractor)_xrPlayerComponents.RightHandBaseInteractor,
                (IXRSelectInteractable)Weapons[weaponIndex].GetComponentInChildren<XRGrabInteractable>());
        }
        else
        {
            Weapons[weaponIndex].transform.position = _xrPlayerComponents.LeftHandBaseInteractor.gameObject.transform.position;
            _interactionManager.SelectEnter(
                (IXRSelectInteractor)_xrPlayerComponents.LeftHandBaseInteractor,
                (IXRSelectInteractable)Weapons[weaponIndex].GetComponentInChildren<XRGrabInteractable>());
        }
    }

    private void GrabRightHandWeapon(InputAction.CallbackContext context)
    {
        GrabWeapon(0);
    }

    private void GrabLeftHandWeapon(InputAction.CallbackContext context)
    {
        GrabWeapon(1);
    }

    public void ReleaseWeapon(GameObject Weapon)
    {
        Weapon.SetActive(false);
    }
}
