using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlBuildingWing : UmlBuildingPart
{
    [SerializeField]
    public Transform _thrusterAttach1;

    [SerializeField]
    public Transform _cannonAttach1;

    [Space, SerializeField]
    public Transform _intersectedThruster;

    [SerializeField]
    public Transform _intersectedCannon;

    [Space, SerializeField]
    public Transform _removedThruster;

    [SerializeField]
    public Transform _removedCannon;

    public void OnEnable()
    {
        OnPartReleased += HandlePartReleased;
    }

    public void OnDestroy()
    {
        OnPartReleased -= HandlePartReleased;
    }

    public override void OnGrabEnd()
    {
        _isGrabbed = false;

        InvokePartReleased(transform);
    }

    public void HandlePartReleased(Transform releasedPart)
    {
        if (_intersectedCannon == releasedPart
            || _intersectedThruster == releasedPart
            || _removedCannon == releasedPart
            || _removedThruster == releasedPart)
        {
            HandleIntersectionChanges();
        }
    }

    public void HandleIntersectionChanges()
    {
        if (_intersectedThruster != null && _thrusterAttach1.childCount == 0)
        {
            if (_intersectedThruster.parent != null
                && _intersectedThruster.parent != transform
                && _intersectedCannon.parent.parent.GetComponent<UmlBuildingPart>() is not UmlBuildingWing
            )
            { // is being dragged directly from other parent
                _intersectedThruster.parent.parent.GetComponent<UmlBuildingShipBody>()._removedThruster = null;
                UmlModel.Instance.RemoveConnectionInModel(
                    _intersectedThruster.GetComponent<UmlBuildingPart>(),
                    _intersectedThruster.parent.parent.GetComponent<UmlBuildingPart>()
                );
            }

            _intersectedThruster.SetParent(_thrusterAttach1);
            _intersectedThruster.localPosition = Vector3.zero;
            _intersectedThruster.localRotation = Quaternion.identity;
            // _intersectedThruster.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            UmlModel.Instance.AddConnectionInModel(this, _intersectedThruster.GetComponent<UmlBuildingThruster>());
            _intersectedThruster = null;
        }
        if (_intersectedCannon != null && _cannonAttach1.childCount == 0)
        {
            if (_intersectedCannon.parent != null
                && _intersectedCannon.parent != transform
                && _intersectedCannon.parent.parent.GetComponent<UmlBuildingPart>() is not UmlBuildingWing)
            { // is being dragged directly from other parent
                _intersectedCannon.parent.parent.GetComponent<UmlBuildingShipBody>()._removedCannon = null;
                UmlModel.Instance.RemoveConnectionInModel(
                    _intersectedCannon.GetComponent<UmlBuildingPart>(),
                    _intersectedCannon.parent.parent.GetComponent<UmlBuildingPart>()
                );
            }

            _intersectedCannon.SetParent(_cannonAttach1);
            _intersectedCannon.localPosition = Vector3.zero;
            _intersectedCannon.localRotation = Quaternion.identity;
            // _intersectedCannon.localScale = new Vector3(1, 1, 1);

            UmlModel.Instance.AddConnectionInModel(this, _intersectedCannon.GetComponent<UmlBuildingCannon>());
            _intersectedCannon = null;
        }
        if (_removedThruster != null
            && _thrusterAttach1.childCount > 0 && _thrusterAttach1.GetChild(0) == _removedThruster)
        {
            UmlModel.Instance.RemoveConnectionInModel(this, _removedThruster.GetComponent<UmlBuildingThruster>());
            _removedThruster = null;
        }
        if (_removedCannon != null
            && _cannonAttach1.childCount > 0 && _cannonAttach1.GetChild(0) == _removedCannon)
        {
            UmlModel.Instance.RemoveConnectionInModel(this, _removedCannon.GetComponent<UmlBuildingCannon>());
            _removedCannon = null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Thruster")
            && !(_thrusterAttach1.childCount > 0 && _thrusterAttach1.GetChild(0) == other.transform))
        {
            // Handle Thruster
            _intersectedThruster = other.transform;
            _intersectedCannon = null;
            if (_intersectedThruster == _removedThruster)
            {
                _removedThruster = null;
            }
        }
        else if (other.CompareTag("Cannon")
            && !(_cannonAttach1.childCount > 0 && _cannonAttach1.GetChild(0) == other.transform))
        {
            // Handle Cannon
            _intersectedCannon = other.transform;
            _intersectedThruster = null;
            if (_intersectedCannon == _removedCannon)
            {
                _removedCannon = null;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Thruster") && other.GetComponent<UmlBuildingPart>()._isGrabbed)
        {
            // Handle Thruster
            _removedThruster = other.transform;
            if (_intersectedThruster == _removedThruster)
            {
                _intersectedThruster = null;
            }
        }
        else if (other.CompareTag("Cannon") && other.GetComponent<UmlBuildingPart>()._isGrabbed)
        {
            // Handle Cannon
            _removedCannon = other.transform;
            if (_intersectedCannon == _removedCannon)
            {
                _intersectedCannon = null;
            }
        }
    }

    public void RemoveSelf()
    {
        if (_thrusterAttach1.childCount > 0)
        {
            _thrusterAttach1.GetChild(0).parent = null;
        }
        if (_cannonAttach1.childCount > 0)
        {
            _cannonAttach1.GetChild(0).parent = null;
        }
    }
}
