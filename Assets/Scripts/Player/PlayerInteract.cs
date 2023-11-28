using System.Collections;
using System.Collections.Generic;
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
