using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class UmlUnlockableElement
{
    public static readonly string Aggregation = "aggregation";

    public static readonly string Inheritance = "inheritance";

    public static readonly string DirectedAssociation = "directedAssociation";

    public static readonly string Composition = "composition";
}

public class GamificationEngineConnection : GenericSingletonClass<GamificationEngineConnection>
{
    [SerializeField] GamificationEngineConfigurationScriptableObject connectionConfiguration;

    private float _playerXp = -1;
    public float PlayerXp
    {
        get => _playerXp;
    }

    private float _playerLives = -1;
    public float PlayerLives
    {
        get => _playerLives;
        set => _playerLives = value;
    }

    private HashSet<string> _unlockedElements = new();
    public HashSet<string> UnlockedElements
    {
        get => _unlockedElements;
    }

    private Dictionary<float, Tuple<bool, float>> _playedGames = new();
    public Dictionary<float, Tuple<bool, float>> PlayedGames
    {
        get => _playedGames;
    }

    private Dictionary<float, Tuple<bool, float>> _playedGamesCopy = new();

    private int _availableUnlocks = 0;
    public int AvailableUnlocks
    {
        get => _availableUnlocks;
    }

    private UmlPlayerStatusResponse _lastPlayerState;

    public delegate void PlayerStatusUpdate();
    public static event PlayerStatusUpdate OnPlayerStatusUpdate;

    public delegate void GamificationEngineConnectionEvent(UnityWebRequest.Result result);
    public static event GamificationEngineConnectionEvent OnError;

    public void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Taken from https://stackoverflow.com/a/39489237
    /// Create Basic Authentication header string.
    /// </summary>
    /// <param name="username">Username of the user to authenticate.</param>
    /// <param name="password">Password used for authentication.</param>
    /// <returns>Basic Authentication string.</returns>
    string CreateAuthenticationString(string username, string password)
    {
        string auth = $"{username}:{password}";
        auth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = $"Basic {auth}";
        return auth;
    }

