using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New RockAbilityInfo", menuName = "Abilities/Examples/Create New RockAbilityInfo")]
/*!<summary>
This is the info for the rock ability set made for the club fair build.
- The offense ability throws a big boulder projectile.
- The defense ability spawns a rock wall in front of the player.
- The utility ability spawns a temporary rock platform under the player.
- The passive ability is a simple defense stat increase. The player takes a set amount less damage.

Documentation updated 8/14/2024
</summary>
\author Stephen Nuttall
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class RockAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set. The offense ability throws a big boulder projectile.
    */
    ///@{
    /// \brief Reference to the projectile prefab this ability will instantiate.
    public GameObject boulderProjectilePrefab;
    ///@}

    [Header("Defense Ability Info")]
    /** @name Defense Ability Info
    *  Information related to the defense ability in this set. The defense ability spawns a rock wall in front of the player.
    */
    ///@{
    /// \brief Reference to the rock wall prefab this ability will instantiate.
    public GameObject rockWallPrefab;
    /// \brief Offset on the x axis where the wall will spawn in relation to the player.
    public float wallXOffset = 3f; 
    /// \brief Offset on the y axis where the wall will spawn in relation to the player.
    public float wallYOffset = 0f;
    ///@}

    [Header("Utility Ability Info")]
    /** @name Utility Ability Info
    *  Information related to the utility ability in this set. The utility ability spawns a temporary rock platform under the player.
    */
    ///@{
    /// \brief Reference to the rock platform prefab this ability will instantiate.
    public GameObject rockPlatformPrefab;
    /// \brief Offset on the x axis where the platform will spawn in relation to the player.
    public float platformXOffset = 0f;
    /// \brief Offset on the y axis where the platform will spawn in relation to the player.
    public float platformYOffset = 0f;
    ///@}

    [Header("Passive Ability Info")]
    /** @name Passive Ability Info
    *  Information related to the passive ability in this set.
    *  The passive ability is a simple defense stat increase. The player takes a set amount less damage.
    */
    ///@{
    /// \brief Reference to the stat modifier this ability will apply.
    public StatModifier passiveModifier;
    ///@}

    /// Throws a big boulder projectile.
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(boulderProjectilePrefab);
    }

    /// Spawns a rock wall in front of the player.
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

    /// Simple defense stat increase. The player takes a set amount less damage.
    protected override void AbilityPassiveEnable(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;  // get owner transform
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();  // get player stats
        playerStats.GetStat(passiveModifier.TargetStat).AddModifier(passiveModifier);  // add passive modifier
    }

    /// Removes the defense stat increase, returning the amount of damage the player takes to normal.
    protected override void AbilityPassiveDisable(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;  // get owner transform
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();  // get player stats
        playerStats.GetStat(passiveModifier.TargetStat).RemoveModifier(passiveModifier);  // remove passive modifier
    }
}
