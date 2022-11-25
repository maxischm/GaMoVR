using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UmlConnectionSide
{
    Target,
    Origin
}

public enum UmlConnectorType
{
    Aggregation,
    DirectedAssociation,
    UndirectedAssociation,
    Inheritance,
    Composition
}

public class UmlConnectorStructure : MonoBehaviour
{
    public UmlConnectorType ConnectionType;
    public string OriginClassName;
    public string TargetClassName;
    public string Description;

    public bool IsCorrect;
    public bool IsCreated;

    public Vector3 InitPosition;

    [SerializeField]
    private Connector connector;

    [SerializeField]
    private Transform _targetGrabVolume;

    [SerializeField]
    private Transform _originGrabVolume;

    [SerializeField]
    private Transform _connectorHead;

    public void Init(
        UmlConnectorType connectionType,
        string description,
        bool isCorrect = true,
        bool isCreated = false,
        string originClassName = null,
        string targetClassName = null,
        bool getTransform = false
    )
    {
        ConnectionType = connectionType;
        Description = description;
        OriginClassName = originClassName;
        TargetClassName = targetClassName;
        IsCorrect = isCorrect;
        IsCreated = isCreated;
    }

    public UmlConnectorStructure(
        UmlConnectorType connectionType,
        string description,
        bool isCorrect,
        string originClassName = null,
        string targetClassName = null,
        bool getTransform = false
    )
    {
        Init(connectionType, description, isCorrect, false, originClassName, targetClassName, getTransform);
    }

    public void ResetToStart(bool resetAttachment = true)
    {
        gameObject.transform.parent.gameObject.SetActive(true);
        connector.ResetToStart(resetAttachment);
        if (resetAttachment)
        {
            transform.parent.position = InitPosition;
            transform.parent.rotation = Quaternion.Euler(0, -90, 0);
            _targetGrabVolume.localPosition = new Vector3(-1.38f, 0, 0);
            _originGrabVolume.localPosition = Vector3.zero;
            connector.UpdateTransform();
        }
    }
}
