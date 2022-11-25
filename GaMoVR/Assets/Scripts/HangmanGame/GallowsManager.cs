using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class managing the gallows animations and reactions to events during the game.
/// </summary>
public class GallowsManager : MonoBehaviour
{
    /// <summary>
    /// Time the player has remaining to finish the game.
    /// </summary>
    private float _availableTime = -1;

    /// <summary>
    /// Initial time given to finish the level.
    /// </summary>
    private float _initialTime = -1;

    /// <summary>
    /// Whether the timer is running or not.
    /// </summary>
    private bool _timerIsRunning;

    /// <summary>
    /// Text field showing the remaining time.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _timerTextField;

    /// <summary>
    /// Level validation instance invoking events depending on player mistakes.
    /// </summary>
    [SerializeField]
    private UmlHangmanLevelValidator _levelValidator;

    /// <summary>
    /// Animator instance.
    /// Used depending on game mode.
    /// </summary>
    [SerializeField]
    private Animator _gallowsAnimator;

    /// <summary>
    /// Script alternative for animating the gallows for easier continuous gallows building.
    /// Used depending on game mode.
    /// </summary>
    [SerializeField]
    private GallowsAnimator _gallowsScriptAnimator;

    /// <summary>
    /// Initial number of lives the player has.
    /// </summary>
    private float _initialLives;

    /// <summary>
    /// Size of the intervals per building step of the gallows.
    /// Required for continuous building of gallows.
    /// </summary>
    private float _intervalSize;

    /// <summary>
    /// Current animation state of the gallows.
    /// </summary>
    private int _currentState;

    public void OnEnable()
    {
        UmlHangmanLevelValidator.OnGameStarted += InitGallows;
        UmlHangmanLevelValidator.OnGameFinished += StopTimer;

        // Depending on game mode, subscribe to specific validation events.
        if (!SceneManager.GetActiveScene().name.Contains("AgainstTheClock"))
        {
            UmlHangmanLevelValidator.OnConnectorValidationFailed += MadeConnectionPlacementError;
            UmlHangmanLevelValidator.OnMethodValidationFailed += MadeFeaturePlacementError;
            UmlHangmanLevelValidator.OnPropertyValidationFailed += MadeFeaturePlacementError;
            UmlHangmanLevelValidator.OnWrongMove += HandleWrongMove;
            UmlHangmanLevelValidator.OnGameFinished += CompleteHangman;
        }
    }

    public void OnDisable()
    {
        UmlHangmanLevelValidator.OnGameStarted -= InitGallows;
        UmlHangmanLevelValidator.OnGameFinished -= StopTimer;

        // Depending on game mode, subscribe to specific validation events.
        if (!SceneManager.GetActiveScene().name.Contains("AgainstTheClock"))
        {
            UmlHangmanLevelValidator.OnConnectorValidationFailed -= MadeConnectionPlacementError;
            UmlHangmanLevelValidator.OnMethodValidationFailed -= MadeFeaturePlacementError;
            UmlHangmanLevelValidator.OnPropertyValidationFailed -= MadeFeaturePlacementError;
            UmlHangmanLevelValidator.OnWrongMove -= HandleWrongMove;
            UmlHangmanLevelValidator.OnGameFinished -= CompleteHangman;
        }
    }

    public void Update()
    {
        // Update function only required for time-constrained game modes (AgainstTheClock).
        if (!_timerIsRunning)
        {
            return;
        }

        if (_availableTime > 0)
        {
            // Player still has time left to finish => update timer based on passed time
            _availableTime -= Time.deltaTime;
            var minutes = Mathf.FloorToInt(_availableTime / 60);
            var seconds = Mathf.FloorToInt(_availableTime % 60);
            var secondsString = seconds < 10 ? "0" + seconds : seconds.ToString();
            _timerTextField.SetText($"{minutes}:{secondsString}");

            // Update gallows animator state
            _currentState = Mathf.RoundToInt(5 - (_availableTime / _intervalSize));
        }
        else
        {
            // Time ran out => Player loses game.
            _availableTime = 0;
            _timerIsRunning = false;
            _timerTextField.SetText("0:00");

            _levelValidator.LoseGame();
        }

        GamificationEngineConnection.Instance.PlayerLives = _availableTime;
    }

