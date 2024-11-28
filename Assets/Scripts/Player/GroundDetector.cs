using UnityEngine;

/** \brief
This script is used by a small trigger zone beneath some entities' feet (including the player) that detects if they are on the ground.
When the entity is on the floor, isGrounded will be true.

NOTE: This detects the layer(s) selected on this script in the inspector, which is not necessarily the "Ground" layer!
The selected layers should usually be set to "Ground" and "Collision"
This script can only detect objects that have colliders.

Documentation updated 11/28/2024
\author Stephen Nuttall, Alexander Art
*/
public class GroundDetector : MonoBehaviour
{
    /// The layers which objects are considered part of the ground are on.
    public LayerMask groundLayer;
    /// True if the entity is touching the ground (after a small delay).
    public bool isGrounded {get; private set;} = false;
    /// Counts how long the entity has been on the ground.
    float groundTime = 0.0f;
    /// How long the entity needs to be on the ground before isGrounded is set to true.
    float isGroundedDelay = 0.025f;

    /// <summary>
    /// This part does nothing now, but it is kept for future reference if ever needed.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    // void OnTriggerEnter2D(Collider2D col)s
    // {
    //     if ((1 << col.gameObject.layer & groundLayer.value) != 0) {isGrounded = true;}
    // }

    /// <summary>
    /// Runs when an object is inside the ground detector zone (Warning: slightly innacurate). If that object is on the groundLayer, increment groundTime.
    /// If groundTime is greater than the delay required to set isGrounded to true, then set isGrounded to true.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if ((1 << col.gameObject.layer & groundLayer.value) != 0)
        {
            groundTime += Time.deltaTime;
        }
        if (groundTime > isGroundedDelay)
        {
            isGrounded = true;
        }
    }

    /// <summary>
    /// Runs when an object exits the ground detector zone. If that object is on the groundLayer, reset groundTime and set isGrounded to false.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if ((1 << col.gameObject.layer & groundLayer.value) != 0)
        {
            groundTime = 0.0f;
            isGrounded = false;
        }
    }
}
