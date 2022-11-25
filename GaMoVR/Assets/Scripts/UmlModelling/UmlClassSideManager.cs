using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UmlClassSideManager : MonoBehaviour
{
    public RectTransform CanvasPropertyBody;
    public RectTransform CanvasMethodBody;
    public RectTransform SeparationLine;

    public GameObject FeatureEntryPrefab;

    [SerializeField]
    private TextMeshProUGUI _className;
    public string ClassName
    {
        get => _className.text;
    }

    private Vector3 _initialSeparationLinePosition;

    public void Awake()
    {
        _initialSeparationLinePosition = SeparationLine.position;
    }

    public void AddClassProperty(string propertyText)
    {
        var propertyGameObject = Instantiate(FeatureEntryPrefab).GetComponent<RectTransform>();
        propertyGameObject.GetComponent<TextMeshProUGUI>().SetText(propertyText);

        // align new property on class canvas
        propertyGameObject.transform.SetParent(CanvasPropertyBody.transform);
        propertyGameObject.anchoredPosition3D =
            new Vector3(
                0,
                -CanvasPropertyBody.childCount * propertyGameObject.rect.height,
                0.1f
            );
        propertyGameObject.transform.rotation = SeparationLine.transform.rotation;
        propertyGameObject.localScale = new Vector3(1, 1, 1);
        propertyGameObject.offsetMin = new Vector2(100, propertyGameObject.offsetMin.y);
        propertyGameObject.offsetMax = new Vector2(100, propertyGameObject.offsetMax.y);

        // Increase height of body for properties
        CanvasPropertyBody.sizeDelta =
            new Vector2(
                CanvasPropertyBody.sizeDelta.x,
                CanvasPropertyBody.sizeDelta.y + propertyGameObject.rect.height
            );
        CanvasPropertyBody.anchoredPosition =
            new Vector2(
                0,
                CanvasPropertyBody.anchoredPosition.y - propertyGameObject.rect.height
            );

        // Move separation line and method body down based on method body height
        SeparationLine.anchoredPosition = new Vector2(0, -CanvasPropertyBody.rect.height - 20);
        CanvasMethodBody.anchoredPosition = new Vector2(
            0,
            SeparationLine.anchoredPosition.y - CanvasMethodBody.rect.height
        );
    }

    public void RemoveClassProperty()
    {
        CanvasPropertyBody.sizeDelta =
            new Vector2(
                CanvasPropertyBody.sizeDelta.x,
                CanvasPropertyBody.sizeDelta.y - FeatureEntryPrefab.GetComponent<RectTransform>().rect.height
            );
        CanvasPropertyBody.anchoredPosition =
            new Vector2(
                0,
                CanvasPropertyBody.anchoredPosition.y + FeatureEntryPrefab.GetComponent<RectTransform>().rect.height
            );

        SeparationLine.anchoredPosition = new Vector2(0, -CanvasPropertyBody.rect.height - 20);
        CanvasMethodBody.anchoredPosition = new Vector2(
            0,
            -CanvasPropertyBody.rect.height - SeparationLine.rect.height - (CanvasMethodBody.transform.childCount > 0 ? 90 : 50)
        );
    }

    public void AddClassMethod(string methodText)
    {
        var methodGameObject = Instantiate(FeatureEntryPrefab).GetComponent<RectTransform>();
        methodGameObject.GetComponent<TextMeshProUGUI>().SetText(methodText);

        // align new method on class canvas
        methodGameObject.transform.SetParent(CanvasMethodBody.transform);
        methodGameObject.anchoredPosition3D =
            new Vector3(
                0,
                -CanvasMethodBody.childCount * methodGameObject.rect.height,
                0.1f
            );
        methodGameObject.transform.rotation = SeparationLine.transform.rotation;
        methodGameObject.localScale = new Vector3(1, 1, 1);
        methodGameObject.offsetMin = new Vector2(100, methodGameObject.offsetMin.y);
        methodGameObject.offsetMax = new Vector2(100, methodGameObject.offsetMax.y);

        // Increase height of body for methods
        CanvasMethodBody.sizeDelta =
            new Vector2(
                CanvasMethodBody.sizeDelta.x,
                CanvasMethodBody.sizeDelta.y + methodGameObject.rect.height
            );
        CanvasMethodBody.anchoredPosition =
            new Vector2(
                0,
                CanvasMethodBody.anchoredPosition.y - methodGameObject.rect.height
            );
    }

    public void RemoveClassMethod()
    {
        CanvasMethodBody.anchoredPosition = new Vector2(
            0,
            CanvasMethodBody.anchoredPosition.y + FeatureEntryPrefab.GetComponent<RectTransform>().rect.height
        );
        CanvasMethodBody.sizeDelta =
            new Vector2(
                CanvasMethodBody.sizeDelta.x,
                CanvasMethodBody.sizeDelta.y - FeatureEntryPrefab.GetComponent<RectTransform>().rect.height
            );
    }

    public void OnTriggerEnter(Collider other)
    {
        // TODO: trigger some form of highlighting that the user knows they will drop it into the class's canvas
    }

    public void SetClassName(string className)
    {
        _className.SetText(className);
    }

    public void ResetToStart(string[] methodsToKeep, string[] propertiesToKeep)
    {
        var methodsToKeepList = new List<string>(methodsToKeep);
        foreach (RectTransform child in CanvasMethodBody)
        {
            if (!methodsToKeepList.Contains(child.GetComponent<TextMeshProUGUI>().text))
            {
                RemoveClassMethod();
                Destroy(child.gameObject);
            }
        }
        var propertiesToKeepList = new List<string>(propertiesToKeep);
        foreach (RectTransform child in CanvasPropertyBody)
        {
            if (!propertiesToKeepList.Contains(child.GetComponent<TextMeshProUGUI>().text))
            {
                RemoveClassProperty();
                Destroy(child.gameObject);
            }
        }
        // SeparationLine.position = _initialSeparationLinePosition;
    }
}
