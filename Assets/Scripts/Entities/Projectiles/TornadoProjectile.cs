/**************************************************
Script for a tornado projectile that deals no damage, instead pushing targets away with its hitbox.
Inherits from BasicProjectile.

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;

public class TornadoProjectile : BasicProjectile
{
    public float despawnTime = 5f;  // in seconds

    void Start()
    {
        AudioManager.Instance.PlaySFX(spawnSFX);
        StartCoroutine(DespawnTimer());
    }

    void Update()
    {
        // move projectile to the left/right by [speed]. Don't flip sprite
        if (facingLeft) {
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
        } else {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
        }
    }

    // this was made before the DespawnTimer script was written
    IEnumerator DespawnTimer()
    {
        // Wait for despawn time to be up
        yield return new WaitForSeconds(despawnTime);
        
        // Destory the projectile
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        // Don't do anything on impact
    }
}
