using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UmlHangmanGameMode
{
    Classic,
    AgainstTheClock,
    TestMode
}

public class StartHangmanLevel : GenericSingletonClass<StartHangmanLevel>
{
    [SerializeField]
    private UmlHangmanLevelDataObject _levels;
    public UmlHangmanLevelDataObject Levels
    {
        get => _levels;
    }

    private List<HangmanLevel> _completeLevelList = new();
    public List<HangmanLevel> CompleteHangmalLevelList
    {
        get => _completeLevelList;
    }

    private int _selectedLevel;
    public int SelectedLevel
    {
        get => _selectedLevel;
    }

    public bool SkippedIntroScreen
    {
        get; set;
    }

    public bool SkippedMainMenuTutorial
    {
        get; set;
    }

    public bool SkippedPlayerStatisticsTutorial
    {
        get; set;
    }

    public override void Awake()
    {
        base.Awake();
        if (SceneManager.GetActiveScene().name.Equals("UmlLearningLobby"))
        {
            _completeLevelList = _levels.classicLevels
                .Concat<HangmanLevel>(_levels.againstTheClockLevel)
                .Concat(_levels.testModeLevels).ToList();
        }
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevel(int selectedLevelIndex)
    {
        Debug.Log("Start Level call received");
        _selectedLevel = selectedLevelIndex;
        // Notify gamification engine about the start of the new level
        if (_completeLevelList[_selectedLevel].levelName.Equals("Tutorial"))
        {
            StartCoroutine(
                GamificationEngineConnection.Instance.StartNewLevel(
                    LocalPlayerReference.Instance.Nickname,
                    _completeLevelList[_selectedLevel].levelId,
                    (_completeLevelList[_selectedLevel] as HangmanClassicLevel).initialLives,
                    null
                )
            );
            SceneManager.LoadScene("HangmanClassicTutorial");
        }
        else if (_completeLevelList[_selectedLevel] is HangmanClassicLevel)
        {
            StartCoroutine(
                GamificationEngineConnection.Instance.StartNewLevel(
                    LocalPlayerReference.Instance.Nickname,
                    _completeLevelList[_selectedLevel].levelId,
                    (_completeLevelList[_selectedLevel] as HangmanClassicLevel).initialLives,
                    null
                )
            );
            SceneManager.LoadScene("HangmanClassic");
        }
        else if (_completeLevelList[_selectedLevel] is HangmanAgainstTheClockLevel)
        {
            StartCoroutine(
                GamificationEngineConnection.Instance.StartNewLevel(
                    LocalPlayerReference.Instance.Nickname,
                    _completeLevelList[_selectedLevel].levelId,
                    -1,
                    null
                )
            );
            SceneManager.LoadScene("HangmanAgainstTheClock");
        }
        else if (_completeLevelList[_selectedLevel] is HangmanTestModeLevel)
        {
            StartCoroutine(
                GamificationEngineConnection.Instance.StartNewLevel(
                    LocalPlayerReference.Instance.Nickname,
                    _completeLevelList[_selectedLevel].levelId,
                    (_completeLevelList[_selectedLevel] as HangmanTestModeLevel).initialLives,
                    null
                )
            );
            SceneManager.LoadScene("HangmanTestMode");
        }
    }

    public void BackToMenu()
    {
        StartCoroutine(GamificationEngineConnection.Instance.QuitLevel(
            LocalPlayerReference.Instance.Nickname,
            _completeLevelList[_selectedLevel].levelId,
            null
        ));
        SceneManager.LoadScene("UmlLearningLobby");
    }
}
