using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatAbilityInfo", menuName = "Abilities/Create New StatAbilityInfo")]
public class StatAbilityInfo : BaseAbilityInfo
{
    [Header("Custom Ability Info")]
    public List<StatModifier> statMods;
    private Dictionary<string, StatModifier> _statModDictField;
    private Dictionary<string, StatModifier> _StatModDict
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
    private string currentStat = "Damage";

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

    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Offense");
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        currentStat = "Damage";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

    }

    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Defense");
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        Debug.Log(playerStats);
        Debug.Log(playerStats.GetStat(currentStat));
        Debug.Log(_StatModDict);
        Debug.Log(_StatModDict[currentStat]);
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        currentStat = "MaxHealth";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
        playerHealth.InvokeHealthChange();
    }

    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Utility.");
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        currentStat = "Speed";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
    }

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
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        if (currentStat == "MaxHealth")
        {
            PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
            playerHealth.InvokeHealthChange();
        }
        base.AbilityDisable(abilityOwner, effectType);
    }
}
