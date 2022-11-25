using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositionHead : ArrowHead
{
    public void Awake()
    {
        ConnectorType = UmlConnectorType.Composition;
        TipDistance = 0.1f;
    }
}
