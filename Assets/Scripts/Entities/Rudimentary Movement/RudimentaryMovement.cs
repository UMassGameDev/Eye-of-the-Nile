using UnityEngine;

/** \brief
An abstract class for simple movement scripts that generally follow the same format.
Scripts that inherit from this are typically used for objects with simple movement, typically projectiles.

Documentation updated 11/13/2024
\author Stephen Nuttall
*/
public class RudimentaryMovement : MonoBehaviour
{
    /// Direction the object should move in.
    public Vector2 movementDirection;

    /// If we want the object to move to left, ensure movementDirection.x is negative. Otherwise, ensure it's positive.
    public void HorizontalDirectionChange(bool movingLeft)
    {
        if (movingLeft)
        {
            movementDirection = new Vector2(-Mathf.Abs(movementDirection.x), movementDirection.y);
        }
        else
        {
            movementDirection = new Vector2(Mathf.Abs(movementDirection.x), movementDirection.y);
        }
    }

    /// If we want the object to move to down, ensure movementDirection.y is negative. Otherwise, ensure it's positive.
    public void VerticalDirectionChange(bool movingDown)
    {
        if (movingDown)
        {
            movementDirection = new Vector2(movementDirection.x, -Mathf.Abs(movementDirection.y));
        }
        else
        {
            movementDirection = new Vector2(movementDirection.x, Mathf.Abs(movementDirection.y));
        }
    }
}
