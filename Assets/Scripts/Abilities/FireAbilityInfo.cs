using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "New FireAbilityInfo", menuName = "Abilities/Create New FireAbilityInfo")]
public class FireAbility : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public GameObject projectilePrefab;
    public int numFireballs = 3;
    public float fireballDelay = 0.5f;

    [Header("Defense Ability Info")]
    public GameObject firewallPrefab;
    public float firewallXOffset = 1.5f;
    public float firewallYOffset = 0.5f;

    [Header("Utility Ability Info")]
    public float fireImmunityTimer = 3f;

    // Shoot burst of fireballs
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectileBurst(projectilePrefab, numFireballs, fireballDelay);
    }

    // Spawn a tall fires on each side of the player
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Instantiate(firewallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + firewallXOffset,
            abilityOwner.OwnerTransform.position.y + firewallYOffset), Quaternion.identity);

        Instantiate(firewallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x - firewallXOffset,
            abilityOwner.OwnerTransform.position.y + firewallYOffset), Quaternion.identity);
    }

    // Makes the player immune to fire for a short time
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().FireImmunity(fireImmunityTimer);
    }

    // Makes the player take less fire damage
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        //
    }
}
