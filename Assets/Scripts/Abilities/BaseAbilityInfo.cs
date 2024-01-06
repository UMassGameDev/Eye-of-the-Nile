using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityForm { Offense, Defense, Utility, Passive };

public abstract class BaseAbilityInfo : ScriptableObject
{
    [Header("Basic Ability Info")]
    public int abilityID;
    public string abilityName = "DEFAULT_ABILITY";
    public Sprite overlapIcon;
    public List<Sprite> abilityIcons;
    public string onCooldownSound = "incorrect_buzzer";

    [Header("Advanced Ability Info")]
    public AbilityForm currentForm;
    public int baseCost = 0;
    public float duration = 0f;
    public float tickRate = 0f;
    public float chargeUp = 0f;
    public float cooldown = 0f;
    // public float endTime = 0f;
    public int damage = 0;

    // TODO: Implement ability effects
    [Header("Ability Effect Info")]
    // public List<AbilityEffect> universalEffects;
    public List<AbilityEffect> offenseEffects;
    public List<AbilityEffect> defenseEffects;
    public List<AbilityEffect> utilityEffects;
    public List<AbilityEffect> passiveEffects;

    /*[Header("Advanced Ability Info")]*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected abstract void AbilityOffense(AbilityOwner abilityOwner);

    protected abstract void AbilityDefense(AbilityOwner abilityOwner);

    protected abstract void AbilityUtility(AbilityOwner abilityOwner);

    protected abstract void AbilityPassive(AbilityOwner abilityOwner);

    protected virtual void ApplyEffects(AbilityOwner abilityOwner,
        AbilityForm abilityForm,
        AbilityEffectType applyType)
    {
        List<AbilityEffect> currentEffects;
        switch (abilityForm)
        {
            case AbilityForm.Offense:
                currentEffects = offenseEffects;
                break;
            case AbilityForm.Defense:
                currentEffects = defenseEffects;
                break;
            case AbilityForm.Utility:
                currentEffects = utilityEffects;
                break;
            case AbilityForm.Passive:
                currentEffects = passiveEffects;
                break;
            default:
                currentEffects = offenseEffects;
                break;
        }

        foreach(AbilityEffect abiEffect in currentEffects)
        {
            if (abiEffect.AbilityEffectType == applyType)
            {
                abiEffect.Apply(abilityOwner);
            }
        }
    }

    protected virtual void DisableEffects(AbilityOwner abilityOwner,
        AbilityForm abilityForm,
        AbilityEffectType disableType)
    {
        List<AbilityEffect> currentEffects;
        switch (abilityForm)
        {
            case AbilityForm.Offense:
                currentEffects = offenseEffects;
                break;
            case AbilityForm.Defense:
                currentEffects = defenseEffects;
                break;
            case AbilityForm.Utility:
                currentEffects = utilityEffects;
                break;
            case AbilityForm.Passive:
                currentEffects = passiveEffects;
                break;
            default:
                currentEffects = offenseEffects;
                break;
        }

        foreach (AbilityEffect abiEffect in currentEffects)
        {
            if (abiEffect.AbilityEffectType == disableType)
            {
                abiEffect.Disable(abilityOwner);
            }
        }
    }

    public virtual void AbilityUpdate(AbilityOwner abilityOwner) { }

    public virtual void AbilityActivate(AbilityOwner abilityOwner)
    {
        switch (currentForm)
        {
            case AbilityForm.Offense:
                AbilityOffense(abilityOwner);
                break;
            case AbilityForm.Defense:
                AbilityDefense(abilityOwner);
                break;
            case AbilityForm.Utility:
                AbilityUtility(abilityOwner);
                break;
            case AbilityForm.Passive:
                AbilityPassive(abilityOwner);
                break;
            default:
                break;
        }
        // endTime = Time.time + duration;
    }

    public virtual void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType) {
        DisableEffects(abilityOwner, currentForm, effectType);
    }
}
