using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorEndType
{
    Origin,
    Target
}

/// <summary>
/// This Volume is a trigger collider that sits at one end of a connector.
/// It is responsible to capture the grab input of the player and telling the connector when it needs to detach/attach to a class and when it has to move based on the player grabbing and moving this volume.
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
public class ConnectorGrabVolume : MonoBehaviour
{
    /// <summary>
    /// The connector this Grab volume belongs to
    /// </summary>
    public Connector connector;

    public bool Locked;

    [Tooltip("The radius of the volume mesh, without the objects local scaling")]
    public float baseMeshRadius = 0.5f;

    /// <summary>
    /// The renderer responsible for rendering the GrabVolume, when a Hand enters it.
    /// </summary>
    private MeshRenderer meshRenderer;

    /// <summary>
    /// A list that contains all player hands that the grab volume currently contains.
    /// The GrabVolume is hidden when this list is empty and visible if the list has at least one element.
    /// </summary>
    private readonly List<GameObject> containedHands = new();

    private bool isGrabbed;

    public GameObject intersectedClass;

    public GameObject removedClass;

    [SerializeField]
    private Material _defaultMaterial;

    [SerializeField]
    private Material _greenMaterial;

    [SerializeField]
    private Material _redMaterial;

    [SerializeField]
    private XROffsetGrabbable _grabInteraction;

    public delegate void UmlConnectorGrabVolumeGrabEvent(bool bothEndsAttached);
    public static event UmlConnectorGrabVolumeGrabEvent OnConnectorGrabVolumeGrabbed;

    public void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public void Update()
    {
        if (isGrabbed && !Locked)
        {
            //This grab volume is grabbed, so it is being moved, so we update the connectors scale and positioning to follow this grab volume
            connector.UpdateTransform();
        }
    }

    /// <summary>
    /// Updates the ConnectorGrabVolume's position if the volume is not currently grabbed
    /// </summary>
    /// <param name="grabVolumePosition"></param>
    public void UpdateTransform(Vector3 grabVolumePosition)
    {
        if (isGrabbed)
        {
            //if the grab volume is currently grabbed the grab is driving the positioning. Therefore, transform update orders from the connector are ignored
            return;
        }

        transform.position = grabVolumePosition;
    }

    public void OnGrabBegin()
    {
        if (!Locked)
        {
            isGrabbed = true;
            connector.LocalDetachFromClass(this);
            meshRenderer.enabled = true;
        }
    }

    public void OnGrabEnd(bool skipValidation = false)
    {
        if (!Locked)
        {
            isGrabbed = false;

            //update this local connectors attach state and transform, the transform gets synchronized via PhotonViews
            if (intersectedClass != null)
            {
                var updatedSide = connector.OriginGrabVolume == this ? UmlConnectionSide.Origin : UmlConnectionSide.Target;
                string originClassName = connector.OriginGrabVolume.intersectedClass != null
                    ? connector.OriginGrabVolume?.intersectedClass?.GetComponent<UmlClassStructure>().Name
                    : null;
                string targetClassName = connector.TargetGrabVolume.intersectedClass != null
                    ? connector.TargetGrabVolume?.intersectedClass?.GetComponent<UmlClassStructure>().Name
                    : null;
                if (skipValidation ||
                    UmlHangmanLevelValidator.Instance.ValidateConnectorAttachment(
                    connectorType: connector.ArrowHead is not null
                        ? connector.ArrowHead.ConnectorType
                        : UmlConnectorType.DirectedAssociation,
                    updatedSide: updatedSide,
                    description: connector.Description,
                    originClassName: originClassName,
                    targetClassName: targetClassName
                ))
                {
                    Locked = true;
                    meshRenderer.material = _greenMaterial;
                    _grabInteraction.enabled = false;
                    if (!skipValidation)
                    {
                        UmlHangmanLevelValidator.Instance.PlayerSoundManager.PlayCorrectMoveSound();
                        OnConnectorGrabVolumeGrabbed?.Invoke(targetClassName is not null && originClassName is not null);
                    }

                    connector.transform.parent.GetComponent<XROffsetGrabbable>().enabled = false;

                    // intersectedClass.GetComponent<ClassConnectionHub>().AddConnector(connector);
                    connector.AttachToClass(
                        intersectedClass.GetComponent<ClassConnectionHub>(),
                        connector.OriginGrabVolume == this ? ConnectorEndType.Origin : ConnectorEndType.Target,
                        connector.OriginGrabVolume == this
                            ? connector.OriginGrabVolume.transform.position
                            : connector.TargetGrabVolume.transform.position
                    );
                }
                else
                {
                    meshRenderer.material = _redMaterial;
                    meshRenderer.enabled = true;
                    UmlHangmanLevelValidator.Instance.PlayerSoundManager.PlayWrongMoveSound();
                }
                intersectedClass.GetComponent<Outline>().enabled = false;
            }
            else
            {
                meshRenderer.material = _defaultMaterial;
                meshRenderer.enabled = false;
            }

            containedHands.Clear();
        }
    }

