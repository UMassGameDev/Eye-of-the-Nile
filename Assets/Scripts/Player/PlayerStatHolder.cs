using System.Collections.Generic;
using UnityEngine;

/** \brief
Contains all the player's stats that can be modified by abilities using StatModifiers (see PlayerStat, StatModifier, and StatsAE).
You can add new player stats using the editor, but make sure you implement the corresponding functionality into the proper script.
To link a value in another script to a player stat, see PlayerHealth.

Documentation updated 9/1/2024
\author Roy Pascual
*/
public class PlayerStatHolder : MonoBehaviour
{
    /// List of all player stats.
    public List<PlayerStat> playerStats;
    /// Dictionary linking each player stat object to its name.
    private Dictionary<string, PlayerStat> playerStatDict;
    /// True if playerStatDict has been populated with every PlayerStat object in playerStats and their corresponding name.
    public bool IsInitialized { get; set; } = false;

    /// Returns the value of the given stat.
    public int GetValue(string statName)
    {
        if (playerStatDict == null)
            InitializeDictionary();
        return playerStatDict[statName].FinalValue();
    }

    /// Returns the PlayerStat object of the given stat.
    public PlayerStat GetStat(string statName)
    {
        if (playerStatDict == null)
            InitializeDictionary();
        return playerStatDict[statName];
    }

    /// Populates playerStatDict with every PlayerStat object in playerStats and their corresponding name.
    public void InitializeDictionary()
    {
        playerStatDict = new Dictionary<string, PlayerStat>();
        foreach (PlayerStat playerStat in playerStats)
        {
            playerStatDict.Add(playerStat.StatName, playerStat);
            // Debug.Log(playerStat.StatName);
        }
    }

    /// Runs InitializeDictionary().
    void Awake()
    {
        InitializeDictionary();
    }
}
