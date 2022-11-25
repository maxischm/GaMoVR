using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UmlTutorialRobotController : MonoBehaviour
{
    [SerializeField]
    private bool _playTutorial;

    [SerializeField]
    private AudioSource _tutorialIntroductionMessage;

    [SerializeField]
    private AudioSource _connectionFirstTutorial;
    private bool _playedConnectionFirstTutorial;

    [SerializeField]
    private AudioSource _connectionSecondTutorial;

    [SerializeField]
    private AudioSource _connectionThirdTutorial;
    private bool _playedConnectionThirdTutorial;

    [SerializeField]
    private AudioSource _featureFirstTutorial;
    private bool _playedFeatureFirstTutorial;

    [SerializeField]
    private AudioSource _featureSecondTutorialMethod;
    private bool _playedFeatureSecondTutorial;

    [SerializeField]
    private AudioSource _featureSecondTutorialProperty;

    private AudioSource _currentlyPlayingAudioSource;

    [SerializeField]
    private Animator _animator;

    [Space, SerializeField]
    private GameObject _helpCategories;

    [SerializeField]
    private AudioSource _explanationAssociations;

    [SerializeField]
    private AudioSource _explanationGameMode;

    [SerializeField]
    private AudioSource _explanationControllerFunctions;

    [SerializeField]
    private AudioSource _helpIntroMessage;

    public void OnEnable()
    {
        if (_playTutorial)
        {
            SceneManager.sceneLoaded += TutorialLoaded;

            Connector.OnConnectorGrabbed += StartConnectionFirstTutorial;

            ConnectorGrabVolume.OnConnectorGrabVolumeGrabbed += StartConnectionSecondTutorial;
            ConnectorGrabVolume.OnConnectorGrabVolumeGrabbed += StartConnectionThirdTutorial;

            ClassFeatureManager.OnClassFeatureGrabbed += StartFeatureFirstTutorial;
            ClassFeatureManager.OnClassFeaturePlaced += StartFeatureSecondTutorial;
        }
    }

    public void OnDestroy()
    {
        if (_playTutorial)
        {
            SceneManager.sceneLoaded -= TutorialLoaded;

            Connector.OnConnectorGrabbed -= StartConnectionFirstTutorial;

            ConnectorGrabVolume.OnConnectorGrabVolumeGrabbed -= StartConnectionSecondTutorial;
            ConnectorGrabVolume.OnConnectorGrabVolumeGrabbed -= StartConnectionThirdTutorial;

            ClassFeatureManager.OnClassFeatureGrabbed -= StartFeatureFirstTutorial;
            ClassFeatureManager.OnClassFeaturePlaced -= StartFeatureSecondTutorial;
        }
    }

    public void TutorialLoaded(Scene scene, LoadSceneMode mode)
    {
        StartTutorialIntroductionMessage();
    }

    public void StartTutorialIntroductionMessage()
    {
        _currentlyPlayingAudioSource?.Stop();
        _currentlyPlayingAudioSource = _tutorialIntroductionMessage;
        _tutorialIntroductionMessage?.Play();

        _animator.SetTrigger("StartTalking");
    }

    public void StartConnectionFirstTutorial(UmlConnectorType type)
    {
        if (type == UmlConnectorType.DirectedAssociation && !_playedConnectionFirstTutorial)
        {
            _currentlyPlayingAudioSource?.Stop();
            _currentlyPlayingAudioSource = _connectionFirstTutorial;
            _connectionFirstTutorial?.Play();

            _playedConnectionFirstTutorial = true;
            Connector.OnConnectorGrabbed -= StartConnectionFirstTutorial;
        }
    }

    public void StartConnectionSecondTutorial(bool bothEndsAttached)
    {
        if (_playedConnectionFirstTutorial && !bothEndsAttached)
        {
            _currentlyPlayingAudioSource?.Stop();
            _currentlyPlayingAudioSource = _connectionSecondTutorial;
            _connectionSecondTutorial?.Play();

            ConnectorGrabVolume.OnConnectorGrabVolumeGrabbed -= StartConnectionSecondTutorial;
        }
    }

    public void StartConnectionThirdTutorial(bool bothEndsAttached)
    {
        if (bothEndsAttached && !_playedConnectionThirdTutorial)
        {
            _currentlyPlayingAudioSource?.Stop();
            _currentlyPlayingAudioSource = _connectionThirdTutorial;
            _connectionThirdTutorial?.Play();

            _playedConnectionThirdTutorial = true;
            ConnectorGrabVolume.OnConnectorGrabVolumeGrabbed -= StartConnectionThirdTutorial;

            _animator.SetTrigger("Clap");
        }
    }

    public void StartFeatureFirstTutorial(ClassFeatureType type)
    {
        if (!_playedFeatureFirstTutorial)
        {
            _currentlyPlayingAudioSource?.Stop();
            _currentlyPlayingAudioSource = _featureFirstTutorial;
            _featureFirstTutorial?.Play();

            ClassFeatureManager.OnClassFeatureGrabbed -= StartFeatureFirstTutorial;
            _playedFeatureFirstTutorial = true;
        }
    }

    public void StartFeatureSecondTutorial(ClassFeatureType type)
    {
        if (!_playedFeatureSecondTutorial)
        {
            _currentlyPlayingAudioSource?.Stop();
            if (type == ClassFeatureType.Property)
            {
                _currentlyPlayingAudioSource = _featureSecondTutorialProperty;
                _featureSecondTutorialProperty?.Play();
            }
            else
            {
                _currentlyPlayingAudioSource = _featureSecondTutorialMethod;
                _featureSecondTutorialMethod?.Play();
            }

            ClassFeatureManager.OnClassFeaturePlaced -= StartFeatureSecondTutorial;
            _playedFeatureSecondTutorial = true;
        }
    }

    public void ToggleHelpCategories()
    {
        _helpCategories.SetActive(!_helpCategories.activeSelf);
        if (_helpCategories.activeSelf)
        {
            _helpIntroMessage.Play();
        }
    }

    public void ReplayLastHint()
    {
        _currentlyPlayingAudioSource?.Stop();
        _currentlyPlayingAudioSource?.Play();
    }

    public void ExplainAssociations()
    {
        _currentlyPlayingAudioSource?.Stop();
        _explanationAssociations.Play();
        _currentlyPlayingAudioSource = _explanationAssociations;
    }

    public void ExplainGameMode()
    {
        _currentlyPlayingAudioSource?.Stop();
        _explanationGameMode.Play();
        _currentlyPlayingAudioSource = _explanationGameMode;
    }

    public void ExplainControllerFunctions()
    {
        _currentlyPlayingAudioSource?.Stop();
        _explanationControllerFunctions.Play();
        _currentlyPlayingAudioSource = _explanationControllerFunctions;
    }
}
