using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UmlLobbyRobotController : MonoBehaviour
{
    [SerializeField]
    private AudioSource _introductionMessage;

    [Space, SerializeField]
    private AudioSource _welcomingBackMessage;

    [SerializeField]
    private AudioClip _welcomeBackAudioClip;

    [SerializeField]
    private AudioClip _welcomeBackWithUnlocksAudioClip;

    [Space, SerializeField]
    private AudioSource _gamesIntroductionMessage;
    private bool _playedGamesIntroductionMessage;

    [SerializeField]
    private AudioSource _levelSelectionMessage;
    private bool _playedLevelSelectionMessage;

    [SerializeField]
    private AudioSource _profileExplanationMessage;

    [Space, SerializeField]
    private Animator _animator;

    [Space, SerializeField]
    private GameObject _helpCategories;

    [SerializeField]
    private AudioSource _whatToDoMessage;

    [SerializeField]
    private AudioSource _helpIntroMessage;

    private AudioSource _currentlyPlayingMessage;

    [Space, SerializeField]
    private List<AudioClip> _retrySuggestionAudioList;

    [SerializeField]
    private AudioClip _newLevelsAvailableClip;

    [SerializeField]
    private AudioSource _retrySuggestionAudioSource;

    [SerializeField]
    private AudioSource _levelLockedNotification;

    [Space, SerializeField]
    private FillLevelList _levelListManager;

    public void OnEnable()
    {
        SceneManager.sceneLoaded += LobbyLoaded;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= LobbyLoaded;
    }

    public void LobbyLoaded(Scene scene, LoadSceneMode mode)
    {
        if (UmlSceneStorage.LastScene is null)
        {
            StartIntroductionMessage();
        }
        else
        {
            StartWelcomingBackMessage();
        }
    }

    public void StartIntroductionMessage()
    {
        _currentlyPlayingMessage?.Stop();
        _introductionMessage.PlayDelayed(1);
        _currentlyPlayingMessage = _introductionMessage;
        _animator.SetTrigger("StartTalking");
    }

    public void StartWelcomingBackMessage()
    {
        _currentlyPlayingMessage?.Stop();

        if (GamificationEngineConnection.Instance.AvailableUnlocks > 0)
        {
            _welcomingBackMessage.clip = _welcomeBackWithUnlocksAudioClip;
        }
        else
        {
            _welcomingBackMessage.clip = _welcomeBackAudioClip;
        }

        _welcomingBackMessage.PlayDelayed(0.5f);
        _playedGamesIntroductionMessage = true;
        _playedLevelSelectionMessage = true;
        _animator.SetTrigger("Greet");
        _currentlyPlayingMessage = _welcomingBackMessage;
    }

    public void StartGamesIntroduction()
    {
        _currentlyPlayingMessage?.Stop();
        if (UmlSceneStorage.LastScene is null && !_playedGamesIntroductionMessage)
        {
            _gamesIntroductionMessage?.Play();
            _currentlyPlayingMessage = _gamesIntroductionMessage;
            _animator.SetTrigger("StartTalking");
            _playedGamesIntroductionMessage = true;
        }
    }

    public void StartProfileViewExplanation()
    {
        _currentlyPlayingMessage?.Stop();
        if (!UmlSceneStorage.PlayedProfileIntroduction)
        {
            _currentlyPlayingMessage = _profileExplanationMessage;
            _profileExplanationMessage.Play();
            UmlSceneStorage.PlayedProfileIntroduction = true;
        }
    }

    public void StartLevelSelectionIntroduction()
    {
        _currentlyPlayingMessage?.Stop();
        if (UmlSceneStorage.LastScene is null && !_playedLevelSelectionMessage)
        {
            _levelSelectionMessage.Play();
            _currentlyPlayingMessage = _levelSelectionMessage;
            _animator.SetTrigger("StartTalking");
            _playedLevelSelectionMessage = true;
        }
        else if (HasNewLevels())
        {
            _retrySuggestionAudioSource.clip = _newLevelsAvailableClip;
            _retrySuggestionAudioSource.Play();
            _currentlyPlayingMessage = _retrySuggestionAudioSource;
        }
        else
        {
            SuggestFailedLevel();
        }
    }

    public void PlayLevelLockedNotification()
    {
        _currentlyPlayingMessage?.Stop();
        _levelLockedNotification.Play();
        _currentlyPlayingMessage = _levelLockedNotification;
    }

    public void ReplayLastHint()
    {
        _currentlyPlayingMessage?.Stop();
        _currentlyPlayingMessage.Play();
    }

    public void ToggleHelpCategories()
    {
        _helpCategories.SetActive(!_helpCategories.activeSelf);
        if (_helpCategories.activeSelf)
        {
            _helpIntroMessage.Play();
        }
    }

    public void PlayWhatToDo()
    {
        _currentlyPlayingMessage.Stop();
        _whatToDoMessage.Play();
        _currentlyPlayingMessage = _whatToDoMessage;
    }

    public void SuggestFailedLevel()
    {
        _currentlyPlayingMessage.Stop();
        var randomIndex = FindRandomFailedLevelIndex();
        if (randomIndex != -1)
        {
            _retrySuggestionAudioSource.clip = _retrySuggestionAudioList[randomIndex];
            _retrySuggestionAudioSource.Play();
            _currentlyPlayingMessage = _retrySuggestionAudioSource;
        }
    }

    private int FindRandomFailedLevelIndex()
    {
        if (_levelListManager.CompleteLevelList.Count > 0)
        {
            var failedLevelIndices = new List<int>();
            for (var i = 0; i < _levelListManager.CompleteLevelList.Count; i++)
            {
                if (GamificationEngineConnection.Instance.PlayedGames.ContainsKey(i)
                    && !GamificationEngineConnection.Instance.PlayedGames[i].Item1)
                {
                    failedLevelIndices.Add(i);
                }
            }
            if (failedLevelIndices.Count > 0)
            {
                return failedLevelIndices[new System.Random().Next(failedLevelIndices.Count)];
            }
        }

        return -1;
    }

    private bool HasNewLevels()
    {
        if (_levelListManager.CompleteLevelList.Count > 0)
        {
            for (var i = 0; i < _levelListManager.CompleteLevelList.Count; i++)
            {
                if (_levelListManager.CompleteLevelList[i].requiredModelingElements.All(element =>
                        GamificationEngineConnection.Instance.UnlockedElements.Contains(element))
                    && !GamificationEngineConnection.Instance.PlayedGames.ContainsKey(i))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
