using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual representation of a Class.
/// Connectors can be attached to this class and their transforms will be updated to stick to this class then the class is being moved.
/// </summary>
public class ClassConnectionHub : MonoBehaviour
{
    public List<Connector> Connectors;

    private bool _isMoving;

    #region MonoBehaviour Callbacks

    // Update is called once per frame
    public void Update()
    {
        if (_isMoving)
        {
            UpdateConnectorTransforms();
        }
    }

    public void OnDestroy()
    {
        foreach (Connector connector in Connectors.ToArray())
        {
            connector.OnDestroyAttachedClass(this);
        }
    }

    #endregion

    /// <summary>
    /// Adds the given connector to the list of connectors attached to this class.
    /// </summary>
    /// <param name="connector"></param>
    public void AddConnector(Connector connector)
    {
        Connectors.Add(connector);
    }

    public void RemoveConnector(Connector connector)
    {
        Connectors.Remove(connector);
    }

    public void UpdateAllConnections()
    {
        Connector[] connectorArray = Connectors.ToArray(); //Cache the initial connector list, because calculateNewAttachment will modify this list (removing and re-adding each element).
        foreach (Connector connector in connectorArray)
        {
            connector.CalculateNewAttachment(this);
        }
    }

    /// <summary>
    /// Makes the class ready to be moved. Connectors attached to this class will now update their positions, scale etc. to stick to the class while moving
    /// </summary>
    public void OnGrabBegin()
    {
        Debug.LogWarning("Beginning Movement");
        _isMoving = true;
    }

    /// <summary>
    /// Signals the end of a movement, so attached connectors do not have to be updated every frame anymore
    /// </summary>
    public void OnGrabEnd()
    {
        _isMoving = false;
    }

    /// <summary>
    /// Updates the transforms of attached connectors to stick to this class.
    /// </summary>
    private void UpdateConnectorTransforms()
    {
        foreach (Connector connector in Connectors)
        {
            connector.UpdateTransform();
        }
    }
}
