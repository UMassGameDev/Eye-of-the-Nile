using UnityEngine;

/** \brief
This script is used by a small trigger zone beneath the player's feet that detects if the player is on the ground.
When the player is on the ground, isGrounded will be true.
NOTE: This detects the ground layer (Tilemap Base), not the collision layer!

Documentation updated 8/30/2024
\author Stephen Nuttall, Alexander Art
*/
public class GroundDetector : MonoBehaviour
{
    /// The layers which objects are considered part of the ground are on.
    public LayerMask groundLayer;
    /// True if the player touching the ground.
    public bool isGrounded {get; private set;} = false;

    /// <summary>
    /// Runs when an object enters the ground detector zone. If that object is on the groundLayer, set isGrounded to true.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }
    /// <summary>
    /// /// This part is not necessary for the groundDetector to work, but is kept for redundancy. Runs when an object is inside the ground detector zone. If that object is on the groundLayer, set isGrounded to true.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }
    /// <summary>
    /// Runs when an object exits the ground detector zone. If that object is on the groundLayer, set isGrounded to false.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = false;
    }
}
