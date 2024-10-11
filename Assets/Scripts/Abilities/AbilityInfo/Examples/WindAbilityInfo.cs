using UnityEngine;

[CreateAssetMenu(fileName = "New WindAbilityInfo", menuName = "Abilities/Examples/Create New WindAbilityInfo")]
/*!<summary>
This is the info for the wind ability set made for the club fair build.
- The offense ability triggers a melee attack with more knockback and range.
- The defense ability shoots a wind projectile that pushes enemies back.
- The utility ability triggers a really high jump (regardless of if the player is normally allowed to jump).
- The passive ability allows the player to triple jump.

Documentation updated 8/14/2024
</summary>
\author Stephen Nuttall
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class WindAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set. The offense ability shoots bursts of fireballs.
    */
    ///@{
    /// \brief Name of the melee attack animation to run when the ability is activated.
    public string meleeAnimName = "Attack";  
    /// \brief How long the player must wait before melee attacking again.
    public float attackCooldown = 1f;  
    /// \brief Range of the melee attack. Larger than a normal melee attack.
    public float offenseRange = 1f;  
    /// \brief The amount of knockback the melee attack should apply.
    public float offenseKnockback = 150f;  
    ///@}

    [Header("Defense Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set. The offense ability shoots bursts of fireballs.
    */
    ///@{
    /// \brief Reference to the tornado projectile prefab this ability will instantiate.
    public GameObject tornadoProjectilePrefab;  
    ///@}

    [Header("Utility Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set. The offense ability shoots bursts of fireballs.
    */
    ///@{
    /// \brief How much force this ability should apply to the super jump.
    public float jumpForce;  
    ///@}

    [Header("Passive Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set. The offense ability shoots bursts of fireballs.
    */
    ///@{
    /// \brief Maximum amount of times the player can jump without touching the ground.
    /// \note 0 would be a normal jump, 1 allows for double jumping, 2 for triple jumping, etc.
    public int newMaxJumpChain = 2;  
    /// \brief Normal amount of times the player can jump without touching the ground.
    /// \note 0 would be a normal jump, 1 allows for double jumping, 2 for triple jumping, etc.
    public int defaultMaxJumpChain = 1;  
    ///@}

    /// Triggers a melee attack with more knockback and range.
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().Melee(meleeAnimName, attackCooldown, offenseRange, offenseKnockback);
    }

    /// Shoots a wind projectile that pushes enemies back.
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(tornadoProjectilePrefab);
    }

    /// Triggers a really high jump (regardless of if the player is normally allowed to jump).
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().Jump(jumpForce);
    }

    /// Allows the player to triple jump.
    protected override void AbilityPassiveEnable(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().maxJumpChain = newMaxJumpChain;
    }

    /// Placeholder AbilityPassiveDisable
    protected override void AbilityPassiveDisable(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerMovement>().maxJumpChain = defaultMaxJumpChain;
    }
}
