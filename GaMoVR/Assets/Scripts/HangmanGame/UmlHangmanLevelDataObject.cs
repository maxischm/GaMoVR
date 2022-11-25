using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangmanLevel
{
    public string levelName;
    public float levelId;
    public string levelDescription;
    public string xmlModelString;
    public UmlHangmanGameMode gameMode;
    public List<string> requiredModelingElements;
    public int xpReward;
}

[System.Serializable]
public class HangmanClassicLevel : HangmanLevel
{
    public int initialLives;
}

[System.Serializable]
public class HangmanAgainstTheClockLevel : HangmanLevel
{
    public float initialTime;
}

[System.Serializable]
public class HangmanTestModeLevel : HangmanLevel
{
    public int initialLives;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HangmanLevelList", order = 1)]
public class UmlHangmanLevelDataObject : ScriptableObject
{
    public List<HangmanClassicLevel> classicLevels;
    public List<HangmanAgainstTheClockLevel> againstTheClockLevel;
    public List<HangmanTestModeLevel> testModeLevels;
}
