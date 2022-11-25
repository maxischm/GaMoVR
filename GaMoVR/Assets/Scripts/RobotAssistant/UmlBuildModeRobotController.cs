using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UmlBuildModeRobotController : GenericSingletonClass<UmlBuildModeRobotController>
{
    [SerializeField]
    private AudioClip _buildModeIntro1;

    private bool _playedBuildModeIntro1;

    [SerializeField]
    private AudioClip _buildModeIntro2;

    private bool _playedBuildModeIntro2;

    [SerializeField]
    private AudioClip _buildModeIntro3;

    private bool _playedBuildModeIntro3;

    [SerializeField]
    private AudioClip _buildModeIntro4;

    private bool _playedBuildModeIntro4;

    [SerializeField]
    private AudioClip _buildModeIntro5;

    private bool _playedBuildModeIntro5;

    [Space, SerializeField]
    private AudioClip _buildModeExplanation;

    [Space, SerializeField]
    private AudioClip _howToSpawn3DObjects;

    [SerializeField]
    private AudioClip _howToSpawnUmlObjects;

    [SerializeField]
    private AudioClip _howToDestroy;

    [SerializeField]
    private AudioClip _controlsExplanation;

    [Space, SerializeField]
    private AudioClip _forbiddenConnection;

    [Space, SerializeField]
    private Animator _animator;

    [SerializeField]
    private AudioSource _currentlyPlayingAudioSource;

    [Space, SerializeField]
    private GameObject _helpButtons;

    [SerializeField]
    private AudioSource _helpIntroMessage;

    [Space, SerializeField]
    private AudioClip _buildModeExplanationSpawnAutoAttach;

    [SerializeField]
    private AudioClip _buildModeExplanationDeleteConnectionAutoAttach;

    [SerializeField]
    private AudioClip _buildModeExplanationDeleteClassAutoAttach;

    [Space, SerializeField]
    private InputActionReference _uiActivationAction;

    public void OnEnable()
    {
        SceneManager.sceneLoaded += StartBuildModeIntro1;
        _uiActivationAction.action.started += StartBuildModeIntro3Wrapper;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= StartBuildModeIntro1;
        _uiActivationAction.action.started -= StartBuildModeIntro3Wrapper;
    }

    public void StartBuildModeIntro1(Scene scene, LoadSceneMode mode)
    {
        if (!_playedBuildModeIntro1)
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeIntro1;
            _currentlyPlayingAudioSource.Play();

            StartCoroutine(PlayTalkingGesture(1));

            _playedBuildModeIntro1 = true;
        }
    }

    public void StartBuildModeIntro2()
    {
        if (!_playedBuildModeIntro2)
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeIntro2;
            _currentlyPlayingAudioSource.Play();

            StartCoroutine(PlayTalkingGesture(1));

            _playedBuildModeIntro2 = true;
        }
    }

    public void StartBuildModeIntro3Wrapper(InputAction.CallbackContext context)
    {
        StartBuildModeIntro3();
        _uiActivationAction.action.started -= StartBuildModeIntro3Wrapper;
    }

    public void StartBuildModeIntro3()
    {
        if (!_playedBuildModeIntro3)
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeIntro3;
            _currentlyPlayingAudioSource.Play();

            StartCoroutine(PlayTalkingGesture(1));

            _playedBuildModeIntro3 = true;
        }
    }

    public void StartBuildModeIntro4()
    {
        if (!_playedBuildModeIntro4)
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeIntro4;
            _currentlyPlayingAudioSource.Play();

            StartCoroutine(PlayTalkingGesture(1));

            _playedBuildModeIntro4 = true;
        }
    }

    public void StartBuildModeIntro5()
    {
        if (!_playedBuildModeIntro5)
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeIntro5;
            _currentlyPlayingAudioSource.Play();

            StartCoroutine(PlayTalkingGesture(1));

            _playedBuildModeIntro5 = true;
        }
    }

    public void PlayModeExplanation()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.clip = _buildModeExplanation;
        _currentlyPlayingAudioSource.Play();

        StartCoroutine(PlayTalkingGesture(1));
    }

    public void RepeatLastHint()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.Play();

        StartCoroutine(PlayTalkingGesture(1));
    }

    public IEnumerator PlayTalkingGesture(float delay)
    {
        yield return new WaitForSeconds(delay);

        _animator.SetTrigger("StartTalking");
    }

    public void PlayUmlSpawnExplanation()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.clip = _howToSpawnUmlObjects;
        _currentlyPlayingAudioSource.Play();

        StartCoroutine(PlayTalkingGesture(1));
    }

    public void Play3DObjectSpawnExplanation()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.clip = _howToSpawn3DObjects;
        _currentlyPlayingAudioSource.Play();

        StartCoroutine(PlayTalkingGesture(1));
    }

    public void PlayObjectDestroyExplanation()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.clip = _howToDestroy;
        _currentlyPlayingAudioSource.Play();

        StartCoroutine(PlayTalkingGesture(1));
    }

    public void PlayControlsExplanation()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.clip = _controlsExplanation;
        _currentlyPlayingAudioSource.Play();
    }

    public void SkipTutorial()
    {
        _playedBuildModeIntro1 = true;
        _playedBuildModeIntro2 = true;
        _playedBuildModeIntro3 = true;
        _playedBuildModeIntro4 = true;
        _playedBuildModeIntro5 = true;

        if (_currentlyPlayingAudioSource == _playedBuildModeIntro1
            || _currentlyPlayingAudioSource == _playedBuildModeIntro2
            || _currentlyPlayingAudioSource == _playedBuildModeIntro3
            || _currentlyPlayingAudioSource == _playedBuildModeIntro4
            || _currentlyPlayingAudioSource == _playedBuildModeIntro5
        )
        {
            _currentlyPlayingAudioSource.Stop();
        }
    }

    public void PlayForbiddenConnectionWarning()
    {
        _currentlyPlayingAudioSource.Stop();
        _currentlyPlayingAudioSource.clip = _forbiddenConnection;
        _currentlyPlayingAudioSource.Play();
    }

    public void ToggleHelpCategories()
    {
        _helpButtons.SetActive(!_helpButtons.activeSelf);
        if (_helpButtons.activeSelf)
        {
            _helpIntroMessage.Play();
        }
    }

    public void PlayBuildModeExplanationSpawnAutoAttach()
    {
        if (!_currentlyPlayingAudioSource.isPlaying
            || (_currentlyPlayingAudioSource.clip != _buildModeIntro1
                && _currentlyPlayingAudioSource.clip != _buildModeIntro2
                && _currentlyPlayingAudioSource.clip != _buildModeIntro3
                && _currentlyPlayingAudioSource.clip != _buildModeIntro4
                && _currentlyPlayingAudioSource.clip != _buildModeIntro5
            )
        )
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeExplanationSpawnAutoAttach;
            _currentlyPlayingAudioSource.Play();
        }
    }

    public void PlayBuildModeExplanationDeleteConnectionAutoAttach()
    {
        if (!_currentlyPlayingAudioSource.isPlaying
            || (_currentlyPlayingAudioSource.clip != _buildModeIntro1
                && _currentlyPlayingAudioSource.clip != _buildModeIntro2
                && _currentlyPlayingAudioSource.clip != _buildModeIntro3
                && _currentlyPlayingAudioSource.clip != _buildModeIntro4
                && _currentlyPlayingAudioSource.clip != _buildModeIntro5
            )
        )
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeExplanationDeleteConnectionAutoAttach;
            _currentlyPlayingAudioSource.Play();
        }
    }

    public void PlayBuildModeExplanationDeleteClassAutoAttach()
    {
        if (!_currentlyPlayingAudioSource.isPlaying
            || (_currentlyPlayingAudioSource.clip != _buildModeIntro1
                && _currentlyPlayingAudioSource.clip != _buildModeIntro2
                && _currentlyPlayingAudioSource.clip != _buildModeIntro3
                && _currentlyPlayingAudioSource.clip != _buildModeIntro4
                && _currentlyPlayingAudioSource.clip != _buildModeIntro5
            )
        )
        {
            _currentlyPlayingAudioSource.Stop();
            _currentlyPlayingAudioSource.clip = _buildModeExplanationDeleteClassAutoAttach;
            _currentlyPlayingAudioSource.Play();
        }
    }
}
