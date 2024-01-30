/**************************************************
Put this script on objects you want to damage entities upon touching with them.
The difference between this and DamageOnCollision is this is for objects whose Collider2D has isTrigger checked off in the editor.
Useful for stage hazards, such as the spikes tile.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    public int colisionDamage = 40;
    public bool damageNonPlayers = true;
    public bool damagePlayer = true;

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
