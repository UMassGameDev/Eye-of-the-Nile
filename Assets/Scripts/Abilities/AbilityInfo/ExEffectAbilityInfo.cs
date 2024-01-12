using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExEffectAbilityInfo", menuName = "Abilities/Create New ExEffectAbilityInfo")]
public class ExEffectAbilityInfo : BaseAbilityInfo
{
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Offense, AbilityEffectType.Immediate);
    }

    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Defense, AbilityEffectType.Immediate);
    }

    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Utility, AbilityEffectType.Immediate);
    }

    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, AbilityForm.Passive, AbilityEffectType.Immediate);
    }

    public override void AbilityUpdate(AbilityOwner abilityOwner)
    {
        base.ApplyEffects(abilityOwner, currentForm, AbilityEffectType.Continuous);
    }
}
