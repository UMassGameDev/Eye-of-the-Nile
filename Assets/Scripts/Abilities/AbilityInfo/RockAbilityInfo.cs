using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New RockAbilityInfo", menuName = "Abilities/Create New RockAbilityInfo")]
public class RockAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public GameObject boulderProjectilePrefab;

    [Header("Defense Ability Info")]
    public GameObject rockWallPrefab;
    public float wallXOffset = 3f;
    public float wallYOffset = 0f;

    [Header("Utility Ability Info")]
    public GameObject rockPlatformPrefab;
    public float platformXOffset = 0f;
    public float platformYOffset = 0f;

    [Header("Passive Ability Info")]
    public StatModifier passiveModifier;

    // Throws a big boulder projectile
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(boulderProjectilePrefab);
    }

    // Spawns a rock wall in front of the player
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

    // Spawns temporary rock platform under the player
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Instantiate(rockPlatformPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + platformXOffset,
            abilityOwner.OwnerTransform.position.y + platformYOffset), Quaternion.identity);
    }

    // Defense stat increase
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(passiveModifier.TargetStat).AddModifier(passiveModifier);
    }
}
