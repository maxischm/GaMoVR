using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionErrorUiHandler : MonoBehaviour
{
    [SerializeField] GameObject errorDisplayObject;

    public void OnEnable()
    {
        GamificationEngineConnection.OnError += ShowError;
    }

    public void OnDisable()
    {
        GamificationEngineConnection.OnError -= ShowError;
    }

    private void ShowError(UnityWebRequest.Result result)
    {
        errorDisplayObject.SetActive(true);
    }
}
