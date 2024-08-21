using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New RockAbilityInfo", menuName = "Abilities/Create New RockAbilityInfo")]
/*!<summary>
This is the info for the rock ability set made for the club fair build.
The offense ability throws a big boulder projectile.
The defense ability spawns a rock wall in front of the player.
The utility ability spawns a temporary rock platform under the player.
The passive ability is a simple defense stat increase. The player takes a set amount less damage.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/14/2024
</summary>*/
public class RockAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public GameObject boulderProjectilePrefab;  // Reference to the projectile prefab this ability will instantiate.

    [Header("Defense Ability Info")]
    public GameObject rockWallPrefab;  // Reference to the rock wall prefab this ability will instantiate.
    public float wallXOffset = 3f;  // Offset on the x axis where the wall will spawn in relation to the player.
    public float wallYOffset = 0f;  // Offset on the y axis where the wall will spawn in relation to the player.

    [Header("Utility Ability Info")]
    public GameObject rockPlatformPrefab;  // Reference to the rock platform prefab this ability will instantiate.
    public float platformXOffset = 0f;  // Offset on the x axis where the platform will spawn in relation to the player.
    public float platformYOffset = 0f;  // Offset on the y axis where the platform will spawn in relation to the player.

    [Header("Passive Ability Info")]
    public StatModifier passiveModifier;  // Reference to the stat modifier this ability will apply.

    // Throws a big boulder projectile.
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(boulderProjectilePrefab);
    }

    // Spawns a rock wall in front of the player.
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

    // Spawns a temporary rock platform under the player.
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Instantiate(rockPlatformPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + platformXOffset,
            abilityOwner.OwnerTransform.position.y + platformYOffset), Quaternion.identity);
    }

    // Defense stat increase.
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;  // get owner transform
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();  // get player stats
        playerStats.GetStat(passiveModifier.TargetStat).AddModifier(passiveModifier);  // add passive modifier
    }
}
