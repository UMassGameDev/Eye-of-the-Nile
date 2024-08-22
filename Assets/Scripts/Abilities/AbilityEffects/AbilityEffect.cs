using UnityEngine;

/*!<summary>
Base class for stat effects for abilities.
StatsAE.cs and BuffAE.cs inherit from this.

Documentation updated 8/13/2024
</summary>
\author Roy Pascual*/
public enum AbilityEffectType { Immediate, Continuous, Constant };

public abstract class AbilityEffect : ScriptableObject // MonoBehaviour
{
    /// <summary>
    /// Determines how the effect should be applied.
    /// </summary>
    /// <value></value>
    public AbilityEffectType AbilityEffectType { get; set; }

    /// <summary>
    /// Run by the ability info object when the ability is activated. Must be filled in to instantiate.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public abstract void Apply(AbilityOwner abilityOwner);

    /// <summary>
    /// Run by the ability info object when the ability is deactivated. Doesn’t have to be filled in to instantiate.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public virtual void Disable(AbilityOwner abilityOwner) { }
}
