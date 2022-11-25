using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class LocalPlayerReference : GenericStaticClass<LocalPlayerReference>
{
    private GameObject _localPlayer;

    public GameObject LocalPlayer
    {
        get
        {
            if (_localPlayer is null)
            {
                Debug.LogError("Local player is not assigned");
            }

            return _localPlayer;
        }
        set => _localPlayer = value;
    }

    public string Nickname { get; set; } = "test";
}