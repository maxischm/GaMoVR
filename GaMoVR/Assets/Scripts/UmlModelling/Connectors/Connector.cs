using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// A connecting line between two classes that possibly has an arrow tip at one end.
/// It sticks to the attached classes and can be moved by the playerwhen he grabs the grab volumes belonging to this connector.
/// </summary>
public class Connector : MonoBehaviour
{
    #region public fields

    /// <summary>
    /// The class where the arrow head that belongs to this connector - if there is one - does not point. If none exists, it is irrelevant which class is the target and which the origin class
    /// </summary>
    public ClassConnectionHub OriginClass;
    /// <summary>
    /// The class where the arrow head that belongs to this connector - if there is one - points. If none exists, it is irrelevant which class is the target and which the origin class
    /// </summary>
    public ClassConnectionHub TargetClass;
    /// <summary>
    /// The arrow head that sits on the target end of this connector or null if this connector is not directed
    /// </summary>
    public ArrowHead ArrowHead;

    public ConnectorGrabVolume OriginGrabVolume;
    public ConnectorGrabVolume TargetGrabVolume;

    public delegate void UmlConnectorGrabEvent(UmlConnectorType type);
    public static event UmlConnectorGrabEvent OnConnectorGrabbed;

    #endregion

    #region private fields

    /// <summary>
    /// The position relative to the origin class where the origin end of the connection should sit
    /// </summary>
    private Vector3 _localOriginConnectionPoint;
    /// <summary>
    /// THe position relative to the target class where the target end of the connection should sit
    /// </summary>
    private Vector3 _localTargetConnectionPoint;

    #endregion

    [SerializeField]
    private GameObject _descriptionCanvas;

    [SerializeField]
    private TextMeshProUGUI _descriptionText;

    public string Description
    {
        get => _descriptionText.text;
    }

    private Rigidbody _rigidbody;

    #region MonoBehaviour Callbacks

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void OnDestroy()
    {
        //Detach the connector from all connected classes
        LocalDetachFromClass(OriginGrabVolume);
        LocalDetachFromClass(TargetGrabVolume);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Detaches the connector from the class that is currently at the end of the connector the given ConnectorGrabVolume corresponds to.
    /// </summary>
    /// <param name="connectorEndGrabVolume"></param>
    public void LocalDetachFromClass(ConnectorGrabVolume connectorEndGrabVolume)
    {
        if (OriginGrabVolume == connectorEndGrabVolume)
        {
            OriginClass?.RemoveConnector(this);
            OriginClass = null;
        }
        else if (TargetGrabVolume == connectorEndGrabVolume)
        {
            TargetClass?.RemoveConnector(this);
            TargetClass = null;
        }
        else
        {
            Debug.LogError("Called DetachFromClass() with a grab volume not belonging to this connector.");
        }
    }

    /// <summary>
    /// Detaches this connector from a class that is being destroyed across the network
    /// Ensures that the cached attach state event is updated accordingly
    /// </summary>
    /// <param name="destroyedClass"></param>
    public void OnDestroyAttachedClass(ClassConnectionHub destroyedClass)
    {
        if (destroyedClass == TargetClass)
        {
            TargetClass?.RemoveConnector(this);
            TargetClass = null;
        }
        else if (destroyedClass == OriginClass)
        {
            OriginClass?.RemoveConnector(this);
            OriginClass = null;
        }
        else
        {
            Debug.LogError("Called DetachFromClass() for a class not attached to this connector.");
        }
    }

    /// <summary>
    /// Tries to attach the end of the connector that the given ConnectorGrabVolume corresponds to a class that this end of the connector currently points at.
    /// If there is no class in that direction the connectors end stays loose.
    /// This updates the Transform of the connector, only call on the client that currently owns the connector
    /// </summary>
    public void CalculateNewAttachment(ConnectorGrabVolume connectorEndGrabVolume, GameObject connectedClass)
    {
        var isEndpointInsideClass =
            connectedClass
                .GetComponent<MeshFilter>().mesh.bounds
                .Contains(connectorEndGrabVolume.transform.position);

        Vector3 attachSearchDirection;
        if ((OriginGrabVolume == connectorEndGrabVolume && !isEndpointInsideClass)
            || (TargetGrabVolume == connectorEndGrabVolume && isEndpointInsideClass))
        {
            attachSearchDirection = transform.forward * -1;
        }
        else
        {
            attachSearchDirection = transform.forward;
        }

        var attachSearchOrigin = OriginGrabVolume.transform.position
            + ((TargetGrabVolume.transform.position - OriginGrabVolume.transform.position) / 2);
        var connectorEnd = OriginGrabVolume == connectorEndGrabVolume ? ConnectorEndType.Origin : ConnectorEndType.Target;

        ClassConnectionHub newAttachedClass = null;
        if (
            Physics.Raycast(
                new Ray(attachSearchOrigin, attachSearchDirection),
                out RaycastHit hitInfo,
                5
            )
            && hitInfo.transform.CompareTag("UmlClass")
        )
        {
            Debug.Log($"Raycast hit {hitInfo.transform.name}");
            ClassConnectionHub hitClass = hitInfo.transform.GetComponent<ClassConnectionHub>();
            newAttachedClass = hitClass ? hitClass : null;
        }

        if (newAttachedClass != null)
        {
            newAttachedClass.GetComponent<Outline>().enabled = false;
            // AttachToClass(newAttachedClass, connectorEnd);
            UpdateConnectionPointToClass(hitInfo.point, connectorEnd);
            UpdateTransform();
        }
    }

    /// <summary>
    /// Recalculates the attachment between connector and class for this connector's end that is attached to the given class
    /// </summary>
    public void CalculateNewAttachment(ClassConnectionHub connectedClass) // called from `ClassScaler.cs` for updating attachment positions of connectors
    {
        if (connectedClass == OriginClass)
        {
            CalculateNewAttachment(OriginGrabVolume, connectedClass.gameObject);
        }
        else if (connectedClass == TargetClass)
        {
            CalculateNewAttachment(TargetGrabVolume, connectedClass.gameObject);
        }
    }

    /// <summary>
    /// Updates the transform values of the connector and its related objects if the respective end of the connector is connected to a class.
    /// If it is not connected, it follows the position of the respective grab Volume.
    /// Position, rotation and scale are updated as needed for the connector line, its arrow head (if it has one) and the grab volumes (if they are not currently grabbed)
    /// </summary>
    public void UpdateTransform()
    {
        Vector3 newOriginPos;
        Vector3 newTargetPos;

        // Calculate origin and target position of the moved connector according to the new positions of its classes
        // This is done by transforming the target/origin positions local to the respective class back to global positions based on the new transform values of the classes
        if (OriginClass != null)
        {
            newOriginPos = OriginClass.transform.TransformPoint(_localOriginConnectionPoint);
        }
        else
        {
            newOriginPos = OriginGrabVolume.transform.position;
        }

        if (TargetClass != null)
        {
            newTargetPos = TargetClass.transform.TransformPoint(_localTargetConnectionPoint);
        }
        else
        {
            newTargetPos = TargetGrabVolume.transform.position;
        }

        // Calculate the vector that describes the orientation and length of the new connector
        Vector3 newConnectorLineSegment = newTargetPos - newOriginPos;

        // Move the connector to the new origin
        transform.position = newOriginPos;
        // Scale the connector to the correct new length
        transform.localScale =
            new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                newConnectorLineSegment.magnitude - (ArrowHead is not null ? ArrowHead.TipDistance : 0)
            );
        // Update the orientation of the connector
        transform.rotation = Quaternion.LookRotation(newConnectorLineSegment, Vector3.up);
        // Update the position of this connectors arrow head if it has one.
        ArrowHead?.UpdateTransform(newTargetPos, newConnectorLineSegment);

        _descriptionCanvas.transform.position = transform.position - (newOriginPos - newTargetPos) / 2 + (transform.up * 0.2f);
        _descriptionCanvas.transform.rotation = transform.rotation;
        _descriptionCanvas.transform.Rotate(new Vector3(0, 90, 0));

        OriginGrabVolume.UpdateTransform(newOriginPos);
        TargetGrabVolume.UpdateTransform(newTargetPos);
    }

