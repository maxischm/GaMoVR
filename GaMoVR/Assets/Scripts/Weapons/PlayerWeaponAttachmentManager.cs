using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponAttachmentManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _backAttachment;

    [SerializeField]
    private Transform _playerCamera;

    [SerializeField]
    private Vector3 _headToBackOffset;

    public void Update()
    {
        transform.position = new Vector3(_playerCamera.transform.position.x, 0, _playerCamera.transform.position.z);

        _backAttachment.transform.position = _playerCamera.TransformPoint(_headToBackOffset);
        _backAttachment.transform.rotation = _playerCamera.rotation;
    }
}
