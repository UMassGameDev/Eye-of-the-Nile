using UnityEngine;

/** \brief
This script is used by a small trigger zone beneath some entities' feet (including the player) that detects if they are on the ground.
When the entity is on the floor, isGrounded will be true.

NOTE: This detects the layer that is selected on this script in the inspector, which is not necessarily the "Ground" layer!
The selected layer should usually be set to "Collision"
This script can only detect objects that have colliders.

Documentation updated 11/23/2024
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
        if ((1 << col.gameObject.layer & groundLayer.value) != 0)
            isGrounded = true;
    }
    /// <summary>
    /// /// This part is not necessary for the groundDetector to work, but is kept for redundancy. Runs when an object is inside the ground detector zone. If that object is on the groundLayer, set isGrounded to true.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if ((1 << col.gameObject.layer & groundLayer.value) != 0)
            isGrounded = true;
    }
    /// <summary>
    /// Runs when an object exits the ground detector zone. If that object is on the groundLayer, set isGrounded to false.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if ((1 << col.gameObject.layer & groundLayer.value) != 0)
            isGrounded = false;
    }
}
