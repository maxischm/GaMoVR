using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetPlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshPro playerName;

    // Start is called before the first frame update
    void Start()
    {
        playerName.text = LocalPlayerReference.Instance.Nickname;
    }
}
