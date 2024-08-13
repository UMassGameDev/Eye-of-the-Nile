/**************************************************
Allows an ability to spawn a projectile where the ability owner (player) is.
Note that this script does not spawn the projectile the standard way the PlayerAttackManager does.
Because of this, it’s recommended that you use the PlayerAttackManager's ShootProjectile() function (see fire/wind/rock ability info for an example).
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/13/2024
**************************************************/
using UnityEngine;

public class SpawnAE : AbilityEffect
{
    public AbilityProjectile projectilePrefab;
    public int objectCount = 1;
    // some spawn pattern variable eventually, or maybe in function

    // Instantiate the projectile prefab at the player’s position.
    // (this is part of the problem. If you spawn a projectile inside the player it will just hit the player)
    public override void Apply(AbilityOwner abilityOwner)
    {
        for (int i = 0; i < objectCount; i++)
        {
            AbilityProjectile spawnedProjectile = Instantiate(projectilePrefab,
            abilityOwner.OwnerTransform.position, Quaternion.identity);
        }
    }
}
