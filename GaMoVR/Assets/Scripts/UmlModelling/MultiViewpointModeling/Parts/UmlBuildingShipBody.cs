using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlBuildingShipBody : UmlBuildingPart
{
    [SerializeField]
    public Transform _wingAttach1;

    [SerializeField]
    public Transform _wingAttach2;

    [SerializeField]
    public Transform _thrusterAttach1;

    [SerializeField]
    public Transform _thrusterAttach2;

    [SerializeField]
    public Transform _cannonAttach1;

    [Space, SerializeField]
    public Transform _intersectedWing;

    [SerializeField]
    public Transform _intersectedThruster;

    [SerializeField]
    public Transform _intersectedCannon;

    [Space, SerializeField]
    public Transform _removedWing;

    [SerializeField]
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
        if (_intersectedWing == releasedPart
            || _intersectedThruster == releasedPart
            || _intersectedCannon == releasedPart
            || transform == releasedPart
            || _removedWing == releasedPart
            || _removedThruster == releasedPart
            || _removedCannon == releasedPart
        )
        {
            HandleIntersectionChanges();
        }
    }

    public void HandleIntersectionChanges()
    {
        if (_intersectedWing != null)
        {
            if (_wingAttach1.childCount == 0)
            {
                _intersectedWing.SetParent(_wingAttach1);
                _intersectedWing.localPosition = Vector3.zero;
                _intersectedWing.localRotation = Quaternion.identity;
                // _intersectedWing.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                UmlModel.Instance.AddConnectionInModel(this, _intersectedWing.GetComponent<UmlBuildingWing>());
            }
            else if (_wingAttach2.childCount == 0)
            {
                var scaleBuffer = _intersectedWing.localScale;
                _intersectedWing.SetParent(_wingAttach2);
                _intersectedWing.localPosition = Vector3.zero;
                _intersectedWing.localRotation = Quaternion.identity;
                // _intersectedWing.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                _intersectedWing.localScale = scaleBuffer;

                UmlModel.Instance.AddConnectionInModel(this, _intersectedWing.GetComponent<UmlBuildingWing>());
            }
            _intersectedWing = null;
        }
        if (_intersectedThruster != null)
        {
            if (_thrusterAttach1.childCount == 0)
            {
                if (_intersectedThruster.parent != null
                    && _intersectedThruster.parent != transform
                    && _intersectedThruster.parent.parent.GetComponent<UmlBuildingPart>() is not UmlBuildingShipBody
                )
                { // is being dragged directly from other parent
                    _intersectedThruster.parent.parent.GetComponent<UmlBuildingWing>()._removedThruster = null;
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
            }
            else if (_thrusterAttach2.childCount == 0)
            {
                if (_intersectedThruster.parent != null
                    && _intersectedThruster.parent != transform
                    && _intersectedThruster.parent.parent.GetComponent<UmlBuildingPart>() is not UmlBuildingShipBody
                )
                { // is being dragged directly from other parent
                    _intersectedThruster.parent.parent.GetComponent<UmlBuildingWing>()._removedThruster = null;
                    UmlModel.Instance.RemoveConnectionInModel(
                        _intersectedThruster.GetComponent<UmlBuildingPart>(),
                        _intersectedThruster.parent.parent.GetComponent<UmlBuildingPart>()
                    );
                }

                _intersectedThruster.SetParent(_thrusterAttach2);
                _intersectedThruster.localPosition = Vector3.zero;
                _intersectedThruster.localRotation = Quaternion.identity;
                // _intersectedThruster.localScale = new Vector3(0.6f, 0.6f, 0.6f);

                UmlModel.Instance.AddConnectionInModel(this, _intersectedThruster.GetComponent<UmlBuildingThruster>());
            }
            _intersectedThruster = null;
        }
        if (_intersectedCannon != null && _cannonAttach1.childCount == 0)
        {
            if (_intersectedCannon.parent != null
                && _intersectedCannon.parent != transform
                && _intersectedCannon.parent.parent.GetComponent<UmlBuildingPart>() is not UmlBuildingShipBody
            )
            { // is being dragged directly from other parent
                _intersectedCannon.parent.parent.GetComponent<UmlBuildingWing>()._removedCannon = null;
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

        if (_removedWing != null
            && (_wingAttach1.childCount > 0 && _wingAttach1.GetChild(0) == _removedWing
                || _wingAttach2.childCount > 0 && _wingAttach2.GetChild(0) == _removedWing))
        {
            UmlModel.Instance.RemoveConnectionInModel(this, _removedWing.GetComponent<UmlBuildingWing>());
            _removedWing.SetParent(null);
            _removedWing = null;
        }

        if (_removedThruster != null
            && (_thrusterAttach1.childCount > 0 && _thrusterAttach1.GetChild(0) == _removedThruster
                || _thrusterAttach2.childCount > 0 && _thrusterAttach2.GetChild(0) == _removedThruster))
        {
            UmlModel.Instance.RemoveConnectionInModel(this, _removedThruster.GetComponent<UmlBuildingThruster>());
            _removedThruster.SetParent(null);
            _removedThruster = null;
        }

        if (_removedCannon != null
            && _cannonAttach1.childCount > 0 && _cannonAttach1.GetChild(0) == _removedCannon)
        {
            UmlModel.Instance.RemoveConnectionInModel(this, _removedCannon.GetComponent<UmlBuildingCannon>());
            _removedCannon.SetParent(null);
            _removedCannon = null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wing")
            && !(_wingAttach1.childCount > 0 && _wingAttach1.GetChild(0) == other.transform)
            && !(_wingAttach2.childCount > 0 && _wingAttach2.GetChild(0) == other.transform))
        {
            // Handle Wing
            _intersectedWing = other.transform;
            if (_intersectedWing == _removedWing)
            {
                _removedWing = null;
            }
        }
        else if (other.CompareTag("Thruster")
        && !(_thrusterAttach1.childCount > 0 && _thrusterAttach1.GetChild(0) == other.transform)
            && !(_thrusterAttach2.childCount > 0 && _thrusterAttach2.GetChild(0) == other.transform))
        {
            // Handle Thruster
            _intersectedThruster = other.transform;
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
            if (_intersectedCannon == _removedCannon)
            {
                _removedCannon = null;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Handle detaching
        if (other.CompareTag("Wing") && other.GetComponent<UmlBuildingPart>()._isGrabbed)
        {
            // Handle Wing
            _removedWing = other.transform;
            if (_intersectedWing == _removedWing)
            {
                _intersectedWing = null;
            }
        }
        else if (other.CompareTag("Thruster") && other.GetComponent<UmlBuildingPart>()._isGrabbed)
        {
            _removedThruster = other.transform;
            if (_intersectedThruster == _removedThruster)
            {
                _intersectedThruster = null;
            }
        }
        else if (other.CompareTag("Cannon") && other.GetComponent<UmlBuildingPart>()._isGrabbed)
        {
            _removedCannon = other.transform;
            if (_intersectedCannon == _removedCannon)
            {
                _intersectedCannon = null;
            }
        }
    }

    public void RemoveSelf()
    {
        if (_wingAttach1.childCount > 0)
        {
            _wingAttach1.GetChild(0).parent = null;
        }
        if (_wingAttach2.childCount > 0)
        {
            _wingAttach2.GetChild(0).parent = null;
        }
        if (_thrusterAttach1.childCount > 0)
        {
            _thrusterAttach1.GetChild(0).parent = null;
        }
        if (_thrusterAttach2.childCount > 0)
        {
            _thrusterAttach2.GetChild(0).parent = null;
        }
        if (_cannonAttach1.childCount > 0)
        {
            _cannonAttach1.GetChild(0).parent = null;
        }
    }
}
