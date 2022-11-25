using System.Collections;
using System.Collections.Generic;
using ModularWorldSpaceUI;
using UnityEngine;
using UnityEngine.UI;

public class UmlHangmanTutorialUiManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _tutorialDescriptions;

    [SerializeField]
    private GameObject _taskDescription;

    [SerializeField]
    private GameObject _previousButton;

    [SerializeField]
    private GameObject _nextButton;

    private int _tutorialDescriptionIndex;

    [SerializeField]
    private UIActionActivation _playerUiActivationAction;

    [SerializeField]
    private GameObject _finishScreen;

    [SerializeField]
    private GameObject _duringPlayScreen;

    [SerializeField]
    private GameObject _confirmMenu;

    [SerializeField]
    private Button _backToMenuButton;

    private bool _showedIntro;

    public void OnEnable()
    {
        UmlHangmanLevelValidator.OnGameFinished += ShowEndScreen;
    }

    public void OnDisable()
    {
        UmlHangmanLevelValidator.OnGameFinished -= ShowEndScreen;
    }

    public void ShowEndScreen(bool won, float gainedXp)
    {
        _previousButton.SetActive(false);
        _nextButton.SetActive(false);
        _tutorialDescriptions.ForEach(d => d.SetActive(false));
    }

    public void Update()
    {
        if (!_showedIntro && LocalPlayerReference.Instance.LocalPlayer != null)
        {
            _playerUiActivationAction.UIActionInvoked(new UnityEngine.InputSystem.InputAction.CallbackContext());
            _showedIntro = true;
        }
    }

    public void NextTutorialDescription()
    {
        if (_tutorialDescriptionIndex == 0)
        {
            _previousButton.SetActive(false);
        }
        if (_tutorialDescriptionIndex + 1 <= _tutorialDescriptions.Count - 1)
        {
            _tutorialDescriptionIndex++;
            _tutorialDescriptions[_tutorialDescriptionIndex - 1].SetActive(false);
            _tutorialDescriptions[_tutorialDescriptionIndex].SetActive(true);
        }
        else
        {
            _tutorialDescriptions[_tutorialDescriptionIndex].SetActive(false);
            _nextButton.SetActive(false);
            _taskDescription.SetActive(true);
        }
    }

    public void PreviousTutorialDescription()
    {
        if (_tutorialDescriptionIndex - 1 >= 0)
        {
            if (!_taskDescription.activeSelf)
            {
                _tutorialDescriptionIndex--;
                _tutorialDescriptions[_tutorialDescriptionIndex + 1].SetActive(false);
                _tutorialDescriptions[_tutorialDescriptionIndex].SetActive(true);
            }
            else
            {
                _taskDescription.SetActive(false);
                _tutorialDescriptions[_tutorialDescriptionIndex].SetActive(true);
            }

            _nextButton.SetActive(true);
            _taskDescription.SetActive(false);
        }

        if (_tutorialDescriptionIndex == 0)
        {
            _previousButton.SetActive(false);
        }
    }

    public void ResetView()
    {
        _finishScreen.SetActive(false);
        _confirmMenu.SetActive(false);
        _backToMenuButton.gameObject.SetActive(true);
        if (_tutorialDescriptionIndex == _tutorialDescriptions.Count - 1)
        {
            _taskDescription.SetActive(true);
            _tutorialDescriptions.ForEach(t => t.SetActive(false));
            _nextButton.SetActive(false);
        }
        else
        {
            _tutorialDescriptions[_tutorialDescriptionIndex].SetActive(true);
            _taskDescription.SetActive(false);
            _nextButton.SetActive(true);

            if (_tutorialDescriptionIndex == 0)
            {
                _previousButton.SetActive(false);
            }
            else
            {
                _previousButton.SetActive(true);
            }
        }
    }
}
