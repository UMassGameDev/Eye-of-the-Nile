/**************************************************
Contains all the player's stats that can be modified by abilities using StatModifiers.
You can add new player stats using the editor, but make sure you implement the corresponding functionality into the proper script.
To link a value in another script to a player stat, see PlayerHealth.cs.

Documentation updated 1/29/2024
**************************************************/
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatHolder : MonoBehaviour
{
    public List<PlayerStat> playerStats;
    private Dictionary<string, PlayerStat> playerStatDict;
    public bool IsInitialized { get; set; } = false;

    public int GetValue(string statName)
    {
        if (playerStatDict == null)
            InitializeDictionary();
        return playerStatDict[statName].FinalValue();
    }

    public PlayerStat GetStat(string statName)
    {
        if (playerStatDict == null)
            InitializeDictionary();
        return playerStatDict[statName];
    }

    public void InitializeDictionary()
    {
        playerStatDict = new Dictionary<string, PlayerStat>();
        foreach (PlayerStat playerStat in playerStats)
        {
            playerStatDict.Add(playerStat.StatName, playerStat);
            // Debug.Log(playerStat.StatName);
        }
    }

    void Awake()
    {
        InitializeDictionary();
    }
}
