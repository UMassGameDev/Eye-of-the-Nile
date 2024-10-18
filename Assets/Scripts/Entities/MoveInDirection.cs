using UnityEngine;

/** \brief
This script moves the object its attached to in the given 2D direction.

Documentation updated 10/17/2024
\author Stephen Nuttall
*/
public class MoveInDirection : MonoBehaviour
{
    /// Moves the object by these vector values every second. Negative numbers change the direction.
    public Vector2 movementDirection;
    
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
