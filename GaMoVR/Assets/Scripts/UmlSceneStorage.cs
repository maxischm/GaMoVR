using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UmlSceneStorage : MonoBehaviour
{
    public static string LastScene
    {
        get; set;
    }

    public static string CurrentScene
    {
        get; set;
    }

    public static bool PlayedProfileIntroduction;

    public void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += UpdateLastScene;
    }

    public void UpdateLastScene(Scene scene, LoadSceneMode mode)
    {
        LastScene = CurrentScene;
        CurrentScene = scene.name;
    }
}
