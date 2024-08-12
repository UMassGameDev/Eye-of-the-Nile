/**************************************************
Base class for stat effects for abilities.
StatsAE.cs and BuffAE.cs inherit from this.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public enum AbilityEffectType { Immediate, Continuous, Constant };

public abstract class AbilityEffect : ScriptableObject // MonoBehaviour
{
    public AbilityEffectType AbilityEffectType { get; set; }
    public abstract void Apply(AbilityOwner abilityOwner);
    public virtual void Disable(AbilityOwner abilityOwner) { }
}
