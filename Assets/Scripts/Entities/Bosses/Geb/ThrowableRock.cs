using UnityEngine;
using System;

/** \brief
Script for the rocks that Geb throws.
Has a chance to summon rock golems when colliding any object on collisionLayers.
collisionLayers is on this script and is set in the Inspector.
If a rock golem is not summoned upon impact, then the rock breaks instead.

Documentation updated 1/1/2025
\author Alexander Art
*/
public class ThrowableRock : MonoBehaviour
{
    /// The layers that the rock will detect when collided with.
    public LayerMask collisionLayers;
    /// Reference to Geb's room controller.
    protected GebRoomController gebRoomController;
    /// Reference to the rock golem prefab that the rocks spawn.
    [SerializeField] protected GameObject rockGolem; 

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
            // 50% chance of spawning a rock golem.
            if (rng.NextDouble() < 0.5)
            {
                // Spawn rock golem.
                Instantiate(rockGolem, transform.position, Quaternion.identity);

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
