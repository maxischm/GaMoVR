using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider), typeof(Image), typeof(XRSimpleInteractable))]
public class CustomUIButton : CustomUIInteractable
{
    [SerializeField] private ColorBlock colorBlock = ColorBlock.defaultColorBlock;

    public UnityEvent onClick;

    private Color _defaultColor;
    private XRSimpleInteractable _interactable;

    private Image _image;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _interactable = GetComponent<XRSimpleInteractable>();
        _interactable.firstHoverEntered.AddListener(OnHoverEntered);
        _interactable.lastHoverExited.AddListener(OnLastHoverExited);
        _interactable.activated.AddListener(OnSelectEntered);
        _interactable.deactivated.AddListener(OnSelectExited);
        _defaultColor = _image.color;
    }

    private void OnSelectExited(DeactivateEventArgs arg0)
    {
        _image.color = _defaultColor;
        Debug.Log("Selected");
    }

    private void OnSelectEntered(ActivateEventArgs arg0)
    {
        _image.color = _defaultColor * colorBlock.colorMultiplier * colorBlock.selectedColor;
        onClick.Invoke();
    }

    private void OnDestroy()
    {
        try
        {
            _interactable.firstHoverEntered.RemoveListener(OnHoverEntered);
            _interactable.lastHoverExited.RemoveListener(OnLastHoverExited);
            _interactable.activated.RemoveListener(OnSelectEntered);
            _interactable.deactivated.RemoveListener(OnSelectExited);
        }
        catch (MissingReferenceException e)
        {
            List<GameObject> list = new List<GameObject>();
            list.Last();
        }
        
    }


    

    private void OnLastHoverExited(HoverExitEventArgs arg0)
    {
        _image.color = _defaultColor;
    }

    private void OnHoverEntered(HoverEnterEventArgs arg0)
    {
        _image.color = _defaultColor * colorBlock.colorMultiplier * colorBlock.highlightedColor;
    }

    [ContextMenu("ExecuteOnClick")]
    public void ExecuteOnClick()
    {
        onClick.Invoke();
    }
}