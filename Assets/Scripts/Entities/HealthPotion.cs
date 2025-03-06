using System;
using UnityEngine;

/** \brief
Functionality for the health potion object that spawns after breaking a pot.
This script simply invokes an event and destroys the object when the player enters the trigger zone.

Documentation updated 3/5/2024
\author Stephen Nuttall
*/
public class HealthPotion : MonoBehaviour
{
    /// Triggers when the player enters the trigger zone.
    public static event Action potionPickedUp;

    /// Invoke potionPickedUp and destroy the object when the player enters the trigger zone.
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            potionPickedUp?.Invoke();
            Destroy(gameObject);
        }
    }
}
