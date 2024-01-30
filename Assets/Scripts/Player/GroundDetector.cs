/**************************************************
This script is used by a small trigger zone beneath the player's feet.
When the player is on the ground, isGrounded will be true.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isGrounded {get; private set;} = false;

    void FixedUpdate()
    {
        isGrounded = false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }
}
