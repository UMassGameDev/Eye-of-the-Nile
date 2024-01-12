using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "New FireAbilityInfo", menuName = "Abilities/Create New FireAbilityInfo")]
public class FireAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public GameObject projectilePrefab;
    public int numFireballs = 3;
    public float fireballDelay = 0.5f;

    [Header("Defense Ability Info")]
    public GameObject firewallPrefab;
    public float firewallXOffset = 1.5f;
    public float firewallYOffset = 0.5f;

    [Header("Utility Ability Info")]
    public float fireImmunityTimer = 3f;

    [Header("Passive Ability Info")]
    public List<StatModifier> statMods;
    private string currentStat = "FireResistance";
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

    private void StatModDictInitializer()
    {
        _statModDictField = new Dictionary<string, StatModifier>();
        foreach (StatModifier statMod in statMods)
        {
            // Debug.Log(statMod.TargetStat);
            _statModDictField.Add(statMod.TargetStat, statMod);
        }
    }

    // Shoot burst of fireballs
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectileBurst(projectilePrefab, numFireballs, fireballDelay);
    }

    // Spawn a tall fires on each side of the player
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Instantiate(firewallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + firewallXOffset,
            abilityOwner.OwnerTransform.position.y + firewallYOffset), Quaternion.identity);

        Instantiate(firewallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x - firewallXOffset,
            abilityOwner.OwnerTransform.position.y + firewallYOffset), Quaternion.identity);
    }

    // Makes the player immune to fire for a short time
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().FireImmunity(fireImmunityTimer);
    }

    // Makes the player take less fire damage
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
    }
}
