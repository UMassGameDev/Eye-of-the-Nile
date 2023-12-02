using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public int colisionDamage = 40;
    public bool damageNonPlayers = true;
    public bool damagePlayer = true;
    public bool destroyOnCollision = true;

    void OnCollisionEnter2D(Collision2D col)
    {
        ObjectHealth health = col.collider.GetComponent<ObjectHealth>();
        if (health != null && damageNonPlayers)
            health.TakeDamage(this.transform, colisionDamage);
        
        PlayerHealth pHealth = col.collider.GetComponent<PlayerHealth>();
        if (pHealth != null && damagePlayer)
            pHealth.TakeDamage(this.transform, colisionDamage);
        
        if (destroyOnCollision)
            Destroy(gameObject);
    }
}
