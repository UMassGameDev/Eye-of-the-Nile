using UnityEngine;

/** \brief
This script is used by a trigger zone that encapsulates some entities' sides (including the player) that detects when they are touching a wall.
When the entity is touching a wall, onWall will be true.

This detects the layer(s) selected on this script in the inspector, which should usually be set to "Ground" and "Collision"
This script can only detect objects that have colliders.

Documentation updated 11/30/2024
\author Alexander Art
*/
public class WallDetector : MonoBehaviour
{
    /// The layers which objects are considered part of the wall are on.
    public LayerMask wallLayer;
    /// True if the entity is touching the wall.
    public bool onWall {get; private set;} = false;

    /// <summary>
    /// Runs when an object enters the wall detector zone. If that object is on the wallLayer, set onWall to true.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallLayer.value) != 0)
            onWall = true;
    }

    /// <summary>
    /// Runs when an object is inside the wall detector zone. If that object is on the wallLayer, set onWall to true.
    /// This part is not necessary for the wall detector to work, but it is kept for redundancy.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallLayer.value) != 0)
            onWall = true;
    }

    /// <summary>
    /// Runs when an object exits the wall detector zone. If that object is on the wallLayer, set onWall to false.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallLayer.value) != 0)
            onWall = false;
    }
}
