/**************************************************
StatModifiers are a variable type that can be used by abilities to modify player stats.

Documentation updated 1/29/2024
**************************************************/
using System;
using UnityEngine;

// public enum StatModType { PreMultiplier, Additive, PostMultiplier }

[Serializable]
public class StatModifier
{
    [field: SerializeField]
    private string _targetStat;
    public string TargetStat { get { return _targetStat; } set { _targetStat = value; } }

    [field: SerializeField]
    private int _modValue;
    public int ModValue { get { return _modValue; } set { _modValue = value; } }
    // public StatModType ModType { get; set; }

    /*public StatModifier(float modValue, StatModType modType)
    {
        ModValue = modValue;
        ModType = modType;
    }*/

    public StatModifier(int modValue)
    {
        ModValue = modValue;
    }
}
