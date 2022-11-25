using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThisTransformLikePlayer : MonoBehaviour
{
    [SerializeField]
    private Transform transformSource;

    public void Update()
    {
        if (transformSource is null)
        {
            transformSource = LocalPlayerReference.Instance.LocalPlayer?.transform.
                GetComponentInChildren<XRPlayerComponents>().NonXRPlayerComponents.HeadTip;
        }
        else
        {
            var transform1 = transform;
            transform1.position = transformSource.position;
            transform1.rotation = transformSource.rotation;
        }
    }
}
