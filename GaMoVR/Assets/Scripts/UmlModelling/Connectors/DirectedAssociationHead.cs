using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedAssociationHead : ArrowHead
{
    public void Awake()
    {
        ConnectorType = UmlConnectorType.DirectedAssociation;
        TipDistance = 0.1f;
    }
}
