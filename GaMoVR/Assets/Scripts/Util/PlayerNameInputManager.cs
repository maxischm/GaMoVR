using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInputManager : MonoBehaviour
{
    [SerializeField] InputField nameInputField;

    // Start is called before the first frame update
    public void Start()
    {
        LocalPlayerReference.Instance.Nickname = nameInputField.text;
    }

    public void SetPlayerNickname(string Name)
    {
        LocalPlayerReference.Instance.Nickname = Name;
    }
}
