using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnColision : MonoBehaviour
{
    public int colisionDamage = 40;

    void OnTriggerEnter2D(Collider2D col)
    {
        ObjectHealth health = col.GetComponent<ObjectHealth>();
        if (health != null)
            health.TakeDamage(this.transform, colisionDamage);
    }
}
