using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDescriptionManager : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Outline _associationOutline;

    [SerializeField]
    private GameObject _associationDescription;

    [SerializeField]
    private UnityEngine.UI.Outline _aggregationOutline;

    [SerializeField]
    private GameObject _aggregationDescription;

    [SerializeField]
    private UnityEngine.UI.Outline _compositionOutline;

    [SerializeField]
    private GameObject _compositionDescription;

    [SerializeField]
    private UnityEngine.UI.Outline _inheritanceOutline;

    [SerializeField]
    private GameObject _inheritanceDescription;

    public void ToggleAssociationDescription()
    {
        // Update all outlines
        _associationOutline.enabled = !_associationOutline.enabled;
        _aggregationOutline.enabled = false;
        _compositionOutline.enabled = false;
        _inheritanceOutline.enabled = false;

        // Update all descriptions
        _associationDescription.SetActive(!_associationDescription.activeSelf);
        _aggregationDescription.SetActive(false);
        _compositionDescription.SetActive(false);
        _inheritanceDescription.SetActive(false);
    }

    public void ToggleAggregationDescription()
    {
        // Update all outlines
        _associationOutline.enabled = false;
        _aggregationOutline.enabled = !_aggregationOutline.enabled;
        _compositionOutline.enabled = false;
        _inheritanceOutline.enabled = false;

        // Update all descriptions
        _associationDescription.SetActive(false);
        _aggregationDescription.SetActive(!_aggregationDescription.activeSelf);
        _compositionDescription.SetActive(false);
        _inheritanceDescription.SetActive(false);
    }

    public void ToggleCompositionDescription()
    {
        // Update all outlines
        _associationOutline.enabled = false;
        _aggregationOutline.enabled = false;
        _compositionOutline.enabled = !_compositionOutline.enabled;
        _inheritanceOutline.enabled = false;

        // Update all descriptions
        _associationDescription.SetActive(false);
        _aggregationDescription.SetActive(false);
        _compositionDescription.SetActive(!_compositionDescription.activeSelf);
        _inheritanceDescription.SetActive(false);
    }

    public void ToggleInheritanceDescription()
    {
        // Update all outlines
        _associationOutline.enabled = false;
        _aggregationOutline.enabled = false;
        _compositionOutline.enabled = false;
        _inheritanceOutline.enabled = !_inheritanceOutline.enabled;

        // Update all descriptions
        _associationDescription.SetActive(false);
        _aggregationDescription.SetActive(false);
        _compositionDescription.SetActive(false);
        _inheritanceDescription.SetActive(!_inheritanceDescription.activeSelf);
    }
}
