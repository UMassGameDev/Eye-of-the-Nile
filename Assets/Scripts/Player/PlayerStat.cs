using System;
using System.Collections.Generic;
using UnityEngine;

/** \brief
Basic properties of a player stat that can be modified by abilities using StatModifiers (see StatModifier, StatsAE, and PlayerStatHolder).
Each stat has a name and a base value, set in the editor.

Documentation updated 10/11/2024
\author Roy Pascaul, Stephen Nuttall
\note This class does not inhert from monobehavior, so it does not have access to unity functions such as Start() or Update().
*/
[Serializable]
public class PlayerStat
{
    [field: SerializeField]
    /// Name of the stat. Can be changed in the Unity Editor.
    private string _statName;
    /// Public version of _statName that can be used by scripts.
    public string StatName { get { return _statName; } set { _statName = value; } }
    [field: SerializeField]
    /// Base value of the stat. Can be changed in the Unity Editor.
    private int _baseValue;
    /// Public version of _baseValue that can be used by scripts.
    public int BaseValue { get { return _baseValue; } set { _baseValue = value; } }
    /*[field: SerializeField]
    private float _maxValue;
    public float MaxValue { get { return _maxValue; } set { _maxValue = value; } }*/
    // private Dictionary<StatModType, List<StatModifier>> statModifiers;
    /// List of currently active stat modifiers.
    List<StatModifier> statModifiers = new List<StatModifier>();

    /// Event that is triggered when a stat modifier is added to the player.
    public static event Action<string, int> modifierAdded;
    /// Event that is triggered when a stat modifier is removed from the player.
    public static event Action<string, int> modifierRemoved;

    /// <summary>
    /// Add a stat modifier. Checks to see if the stat modifer is already applied.
    /// </summary>
    /// <param name="statMod">Stat modifier to add.</param>
    public void AddModifier(StatModifier statMod)
    {
        // statModifiers[statMod.ModType].Add(statMod);
        if (!statModifiers.Contains(statMod))
        {
            statModifiers.Add(statMod);
            modifierAdded?.Invoke(statMod.TargetStat, statMod.ModValue);
        }
    }

    /// <summary>
    /// Remove a stat modifier. Checks to see if the stat modifer is actually applied.
    /// </summary>
    /// <param name="statMod">Stat modifer to remove.</param>
    /// <returns>true if the stat modifier was successfully removed. Otherwise, false.</returns>
    public bool RemoveModifier(StatModifier statMod)
    {
        // return statModifiers[statMod.ModType].Remove(statMod);
        if (statModifiers.Contains(statMod))
        {
            modifierRemoved?.Invoke(statMod.TargetStat, statMod.ModValue);
            return statModifiers.Remove(statMod);
        }
        else
        {
            return false;
        }
    }

    /// Returns the final value of this stat, based on baseValue and all active stat modifiers.
    public int FinalValue()
    {
        // float preMultSum = 0f;
        int additiveSum = 0;
        // float postMultSum = 0f;
        int finalValue = BaseValue;

        /*foreach(StatModifier statMod in statModifiers[StatModType.PreMultiplier])
        {
            preMultSum += statMod.ModValue;
        }
        finalValue = finalValue * preMultSum;*/
        foreach (StatModifier statMod in statModifiers)
        {
            additiveSum += statMod.ModValue;
        }
        finalValue = finalValue + additiveSum;
        /*foreach (StatModifier statMod in statModifiers[StatModType.PostMultiplier])
        {
            postMultSum += statMod.ModValue;
        }
        finalValue = finalValue * postMultSum;*/

        /*if (finalValue > MaxValue)
            return MaxValue;*/
        return finalValue;
    }
}
