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

    [Header("Defense Ability Info")]
    public GameObject tornadoProjectilePrefab;

    [Header("Utility Ability Info")]
    public float jumpForce;

    [Header("Passive Ability Info")]
    public int newMaxJumpChain = 2;
    public int defaultMaxJumpChain = 1;

    // Does a melee attack with more knockback and range
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().Melee(meleeAnimName, attackCooldown, offenseRange, offenseKnockback);
    }

    // Shoots a wind projectile that pushes enemies back
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(tornadoProjectilePrefab);
    }

    // Does a really high jump
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().SpecialJump(jumpForce);
    }

    // Allows the player to triple jump
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().maxJumpChain = newMaxJumpChain;
    }
}
