using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : BasicProjectile
{
    public int damageCount = 5;
    public float damageSpeed = 0.5f;

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
