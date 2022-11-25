using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillLevelList : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private UmlHangmanLevelDataObject levelData;

    private List<HangmanLevel> _completeLevelList = new();

    public List<HangmanLevel> CompleteLevelList
    {
        get => _completeLevelList;
    }

    private List<Image> _newLevelNotificationImages = new();
    private List<Image> _wonLevelNotificationImages = new();

    [SerializeField]
    private TextMeshProUGUI descriptionBox;

    [SerializeField]
    private TextMeshProUGUI _gameModeInput;

    [SerializeField]
    private GameObject _startButtonBlockBanner;

    [SerializeField]
    private int selectedLevelIndex = 0;
    public int SelectedLevelIndex
    {
        get => selectedLevelIndex;
    }

    [SerializeField]
    private AudioSource _selectSound;

    [Space, SerializeField]
    private TextMeshProUGUI _gameStateInput;

    private List<LevelEntryToggle> listEntryToggles = new();

    [Space, SerializeField]
    private UmlLobbyRobotController _robotController;

    public void OnEnable()
    {
        if (content.childCount > 0)
        {
            return;
        }

        _completeLevelList = levelData.classicLevels
                    .Concat<HangmanLevel>(levelData.againstTheClockLevel)
                    .Concat<HangmanLevel>(levelData.testModeLevels).ToList();

        foreach (var level in _completeLevelList)
        {
            // Create all level list entries on loading into the lobby
            // Must be local instantiation because the entries are not networked objects
            var entry = Instantiate(prefab, content);

            // Set level text to fill list correctly
            entry.GetComponentInChildren<TextMeshProUGUI>().SetText(level.levelName);

            _newLevelNotificationImages.Add(entry.transform.GetChild(0).Find("NewLevelNotification").GetComponent<Image>());
            _wonLevelNotificationImages.Add(entry.transform.GetChild(0).Find("LevelWon").GetComponent<Image>());

            // Init toggle group & cache reference for managing highlighting
            var toggle = entry.GetComponentInChildren<LevelEntryToggle>();
            listEntryToggles.Add(toggle);
            toggle.GetComponent<LevelEntryToggle>().levelListManager = this;
        }
        listEntryToggles[0].LevelSelected();

        StringBuilder descriptionBuilder = new();
        descriptionBuilder.Append(_completeLevelList[0].levelDescription);
        if (_completeLevelList[0].requiredModelingElements.Count > 0)
        {
            descriptionBuilder.Append("<br><br><b>Requires:</b> ");
            foreach (var reqElem in _completeLevelList[0].requiredModelingElements)
            {
                descriptionBuilder.Append(reqElem).Append(", ");
            }
        }
        descriptionBox.SetText(descriptionBuilder.ToString());

        _gameModeInput.SetText(_completeLevelList[0].gameMode.ToString());
        SetGameState(0);
        selectedLevelIndex = 0;

        CheckLevelPlayability();
    }

    private void SetGameState(int levelIndex)
    {
        var level = _completeLevelList[levelIndex];

        if (!GamificationEngineConnection.Instance.PlayedGames.ContainsKey(level.levelId))
        {
            _gameStateInput.SetText("Not played yet");
        }
        else if (!GamificationEngineConnection.Instance.PlayedGames[level.levelId].Item1)
        {
            _gameStateInput.SetText("Not won yet");
        }
        else if (GamificationEngineConnection.Instance.PlayedGames[level.levelId].Item1)
        {
            if (level.gameMode == UmlHangmanGameMode.Classic)
            {
                _gameStateInput.SetText($"Won with {GamificationEngineConnection.Instance.PlayedGames[level.levelId].Item2} / {(level as HangmanClassicLevel).initialLives} Lives");
            }
            else if (level.gameMode == UmlHangmanGameMode.TestMode)
            {
                _gameStateInput.SetText($"Won with {GamificationEngineConnection.Instance.PlayedGames[level.levelId].Item2} / {(level as HangmanClassicLevel).initialLives} Lives");
            }
            else if (level.gameMode == UmlHangmanGameMode.AgainstTheClock)
            {
                var originalMinutes = Mathf.FloorToInt((level as HangmanAgainstTheClockLevel).initialTime / 60);
                var originalSeconds = Mathf.FloorToInt((level as HangmanAgainstTheClockLevel).initialTime % 60);
                var secondsString = originalSeconds < 10 ? "0" + originalSeconds : originalSeconds.ToString();

                var remainingMinutes = Mathf.FloorToInt(GamificationEngineConnection.Instance.PlayedGames[level.levelId].Item2 / 60);
                var remainingSeconds = Mathf.FloorToInt(GamificationEngineConnection.Instance.PlayedGames[level.levelId].Item2 % 60);
                var remainingSecondsString = remainingSeconds < 10 ? "0" + remainingSeconds : remainingSeconds.ToString();

                _gameStateInput.SetText($"Won in {remainingMinutes}:{remainingSecondsString} / {originalMinutes}:{secondsString}");
            }
        }
    }

    public void ToggleLevel(LevelEntryToggle changedToggle)
    {
        var toggleIndex = listEntryToggles.IndexOf(changedToggle);
        selectedLevelIndex = toggleIndex;

        // Update toggle value
        foreach (var levelEntry in listEntryToggles)
        {
            levelEntry.LevelDeselected();
        }
        listEntryToggles[toggleIndex].LevelSelectedFromRemote();

        StringBuilder descriptionBuilder = new();
        if (_completeLevelList[toggleIndex].requiredModelingElements.Count > 0)
        {
            descriptionBuilder.Append("<b>Requires:</b> ");
            foreach (var reqElem in _completeLevelList[toggleIndex].requiredModelingElements)
            {
                descriptionBuilder.Append(reqElem).Append(", ");
            }
            descriptionBuilder.Append("<br><br>");
        }
        descriptionBuilder.Append(_completeLevelList[toggleIndex].levelDescription);
        descriptionBox.SetText(descriptionBuilder.ToString());

        _gameModeInput.SetText(_completeLevelList[toggleIndex].gameMode.ToString());
        SetGameState(toggleIndex);

        // Check if player has unlocked all required elements
        CheckLevelPlayability();

        _selectSound.Play();
    }

    public void CheckLevelPlayability()
    {
        // Check if player has unlocked all required elements
        _startButtonBlockBanner.SetActive(false);
        if (_completeLevelList.Count > 0)
        {
            foreach (var requiredElement in _completeLevelList[selectedLevelIndex].requiredModelingElements)
            {
                if (!GamificationEngineConnection.Instance.UnlockedElements.Contains(requiredElement))
                {
                    _startButtonBlockBanner.SetActive(true);
                    _robotController.PlayLevelLockedNotification();
                    break;
                }
            }
        }
    }

    public void FindNewLevels()
    {
        if (_completeLevelList.Count > 0)
        {
            for (var i = 0; i < _completeLevelList.Count; i++)
            {
                _newLevelNotificationImages[i].enabled = _completeLevelList[i].requiredModelingElements.All(element =>
                        GamificationEngineConnection.Instance.UnlockedElements.Contains(element))
                    && !GamificationEngineConnection.Instance.PlayedGames.ContainsKey(_completeLevelList[i].levelId);

                _wonLevelNotificationImages[i].enabled =
                    GamificationEngineConnection.Instance.PlayedGames.ContainsKey(_completeLevelList[i].levelId)
                    && GamificationEngineConnection.Instance.PlayedGames[_completeLevelList[i].levelId].Item1;
            }
        }
    }
}
