using System;
using UnityEngine;

// public enum StatModType { PreMultiplier, Additive, PostMultiplier }

/** \brief
StatModifiers are a variable type that can be used by abilities to modify player stats (see PlayerStat, StatsAE, and PlayerStatHolder).
This script is not a monobehavior so it does not have access to unity functions such as Start() or Update().

Documentation updated 9/1/2024
\author Roy Pascual
*/
[Serializable]
public class StatModifier
{
    [field: SerializeField]
    /// Name of the stat that this modifier targets. Can be changed in the Unity Editor.
    private string _targetStat;
    /// Public version of _targetStat that can be used by scripts.
    public string TargetStat { get { return _targetStat; } set { _targetStat = value; } }

    [field: SerializeField]
    /// Amount that this stat modifier will change the target stat by. Can be changed in the Unity Editor.
    private int _modValue;
    /// Public version of _modValue that can be used by scripts.
    public int ModValue { get { return _modValue; } set { _modValue = value; } }
    // public StatModType ModType { get; set; }

    /*public StatModifier(float modValue, StatModType modType)
    {
        ModValue = modValue;
        ModType = modType;
    }*/

    /// Constructor (function the automatically runs when a new StatModifier is created in another script). Initializes mod value to the given value.
    public StatModifier(int modValue)
    {
        ModValue = modValue;
    }
}
