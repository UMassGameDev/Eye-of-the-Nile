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

    /// The minimum x position that the rock golems are able to be. This should be after the left wall of Geb's bossroom.
    public float minPosLeft;
    /// The maximum x position that the rock golems are able to be. This should be before the right wall of Geb's bossroom.
    public float maxPosRight;

    void Awake()
    {
        gebRoomController = GameObject.Find("Geb").GetComponent<GebRoomController>();
    }

    /// Prevent the rock golems from getting stuck in the wall.
    void Update()
    {
        // Calculate the width of the golem.
        float golemWidth = Math.Abs(transform.localScale.x * transform.parent.localScale.x * GetComponent<BoxCollider2D>().size.x);

        // If the golem is past the left boundary, move it right.
        // If the golem is past the right boundary, move it left.
        if (minPosLeft > transform.position.x - golemWidth / 2)
        {
            transform.position = new Vector2(minPosLeft + golemWidth / 2, transform.position.y);
        }
        else if (maxPosRight < transform.position.x + golemWidth / 2)
        {
            transform.position = new Vector2(maxPosRight - golemWidth / 2, transform.position.y);
        }
    }

    /// Subtracts 1 from Geb's bossroom's rock golem count. Called by the rock golem's ObjectHealth when it dies.
    public void DecrementRockGolemCount()
    {
        // Update rockGolemCount.
        gebRoomController.rockGolemCount--;
    }
}
