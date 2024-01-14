using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "New WindAbilityInfo", menuName = "Abilities/Create New WindAbilityInfo")]
public class WindAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public string meleeAnimName = "Attack";
    public float attackCooldown = 1f;
    public float offenseRange = 1f;
    public float offenseKnockback = 150f;

    [Header("Passive Ability Info")]
    public int newMaxJumpChain = 2;
    public int defaultMaxJumpChain = 1;

    // Gives Player's Melee Attack Knockback
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().Melee(meleeAnimName, attackCooldown, offenseRange, offenseKnockback);
    }

    // Fires Wind Projectile that pushes enemies back
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        //
    }

    // ???
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        //
    }

    // Triple Jump
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().maxJumpChain = newMaxJumpChain;
    }
}