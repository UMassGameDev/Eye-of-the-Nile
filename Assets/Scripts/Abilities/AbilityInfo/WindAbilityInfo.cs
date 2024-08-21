using UnityEngine;

[CreateAssetMenu(fileName = "New WindAbilityInfo", menuName = "Abilities/Create New WindAbilityInfo")]
/*!<summary>
This is the info for the wind ability set made for the club fair build.
The offense ability triggers a melee attack with more knockback and range.
The defense ability shoots a wind projectile that pushes enemies back.
The utility ability triggers a really high jump (regardless of if the player is normally allowed to jump).
The passive ability allows the player to triple jump.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/14/2024
</summary>*/
public class WindAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public string meleeAnimName = "Attack";  // Name of the melee attack animation to run when the ability is activated.
    public float attackCooldown = 1f;  // How long the player must wait before melee attacking again.
    public float offenseRange = 1f;  // Range of the melee attack. Larger than a normal melee attack.
    public float offenseKnockback = 150f;  // The amount of knockback the melee attack should apply.

    [Header("Defense Ability Info")]
    public GameObject tornadoProjectilePrefab;  // Reference to the tornado projectile prefab this ability will instantiate.

    [Header("Utility Ability Info")]
    public float jumpForce;  // How much force this ability should apply to the super jump.

    [Header("Passive Ability Info")]
    public int newMaxJumpChain = 2;  // Maximum amount of times the player can jump without touching the ground.*
    public int defaultMaxJumpChain = 1;  // Normal amount of times the player can jump without touching the ground.*
    // * 0 would be a normal jump, 1 allows for double jumping, 2 for triple jumping, etc.

    // Triggers a melee attack with more knockback and range.
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().Melee(meleeAnimName, attackCooldown, offenseRange, offenseKnockback);
    }

    // Shoots a wind projectile that pushes enemies back.
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(tornadoProjectilePrefab);
    }

    // Triggers a really high jump (regardless of if the player is normally allowed to jump).
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().SpecialJump(jumpForce);
    }

    // Allows the player to triple jump.
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().maxJumpChain = newMaxJumpChain;
    }
}
