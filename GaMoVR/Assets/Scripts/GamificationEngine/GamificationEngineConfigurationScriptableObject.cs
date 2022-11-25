using UnityEngine;

[CreateAssetMenu(fileName = "GamificationEngineConfiguration", menuName = "GaMoVR/Gamification Engine Configuration Object", order = 0)]
public class GamificationEngineConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] string baseUrl = "localhost:8010";

    [SerializeField] string username = "";

    [SerializeField] string password = "";

    [SerializeField] string gameId = "";

    public string Username
    {
        get => username;
    }

    public string Password
    {
        get => password;
    }

    public string GameId
    {
        get => gameId;
    }

    public string GamificationEngineUrl
    {
        get { return $"http://{baseUrl}/gamification"; }
    }

    public string PostActionUrl
    {
        get { return $"{GamificationEngineUrl}/gengine/execute"; }
    }
}