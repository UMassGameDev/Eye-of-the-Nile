using UnityEngine;

/** \brief
Script for a basic projectile that sets a target on fire rather than dealing a flat damage amount.
Inherits from BasicProjectile.

Documentation updated 9/7/2024
\author Stephen Nuttall
*/
public class FireProjectile : BasicProjectile
{
    /// Amount of times the fire should deal damage.
    public int damageCount = 5;
    /// How fast the fire should deal damage.
    public float damageSpeed = 0.5f;

    /// <summary>
    /// Same as the base version, but sets the target on fire rather than dealing a flat damage amount.
    /// </summary>
    /// <param name="collisionInfo">Represents the object the projectile collided with.</param>
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        // if we collided with something we can damage, damage it
        if (collisionInfo.collider.tag == "DamagableByProjectile" && damageNonPlayers) {
            collisionInfo.collider.GetComponent<ObjectHealth>().SetOnFire(damageCount, damageSpeed, damage);
        } else if (collisionInfo.collider.tag == "Player" && damagePlayers) {
            collisionInfo.collider.GetComponent<PlayerHealth>().SetOnFire(damageCount, damageSpeed, damage);
        }
        
        // Destory the projectile
        Destroy(sprite);
        Destroy(gameObject);
    }
}
