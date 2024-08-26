using UnityEngine;

/**
Put this script on objects you want to damage entities upon colliding with them.
Useful for stage hazards, such as the \ref Prefabs_FallingSpike.

Documentation updated 8/26/2024
\author Stephen Nuttall
\note There is a similar script called DamageOnTrigger that does the same thing for objects whose Collider2D has isTrigger
checked off in the editor. This requires a different function to be used, which is why there's a separate script for it.
\todo Replace damageNonPlayers and damagePlayer bools with one layer mask that allows for more choices in what is damaged.
*/
public class DamageOnCollision : MonoBehaviour
{
    /// Amount of damage to apply when upon colliding with an object (that can be damaged).
    public int colisionDamage = 40;
    /// If true, entities that are not the player will take damage when colliding with this object.
    public bool damageNonPlayers = true;
    /// If true, the player will take damage when colliding with this object.
    public bool damagePlayer = true;
    /// If true, the object will be destroyed as soon as it collides with something (regardless of if it's damagable or not).
    public bool destroyOnCollision = true;

    /// <summary>
    /// When an object collides with this object, damage it if it has an object health (unless damageNonPlayers or damagePlayer
    /// dictates otherwise). Then destroy the game object (if destroyOnCollision is true).
    /// </summary>
    /// <param name="col">Represents the object that collided with this object.</param>
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
