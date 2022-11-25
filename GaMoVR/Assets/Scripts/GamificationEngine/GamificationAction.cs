using System;
using System.Collections.Generic;

public class GamificationAction
{
    public string gameId;
    public string playerId;
    public Dictionary<string, object> data = new();
    public string actionId;

    public GamificationAction(string gameId, string playerId, string actionId)
    {
        this.gameId = gameId;
        this.playerId = playerId;
        this.actionId = actionId;
    }
}