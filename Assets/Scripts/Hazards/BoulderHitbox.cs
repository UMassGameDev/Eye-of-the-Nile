using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderHitbox : MonoBehaviour
{
    BoulderHazard boulderHazard;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & boulderHazard.collisionLayers.value) > 0)
        {
            ObjectHealth collisionObjHealth = collision.transform.GetComponent<ObjectHealth>();
            if (collisionObjHealth != null)
                collisionObjHealth.TakeDamage(transform, boulderHazard.damage);
            
            PlayerHealth collisionPlayerHealth = collision.transform.GetComponent<PlayerHealth>();
            if (collisionPlayerHealth != null)
                collisionPlayerHealth.TakeDamage(transform, boulderHazard.damage);

            boulderHazard.BreakBoulder();
        }
    }

    void Awake()
    {
        boulderHazard = transform.parent.GetComponent<BoulderHazard>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
