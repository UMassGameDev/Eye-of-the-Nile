using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Possible forms an ability info can be in. This determines which ability in the set is being used.
/// </summary>
public enum AbilityForm
{
    /// The ability set is in its offense form and its offense ability will be used.
    Offense,
    /// The ability set is in its defense form and its defense ability will be used.
    Defense,
    /// The ability set is in its utility form and its utility ability will be used.
    Utility,
    /// The ability set is in its passive form and its passive ability will be used.
    Passive
};

/*!<summary>
This is an abstract scriptable object that all abilities inherit from.
Important: as of writing, passive abilities work the same as all other abilities, rather than always being active.
This contains the basic functionality for abilities. If you want to make your own ability, inherit from this!
Every ability info must replace the four abstract functions, and can optionally add to the virtual functions too.

Documentation updated 8/13/2024
</summary>
\author Roy Pascual, Stephen Nuttall
\note This (any anything that inherits from it) is a scriptable object, meaning you can make an instance of it in
the Unity Editor that exists in the file explorer.
*/
public abstract class BaseAbilityInfo : ScriptableObject
{
    [Header("Basic Ability Info")]
    /** @name Basic Ability Info
    *  Basic information about this ability set. Includes basic functionality like the ability's current level, identifying information like the
    *  ability's name and ID, and UI related information like what icons to use and what the descriptions of each ability in the set should be.
    */
    ///@{

    /// \brief A number that is used to identify the ability info object, usually to determine if two ability infos are the same.
    public int abilityID;
    /// \brief A string that is used to identify the ability, as well as display its name in UIs.
    public string abilityName = "DEFAULT_ABILITY";
    /// \brief Sprite used to represent the ability in UI elements, such as the ability inventory and ability hotbar.
    public Sprite overlapIcon;
    /// \brief The icons for each of the ability’s forms: offense, defense, utility, and passive.
    public List<Sprite> abilityIcons;
    /// \brief Name of each ability in the set. Only used to display in UIs.
    [SerializeField] string[] abilityNames;
    /// \brief Description of what each ability in the set does. Only used to display in UIs.
    [SerializeField] string[] abilityDescriptions;
    /// \brief A quote from this god that can be displayed in UIs.
    [SerializeField] string godQuote;
    /// \brief Reference to the sound that plays when the player tries to use an ability that's currently on cooldown.
    public EventReference onCooldownSound;
    /// \brief Current level of the ability. The ability is locked at level 0.
    public int abilityLevel = 0;
    /// \brief The maximum level the ability can be upgraded to.
    public int maxLevel = 3;
    ///@}

    [Header("Advanced Ability Info")]
    /** @name Advanced Ability Info
    *  More advanced information, such as what form the ability set is currently in, cool down duration, and tick rate. Some unimplemented/deprecated
    *  features are also included, namely baseCost and damage. Additionally, some of these features need to be adapted to be configurable for each
    *  ability in the set, rather than for the set as a whole.
    */
    ///@{

    /// \brief Which of the 4 abilities in the set are currently in use.
    public AbilityForm currentForm;
    /// \brief Unimplemented feature that would see abilities cost a currency to use.
    /// \deprecated There are currently no plans for abilities to have a cost to them.
    public int baseCost = 0;
    /// \brief How long (in seconds) the ability should last for.
    /// \todo This functionality should be expanded to be configurable for each ability, not all 4 at the same time.
    /// Right now, changing the cooldown changes it for all 4 abilities, which may not be desirable.
    public float duration = 0f;
    /// \brief How many times a second AbilityUpdate() is run. Use for things like repeatedly regenerating health.
    public float tickRate = 0f;
    /// \brief How long (in seconds) the ability’s charge up should last for.
    /// \todo This functionality should be expanded to be configurable for each ability, not all 4 at the same time.
    /// Right now, changing the cooldown changes it for all 4 abilities, which may not be desirable.
    public float chargeUp = 0f;
    /// \brief How long (in seconds) the ability’s cooldown should last for.
    /// \todo This functionality should be expanded to be configurable for each ability, not all 4 at the same time.
    /// Right now, changing the cooldown changes it for all 4 abilities, which may not be desirable. 
    public float cooldown = 0f;
    /// \brief Unimplemented feature that's occasionally used when an ability needs a damage value
    /// \deprecated While it can be used, it generally makes more sense to make your own damage variable.
    public int damage = 0;
    ///@}

    // TODO: Implement ability effects
    [Header("Ability Effect Info")]
    /** @name Ability Effect Info
    *  Each ability has a list of effects it can apply while active.
    */
    ///@{

