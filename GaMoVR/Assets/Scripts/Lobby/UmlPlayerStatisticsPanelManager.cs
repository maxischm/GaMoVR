using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UmlPlayerStatisticsPanelManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _playerNameField;

    [SerializeField]
    private TextMeshProUGUI _xpField;

    [SerializeField]
    private TextMeshProUGUI _availableUnlocksField;

    [Space, SerializeField]
    private GameObject _associationUnlockBanner;

    [SerializeField]
    private GameObject __associationUnlockConfirmMenu;

    [Space, SerializeField]
    private GameObject _aggregationUnlockBanner;

    [SerializeField]
    private GameObject _aggregationUnlockConfirmMenu;

    [Space, SerializeField]
    private GameObject _compositionUnlockBanner;

    [SerializeField]
    private GameObject _compositionUnlockConfirmMenu;

    [Space, SerializeField]
    private GameObject _inheritanceUnlockBanner;

    [SerializeField]
    private GameObject _inheritanceUnlockConfirmMenu;

    [Space, SerializeField]
    private AudioSource _unlockSound;

    public void OnEnable()
    {
        GamificationEngineConnection.OnPlayerStatusUpdate += UpdatePlayerStatus;
    }

    public void OnDestroy()
    {
        GamificationEngineConnection.OnPlayerStatusUpdate -= UpdatePlayerStatus;
    }

    public void Start()
    {
        _playerNameField.SetText(LocalPlayerReference.Instance.Nickname);
    }

    public void SetPlayerName(string name)
    {
        _playerNameField.SetText(name);
    }

    public void SetPlayerXp(float xp)
    {
        _xpField.SetText(xp.ToString());
    }

    public void FetchPlayerState()
    {
        StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(
           () => SetPlayerXp(GamificationEngineConnection.Instance.PlayerXp)
        ));
    }

    private void UpdatePlayerStatus()
    {
        if (!this)
        {
            return;
        }
        SetPlayerXp(GamificationEngineConnection.Instance.PlayerXp);
        _availableUnlocksField.SetText(GamificationEngineConnection.Instance.AvailableUnlocks.ToString());

        // Update unlock status of Connections
        if (GamificationEngineConnection.Instance.UnlockedElements.Contains("aggregation"))
        {
            _aggregationUnlockBanner.SetActive(false);
        }
        if (GamificationEngineConnection.Instance.UnlockedElements.Contains("inheritance"))
        {
            _inheritanceUnlockBanner.SetActive(false);
        }
        if (GamificationEngineConnection.Instance.UnlockedElements.Contains("composition"))
        {
            _compositionUnlockBanner.SetActive(false);
        }
        if (GamificationEngineConnection.Instance.UnlockedElements.Contains("directedAssociation"))
        {
            _associationUnlockBanner.SetActive(false);
        }

        // Update unlock status of class variations
        // if (_gamificationEngineConnection.UnlockedElements.Contains("interface"))
        // {
        //     _interfaceUnlockBanner.SetActive(false);
        // }
        // if (_gamificationEngineConnection.UnlockedElements.Contains("abstractClass"))
        // {
        //     _abstractClassUnlockBanner.SetActive(false);
        // }
    }

    public void UnlockAggregation()
    {
        if (GamificationEngineConnection.Instance.AvailableUnlocks > 0)
        {
            _aggregationUnlockBanner.SetActive(false);
            _unlockSound.Play();
            StartCoroutine(
                GamificationEngineConnection.Instance.UnlockElement(UmlUnlockableElement.Aggregation,
                    (_) => StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(
                        () => Debug.Log("fetched after unlocking aggregation")))));
            _aggregationUnlockConfirmMenu.SetActive(false);
        }
    }

    public void UnlockInheritance()
    {
        if (GamificationEngineConnection.Instance.AvailableUnlocks > 0)
        {
            _inheritanceUnlockBanner.SetActive(false);
            _unlockSound.Play();
            StartCoroutine(
                GamificationEngineConnection.Instance.UnlockElement(
                    UmlUnlockableElement.Inheritance,
                    (_) => GamificationEngineConnection.Instance.FetchPlayerState(null)));
            _inheritanceUnlockConfirmMenu.SetActive(false);
        }
    }

    public void UnlockDirectedAssociation()
    {
        if (GamificationEngineConnection.Instance.AvailableUnlocks > 0)
        {
            _associationUnlockBanner.SetActive(false);
            _unlockSound.Play();
            StartCoroutine(
                GamificationEngineConnection.Instance.UnlockElement(UmlUnlockableElement.DirectedAssociation, (_) =>
                    GamificationEngineConnection.Instance.FetchPlayerState(null)));
            __associationUnlockConfirmMenu.SetActive(false);
        }
    }

    public void UnlockComposition()
    {
        if (GamificationEngineConnection.Instance.AvailableUnlocks > 0)
        {
            _compositionUnlockBanner.SetActive(false);
            _unlockSound.Play();
            StartCoroutine(
                GamificationEngineConnection.Instance.UnlockElement(
                    UmlUnlockableElement.Composition,
                    (_) => GamificationEngineConnection.Instance.FetchPlayerState(null)));
            _compositionUnlockConfirmMenu.SetActive(false);
        }
    }

    // public void UnlockInterface()
    // {
    //     if (_gamificationEngineConnection.AvailableUnlocks > 0 && StartHangmanLevel.Instance.SkippedPlayerStatisticsTutorial)
    //     {
    //         _interfaceUnlockBanner.SetActive(false);
    //         StartCoroutine(
    //             _gamificationEngineConnection.UnlockElement("interface", (_) => _gamificationEngineConnection.FetchPlayerState(null)));
    //     }
    // }

    // public void UnlockAbstractClass()
    // {
    //     if (_gamificationEngineConnection.AvailableUnlocks > 0 && StartHangmanLevel.Instance.SkippedPlayerStatisticsTutorial)
    //     {
    //         _abstractClassUnlockBanner.SetActive(false);
    //         StartCoroutine(
    //             _gamificationEngineConnection.UnlockElement("abstractClass", (_) => _gamificationEngineConnection.FetchPlayerState(null)));
    //     }
    // }
}
