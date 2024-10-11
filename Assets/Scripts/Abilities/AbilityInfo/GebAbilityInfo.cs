using System;
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
    /// Reference to the projectile prefab this ability will instantiate.
    [SerializeField] GameObject earthquakeProjectilePrefab;
    ///@}

    [Header("Defense Ability Info")]
    /** @name Defense Ability Info
    *  Information related to the defense ability in this set.
    *  The defense ability spawns a rock wall in front of the player. The rock wall pushes forward slightly with each upgrade.
    */
    ///@{
    /// Reference to the rock wall prefab this ability will instantiate.
    [SerializeField] GameObject rockWallPrefab;
    /// Offset on the x axis where the wall will spawn in relation to the player.
    [SerializeField] float wallXOffset = 3f; 
    /// Offset on the y axis where the wall will spawn in relation to the player.
    [SerializeField] float wallYOffset = 0f;
    ///@}

    [Header("Utility Ability Info")]
    /** @name Utility Ability Info
    *  Information related to the utility ability in this set.
    *  The utility ability spawns a temporary rock platform under the player.
    */
    ///@{
    /// Reference to the rock platform prefab this ability will instantiate.
    [SerializeField] GameObject rockPlatformPrefab;
    /// Offset on the x axis where the platform will spawn in relation to the player.
    [SerializeField] float platformXOffset = 0f;
    /// Offset on the y axis where the platform will spawn in relation to the player.
    [SerializeField] float platformYOffset = 0f;
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
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(earthquakeProjectilePrefab);
    }

    /// Spawns a rock wall in front of the player. The rock wall pushes forward slightly with each upgrade.
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        if (abilityOwner.OwnerTransform.localScale.x > 0) {
            wallXOffset = -Math.Abs(wallXOffset);
        } else {
            wallXOffset = Math.Abs(wallXOffset);
        }
        
        Instantiate(rockWallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + wallXOffset,
            abilityOwner.OwnerTransform.position.y + wallYOffset), Quaternion.identity);
    }

    /// Spawns a temporary rock platform under the player.
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Instantiate(rockPlatformPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + platformXOffset,
            abilityOwner.OwnerTransform.position.y + platformYOffset), Quaternion.identity);
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