    // public List<AbilityEffect> universalEffects;
    /// \brief List of effects the offense ability will trigger.
    public List<AbilityEffect> offenseEffects;
    /// \brief List of effects the defense ability will trigger.
    public List<AbilityEffect> defenseEffects;
    /// \brief List of effects the utility ability will trigger.
    public List<AbilityEffect> utilityEffects;
    /// \brief List of effects the passive ability will trigger.
    public List<AbilityEffect> passiveEffects;
    ///@}

    /******************************
    ABSTRACT FUNCTIONS
        These functions are where you add the functionality for your 4 abilities in this set.
        They must be overridden in any class inheriting from BaseAbilityInfo.cs.
    ******************************/
    /** @name Abstract Functions
    *  These functions are where you add the functionality for your 4 abilities in this set.
    *  They must be overridden in any class inheriting from BaseAbilityInfo.cs.
    */
    ///@{

    /// \brief The functionality for the offense ability goes here!
    protected abstract void AbilityOffense(AbilityOwner abilityOwner);

    /// \brief The functionality for the defense ability goes here!
    protected abstract void AbilityDefense(AbilityOwner abilityOwner);

    /// \brief The functionality for the utility ability goes here!
    protected abstract void AbilityUtility(AbilityOwner abilityOwner);

    /// \brief The functionality for the passive ability goes here!
    /// This function will automatically run when the ability is equipped.
    protected abstract void AbilityPassiveEnable(AbilityOwner abilityOwner);

    /// \brief Use thus function will disable the functionality of the passive ability.
    /// This function will automatically run when the ability is unequipped.
    protected abstract void AbilityPassiveDisable(AbilityOwner abilityOwner);
    ///@}

    /******************************
    VIRTUAL FUNCTIONS
        These functions can be optionally overridden to add extra functionality.
        They can be left untouched if you do not need to change them.
    ******************************/
    /** @name Virtual Functions
    *  These functions can be optionally overridden to add extra functionality. They can be left untouched if you do not need to change them.
    */
    ///@{

    /// <summary>
    /// Applies all effects for the given ability form.
    /// </summary>
    /// <param name="abilityOwner">An object that, among other things, stores what ability in the set is currently is use.</param>
    /// <param name="abilityForm">The ability in the set (offense, defense, utility, passive) that we want to apply the effects of.</param>
    /// <param name="applyType">Describes the way we want the ability to be applied.</param>
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

        foreach (AbilityEffect abiEffect in currentEffects)
        {
            if (abiEffect.AbilityEffectType == applyType)
            {
                abiEffect.Apply(abilityOwner);
            }
        }
    }

    /// <summary>
    /// Disables all effects for the given ability form.
    /// </summary>
    /// <param name="abilityOwner">An object that, among other things, stores what ability in the set is currently is use.</param>
    /// <param name="abilityForm">The ability in the set (offense, defense, utility, passive) that we want to remove the effects of.</param>
    /// <param name="applyType">Describes the way the ability is applied.</param>
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

    /// Runs every tickRate seconds while the ability is active. Useful for things like repeatedly regenerating health.
    public virtual void AbilityUpdate(AbilityOwner abilityOwner) { }

    /// Runs the ability function of the given form.
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
                AbilityPassiveEnable(abilityOwner);
                break;
            default:
                break;
        }
        // endTime = Time.time + duration;
    }

    /// Disables the passive ability if the 
    public virtual void DisablePassive(AbilityOwner abilityOwner)
    {
        if (currentForm != AbilityForm.Passive)
        {
            AbilityPassiveDisable(abilityOwner);
        }
    }

    /// Runs when the ability duration runs out. Disables effects.
    public virtual void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        DisableEffects(abilityOwner, currentForm, effectType);
    }
    ///@}

    /******************************
    GENERAL FUNCTIONS
        These functions (mostly getters) handle basic functionality that all abilities should have.
        They cannot be changed by classes inheriting from BaseAbilityInfo.cs.
    ******************************/
    /** @name General Functions
    *  These functions (mostly getters) handle basic functionality that all abilities should have.
    *  They cannot be changed by classes inheriting from BaseAbilityInfo.cs.
    */
    ///@{

    /// Increases the level by one, unless already at max level.
    public void UpgradeAbility()
    {
        if (abilityLevel < maxLevel)
        {
            abilityLevel++;
        }
        else
        {
            Debug.Log("Ability already at max level (" + abilityLevel + " >= " + maxLevel + ")");
        }
    }

    /// Returns the name of the given ability form.
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

    /// Returns the description of the given ability form.
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

    /// Returns all the names of the abilities in each set.
    public string[] GetAllAbilityNames() { return abilityNames; }
    /// Returns all the descriptions of the abilities in each set.
    public string[] GetAllAbilityDescriptions() { return abilityDescriptions; }
    /// Returns all the god's quote for this ability.
    public string GetQuote() { return godQuote; }
    ///@}
}
