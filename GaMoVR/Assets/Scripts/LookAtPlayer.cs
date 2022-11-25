using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform transformSource;

    // Update is called once per frame
    public void Update()
    {
        if (transformSource is null)
        {
            transformSource = LocalPlayerReference.Instance.LocalPlayer?.transform.
                GetComponentInChildren<XRPlayerComponents>().NonXRPlayerComponents.HeadTip;
        }
        else
        {
            var targetDir = transformSource.position - transform.position;
            targetDir.y = 0;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(targetDir),
                Time.deltaTime * 70
            );
        }
    }
}
