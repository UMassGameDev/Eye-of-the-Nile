using UnityEngine;

/*!<summary>
Allows an ability to spawn a projectile where the ability owner (player) is.

Documentation updated 8/13/2024
</summary>
\deprecated This script does not spawn the projectile the standard way the PlayerAttackManager does.
Because of this, it’s recommended that you use the PlayerAttackManager's ShootProjectile() function (see fire/wind/rock ability info for an example).
This is a scriptable object, meaning you can make an instance of it in the editor.
This script may be improved or repurposed in the future, but for now I would not recommend using it.
\todo some spawn pattern variable eventually, or maybe in function*/
public class SpawnAE : AbilityEffect
{
    /// \brief Reference to the projectile prefab we want to spawn.
    public AbilityProjectile projectilePrefab;
    /// \brief Amount of objects to spawn. Hardcoded to 1.
    public int objectCount = 1;

    // some spawn pattern variable eventually, or maybe in function

    /// <summary>
    /// Instantiate the projectile prefab at the player’s position.
    /// (this is part of the problem. If you spawn a projectile inside the player it will just hit the player)
    /// </summary>
    /// <param name="abilityOwner"></param>
    public override void Apply(AbilityOwner abilityOwner)
    {
        for (int i = 0; i < objectCount; i++)
        {
            AbilityProjectile spawnedProjectile = Instantiate(projectilePrefab,
            abilityOwner.OwnerTransform.position, Quaternion.identity);
        }
    }
}
