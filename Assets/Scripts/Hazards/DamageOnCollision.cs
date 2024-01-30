/**************************************************
Put this script on objects you want to damage entities upon colliding with them.
Useful for stage hazards, such as the falling spike.

Documentation updated 1/29/2024
**************************************************/
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

        if (health != null)
        {
            if (!col.collider.CompareTag("Player") && damageNonPlayers) {
                health.TakeDamage(this.transform, colisionDamage);
            } else if (col.collider.CompareTag("Player") && damagePlayer) {
                health.TakeDamage(this.transform, colisionDamage);
            }
        }
        
        if (destroyOnCollision)
            Destroy(gameObject);
    }
}
