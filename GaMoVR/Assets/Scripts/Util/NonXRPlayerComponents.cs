using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonXRPlayerComponents : MonoBehaviour
{

    [SerializeField] private Transform headTip;
    [SerializeField] private Transform cameraAttachmentPosition;

    [SerializeField] private Transform hipPosition;

    public Transform CameraAttachmentPosition => cameraAttachmentPosition;

    public Transform HipPosition => hipPosition;
    public Transform HeadTip => headTip;
}
