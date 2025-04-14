using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** \brief
This script controls the functionality of the speech bubbles/messages that are triggered when the player gets within range.

Documentation updated 4/13/2025
\author Alexander Art
*/
public class SpeechBubble : MonoBehaviour
{
    /// Objects on this layer are detected as the player.
    [SerializeField] protected LayerMask playerLayer;
    /// The GameObject that will appear as a speech bubble.
    [SerializeField] protected GameObject message;
    // Animator for the speech bubble.
    public Animator messageAnimator;

    /// How close the player must be for the message to appear.
    [SerializeField] protected float detectionRange = 4f;

    /// True if the player is close enough for the message to appear.
    protected bool playerInRange = false;
    /// True if the message is active (visible).
    protected bool messageActive = false;

    void ActivateMessage()
    {
        messageActive = true;
        messageAnimator.SetTrigger("Appear");
    }

    void DeactivateMessage()
    {
        messageActive = false;
        messageAnimator.SetTrigger("Disappear");
    }

    void Update()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 0.5f, 0f),
            detectionRange,
            playerLayer);

        playerInRange = player != null;

        if (playerInRange == true && messageActive == false)
        {
            ActivateMessage();
        }
        else if (playerInRange == false && messageActive == true)
        {
            DeactivateMessage();
        }
    }
}
