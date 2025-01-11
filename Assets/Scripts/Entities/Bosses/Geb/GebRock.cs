using UnityEngine;
using System;

/** \brief
Script for both the rocks that Geb throws and the debris that fall when Geb's walls break.
This script detects when the rock collides with any object on collisionLayers (defined on this script in the Inspector).
Optionally, this script can have a chance to spawn a rock golem upon impact.
If a rock golem is not spawned, the rock will break instead.

Documentation updated 1/11/2025
\author Alexander Art
*/
public class GebRock : MonoBehaviour
{
    /// The layers that the rock will detect when collided with.
    public LayerMask collisionLayers;
    /// Reference to Geb's room controller.
    protected GebRoomController gebRoomController;
    /// Reference to the rock golem prefab that the rocks spawn.
    [SerializeField] protected GameObject rockGolem;

    /// The probability for a rock golem to spawn on collision.
    public float spawnProbability = 0.5f;

    /// Create random number generator.
    private System.Random rng = new System.Random();

    void Awake()
    {
        gebRoomController = GameObject.Find("Geb").GetComponent<GebRoomController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // If the collided object was on any of the collisionLayers.
        if (((1 << col.gameObject.layer) & collisionLayers.value) != 0)
        {
            // If there is room for another golem to be spawned, have a spawnProbability chance of spawning a rock golem.
            // Otherwise, break the rock.
            if (gebRoomController.rockGolemCount < gebRoomController.maxRockGolems && rng.NextDouble() < spawnProbability)
            {
                // Spawn rock golem.
                Instantiate(rockGolem, transform.position, Quaternion.identity);

                // Increment rock golem count.
                gebRoomController.rockGolemCount++;

                // Get rid of rock object.
                Destroy(gameObject);
            }
            else
            {
                // Destroy rock on impact.
                Destroy(gameObject);
            }
        }
    }
}
