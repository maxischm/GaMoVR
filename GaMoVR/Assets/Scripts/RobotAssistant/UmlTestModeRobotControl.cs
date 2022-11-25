using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UmlTestModeRobotControl : MonoBehaviour
{
    [SerializeField]
    private AudioSource _testModeIntroductionMessage;

    [Space, SerializeField]
    private GameObject _helpPanel;

    [SerializeField]
    private GameObject _connectorHelpPanel;

    [SerializeField]
    private AudioSource _associationExplanation;

    [SerializeField]
    private AudioSource _compositionExplanation;

    [SerializeField]
    private AudioSource _inheritanceExplanation;

    [SerializeField]
    private AudioSource _gameModeExplanation;

    [SerializeField]
    private AudioSource _explanationControllerFunctions;

    [Space, SerializeField]
    private GameObject _helpCategories;

    [SerializeField]
    private AudioSource _helpIntroMessage;

    [SerializeField]
    private Animator _animator;

    private AudioSource _currentlyPlayingAudioSource;

    public void OnEnable()
    {
        if (!GamificationEngineConnection.Instance.PlayedGames.ContainsKey(StartHangmanLevel.Instance.SelectedLevel)
            || !GamificationEngineConnection.Instance.PlayedGames[StartHangmanLevel.Instance.SelectedLevel].Item1)
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
        _testModeIntroductionMessage.Play();

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
        _connectorHelpPanel.SetActive(!_connectorHelpPanel.activeSelf);
    }

    public void ReplayLastHint()
    {
        _currentlyPlayingAudioSource?.Stop();
        _currentlyPlayingAudioSource?.Play();
    }

    public void PlayAssociationExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _associationExplanation.Play();
        _currentlyPlayingAudioSource = _associationExplanation;
    }

    public void PlayCompositionExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _compositionExplanation.Play();
        _currentlyPlayingAudioSource = _compositionExplanation;
    }

    public void PlayInheritanceExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _inheritanceExplanation.Play();
        _currentlyPlayingAudioSource = _inheritanceExplanation;
    }

    public void PlayGameModeExplanation()
    {
        _currentlyPlayingAudioSource?.Stop();
        _gameModeExplanation.Play();
        _currentlyPlayingAudioSource = _gameModeExplanation;
    }

    public void ExplainControllerFunctions()
    {
        _currentlyPlayingAudioSource?.Stop();
        _explanationControllerFunctions.Play();
        _currentlyPlayingAudioSource = _explanationControllerFunctions;
    }
}
