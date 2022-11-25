using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UmlGameSelection : MonoBehaviour
{
    [SerializeField]
    private GameObject _introductionPanel;

    #region Main Menu

    [SerializeField]
    private GameObject _mainMenuPanel;

    [SerializeField]
    private GameObject _playButtonTutorial;

    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private GameObject _playerStatisticsButtonTutorial;

    [SerializeField]
    private Button _playerStatisticsButton;

    [SerializeField]
    private GameObject _unlockAvailableNotification;
    [SerializeField]
    private TMPro.TextMeshProUGUI _availableUnlocksText;

    #endregion

    #region Player Profile

    [SerializeField]
    private GameObject _playerStatisticsPanel;

    [SerializeField]
    private GameObject _playerInformationTutorial;

    [SerializeField]
    private GameObject _connectionUnlockTutorial;

    [SerializeField]
    private GameObject _classOptionUnlockTutorial;

    [SerializeField]
    private GameObject _playerInformationPanel;

    [SerializeField]
    private GameObject _connectorUnlockPanel;

    [SerializeField]
    private UmlPlayerStatisticsPanelManager playerStatisticsManager;

    // [SerializeField]
    // private GameObject _classOptionUnlockPanel;

    #endregion

    [SerializeField]
    private GameObject _gameSelectionPanel;

    [SerializeField]
    private GameObject _hangmanLevelSelectionPanel;

    [SerializeField]
    private FillLevelList hangmanLevelListManager;

    [SerializeField]
    private GameObject _buildModeLockOverlay;

    public void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "UmlLearningLobby")
        {
            StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(null));
            if (StartHangmanLevel.Instance.SkippedIntroScreen)
            {
                SelectMainMenu();
            }
        }
    }

    public void Start()
    {
        StartCoroutine(GamificationEngineConnection.Instance.UnlockElement(UmlUnlockableElement.DirectedAssociation, (_) =>
            StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(null))));
    }

    #region Main Menu actions
    public void SelectPlay()
    {
        _mainMenuPanel.SetActive(false);
        _gameSelectionPanel.SetActive(true);
        _hangmanLevelSelectionPanel.SetActive(false);

        if (GamificationEngineConnection.Instance.PlayerXp >= 600)
        {
            _buildModeLockOverlay.SetActive(false);
        }
    }

    public void SelectPlayerStatistics()
    {
        playerStatisticsManager.FetchPlayerState();
        _mainMenuPanel.SetActive(false);
        _playerStatisticsPanel.SetActive(true);
        // if (!StartHangmanLevel.Instance.SkippedPlayerStatisticsTutorial)
        // {
        //     _playerInformationTutorial.SetActive(true);
        //     _connectorUnlockPanel.SetActive(false);
        //     // _classOptionUnlockPanel.SetActive(false);
        // }
    }

    public void SelectMainMenu()
    {
        int availableUnlocks = GamificationEngineConnection.Instance.AvailableUnlocks;

        _mainMenuPanel.SetActive(true);
        _gameSelectionPanel.SetActive(false);
        _hangmanLevelSelectionPanel.SetActive(false);
        _playerStatisticsPanel.SetActive(false);
        _introductionPanel.SetActive(false);

        if (availableUnlocks > 0)
        {
            _unlockAvailableNotification.SetActive(true);
            _availableUnlocksText.SetText(availableUnlocks.ToString());
        }
        else
        {
            _unlockAvailableNotification.SetActive(false);
        }

        StartHangmanLevel.Instance.SkippedIntroScreen = true;

        // if (!StartHangmanLevel.Instance.SkippedMainMenuTutorial)
        // {
        //     _playButtonTutorial.SetActive(true);
        //     _playButton.enabled = false;
        //     _playerStatisticsButton.enabled = false;
        // }
    }

    #endregion

    #region Play Menu Actions

    public void SelectHangman()
    {
        _gameSelectionPanel.SetActive(false);
        _hangmanLevelSelectionPanel.SetActive(true);
    }

    #endregion

    public void StartLevel()
    {
        StartHangmanLevel.Instance.StartLevel(hangmanLevelListManager.SelectedLevelIndex);
    }

    public void OpenPlayButtonTutorial()
    {
        SelectMainMenu();
        _playButtonTutorial.SetActive(true);
    }

    public void OpenPlayerStatisticsButtonTutorial()
    {
        _playButtonTutorial.SetActive(false);
        _playerStatisticsButtonTutorial.SetActive(true);
    }

    public void ClosePlayerStatisticButtonTutorial()
    {
        _playerStatisticsButtonTutorial.SetActive(false);
        StartHangmanLevel.Instance.SkippedMainMenuTutorial = true;

        _playButton.enabled = true;
        _playerStatisticsButton.enabled = true;
    }

    public void OpenConnectionUnlockTutorial()
    {
        _connectionUnlockTutorial.SetActive(true);
        _playerInformationTutorial.SetActive(false);
        _connectorUnlockPanel.SetActive(true);
        _playerInformationPanel.SetActive(false);
    }

    public void CloseConnectionUnlockTutorial()
    {
        _connectionUnlockTutorial.SetActive(false);
        _connectorUnlockPanel.SetActive(true);
        _playerInformationPanel.SetActive(true);

        StartHangmanLevel.Instance.SkippedPlayerStatisticsTutorial = true;
    }

    public void SelectBuildMode()
    {
        SceneManager.LoadScene("UmlMultiViewpointModeling");
    }

    // public void OpenClassOptionUnlockTutorial()
    // {
    //     _connectionUnlockTutorial.SetActive(false);
    //     _classOptionUnlockTutorial.SetActive(true);
    //     _connectorUnlockPanel.SetActive(false);
    //     _classOptionUnlockPanel.SetActive(true);
    // }

    // public void CloseClassOpenUnlockTutorial()
    // {
    //     _classOptionUnlockTutorial.SetActive(false);
    //     _playerInformationPanel.SetActive(true);
    //     _connectorUnlockPanel.SetActive(true);
    //     _classOptionUnlockPanel.SetActive(true);        
    // }
}
