using UnityEngine;

[CreateAssetMenu(fileName = "New ExEffectAbilityInfo", menuName = "Abilities/Create New ExEffectAbilityInfo")]
/*!<summary>
This is an example of the effect system for abilities.
What effects to apply can be added in the editor.
When an ability is triggered, the effect(s) will be applied.
When the duration runs out, the effects(s) are automatically removed.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/14/2024
</summary>
\author Roy Pascual*/
public class ExEffectAbilityInfo : BaseAbilityInfo
{
    /// <summary>
    /// Applies offense effects.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Offense, AbilityEffectType.Immediate);
    }

    /// <summary>
    /// Applies defense effects.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Defense, AbilityEffectType.Immediate);
    }

    /// <summary>
    /// Applies utility effects.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Utility, AbilityEffectType.Immediate);
    }

    /// <summary>
    /// Applies passive effects.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Passive, AbilityEffectType.Immediate);
    }

    /// <summary>
    /// Continuously applies the effects of the current ability form.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public override void AbilityUpdate(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, currentForm, AbilityEffectType.Continuous);
    }
}
