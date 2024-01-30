/**************************************************
Allows an ability to spawn a projectile where the ability owner (player) is.
Note that this script does not spawn the projectile the standard way the PlayerAttackManager does.
Because of this, I (Stephen) prefer to use the PlayerAttackManager's ShootProjectile() function (see fire/wind/rock ability info for an example).
This is a scriptable object, meaning you can make and instance of it in the editor.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class SpawnAE : AbilityEffect
{
    public AbilityProjectile projectilePrefab;
    public int objectCount = 1;
    // some spawn pattern variable eventually, or maybe in function

    public override void Apply(AbilityOwner abilityOwner)
    {
        for (int i = 0; i < objectCount; i++)
        {
            AbilityProjectile spawnedProjectile = Instantiate(projectilePrefab,
            abilityOwner.OwnerTransform.position, Quaternion.identity);
        }
    }
}
