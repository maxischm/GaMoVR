using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public void CreateRoom()
    {
        SceneManager.LoadScene("UmlLearningLobby");
    }
}