    public void OnGrabEndBuildingMode(bool skipAttachmentCheck = false)
    {
        isGrabbed = false;

        if (removedClass != null)
        {
            meshRenderer.material = _greenMaterial;
            // _grabInteraction.enabled = false;

            if (connector.OriginGrabVolume.intersectedClass == null && connector.TargetGrabVolume.intersectedClass == null)
            {
                connector.transform.parent.GetComponent<XROffsetGrabbable>().enabled = true;
            }

            if (connector.OriginGrabVolume == this && connector.TargetGrabVolume.intersectedClass != null
                || connector.TargetGrabVolume == this && connector.OriginGrabVolume.intersectedClass != null)
            {
                // origin grab volume has removed class != null but target grab volume is still intersecting
                // => remove connection in UML between removed class & target volume intersected class
                UmlModel.Instance.RemoveConnectionInUml(connector.transform.parent.gameObject, true, false);
            }
        }

        //update this local connectors attach state and transform, the transform gets synchronized via PhotonViews
        if (intersectedClass != null)
        {
            string originClassName = connector.OriginGrabVolume.intersectedClass != null
                ? connector.OriginGrabVolume?.intersectedClass?.GetComponent<UmlClassStructure>().Name
                : null;
            string targetClassName = connector.TargetGrabVolume.intersectedClass != null
                ? connector.TargetGrabVolume?.intersectedClass?.GetComponent<UmlClassStructure>().Name
                : null;

            meshRenderer.material = _greenMaterial;
            // _grabInteraction.enabled = false;

            connector.AttachToClass(
                intersectedClass.GetComponent<ClassConnectionHub>(),
                connector.OriginGrabVolume == this ? ConnectorEndType.Origin : ConnectorEndType.Target,
                connector.OriginGrabVolume == this
                    ? connector.OriginGrabVolume.transform.position
                    : connector.TargetGrabVolume.transform.position
            );
            connector.transform.parent.GetComponent<XROffsetGrabbable>().enabled = false;

            intersectedClass.GetComponent<Outline>().enabled = false;

            if (!skipAttachmentCheck && originClassName is not null && targetClassName is not null)
            {
                if (originClassName == "Wing" && targetClassName == "ShipBody")
                {
                    connector.UpdateTransform();
                    UmlModel.Instance.AddConnectionInUml("Wing", "ShipBody", transform.parent.gameObject);
                }
                else if (originClassName == "Thruster" && targetClassName == "Wing")
                {
                    connector.UpdateTransform();
                    UmlModel.Instance.AddConnectionInUml("Thruster", "Wing", transform.parent.gameObject);
                }
                else if (originClassName == "Thruster" && targetClassName == "ShipBody")
                {
                    connector.UpdateTransform();
                    UmlModel.Instance.AddConnectionInUml("Thruster", "ShipBody", transform.parent.gameObject);
                }
                else if (originClassName == "Cannon" && targetClassName == "ShipBody")
                {
                    connector.UpdateTransform();
                    UmlModel.Instance.AddConnectionInUml("Cannon", "ShipBody", transform.parent.gameObject);
                }
                else if (originClassName == "Cannon" && targetClassName == "Wing")
                {
                    connector.UpdateTransform();
                    UmlModel.Instance.AddConnectionInUml("Cannon", "Wing", transform.parent.gameObject);
                }
                else if (originClassName == "Cannon" && targetClassName == "Thruster")
                {
                    UmlBuildModeRobotController.Instance.PlayForbiddenConnectionWarning();
                    connector.TargetGrabVolume.meshRenderer.material = connector.TargetGrabVolume._redMaterial;
                    connector.TargetGrabVolume.meshRenderer.enabled = true;
                    connector.OriginGrabVolume.meshRenderer.material = connector.OriginGrabVolume._redMaterial;
                    connector.OriginGrabVolume.meshRenderer.enabled = true;
                }
                else
                {
                    SwitchOriginAndTarget();
                }
            }
        }
        else
        {
            meshRenderer.material = _defaultMaterial;
            meshRenderer.enabled = false;
        }
    }

