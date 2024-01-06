using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "New FireAbilityInfo", menuName = "Abilities/Create New FireAbilityInfo")]
public class FireAbility : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public GameObject projectilePrefab;
    public int numFireballs = 3;
    public float fireballDelay = 0.5f;

    PlayerAttackManager playerAttackManager;

    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        playerAttackManager = GameObject.Find("Player").GetComponent<PlayerAttackManager>();
        playerAttackManager.ShootProjectileBurst(projectilePrefab, numFireballs, fireballDelay);
    }

    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        //
    }

    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        //
    }

    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        //
    }
}
