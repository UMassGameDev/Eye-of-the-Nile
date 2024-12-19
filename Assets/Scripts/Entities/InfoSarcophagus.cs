using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** \brief
This script controls the functionality of tutorial sarcophagi.
"The player will encounter sarcophaguses with the ghost of Osiris in it to receive tips from him at relevant moments.
This consists of a basic animation and speech bubble that’s triggered when the player gets within range."
This script is simple enough that it could probably be applied to other types of objects and it would still work.

Documentation updated 12/19/2024
\author Alexander Art
*/

public class InfoSarcophagus : MonoBehaviour
{
    /// Objects on this layer will be considered a player, and if detected, this sarcophagus will say its message.
    [SerializeField] protected LayerMask playerLayer;
    /// The GameObject that the sarcophagus will make appear as its message.
    [SerializeField] protected GameObject message;

    /// How close the sarcophagus must be to the player to say its message.
    [SerializeField] protected float activateMessageRange = 4f;

    /// True if the player is close enough to the sarcophagus to say its message.
    protected bool playerInRange = false;

    void ActivateMessage()
    {
        message.SetActive(true);
    }

    void DeactivateMessage()
    {
        message.SetActive(false);
    }

    void Update()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 0.5f, 0f),
            activateMessageRange,
            playerLayer);

        playerInRange = player != null;

        if (playerInRange == true)
        {
            ActivateMessage();
        }
        else if (playerInRange == false)
        {
            DeactivateMessage();
        }
    }
}
