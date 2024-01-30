/**************************************************
This is an example of an ability set that uses stat modifiers for each ability.
The code looks complicated, but you can simple copy-paste most of it for your own ability.
This is a scriptable object, meaning you can make and instance of it in the editor.

I (Stephen) am not the person who wrote this code, so I'm not sure if the initializing part at the top is necessary.
If you're looking to make your own ability, you can just use the stat modifier object like in RockAbilityInfo.
You should still remove any previous stat modifiers if your using multiple in a given ability set.

Documentation updated 1/29/2024
**************************************************/
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatAbilityInfo", menuName = "Abilities/Create New StatAbilityInfo")]
public class StatAbilityInfo : BaseAbilityInfo
{
    [Header("Custom Ability Info")]
    public List<StatModifier> statMods;  // unitialized StatModifiers
    private Dictionary<string, StatModifier> _statModDictField;  // this is where we store our StatModifiers after initializing them
    private Dictionary<string, StatModifier> _StatModDict  // this initalizes all of our StatModifiers
    {
        get
        {
            if (_statModDictField != null)
                return _statModDictField;
            else
            {
                StatModDictInitializer();
                return _statModDictField;
            }
        }
        set { _statModDictField = value; }
    }
    private string currentStat = "Damage";  // remembers what the current stat we're modifying is

    private void StatModDictInitializer()
    {
        _statModDictField = new Dictionary<string, StatModifier>();
        foreach (StatModifier statMod in statMods)
        {
            // Debug.Log(statMod.TargetStat);
            _statModDictField.Add(statMod.TargetStat, statMod);
        }
    }

    void Awake()
    {
        StatModDictInitializer();
    }

    // increase player damage
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Offense");

        // get transform and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        // remove any current stat modifier
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        // apply new stat modifier
        currentStat = "Damage";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

    }

    // increase player's max health
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Defense");

        // get transform, player health, and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        // debug messages
        Debug.Log(playerStats);
        Debug.Log(playerStats.GetStat(currentStat));
        Debug.Log(_StatModDict);
        Debug.Log(_StatModDict[currentStat]);

        // remove any current stat modifier
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        // apply new stat modifier
        currentStat = "MaxHealth";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

        // update player health
        playerHealth.InvokeHealthChange();
    }

    // increase player speed
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Utility.");

        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        currentStat = "Speed";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
    }

    // increase player's max health
    // this is the same code as the defense ability, but without the debug messages
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive.");

        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        currentStat = "MaxHealth";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

        playerHealth.InvokeHealthChange();
    }

    public override void AbilityUpdate(AbilityOwner abilityOwner)
    {
        
    }

    public override void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        // get transform and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        // remove any current stat modifier
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        // if we changed the max health, update player health
        if (currentStat == "MaxHealth")
        {
            PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
            playerHealth.InvokeHealthChange();
        }

        // trigger the base version of AbilityDisable()
        base.AbilityDisable(abilityOwner, effectType);
    }
}
