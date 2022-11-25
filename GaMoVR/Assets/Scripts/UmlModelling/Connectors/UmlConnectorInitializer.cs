using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UmlConnectorInitializer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _description;

    public void InitConnector(string description = "")
    {
        if (!string.IsNullOrEmpty(description))
        {
            _description.SetText(description);
        }
    }
}