    #endregion

    #region private Methods

    /// <summary>
    /// Resets the attaching position (local to the attached class) of the connectors given end to the given world position
    /// If the origin end is currently not attached to a class nothing happens.
    /// </summary>
    private void UpdateConnectionPointToClass(Vector3 connectionPointWorldPos, ConnectorEndType connectorEnd)
    {
        switch (connectorEnd)
        {
            case ConnectorEndType.Origin:
                if (OriginClass != null)
                {
                    _localOriginConnectionPoint = OriginClass.transform.InverseTransformPoint(connectionPointWorldPos);
                }
                break;
            case ConnectorEndType.Target:
                if (TargetClass != null)
                {
                    _localTargetConnectionPoint = TargetClass.transform.InverseTransformPoint(connectionPointWorldPos);
                }
                break;
        }
    }

    /// <summary>
    /// Attaches this connector to the given `ClassConnectionHub`.
    /// Attaches the end given in `connectorEndType`.
    /// </summary>
    /// <param name="attachClass"></param>
    /// <param name="connectorEndType"></param>
    public void AttachToClass(ClassConnectionHub attachClass, ConnectorEndType connectorEndType, Vector3 attachPosition)
    {
        //In case this end is still attached to another class, we first detach it
        DetachFromClass(connectorEndType);

        switch (connectorEndType)
        {
            case ConnectorEndType.Origin:
                attachClass.AddConnector(this);
                OriginClass = attachClass;
                _localOriginConnectionPoint = OriginClass.transform.InverseTransformPoint(attachPosition);
                break;
            case ConnectorEndType.Target:
                attachClass.AddConnector(this);
                TargetClass = attachClass;
                _localTargetConnectionPoint = TargetClass.transform.InverseTransformPoint(attachPosition);
                break;
        }
    }

    /// <summary>
    /// Detaches this connector's `connectorEndType` from the class attached to that end.
    /// (Removes the `ClassConnectionHub`'s reference as well.)
    /// </summary>
    /// <param name="connectorEndType"></param>
    private void DetachFromClass(ConnectorEndType connectorEndType)
    {
        switch (connectorEndType)
        {
            case ConnectorEndType.Origin:
                if (OriginClass != null)
                {
                    OriginClass.RemoveConnector(this);
                    OriginClass = null;
                }
                break;
            case ConnectorEndType.Target:
                if (TargetClass != null)
                {
                    TargetClass.RemoveConnector(this);
                    TargetClass = null;
                }
                break;
        }
    }

    #endregion

    public void ResetToStart(bool resetAttachment = true)
    {
        gameObject.GetComponentInParent<UmlElementHealth>()?.ResetHealth();

        if (resetAttachment)
        {
            LocalDetachFromClass(OriginGrabVolume);
            LocalDetachFromClass(TargetGrabVolume);

            OriginGrabVolume.ResetToStart();
            TargetGrabVolume.ResetToStart();
        }
    }

    public void InvokeOnConnectorGrabbed(SelectEnterEventArgs args)
    {
        OnConnectorGrabbed?.Invoke(GetComponent<UmlConnectorStructure>().ConnectionType);
    }

    public void SanityPrint()
    {
        Debug.Log("foo");
    }
}
