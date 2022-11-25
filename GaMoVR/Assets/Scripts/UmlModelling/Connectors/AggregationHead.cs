using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggregationHead : ArrowHead
{
    public void Awake()
    {
        ConnectorType = UmlConnectorType.Aggregation;
        TipDistance = 0.1f;
    }
}
