/**************************************************
Detects collisions for the boulder hazard (BoulderHazard.cs).
When colliding with something on the boulder's collision layers, it will damage it and break the boulder.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class BoulderHitbox : MonoBehaviour
{
    BoulderHazard boulderHazard;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & boulderHazard.collisionLayers.value) > 0)
        {
            if (collision.transform.TryGetComponent<ObjectHealth>(out var collisionObjHealth))
                collisionObjHealth.TakeDamage(transform, boulderHazard.damage);

            boulderHazard.BreakBoulder();
        }
    }

    void Awake()
    {
        boulderHazard = transform.parent.GetComponent<BoulderHazard>();
    }
}
