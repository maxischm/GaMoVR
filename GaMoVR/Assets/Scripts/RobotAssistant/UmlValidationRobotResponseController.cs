using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlValidationRobotResponseController : MonoBehaviour
{
    [SerializeField]
    private AudioSource _noSuchConnectorConnectedToClass;

    [SerializeField]
    private AudioSource _noSuchConnectorWithOneEndToClass;

    [SerializeField]
    private AudioSource _noSuchConnectorWithBothEndsToClass;

    [SerializeField]
    private AudioSource _noConnectorBetweenTheseClasses;

    [SerializeField]
    private AudioSource _allConnectionsAlreadyAttachedExactMatch;

    [SerializeField]
    private AudioSource _allConnectionsOfTypeWithThatEndAlreadyAttached;

    [SerializeField]
    private AudioSource _methodValidationFailed;

    [SerializeField]
    private AudioSource _propertyValidationFailed;

    [Space, SerializeField]
    private GameObject _helpCategories;

    [SerializeField]
    private AudioSource _helpIntroMessage;

    private AudioSource _currentlyPlayingAudioSource;

    public void OnEnable()
    {
        UmlHangmanLevelValidator.OnConnectorValidationFailed += PlayConnectorValidationFailed;

        UmlHangmanLevelValidator.OnMethodValidationFailed += PlayMethodValidationFailed;

        UmlHangmanLevelValidator.OnPropertyValidationFailed += PlayPropertyValidationFailed;
    }

    public void OnDestroy()
    {
        UmlHangmanLevelValidator.OnConnectorValidationFailed -= PlayConnectorValidationFailed;

        UmlHangmanLevelValidator.OnMethodValidationFailed -= PlayMethodValidationFailed;

        UmlHangmanLevelValidator.OnPropertyValidationFailed -= PlayPropertyValidationFailed;
    }

    public void PlayConnectorValidationFailed(ConnectionPlacementError errorType)
    {
        _currentlyPlayingAudioSource?.Stop();

        switch (errorType)
        {
            case ConnectionPlacementError.NoSuchConnectorConnectedToClass:
                _noSuchConnectorConnectedToClass.Play();
                _currentlyPlayingAudioSource = _noSuchConnectorConnectedToClass;
                break;
            case ConnectionPlacementError.NoSuchConnectorWithOneEndToClass:
                _noSuchConnectorWithOneEndToClass.Play();
                _currentlyPlayingAudioSource = _noSuchConnectorWithOneEndToClass;
                break;
            case ConnectionPlacementError.NoSuchConnectorWithBothEndsToClass:
                _noSuchConnectorWithBothEndsToClass.Play();
                _currentlyPlayingAudioSource = _noSuchConnectorWithBothEndsToClass;
                break;
            case ConnectionPlacementError.NoConnectorBetweenTheseClasses:
                _noConnectorBetweenTheseClasses.Play();
                _currentlyPlayingAudioSource = _noConnectorBetweenTheseClasses;
                break;
            case ConnectionPlacementError.AllConnectionsAlreadyAttachedExactMatch:
                _allConnectionsAlreadyAttachedExactMatch.Play();
                _currentlyPlayingAudioSource = _allConnectionsAlreadyAttachedExactMatch;
                break;
            case ConnectionPlacementError.AllConnectionsOfTypeWithThatEndAlreadyAttached:
                _allConnectionsOfTypeWithThatEndAlreadyAttached.Play();
                _currentlyPlayingAudioSource = _allConnectionsOfTypeWithThatEndAlreadyAttached;
                break;
        }
    }

    public void PlayMethodValidationFailed()
    {
        _currentlyPlayingAudioSource?.Stop();
        _methodValidationFailed.Play();
        _currentlyPlayingAudioSource = _methodValidationFailed;
    }

    public void PlayPropertyValidationFailed()
    {
        _currentlyPlayingAudioSource?.Stop();
        _propertyValidationFailed.Play();
        _currentlyPlayingAudioSource = _propertyValidationFailed;
    }

    public void RepeatLastMessage()
    {
        _currentlyPlayingAudioSource?.Stop();
        _currentlyPlayingAudioSource?.Play();
    }

    public void ToggleHelpCategories()
    {
        _helpCategories.SetActive(!_helpCategories.activeSelf);
        if (_helpCategories.activeSelf)
        {
            _helpIntroMessage.Play();
        }
    }
}
