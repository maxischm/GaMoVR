using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UmlAgainstTheClockRobotControl : MonoBehaviour
{
    [SerializeField]
    private AudioSource _againstTheClockIntroductionMessage;

    [SerializeField]
    private Animator _animator;

    [Space, SerializeField]
    private GameObject _helpCategories;

    [SerializeField]
    private GameObject _connectorExplanationPanel;

    [SerializeField]
    private AudioSource _associationExplanation;

    [SerializeField]
    private AudioSource _inheritanceExplanation;

    [SerializeField]
    private AudioSource _aggregationExplanation;

    [SerializeField]
    private AudioSource _compositionExplanation;

    [SerializeField]
    private AudioSource _gameModeExplanation;

    [SerializeField]
    private AudioSource _controlsExplanation;

    [SerializeField]
    private AudioSource _helpIntroMessage;

    private AudioSource _currentlyPlayingAudioSource;

    public void OnEnable()
    {
        if (!GamificationEngineConnection.Instance.PlayedGames.ContainsKey(StartHangmanLevel.Instance.SelectedLevel)
            || GamificationEngineConnection.Instance.PlayedGames[StartHangmanLevel.Instance.SelectedLevel].Item1)
        {
            SceneManager.sceneLoaded += TutorialLoaded;
        }
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= TutorialLoaded;
    }

    public void TutorialLoaded(Scene scene, LoadSceneMode mode)
    {
        StartTutorialIntroductionMessage();
    }

    public void StartTutorialIntroductionMessage()
    {
        _againstTheClockIntroductionMessage.Play();

        _animator.SetTrigger("StartTalking");
    }

    public void ToggleHelpCategories()
    {
        _helpCategories.SetActive(!_helpCategories.activeSelf);
        if (_helpCategories.activeSelf)
        {
            _helpIntroMessage.Play();
        }
    }

    public void ToggleConnectorPanel()
    {
        _connectorExplanationPanel.SetActive(!_connectorExplanationPanel.activeSelf);
    }

    public void PlayAssociationExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _associationExplanation.Play();
        _currentlyPlayingAudioSource = _associationExplanation;
    }

    public void PlayAggregationExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _aggregationExplanation.Play();
        _currentlyPlayingAudioSource = _aggregationExplanation;
    }

    public void PlayInheritanceExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _inheritanceExplanation.Play();
        _currentlyPlayingAudioSource = _inheritanceExplanation;
    }

    public void PlayCompositionExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _compositionExplanation.Play();
        _currentlyPlayingAudioSource = _compositionExplanation;
    }

    public void PlayGameModeExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _gameModeExplanation.Play();
        _currentlyPlayingAudioSource = _gameModeExplanation;
    }

    public void ReplayLastHint()
    {
        _currentlyPlayingAudioSource?.Stop();
        _currentlyPlayingAudioSource?.Play();
    }

    public void PlayControlsExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _controlsExplanation.Play();
        _currentlyPlayingAudioSource = _controlsExplanation;
    }
}
