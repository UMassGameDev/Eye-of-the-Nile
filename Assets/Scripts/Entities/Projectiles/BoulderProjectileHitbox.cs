/**************************************************
The boulder projectile has a separate hitbox to determine when it collides with something

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class BoulderProjectileHitbox : MonoBehaviour
{
    BoulderProjectile boulderProjectile;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // if we can damage what we collided with, damage it and break the boulder
        if (((1 << collision.gameObject.layer) & boulderProjectile.collisionLayers.value) > 0)
        {
            if (collision.transform.TryGetComponent<ObjectHealth>(out var collisionObjHealth))
                collisionObjHealth.TakeDamage(transform, boulderProjectile.damage);

            boulderProjectile.BreakBoulder();
        }
    }

    void Awake()
    {
        boulderProjectile = transform.parent.GetComponent<BoulderProjectile>();
    }
}