    private void SwitchOriginAndTarget()
    {
        // Rotate to get directions uniform for every association between the two
        var oldTargetGrabVolumePosition = connector.TargetGrabVolume.transform.position;
        var oldOriginGrabVolumePosition = connector.OriginGrabVolume.transform.position;

        connector.TargetGrabVolume.transform.position = oldOriginGrabVolumePosition;
        connector.OriginGrabVolume.transform.position = oldTargetGrabVolumePosition;

        var targetClass = connector.OriginGrabVolume.intersectedClass;
        var originClass = connector.TargetGrabVolume.intersectedClass;

        connector.OriginGrabVolume.intersectedClass = originClass;
        connector.TargetGrabVolume.intersectedClass = targetClass;

        connector.OriginGrabVolume.OnGrabEndBuildingMode(true);
        connector.TargetGrabVolume.OnGrabEndBuildingMode();

        // connector.OriginClass = null;
        // connector.TargetClass = null;

        // connector.UpdateTransform();
    }

    /// <summary>
    /// Adds the entering game object to a list of hands, if the entered collider belongs to a GrabHand and shows the visual representation of the GrabVolume
    /// </summary>
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OpenXRRightHand") || other.CompareTag("OpenXRLeftHand"))
        {
            containedHands.Add(other.gameObject);
            meshRenderer.enabled = true;
        }
        else if (other.CompareTag("UmlClass") && isGrabbed)
        {
            intersectedClass = other.gameObject;
            intersectedClass.GetComponent<Outline>().enabled = true;
        }
        else if (other.CompareTag("UmlClassCanvas") && isGrabbed)
        {
            intersectedClass = other.transform.parent.gameObject;
            intersectedClass.GetComponent<Outline>().enabled = true;
        }
    }

    /// <summary>
    /// Removes the entering game object from the list of hands, if the entered collider belongs to a GrabHand.
    /// And hides the visual representation of the GrabVolume if there is no other hand still in the grabVolume.
    /// </summary>
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OpenXRRightHand") || other.CompareTag("OpenXRLeftHand"))
        {
            containedHands.Remove(other.gameObject);
            if (containedHands.Count == 0 && !Locked && meshRenderer.material != _redMaterial)
            {
                meshRenderer.enabled = false;
            }
        }
        else if ((other.CompareTag("UmlClass") || other.CompareTag("UmlClassCanvas")) && intersectedClass is not null)
        {
            removedClass = intersectedClass;
            intersectedClass.GetComponent<Outline>().enabled = false;
            intersectedClass = null;
        }
    }

    public void ResetToStart()
    {
        Locked = false;
        isGrabbed = false;
        meshRenderer.material = _defaultMaterial;
        meshRenderer.enabled = false;
        containedHands.Clear();
        intersectedClass = null;
        _grabInteraction.enabled = true;
    }
}