    /// <summary>
    /// Increase the timer again if the player earns more time.
    /// </summary>
    /// <param name="addedTime">Amount of time (in seconds) added to the player's time.</param>
    public void IncreaseTimer(int addedTime)
    {
        _availableTime = _availableTime + addedTime > _initialTime ? _initialTime : _availableTime + addedTime;

        // reset animation
        _gallowsScriptAnimator.RevertAnimation(addedTime);
    }

    /// <summary>
    /// Initializes gallows considering the game mode.
    /// </summary>
    /// <param name="gameMode">Which game mode the gallows should be initialized for.</param>
    /// <param name="initialTimeOrLives">Number of lives/time (depending on game mode) the player has.</param>
    public void InitGallows(UmlHangmanGameMode gameMode, float? initialTimeOrLives)
    {
        if (gameMode == UmlHangmanGameMode.AgainstTheClock && initialTimeOrLives is not null)
        {
            _initialTime = (float)initialTimeOrLives + 1;
            _availableTime = _initialTime;
            _intervalSize = _initialTime / 5;
            _gallowsScriptAnimator.StartGallowsAnimation(_intervalSize);

            StartTimer();
        }
        else if ((gameMode != UmlHangmanGameMode.Classic || gameMode != UmlHangmanGameMode.TestMode) && initialTimeOrLives is not null)
        {
            _initialLives = (float)initialTimeOrLives;

            if (_currentState > 0)
            {
                _gallowsAnimator?.SetTrigger("Reset");
                _gallowsAnimator?.ResetTrigger("MadeMistake");
                _currentState = 0;
            }
        }
    }

    public void StartTimer()
    {
        _timerIsRunning = true;
    }

    public void StopTimer(bool won, float gainedXp)
    {
        _timerIsRunning = false;
    }

    /// <summary>
    /// Wrapper function to finish the remainder of the hangman in one go.
    /// </summary>
    /// <param name="won">Whether the player won the game or not.</param>
    /// <param name="gainedXp">Number of XP the player earned.</param>
    public void CompleteHangman(bool won, float gainedXp)
    {
        if (!won)
        {
            StartCoroutine(CompleteHangman());
        }
    }

    /// <summary>
    /// Coroutine to finish the hangman.
    /// </summary>
    public IEnumerator CompleteHangman()
    {
        while (_currentState < 7)
        {
            _gallowsAnimator?.SetTrigger("MadeMistake");
            _currentState++;
            yield return new WaitForSeconds(0.6f);
        }
    }

    #region Event Handlers

    public void MadeConnectionPlacementError(ConnectionPlacementError errorType)
    {
        AnimateError();
    }

    public void MadeFeaturePlacementError()
    {
        AnimateError();
    }

    public void HandleWrongMove(bool won, float xp)
    {
        AnimateError();
    }

    #endregion

    /// <summary>
    /// Updates the animation state of the gallows Animator if the player made a mistake.
    /// </summary>
    public void AnimateError()
    {
        if ((_initialLives == 4 && _currentState == 0) || (_initialLives == 3 && (_currentState == 0 || _currentState == 2)))
        {
            _gallowsAnimator?.SetTrigger("MadeMistake");
            StartCoroutine(ApplyDelayedTrigger("MadeMistake", 0.75f));
            _currentState++;
        }
        else
        {
            _gallowsAnimator?.SetTrigger("MadeMistake");
            _currentState++;
        }
    }

    /// <summary>
    /// Coroutine for applying the trigger for continuing the animation of the gallows with a given delay.
    /// </summary>
    /// <param name="trigger">Trigger name.</param>
    /// <param name="delay">Amount of delay (in seconds) to wait before applying trigger.</param>
    public IEnumerator ApplyDelayedTrigger(string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);

        _gallowsAnimator?.SetTrigger(trigger);
        _currentState++;
    }
}
