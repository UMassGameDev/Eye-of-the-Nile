using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/** \brief
When the player presses the interactKey, this script will search for nearby interactable objects.
If an interactable object is within range, this script will tell that object to trigger its functionality.
It will also wait to see if the user holds the interact key down for long enough for it to be considered a "long press" and triggers any additional functionality.

Documentation updated 4/6/2025
\author Stephen Nuttall, Alexander Art
*/
public class PlayerInteract : MonoBehaviour
{
    /// \brief Reference to the player's's attack point. It's a point in space that's a child of the player, existing some distance in
    /// front of it. Projectiles spawn from the attack point, and melee attacks scan for enemies to damage from a certain radius around it.
    public Transform attackPoint;
    /// List of layers that the player can interact with objects on.
    public LayerMask interactableLayers;
    /// Reference to the DataManager. Used for checking if the Skyhub is unlocked.
    DataManager dataManager;

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
    /// Keep track of which interactables were in range on the previous frame/game loop cycle. Used for triggerProximityEnter/Stay/Exit.
    public Collider2D[] previouslyInRange;

    /// An event that reports the current progress on a long press interaction. Used for InteractProgressBar
    public static event Action<float> interactProgress;

    /// Set reference to dataManager.
    void Awake()
    {
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    /// <summary>
    /// Get all interactables currently overlapping with the player's hitbox (OnTriggerEnter() and OnTriggerExit())
    ///     - Scan for objects on the interactable layers that are interactRange distance away from the attackPoint.
    ///     - Combine the list of interactables near the attackPoint and the interactables currently touching the player.
    /// Tell all interactables within range that they are able to be interacted with.
    /// If the interact key is being pressed this frame...
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
        // All interactables within a radius (interactRange) of the attack point
        Collider2D[] hitInteractables = Physics2D.OverlapCircleAll(attackPoint.position, interactRange, interactableLayers);
        // Union of hitInteractables and touchedInteractables
        Collider2D[] allInRange = hitInteractables.Union(touchedInteractables).ToArray();

        // For each interactable in range, check if it was in range the previous cycle, and trigger proximity Enter/Stay accordingly.
        foreach (Collider2D interactable in allInRange)
        {
            // Check if the interactable was in range on the previous cycle.
            bool interactableFound = false;
            foreach (Collider2D previous in previouslyInRange)
            {
                if (interactable.transform == previous.transform)
                {
                    interactableFound = true;
                }
            }

            // If the interactable was in range the previous cycle, triggerProximityStay(). Otherwise, triggerProximityEnter().
            if (interactableFound == true)
            {
                interactable.GetComponent<ObjectInteractable>().triggerProximityStay();
            }
            else
            {
                interactable.GetComponent<ObjectInteractable>().triggerProximityEnter();
            }
        }

        // For each interactable in range on the previous cycle, check if it is currently in range, and trigger proximity Exit if it is not.
        foreach (Collider2D previous in previouslyInRange)
        {
            // Check if the interactable is still in range.
            bool interactableFound = false;
            foreach (Collider2D interactable in allInRange)
            {
                if (previous.transform == interactable.transform)
                {
                    interactableFound = true;
                }
            }

            // If the interactable that was previously in range is no longer in range, triggerProximityExit().
            if (interactableFound == false)
            {
                previous.GetComponent<ObjectInteractable>().triggerProximityExit();
            }
        }

        // Update previouslyInRange.
        previouslyInRange = allInRange;

        // When the interact key is pressed and there is an interactable in range, then invoke the interaction event
        if (Input.GetKeyDown(interactKey))
        {
            foreach (Collider2D interactable in allInRange)
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
            foreach (Collider2D interactable in allInRange)
            {
                // If object has any long press interaction events, allow the long press timer and cooldown to start unless it is a conditional warp to Skyhub and the Skyhub is not unlocked
                if (interactable.GetComponent<ObjectInteractable>().InvokeOnLongPress.GetPersistentEventCount() != 0 && (interactable.GetComponent<ObjectInteractable>().InvokeOnLongPress.GetPersistentMethodName(0) == "ConditionalWarpToSkyhub" && dataManager.skyhubUnlocked == false) == false)
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
