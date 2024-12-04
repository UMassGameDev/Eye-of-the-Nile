using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/** \brief
When the player presses the interactKey, this script will search for nearby interactable objects.
If an interactable object is within range, this script will tell that object to trigger its functionality.
It will also wait to see if the user holds the interact key down for long enough for it to be considered a "long press" and triggers any additional functionality.

Documentation updated 9/1/2024
\author Stephen Nuttall
*/
public class PlayerInteract : MonoBehaviour
{
    /// \brief Reference to the player's's attack point. It's a point in space that's a child of the player, existing some distance in
    /// front of it. Projectiles spawn from the attack point, and melee attacks scan for enemies to damage from a certain radius around it.
    public Transform attackPoint;
    /// List of layers that the player can interact with objects on.
    public LayerMask interactableLayers;

    /// Distance from the attack point an object can be to allow interaction.
    public float interactRange = 0.5f;
    /// Name of the key the player must press to interact with an object
    public string interactKey = "e";
    /// Cooldown time after an interaction has happened, in seconds.
    public float interactCooldown = 0.5f;
    /// Time when cooldown time is over (interactCooldown seconds in the future).
    float cooldownTimer = 0;
    /// True if the player has already interacted with an object and the cooldown needs to start.
    bool interactUsed;

    /// Amount of seconds the user should have to hold the interact button to trigger a long press interaction.
    public float longPressLength = 1f;
    /// Time when long press interaction will be complete (longPressLength seconds in the future).
    float interactTimer;
    /// True during a long press interaction.
    bool longPressInteractionActive = false;
    /// List of all interactables overlapping with the player's hitbox.
    public List<Collider2D> touchedInteractables;

    /// An event that reports the current progress on a long press interaction. Used for InteractProgressBar
    public static event Action<float> interactProgress;

    /// <summary>
    /// Get all interactables currently overlapping with the player's hitbox (OnTriggerEnter() and OnTriggerExit())
    /// If the interact key is being pressed this frame...
    ///     - Scan for objects on the interactable layers that are interactRange distance away from the attackPoint.
    ///     - Combine the list of interactables near the attackPoint and the interactables currently touching the player.
    ///     - If the interaction key is pressed, trigger the interaction for each object found in our scan and start the cooldown.
    ///     - If the interaction key is held, then for each object found in our scan...
    ///         - If the object has long press functionality...
    ///             - Start and update the interaction timer
    ///             - If the long press is timer is up, reset the timer and trigger the long press interaction.
    ///         - Invoke the interactProgress event to update the interact progress bar.
    ///     - If the player just interacted with something, start the cooldown timer and set 
    ///     
    /// Otherwise...
    ///     - The interact key is not being held down, so set keyDown to false.
    ///     - Reset the interactTimer.
    ///     - Invoke the interactProgress event to update the interact progress bar.
    /// </summary>
    void Update()
    {
        // When the interact key is pressed and there is an interactable in range, then invoke the interaction event
        if (Input.GetKeyDown(interactKey))
        {
            // All interactables within a radius (interactRange) of the attack point
            Collider2D[] hitInteractables = Physics2D.OverlapCircleAll(attackPoint.position, interactRange, interactableLayers);
            // Union of hitInteractables and touchedInteractables
            Collider2D[] allInteracted = hitInteractables.Union(touchedInteractables).ToArray();

            foreach (Collider2D interactable in allInteracted)
            {
                if (Time.time >= cooldownTimer)
                {
                    interactable.GetComponent<ObjectInteractable>().triggerInteraction();
                    interactUsed = true;
                }
            }

            // This ensures that all objects in range are interacted with before the cooldown starts
            if (interactUsed)
            {
                cooldownTimer = Time.time + interactCooldown;
                interactUsed = false;
            }
        }

        // Handle long press interactions when the interact key is held
        if (Input.GetKey(interactKey))
        {
            // All interactables within a radius (interactRange) of the attack point
            Collider2D[] hitInteractables = Physics2D.OverlapCircleAll(attackPoint.position, interactRange, interactableLayers);
            // Union of hitInteractables and touchedInteractables
            Collider2D[] allInteracted = hitInteractables.Union(touchedInteractables).ToArray();

            foreach (Collider2D interactable in allInteracted)
            {
                // If object has any long press interaction events, allow the long press timer and cooldown to start
                if (interactable.GetComponent<ObjectInteractable>().InvokeOnLongPress.GetPersistentEventCount() != 0)
                {
                    if (!longPressInteractionActive)
                    {
                        interactTimer = Time.time + longPressLength;
                        longPressInteractionActive = true;
                    }
                    interactProgress?.Invoke((interactTimer - Time.time) / longPressLength);
                    interactUsed = true;
                }

                // If the interact timer is met or surpassed, reset it and invoke the long press event
                if (Time.time >= interactTimer)
                {
                    interactTimer = 0;
                    interactable.GetComponent<ObjectInteractable>().triggerLongPress();
                }
            }
    
            // This ensures that all objects in range are interacted with before the cooldown or timer starts
            if (interactUsed)
            {
                cooldownTimer = Time.time + interactCooldown;
                interactUsed = false;
            }
            else
            {
                longPressInteractionActive = false;
                interactTimer = 0;
                interactProgress?.Invoke(0);
            }
        }
        else
        {
            longPressInteractionActive = false;
            interactTimer = 0;
            interactProgress?.Invoke(0);
        }
    }

    /// \brief Runs when an object enters the player's hitbox.
    /// Adds any newly touched interactables to the touchedInteractables list.
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & interactableLayers) != 0)
        {
            if (!touchedInteractables.Contains(collision))
                touchedInteractables.Add(collision);
        }
    }

    /// \brief Runs when an object exits the player's hitbox.
    /// Remove any untouched interactables from the touchedInteractables list.
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & interactableLayers) != 0)
        {
            if (touchedInteractables.Contains(collision))
                touchedInteractables.Remove(collision);
        }
    }
}
