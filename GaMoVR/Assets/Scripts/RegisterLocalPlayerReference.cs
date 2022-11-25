using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterLocalPlayerReference : MonoBehaviour
{
    // Start is called before the first frame update
    public void Awake()
    {
        LocalPlayerReference.Instance.LocalPlayer = gameObject;
    }
}
