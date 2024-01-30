/**************************************************
When the player presses the interactKey, this script will search for nearby interactable objects.
If an interactable object is within range, this script will tell that object to trigger its functionality.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask interactableLayers;

    public float interactRange = 0.5f;
    public float interactCooldown = 1f;
    public string interactKey = "e";

    void Update()
    {
        if (Input.GetKey(interactKey))
        {
            Collider2D[] hitInteractables = Physics2D.OverlapCircleAll(attackPoint.position, interactRange, interactableLayers);

            foreach (Collider2D enemy in hitInteractables)
            {
                enemy.GetComponent<ObjectInteractable>().triggerInteraction();
            }
        }
    }
}
