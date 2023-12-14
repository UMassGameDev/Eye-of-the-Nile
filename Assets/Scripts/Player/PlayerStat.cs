using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    [field: SerializeField]
    private string _statName;
    public string StatName { get { return _statName; } set { _statName = value; } }
    [field: SerializeField]
    private int _baseValue;
    public int BaseValue { get { return _baseValue; } set { _baseValue = value; } }
    /*[field: SerializeField]
    private float _maxValue;
    public float MaxValue { get { return _maxValue; } set { _maxValue = value; } }*/
    // private Dictionary<StatModType, List<StatModifier>> statModifiers;
    List<StatModifier> statModifiers = new List<StatModifier>();

    public void AddModifier(StatModifier statMod)
    {
        // statModifiers[statMod.ModType].Add(statMod);
        if (!statModifiers.Contains(statMod))
            statModifiers.Add(statMod);
    }

    public bool RemoveModifier(StatModifier statMod)
    {
        // return statModifiers[statMod.ModType].Remove(statMod);
        if (statModifiers.Contains(statMod))
            return statModifiers.Remove(statMod);
        else
            return false;
    }

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
