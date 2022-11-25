using System.Collections;
using System.Collections.Generic;
using ModularWorldSpaceUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangmanInGameUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _winText;

    [SerializeField]
    private GameObject _lossText;

    [SerializeField]
    private Slider _xpBar;

    [SerializeField]
    private TextMeshProUGUI _gainedXpTextField;

    [SerializeField]
    private GameObject _newUnlockAvailableText;

    [SerializeField]
    private GameObject _retryButton;

    [SerializeField]
    private Button _backToMenuButton;

    [SerializeField]
    private Button _backToMenuInGameButton;

    [SerializeField]
    private Button _confirmBackToMenuButton;

    [SerializeField]
    private GameObject _finishScreen;

    [SerializeField]
    private GameObject _duringPlayScreen;

    [SerializeField]
    private GameObject _confirmMenu;

    [SerializeField]
    private UIActionActivation _playerUiActivationAction;

    [SerializeField]
    private bool _showTaskOnSceneLoad;

    private float _currXpSlow;

    private float _newXp;

    private float _maxXpPerLevel = 200;

    private float _t;

    private bool _showedIntro;

    // Start is called before the first frame update
    public void OnEnable()
    {
        GamificationEngineConnection.OnPlayerStatusUpdate += UpdatePlayerXp;
        UmlHangmanLevelValidator.OnGameFinished += ShowEndScreen;

        _confirmBackToMenuButton.onClick.AddListener(StartHangmanLevel.Instance.BackToMenu);

        _showedIntro = !_showTaskOnSceneLoad;
    }

    public void OnDisable()
    {
        GamificationEngineConnection.OnPlayerStatusUpdate -= UpdatePlayerXp;
        UmlHangmanLevelValidator.OnGameFinished -= ShowEndScreen;
    }

    // Update is called once per frame
    public void Update()
    {
        // Show UI for intro if player is initialized
        if (_playerUiActivationAction is not null && !_showedIntro && LocalPlayerReference.Instance.LocalPlayer != null)
        {
            _playerUiActivationAction.UIActionInvoked(new UnityEngine.InputSystem.InputAction.CallbackContext());
            _showedIntro = true;
        }

        if (gameObject.activeInHierarchy && _currXpSlow < _newXp)
        {
            _currXpSlow = Mathf.Lerp(_currXpSlow, _newXp, _t);
            _t += 0.1f * Time.deltaTime;
            _xpBar.value = _currXpSlow % _maxXpPerLevel / _maxXpPerLevel;
        }
        else
        {
            _t = 0;
        }
    }

    public void ShowEndScreen(bool won, float gainedXp)
    {
        _finishScreen.SetActive(true);
        _duringPlayScreen.SetActive(false);

        _backToMenuButton.onClick.RemoveAllListeners();
        _backToMenuButton.onClick.AddListener(() => SceneManager.LoadScene("UmlLearningLobby"));

        if (won)
        {
            _winText.SetActive(true);
            _lossText.SetActive(false);
        }
        else
        {
            _winText.SetActive(false);
            _lossText.SetActive(true);
            _retryButton.SetActive(true);
        }

        _gainedXpTextField.SetText($"+ {gainedXp} XP");

        if (Mathf.FloorToInt(_currXpSlow / _maxXpPerLevel) < Mathf.FloorToInt((_currXpSlow + gainedXp) / _maxXpPerLevel))
        {
            _newUnlockAvailableText.SetActive(true);
        }
        else
        {
            _newUnlockAvailableText.SetActive(false);
        }
    }

    public void UpdatePlayerXp()
    {
        _newXp = GamificationEngineConnection.Instance.PlayerXp;
    }

    public void ResetView()
    {
        _finishScreen.SetActive(false);
        _duringPlayScreen.SetActive(true);
        _confirmMenu.SetActive(false);
        _backToMenuButton.enabled = true;
    }

    public void OpenLeaveConfirmMenu()
    {
        _confirmMenu.SetActive(true);
        _backToMenuButton.enabled = false;
    }

    public void ContinuePlayingClick()
    {
        _confirmMenu.SetActive(false);
        _backToMenuButton.enabled = true;
    }
}
