/**************************************************
This is an abstract scriptable object that all abilities inherit from.
Important: as of writing, passive abilities work the same as all other abilities, rather than always being active.
This contains the basic functionality for abilities. If you want to make your own ability, inherit from this!
Every ability info must replace the four abstract functions, and can optionally add to the virtual functions too.

Documentation updated 8/13/2024
**************************************************/
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum AbilityForm { Offense, Defense, Utility, Passive };

public abstract class BaseAbilityInfo : ScriptableObject
{
    [Header("Basic Ability Info")]
    public int abilityID;  // A number that is used to identify the ability info object, usually to determine if two ability infos are the same.
    public string abilityName = "DEFAULT_ABILITY";  // A string that is used to identify the ability, as well as display its name in UIs.
    public Sprite overlapIcon;  // Sprite used to represent the ability in UI elements, such as the ability inventory and ability hotbar.
    public List<Sprite> abilityIcons;  // The icons for each of the ability’s forms: offense, defense, utility, and passive.
    [SerializeField] string[] abilityNames;  // Name of each ability in the set. Only used to display in UIs.
    [SerializeField] string[] abilityDescriptions;  // Description of what each ability in the set does. Only used to display in UIs.
    [SerializeField] string godQuote;  // A quote from this god that can be displayed in UIs.
    public string onCooldownSound = "incorrect_buzzer";  // Reference to the sound that plays when the player tries to use an ability that's currently on cooldown.
    public int abilityLevel = 0;  // Current level of the ability. The ability is locked at level 0.
    public int maxLevel = 3;  // The maximum level the ability can be upgraded to.

    [Header("Advanced Ability Info")]
    public AbilityForm currentForm;  // Which of the 4 abilities in the set are currently in use.
    public int baseCost = 0;  // Unimplemented feature that would see abilities cost a currency to use.
    public float duration = 0f;  // How long (in seconds) the ability should last for.*
    public float tickRate = 0f;  // How many times a second AbilityUpdate() is run. Use for things like repeatedly regenerating health.
    public float chargeUp = 0f;  // How long (in seconds) the ability’s charge up should last for.*
    public float cooldown = 0f;  // How long (in seconds) the ability’s cooldown should last for.*
    public int damage = 0;  // Unimplemented feature.
    // * This functionality should be expanded to be configurable for each ability, not all 4 at the same time.
    //   Right now, changing the cooldown changes it for all 4 abilities, which may not be desirable.

    // TODO: Implement ability effects
    [Header("Ability Effect Info")]
    // public List<AbilityEffect> universalEffects;
    public List<AbilityEffect> offenseEffects;  // List of effects the offense ability will trigger.
    public List<AbilityEffect> defenseEffects;  // List of effects the defense ability will trigger.
    public List<AbilityEffect> utilityEffects;  // List of effects the utility ability will trigger.
    public List<AbilityEffect> passiveEffects;  // List of effects the passive ability will trigger.

    /******************************
    ABSTRACT FUNCTIONS
        These functions are where you add the functionality for your 4 abilities in this set.
        They must be overridden in any class inheriting from BaseAbilityInfo.cs.
    ******************************/
    protected abstract void AbilityOffense(AbilityOwner abilityOwner);  // The functionality for the offense ability goes here!
    
    protected abstract void AbilityDefense(AbilityOwner abilityOwner);  // The functionality for the defense ability goes here!
    
    protected abstract void AbilityUtility(AbilityOwner abilityOwner);  // The functionality for the utility ability goes here!
    
    protected abstract void AbilityPassive(AbilityOwner abilityOwner);  // The functionality for the passive ability goes here!
    // Important: as of writing, passive abilities work the same as all other abilities, rather than always being active.

    /******************************
    VIRTUAL FUNCTIONS
        These functions can be optionally overridden to add extra functionality.
        They can be left untouched if you do not need to change them.
    ******************************/

    // Applies all effects for the given ability form.
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

    // Disables all effects for the given ability form.
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

    // Runs every tickRate seconds while the ability is active. Useful for things like repeatedly regenerating health.
    public virtual void AbilityUpdate(AbilityOwner abilityOwner) { }

    // Runs the ability function of the given form.
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

    // Runs when the ability duration runs out. Disables effects.
    public virtual void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType) {
        DisableEffects(abilityOwner, currentForm, effectType);
    }

    /******************************
    GENERAL FUNCTIONS
        These functions (mostly getters) handle basic functionality that all abilities should have.
        They cannot be changed by classes inheriting from BaseAbilityInfo.cs.
    ******************************/

    // Increases the level by one, unless already at max level.
    public void UpgradeAbility() {
        if (abilityLevel < maxLevel) {
            abilityLevel++;
        } else {
            Debug.Log("Ability already at max level (" + abilityLevel + " >= " + maxLevel + ")");
        }
    }

    // Returns the name of the given ability form.
    public string GetAbilityName(AbilityForm form)
    {
        switch (form)
        {
            case (AbilityForm.Offense): return abilityNames[0];
            case (AbilityForm.Defense): return abilityNames[1];
            case (AbilityForm.Utility): return abilityNames[2];
            case (AbilityForm.Passive): return abilityNames[3];
            default: return "Error: Invalid input into BaseAbilityInfo.GetAbilityName()";
        }
    }

    // Returns the description of the given ability form.
    public string GetAbilityDescription(AbilityForm form)
    {
        switch (form)
        {
            case (AbilityForm.Offense): return abilityDescriptions[0];
            case (AbilityForm.Defense): return abilityDescriptions[1];
            case (AbilityForm.Utility): return abilityDescriptions[2];
            case (AbilityForm.Passive): return abilityDescriptions[3];
            default: return "Error: Invalid input into BaseAbilityInfo.GetAbilityName()";
        }
    }

    // Self explanatory
    public string[] GetAllAbilityNames() { return abilityNames; }
    public string[] GetAllAbilityDescriptions() { return abilityDescriptions; }
    public string GetQuote() { return godQuote; }
}
