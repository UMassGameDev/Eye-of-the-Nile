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

    // [Header("Utility Ability Info")]

    // [Header("Passive Ability Info")]

    // Throws a big boulder
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectile(boulderProjectilePrefab);
    }

    // Puts up a big rock wall
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

    // Spawn temporary rock platform
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        //
    }

    // Defense Stat Increase
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        //
    }
}
