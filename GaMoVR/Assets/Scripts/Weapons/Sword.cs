using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword : MonoBehaviour
{
    [SerializeField]
    private float _damage = 10;

    private bool _inCollider;

    private ActionBasedController _grabbingController;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");

        var health = other.gameObject.GetComponentInParent<Health>();
        if (health is UmlElementHealth)
        {
            Debug.Log("Hit UML Element");
            if (!_inCollider)
            {
                (health as UmlElementHealth)?.Hit(_damage, WeaponTypeTag.Sword);
                _grabbingController?.SendHapticImpulse(0.5f, 0.15f);
            }
            _inCollider = true;
        }
        else if (health is UmlBuildingConnectionHealth)
        {
            Debug.Log("Hit UML Element in Build Mode");
            if (!_inCollider)
            {
                (health as UmlBuildingConnectionHealth)?.Hit(_damage, WeaponTypeTag.Sword);
                _grabbingController?.SendHapticImpulse(0.5f, 0.15f);
            }
            _inCollider = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        _inCollider = false;
    }

    public void EnableHapticFeedback(SelectEnterEventArgs args)
    {
        _grabbingController = (args.interactorObject as XRDirectInteractor)?.GetComponent<ActionBasedController>();
    }

    public void DisableHapticFeedback()
    {
        _grabbingController = null;
    }
}
