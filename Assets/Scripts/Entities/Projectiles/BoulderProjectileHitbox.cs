using UnityEngine;

public class BoulderProjectileHitbox : MonoBehaviour
{
    BoulderProjectile boulderProjectile;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (((1 << collision.gameObject.layer) & boulderProjectile.collisionLayers.value) > 0)
        {
            Debug.Log("!!!!!!!!!!!");
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
