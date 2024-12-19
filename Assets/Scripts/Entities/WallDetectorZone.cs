using UnityEngine;

/** \brief
This script is used by trigger zone(s) around some entities' sides that detects when the entity is touching a wall.
This is also used by the player!
Options:
1. Attach this script to a single wall detector object that encapsulates both of the entity's sides.
-   Set "Side" to "Both" in the inspector.
-   A Collider2D must be attached to this object.
-   Also attach WallDetectorInfo.cs to this object.
2. Have a wall detector (attach WallDetectorInfo.cs to it) and give it two children:
-   A trigger zone that encapsulates the front side of the entity.
-   A trigger zone that encapsulates the back side of the entity.
-   Attach this script to both children and set "Side" to "Front" and "Back" on each, respectively.
-   Each trigger zone must have some kind of Collider2D attached.

This detects the layer(s) selected on this script in the inspector, which should usually be set to "Ground" and "Collision"
This script can only detect objects that have colliders.

Documentation updated 12/19/2024
\author Alexander Art
*/
public class WallDetectorZone : MonoBehaviour
{
    /// Tell this script in the inspector which side of the entity this trigger zone is attached to.
    [SerializeField] private WallDetectorSide side = WallDetectorSide.Both; // Creates dropdown menu.
    /// WallDetectorInfo is what keeps track of when the entity is touching a wall and which layers should count as wall.
    /// If the wall detector has one trigger zone, WallDetectorInfo will be on this script.
    /// If the wall detector has two trigger zones, WallDetectorInfo will be on the parent object of this script.
    private WallDetectorInfo wallDetectorInfo;

    void Awake()
    {
        if (side == WallDetectorSide.Both)
        {
            wallDetectorInfo = GetComponent<WallDetectorInfo>();
        }
        else if (side == WallDetectorSide.Front || side == WallDetectorSide.Back)
        {
            wallDetectorInfo = transform.parent.GetComponent<WallDetectorInfo>();
        }
    }

    /// <summary>
    /// Runs when an object enters the wall detector zone. Updates WallDetectorInfo.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallDetectorInfo.wallLayer.value) != 0)
            wallDetectorInfo.wallTouched(side);
    }

    /// <summary>
    /// Runs when an object is inside the wall detector zone. Updates WallDetectorInfo.
    /// This part is not necessary for the wall detector to work, but it is kept for redundancy.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallDetectorInfo.wallLayer.value) != 0)
            wallDetectorInfo.wallTouched(side);
    }

    /// <summary>
    /// Runs when an object exits the wall detector zone. Updates WallDetectorInfo.
    /// </summary>
    /// <param name="col">Represents the object inside the trigger zone.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & wallDetectorInfo.wallLayer.value) != 0)
            wallDetectorInfo.wallUntouched(side);
    }
}
