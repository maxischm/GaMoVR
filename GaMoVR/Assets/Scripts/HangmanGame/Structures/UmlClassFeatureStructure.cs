using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlClassFeatureStructure : MonoBehaviour
{
    public Vector3 InitPosition;

    public void Init(Vector3 initPosition)
    {
        InitPosition = initPosition;
    }

    public void ResetToStart()
    {
        gameObject.SetActive(true);

        gameObject.transform.position = InitPosition;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
