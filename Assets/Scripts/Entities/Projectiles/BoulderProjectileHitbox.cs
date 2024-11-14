using UnityEngine;

/** \brief
The boulder projectile has a separate hitbox to determine when it collides with something.
Adapted from BoulderHitbox.

Documentation updated 9/7/2024
\author Stephen Nuttall
*/
public class BoulderProjectileHitbox : MonoBehaviour
{
    /// Reference to the BoulderProjectile component of the parent object.
    BoulderProjectile boulderProjectile;

    /// <summary>
    /// If an object on the BoulderProjectile.collisionLayers enters the hitbox, damage it and break the boulder.
    /// </summary>
    /// <param name="collision">Represents the object that entered the hitbox.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // if we can damage what we collided with, damage it and break the boulder
        if (((1 << collision.gameObject.layer) & boulderProjectile.collisionLayers.value) > 0)
        {
            if (collision.transform.TryGetComponent<ObjectHealth>(out var collisionObjHealth))
                collisionObjHealth.TakeDamage(transform, boulderProjectile.refDamage);

            boulderProjectile.BreakBoulder();
        }
    }

    /// Set reference to the BoulderProjectile component.
    void Awake()
    {
        boulderProjectile = transform.parent.GetComponent<BoulderProjectile>();
    }
}
