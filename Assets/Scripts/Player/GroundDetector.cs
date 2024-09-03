using UnityEngine;

/** \brief
This script is used by a small trigger zone beneath the player's feet that detects if the player is on the ground.
When the player is on the ground, isGrounded will be true.

Documentation updated 8/30/2024
\author Stephen Nuttall
*/
public class GroundDetector : MonoBehaviour
{
    /// The layers which objects are considered part of the ground are on.
    public LayerMask groundLayer;
    /// True if the player touching the ground.
    public bool isGrounded {get; private set;} = false;

    /// \brief Set isGrounded to false every frame.
    /// This is called before OnTriggerStay2D() is, allowing it to set it back to true if the player is on the ground.
    void FixedUpdate()
    {
        isGrounded = false;
    }

    /// <summary>
    /// Runs when an object is inside the ground detector zone. If that object is on the groundLayer, set isGrounded to true.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }
}
