using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipPartMenuEntry : MonoBehaviour
{
    [SerializeField]
    private Image _backgroundImage;

    public void ToggleBackgroundImage()
    {
        _backgroundImage.enabled = !_backgroundImage.enabled;
    }
}
