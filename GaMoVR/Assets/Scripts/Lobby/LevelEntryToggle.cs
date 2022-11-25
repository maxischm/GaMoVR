using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEntryToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject deselectedBackground;
    [SerializeField]
    private GameObject selectedBackground;

    public FillLevelList levelListManager;

    public void LevelSelected()
    {
        levelListManager.ToggleLevel(this);
    }

    public void LevelDeselected()
    {
        deselectedBackground.SetActive(true);
        selectedBackground.SetActive(false);
    }

    public void LevelSelectedFromRemote()
    {
        deselectedBackground.SetActive(false);
        selectedBackground.SetActive(true);
    }
}
