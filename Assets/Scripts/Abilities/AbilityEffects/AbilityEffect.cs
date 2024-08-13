/**************************************************
Base class for stat effects for abilities.
StatsAE.cs and BuffAE.cs inherit from this.

Documentation updated 8/13/2024
**************************************************/
using UnityEngine;

public enum AbilityEffectType { Immediate, Continuous, Constant };

public abstract class AbilityEffect : ScriptableObject // MonoBehaviour
{
    public AbilityEffectType AbilityEffectType { get; set; }  // Determines how the effect should be applied.

    // Run by the ability info object when the ability is activated. Must be filled in to instantiate.
    public abstract void Apply(AbilityOwner abilityOwner);

    // Run by the ability info object when the ability is deactivated. Doesn’t have to be filled in to instantiate.
    public virtual void Disable(AbilityOwner abilityOwner) { }
}
