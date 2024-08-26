using UnityEngine;

/** \brief
Put this script on objects you want to damage entities upon touching with them.
The difference between this and DamageOnCollision is this is for objects whose Collider2D has isTrigger checked off in the editor.
Useful for stage hazards, such as the spikes tile.

Documentation updated 8/26/2024
\author Stephen Nuttall
\note There is a similar script called DamageOnCollision that does the same thing for objects whose Collider2D has isTrigger
NOT checked off in the editor. This requires a different function to be used, which is why there's a separate script for it.
\todo Replace damageNonPlayers and damagePlayer bools with one layer mask that allows for more choices in what is damaged.
*/
public class DamageOnTrigger : MonoBehaviour
{
    /// Amount of damage to apply when upon colliding with an object (that can be damaged).
    public int colisionDamage = 40;
    /// If true, entities that are not the player will take damage when colliding with this object.
    public bool damageNonPlayers = true;
    /// If true, the player will take damage when colliding with this object.
    public bool damagePlayer = true;

    /// <summary>
    /// When an object collides with this object, damage it if it has an object health (unless damageNonPlayers or damagePlayer
    /// dictates otherwise). Then destroy the game object (if destroyOnCollision is true).
    /// </summary>
    /// <param name="col">Represents the object that collided with this object.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        ObjectHealth health = col.GetComponent<ObjectHealth>();
        
        if (health != null)
        {
            if (!col.CompareTag("Player") && damageNonPlayers) {
                health.TakeDamage(this.transform, colisionDamage);
            } else if (col.CompareTag("Player") && damagePlayer) {
                health.TakeDamage(this.transform, colisionDamage);
            }
        }
    }
}
