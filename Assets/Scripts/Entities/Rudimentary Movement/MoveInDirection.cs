using UnityEngine;

/** \brief
This script moves the object its attached to in the given 2D direction continuously and consistently.
Great for simple projectile movement.

Documentation updated 11/13/2024
\author Stephen Nuttall
*/
public class MoveInDirection : RudimentaryMovement
{
    /// \breif Moves the object by (movementDirection.x * Time.deltaTime) every frame,
    /// ensuring the object moves at a consistent speed regardless of framerate.
    void Update()
    {
        transform.position = new Vector2(
            transform.position.x + (movementDirection.x * Time.deltaTime),
            transform.position.y + (movementDirection.y * Time.deltaTime)
        );
    }
}
