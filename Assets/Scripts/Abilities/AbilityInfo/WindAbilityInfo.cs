/**************************************************
This is the info for the wind ability set made for the club fair build.
The offense ability triggers a melee attack with more knockback and range.
The defense ability shoots a wind projectile that pushes enemies back.
The utility ability triggers a really high jump (regardless of if the player is normally allowed to jump).
The passive ability allows the player to triple jump.
This is a scriptable object, meaning you can make and instance of it in the editor.

Documentation updated 1/29/2024
**************************************************/
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

    // Triggers a melee attack with more knockback and range
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().Melee(meleeAnimName, attackCooldown, offenseRange, offenseKnockback);
    }

    // Shoots a wind projectile that pushes enemies back
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(tornadoProjectilePrefab);
    }

    // Triggers a really high jump (regardless of if the player is normally allowed to jump)
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
