using UnityEngine;

[CreateAssetMenu(fileName = "GebAbilityInfo", menuName = "Abilities/Create New GebAbilityInfo")]
/*!<summary>
This is the info for Geb's ability set.
- The offense ability throws a projectile summons a small earthquake. It also changes the player's melee attack to be slower but do more damage.
- The defense ability spawns a rock wall in front of the player. The rock wall pushes forward slightly with each upgrade.
- The utility ability spawns a temporary rock platform under the player.
- The passive ability gives the player damage resistance and knockback resistance.

Documentation updated 10/10/2024
</summary>
\author Stephen Nuttall
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class GebAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set.
    *  The offense ability throws a projectile summons a small earthquake. It also changes the player's melee attack to be slower but do more damage.
    */
    ///@{
    /// Reference to the projectile prefab this ability will instantiate for each level - 1 (array starts at 0).
    [SerializeField] GameObject[] earthquakeProjectilePrefab;
    ///@}

    [Header("Defense Ability Info")]
    /** @name Defense Ability Info
    *  Information related to the defense ability in this set.
    *  The defense ability spawns a rock wall in front of the player. The rock wall pushes forward slightly with each upgrade.
    */
    ///@{
    /// Reference to the rock wall prefab this ability will instantiate for each level - 1 (array starts at 0).
    [SerializeField] GameObject[] rockWallPrefab;
    /// Where the wall will spawn in relation to the player.
    [SerializeField] Vector2 wallOffset;
    ///@}

    [Header("Utility Ability Info")]
    /** @name Utility Ability Info
    *  Information related to the utility ability in this set.
    *  The utility ability spawns a temporary rock platform under the player.
    */
    ///@{
    /// Reference to the rock platform prefabs this ability will instantiate for each level - 1 (array starts at 0).
    [SerializeField] GameObject[] rockPlatformPrefab;
    /// Where the platform will spawn in relation to the player.
    [SerializeField] Vector2 platformOffset;
    ///@}

    [Header("Passive Ability Info")]
    /** @name Passive Ability Info
    *  Information related to the passive ability in this set.
    *  The passive ability gives the player damage resistance and knockback resistance.
    */
    ///@{
    /// The amount of damage resistance that should be applied for each level - 1 (array starts at 0).
    [SerializeField] int[] damageResistanceModValues;
    /// The amount of knockback resistance that should be applied for each level - 1 (array starts at 0).
    [SerializeField] int[] kbResistanceModValues;
    /// Reference to the damage resistance stat modifier this ability will apply.
    StatModifier damageResistanceMod;
    /// Reference to the knockback resistance stat modifier this ability will apply.
    StatModifier kbResistanceMod;
    ///@}

    /// Throws a projectile summons a small earthquake. It also changes the player's melee attack to be slower but do more damage.
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(earthquakeProjectilePrefab[abilityLevel - 1]);
    }

    /// Spawns a rock wall in front of the player. The rock wall pushes forward slightly with each upgrade.
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        float xOffset = wallOffset.x;
        if (abilityOwner.OwnerTransform.localScale.x > 0)
        {
            xOffset = -wallOffset.x;
        }
        
        GameObject rockWall = Instantiate(rockWallPrefab[abilityLevel - 1], new Vector2(
            abilityOwner.OwnerTransform.position.x + xOffset,
            abilityOwner.OwnerTransform.position.y + wallOffset.y), Quaternion.identity);
        
        if (abilityOwner.OwnerTransform.localScale.x > 0 && rockWall.TryGetComponent<MoveInDirection>(out var moveInDirection))
        {
            moveInDirection.movementDirection.x = -moveInDirection.movementDirection.x;
        }
    }

    /// Spawns a temporary rock platform under the player.
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        float xOffset = platformOffset.x;
        if (abilityOwner.OwnerTransform.localScale.x > 0) {
            xOffset = -platformOffset.x;
        }
        
        Instantiate(rockPlatformPrefab[abilityLevel - 1], new Vector2(
            abilityOwner.OwnerTransform.position.x + xOffset,
            abilityOwner.OwnerTransform.position.y + platformOffset.y), Quaternion.identity);
    }

    /// Gives the player damage resistance and knockback resistance.
    protected override void AbilityPassiveEnable(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;  // get owner transform
        PlayerStatHolder playerStatHolder = ownerTransform.GetComponent<PlayerStatHolder>();  // get player stats

        // set target stat strings for Stat modifiers
        damageResistanceMod.TargetStat = "DamageResistance";
        kbResistanceMod.TargetStat = "KnockbackResistance";

        // set mod values for stat modifiers (if abilityLevel is within range).
        if (abilityLevel > 0 && abilityLevel <= maxLevel)
        {
            damageResistanceMod.ModValue = damageResistanceModValues[abilityLevel - 1];
            kbResistanceMod.ModValue = kbResistanceModValues[abilityLevel - 1];
        }

        // add stat modifiers to player
        playerStatHolder.GetStat(damageResistanceMod.TargetStat).AddModifier(damageResistanceMod);
        playerStatHolder.GetStat(kbResistanceMod.TargetStat).AddModifier(kbResistanceMod);
    }

    /// Removes the passive ability's damage resistance and knockback resistance stat increases.
    protected override void AbilityPassiveDisable(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;  // get owner transform
        PlayerStatHolder playerStatHolder = ownerTransform.GetComponent<PlayerStatHolder>();  // get player stats

        // set target stat strings for Stat modifiers
        damageResistanceMod.TargetStat = "DamageResistance";
        kbResistanceMod.TargetStat = "KnockbackResistance";

        playerStatHolder.GetStat(damageResistanceMod.TargetStat).RemoveModifier(damageResistanceMod);  // remove damage resistance modifier
        playerStatHolder.GetStat(kbResistanceMod.TargetStat).RemoveModifier(kbResistanceMod);  // remove kb resistance modifier
    }
}