    /// <summary>
    /// Taken from https://stackoverflow.com/a/39489237
    /// </summary>
    /// <returns>IEnumerator of the sent request.</returns>
    public IEnumerator SendRequest(UnityWebRequest request, Action<string> resultCallback)
    {
        string authorization = CreateAuthenticationString(connectionConfiguration.Username, connectionConfiguration.Password);

        request.SetRequestHeader("AUTHORIZATION", authorization);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Request failed");
        }
        else if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Connection Error: Gamification Engine not reachable");
            OnError?.Invoke(UnityWebRequest.Result.ConnectionError);
        }
        else if (request.downloadHandler != null)
        {
            resultCallback?.Invoke(request.downloadHandler.text);
        }
        else
        {
            resultCallback?.Invoke(request.result.ToString());
        }
    }

    /// <summary>
    /// Reads the state of the game with `gameId` and for player `playerId` and executes `resultCallback`
    /// on the result if available.
    /// </summary>
    /// <param name="playerId">ID of the player to read the state for.</param>
    /// <param name="resultCallback">Action to execute on the result body.</param>
    /// <returns>IEnumerator with the sent request.</returns>
    public IEnumerator ReadPlayerState(string playerId, Action<string> resultCallback)
    {
        string getStateUrl = $"{connectionConfiguration.GamificationEngineUrl}/data/game/{connectionConfiguration.GameId}/player/{playerId}/state";

        UnityWebRequest request = UnityWebRequest.Get(getStateUrl);

        return SendRequest(request, resultCallback);
    }

    public IEnumerator FetchPlayerState(Action playerStateFetchingCallback)
    {
        if (LocalPlayerReference.Instance.Nickname.Length == 0)
        {
            Debug.LogError("Player name was empty");
            yield break;
        }

        yield return StartCoroutine(
            ReadPlayerState(LocalPlayerReference.Instance.Nickname, (result) =>
                _lastPlayerState = JsonConvert.DeserializeObject<UmlPlayerStatusResponse>(result)));

        if (_lastPlayerState.State != null && _lastPlayerState.State.PointConcept != null)
        {
            _playerXp = (float)_lastPlayerState.State.PointConcept.Find(pc => pc.Name == "xp").Score;
            _playerLives = (float)_lastPlayerState.State?.PointConcept.Find(pc => pc.Name == "lives").Score;
        }

        if (_lastPlayerState.CustomData != null && _lastPlayerState.CustomData.PlayedGames != null)
        {
            foreach (var game in _lastPlayerState.CustomData.PlayedGames)
            {
                _playedGames[(float)game.Level] = new((bool)game.Won, (float)game.Lives);
            }
        }
        if (_lastPlayerState.CustomData != null && _lastPlayerState.CustomData.UnlockedElements != null)
        {
            foreach (var unlockedElement in _lastPlayerState.CustomData.UnlockedElements)
            {
                _unlockedElements.Add(unlockedElement.Key);
            }
        }

        _availableUnlocks = ((int)_playerXp / 200) - _unlockedElements.Count + 1; // +1 because the directedAssociation is unlocked without needing XP for it

        playerStateFetchingCallback?.Invoke();

        OnPlayerStatusUpdate?.Invoke();
    }

    public IEnumerator StartNewLevel(string playerId, float levelId, float initialLives, Action<string> resultCallback)
    {
        _playedGamesCopy = _playedGames.Copy();

        var action = new GamificationAction(connectionConfiguration.GameId, playerId, "startNewLevel");
        action.data.Add("levelID", levelId);
        action.data.Add("initialLives", initialLives);

        var request = new UnityWebRequest(connectionConfiguration.PostActionUrl, "POST");
        var uploadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action, Formatting.Indented));
        request.uploadHandler = new UploadHandlerRaw(uploadData) { contentType = "application/json" };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        return SendRequest(request, resultCallback);
    }

    public IEnumerator ReduceLives(string playerId, Action<string> resultCallback)
    {
        var action = new GamificationAction(connectionConfiguration.GameId, playerId, "executedWrongMove");

        var request = new UnityWebRequest(connectionConfiguration.PostActionUrl, "POST");
        var uploadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action, Formatting.Indented));
        request.uploadHandler = new UploadHandlerRaw(uploadData) { contentType = "application/json" };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        return SendRequest(request, resultCallback);
    }

    public IEnumerator FinishGame(
        string playerId,
        float levelId,
        bool won,
        float xp,
        float remainingLivesUponCompletion,
        Action<string> resultCallback
    )
    {
        var action = new GamificationAction(connectionConfiguration.GameId, playerId, "finishGame");
        action.data.Add("levelID", levelId);
        action.data.Add("xp", xp);

        // Only update if game was not won before
        if (won || (_playedGames.ContainsKey(levelId) && _playedGames[levelId].Item1))
        {
            action.data.Add("won", true);
            action.data.Add("remainingLivesUponCompletion", remainingLivesUponCompletion);
        }
        else
        {
            action.data.Add("won", false);
            action.data.Add("remainingLivesUponCompletion", remainingLivesUponCompletion);
        }

        var request = new UnityWebRequest(connectionConfiguration.PostActionUrl, "POST");
        var uploadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action, Formatting.Indented));
        request.uploadHandler = new UploadHandlerRaw(uploadData) { contentType = "application/json" };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        return SendRequest(request, resultCallback);
    }

    public IEnumerator UnlockElement(string elementId, Action<string> resultCallback)
    {
        if (_unlockedElements.Contains(elementId))
        {
            yield break;
        }

        _availableUnlocks--;
        _unlockedElements.Add(elementId);

        var action = new GamificationAction(connectionConfiguration.GameId, LocalPlayerReference.Instance.Nickname, "unlockElement");
        action.data.Add("key", elementId);
        action.data.Add("name", elementId);

        var request = new UnityWebRequest(connectionConfiguration.PostActionUrl, "POST");
        var uploadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action, Formatting.Indented));
        request.uploadHandler = new UploadHandlerRaw(uploadData) { contentType = "application/json" };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        yield return SendRequest(request, resultCallback);

        OnPlayerStatusUpdate?.Invoke();
    }

    public IEnumerator QuitLevel(string playerId, float levelId, Action<string> resultCallback)
    {
        var action = new GamificationAction(connectionConfiguration.GameId, playerId, "finishGame");
        action.data.Add("levelID", levelId);
        action.data.Add("xp", 0.0f);

        // Set to value before level was started due to not finishing level -> not change to state
        action.data.Add("won", _playedGamesCopy.ContainsKey(levelId) && _playedGamesCopy[levelId].Item1);
        action.data.Add(
            "remainingLivesUponCompletion",
            _playedGamesCopy.ContainsKey(levelId) ? _playedGamesCopy[levelId].Item2 : 0
        );

        var request = new UnityWebRequest(connectionConfiguration.PostActionUrl, "POST");
        var uploadData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action, Formatting.Indented));
        request.uploadHandler = new UploadHandlerRaw(uploadData) { contentType = "application/json" };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        return SendRequest(request, resultCallback);
    }
}
