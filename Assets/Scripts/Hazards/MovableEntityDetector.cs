using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/** \brief
Used by a trigger zone on moving platforms and spring tiles to detect movable entities.
Objects that have colliders but should not be pushed around, such as tilemaps or GroundDetectors, should not get detected.

Documentation updated 12/25/2024
\author Alexander Art
\todo Make sure that nothing that shouldn't be moved can be detected by this script. Currently, this script can detect other moving platforms, which is a problem.
*/

public class MovableEntityDetector : MonoBehaviour
{
    /// List of all objects standing in the trigger zone. Must have Rigidbody2D's attached and not be a tilemap.
    public List<GameObject> entitiesInDetector;

    /// \brief Runs when a collider stands on the object.
    /// Adds any valid entities that are touching the trigger zone to the list.
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collided object has a Rigidbody2D and is not a tilemap...
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null && collision.transform.gameObject.GetComponent<Tilemap>() == null)
        {
            // And the entity is not already in the detected entity list, then add it.
            if (entitiesInDetector.Contains(collision.transform.gameObject) == false)
            {
                entitiesInDetector.Add(collision.transform.gameObject);
            }
        }
    }

    /// \brief Runs when a collider gets off of the object.
    /// Removes any objects that are no longer touching the trigger zone from the list.
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the collided object is in the list, then remove it.
        if (entitiesInDetector.Contains(collision.transform.gameObject) == true)
            entitiesInDetector.Remove(collision.transform.gameObject);
    }
}