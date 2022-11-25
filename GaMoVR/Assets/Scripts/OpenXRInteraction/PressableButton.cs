using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PressableButton : XRBaseInteractable
{
    private float _yMin = 0.0f;
    private float _yMax = 0.0f;

    private float _previousHandHeight = 0.0f;
    private IXRHoverInteractor _hoverInteractor = null;

    private bool _previousPress = false;

    [Space, SerializeField] private float yMovement = 0.05f;
    [Space, SerializeField] public UnityEvent onPress;
    [Space, SerializeField] private UnityEvent onRelease;

    protected override void Awake()
    {
        base.Awake();
        hoverEntered.AddListener(StartPress);
        hoverExited.AddListener(EndPress);
    }

    protected override void OnDestroy()
    {
        hoverEntered.RemoveListener(StartPress);
        hoverExited.RemoveListener(EndPress);
    }

    private void Start()
    {
        SetMinMax();
    }

    private void StartPress(HoverEnterEventArgs enterEventArgs)
    {
        _hoverInteractor = enterEventArgs.interactorObject;
        _previousHandHeight = GetLocalYPosition(_hoverInteractor.transform.position);
    }

    private void EndPress(HoverExitEventArgs exitEventArgs)
    {
        _hoverInteractor = null;
        _previousHandHeight = 0.0f;
        _previousPress = false;
        SetYPosition(_yMax);
        onRelease?.Invoke();
    }

    private void SetMinMax()
    {
        var localPosition = transform.localPosition;
        _yMin = localPosition.y - yMovement;
        _yMax = localPosition.y;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (_hoverInteractor is not null)
        {
            float newHandHeight = GetLocalYPosition(_hoverInteractor.transform.position);
            float handDifference = _previousHandHeight - newHandHeight;
            _previousHandHeight = newHandHeight;

            float newPosition = transform.localPosition.y - handDifference;
            SetYPosition(newPosition);
            CheckPress();
        }
    }

    private float GetLocalYPosition(Vector3 position)
    {
        Vector3 localPosition = transform.parent.InverseTransformPoint(position);
        return localPosition.y;
    }

    private void SetYPosition(float position)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = Mathf.Clamp(position, _yMin, _yMax);
        transform.localPosition = newPosition;
    }

    private void CheckPress()
    {
        bool inPosition = InPosition();
        if (inPosition && !_previousPress)
        {
            onPress.Invoke();
        }
        _previousPress = inPosition;
    }

    private bool InPosition()
    {
        var localPosition = transform.localPosition;
        float inRange = Mathf.Clamp(localPosition.y, _yMin, _yMin + 0.01f);
        return Math.Abs(localPosition.y - inRange) < 0.000001f;
    }
}