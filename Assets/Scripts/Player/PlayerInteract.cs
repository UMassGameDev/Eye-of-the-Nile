/**************************************************
When the player presses the interactKey, this script will search for nearby interactable objects.
If an interactable object is within range, this script will tell that object to trigger its functionality.
It will also wait to see if the user holds the interact key down for long enough for it to be considered a "long press" and triggers any additional functionality.

Documentation updated 1/29/2024
**************************************************/
using System;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask interactableLayers;

    public float interactRange = 0.5f;
    public string interactKey = "e";
    public float interactCooldown = 0.5f;
    float cooldownTimer = 0;
    bool interactUsed;

    public float longPressLength = 1f;
    float interactTimer;
    bool keyDown = false;

    public static event Action<float> interactProgress;

    void Update()
    {
        if (Input.GetKey(interactKey))
        {
            Collider2D[] hitInteractables = Physics2D.OverlapCircleAll(attackPoint.position, interactRange, interactableLayers);

            if (!keyDown)
            {
                keyDown = true;
                interactTimer = Time.time + longPressLength;
            }

            foreach (Collider2D enemy in hitInteractables)
            {
                if (Time.time >= cooldownTimer)
                {
                    enemy.GetComponent<ObjectInteractable>().triggerInteraction();
                    interactUsed = true;
                }
                if (Time.time >= interactTimer)
                {
                    interactTimer = 0;
                    enemy.GetComponent<ObjectInteractable>().triggerLongPress();
                }
                interactProgress?.Invoke((interactTimer - Time.time)/longPressLength);
            }

            // this ensures that all objects in range are interacted with before the cooldown starts
            if (interactUsed)
            {
                cooldownTimer = Time.time + interactCooldown;
                interactUsed = false;
            }
        }
        else
        {
            keyDown = false;
            interactTimer = 0;
            interactProgress?.Invoke(0);
        }
    }
}
