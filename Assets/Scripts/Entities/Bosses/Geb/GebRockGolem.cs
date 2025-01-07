using UnityEngine;
using System;

/** \brief
Script for the rock golems in Geb's bossroom.
What this script does:
- Decrements the rock golem count in Geb's bossroom when the rock golems die (attached to OnDeath() in ObjectHealth).
- Prevents the rock golems from getting stuck in the outer walls of Geb's bossroom.

Documentation updated 1/5/2025
\author Alexander Art
*/
public class GebRockGolem : MonoBehaviour
{
    /// Reference to Geb's room controller.
    protected GebRoomController gebRoomController;

    /// The minimum x position that the rock golems can have. Calculated using the golem's width and the bounds of the room.
    protected float minPosX;
    /// The maximum x position that the rock golems can have. Calculated using the golem's width and the bounds of the room.
    protected float maxPosX;

    void Awake()
    {
        gebRoomController = GameObject.Find("Geb").GetComponent<GebRoomController>();
    }

    void Start()
    {
        // Get the width of the golem.
        float golemWidth = GetComponent<BoxCollider2D>().bounds.size.x;
        // Calculate the minimum x position for the golems, factoring in the width of the golem.
        minPosX = gebRoomController.bounds.LeftPoint().x + golemWidth / 2;
        // Calculate the maximum x position for the golems, factoring in the width of the golem.
        maxPosX = gebRoomController.bounds.RightPoint().x - golemWidth / 2;
    }

    /// Prevent the rock golems from getting stuck in the wall.
    void Update()
    {
        // If the golem is past the left boundary, move it right.
        // If the golem is past the right boundary, move it left.
        if (minPosX > transform.position.x )
        {
            transform.position = new Vector2(minPosX, transform.position.y);
        }
        else if (maxPosX < transform.position.x)
        {
            transform.position = new Vector2(maxPosX, transform.position.y);
        }
    }

    /// Subtracts 1 from Geb's bossroom's rock golem count. Called by the rock golem's ObjectHealth when it dies.
    public void DecrementRockGolemCount()
    {
        // Update rockGolemCount.
        gebRoomController.rockGolemCount--;
    }
}
