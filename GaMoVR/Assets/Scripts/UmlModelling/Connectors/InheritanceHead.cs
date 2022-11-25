using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritanceHead : ArrowHead
{
    public void Awake()
    {
        ConnectorType = UmlConnectorType.Inheritance;
        TipDistance = 0.1f;
    }
}
