using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    public int colisionDamage = 40;
    public bool damageNonPlayers = true;
    public bool damagePlayer = true;

    void OnTriggerEnter2D(Collider2D col)
    {
        ObjectHealth health = col.GetComponent<ObjectHealth>();
        if (health != null && damageNonPlayers)
            health.TakeDamage(this.transform, colisionDamage);
        
        PlayerHealth pHealth = col.GetComponent<PlayerHealth>();
        if (pHealth != null && damagePlayer)
            pHealth.TakeDamage(this.transform, colisionDamage);
    }
}
