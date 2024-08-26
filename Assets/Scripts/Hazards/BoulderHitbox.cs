using UnityEngine;

/** \brief
Detects collisions for the boulder hazard (\ref BoulderHazard).
When colliding with something on the boulder's collision layers, it will damage it and break the boulder.

Documentation updated 8/26/2024
\author Roy Pascual
*/
public class BoulderHitbox : MonoBehaviour
{
    /// Reference to the BoulderHazard component on the boulder.
    BoulderHazard boulderHazard;

    /// <summary>
    /// If an object on one of boulder's collision layers is in the boulder hitbox, damage it if possible, then destroy the boulder.
    /// </summary>
    /// <param name="collision">Represents the object that collided with the boulder hitbox.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & boulderHazard.collisionLayers.value) > 0)
        {
            if (collision.transform.TryGetComponent<ObjectHealth>(out var collisionObjHealth))
                collisionObjHealth.TakeDamage(transform, boulderHazard.damage);

            boulderHazard.BreakBoulder();
        }
    }

    /// Set reference to the BoulderHazard component.
    void Awake()
    {
        boulderHazard = transform.parent.GetComponent<BoulderHazard>();
    }
}
