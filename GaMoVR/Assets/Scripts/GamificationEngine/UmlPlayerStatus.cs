using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class UmlPlayerStatusResponse
{
    public string? PlayerId
    {
        get; set;
    }
    public string? GameId
    {
        get; set;
    }
    public UmlPlayerStatus? State
    {
        get; set;
    }
    public UmlCustomData? CustomData
    {
        get; set;
    }

    [JsonConstructor]
    public UmlPlayerStatusResponse(string playerId, string gameId, UmlPlayerStatus state, UmlCustomData customData)
    {
        PlayerId = playerId;
        GameId = gameId;
        State = state;
        CustomData = customData;
    }
}

public class UmlPlayerStatus
{
    public List<GamificationEnginePointConcept>? PointConcept
    {
        get; set;
    }

    [JsonConstructor]
    public UmlPlayerStatus(List<GamificationEnginePointConcept> PointConcept)
    {
        this.PointConcept = PointConcept;
    }
}

public class GamificationEnginePointConcept
{
    public string? Id
    {
        get; set;
    }
    public string? Name
    {
        get; set;
    }
    public float? Score
    {
        get; set;
    }

    [JsonConstructor]
    public GamificationEnginePointConcept(string id, string name, float score, object periods)
    {
        Id = id;
        Name = name;
        Score = score;
    }
}

public class UmlCustomData
{
    public List<UmlPlayedGame>? PlayedGames
    {
        get; set;
    }
    public List<UmlUnlockedElement>? UnlockedElements
    {
        get; set;
    }

    [JsonConstructor]
    public UmlCustomData(List<UmlPlayedGame> playedGames, List<UmlUnlockedElement> unlockedElements)
    {
        PlayedGames = playedGames;
        UnlockedElements = unlockedElements;
    }
}

public class UmlPlayedGame
{
    public float? Level
    {
        get; set;
    }
    public bool? Won
    {
        get; set;
    }
    public float? Lives
    {
        get; set;
    }

    [JsonConstructor]
    public UmlPlayedGame(float level, bool won, float lives)
    {
        Level = level;
        Won = won;
        Lives = lives;
    }
}

public class UmlUnlockedElement
{
    public string? Key
    {
        get; set;
    }
    public string? Name
    {
        get; set;
    }

    [JsonConstructor]
    public UmlUnlockedElement(string key, string name)
    {
        Key = key;
        Name = name;
    }
}
