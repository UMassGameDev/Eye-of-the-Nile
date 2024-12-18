using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/** \brief
Used by a trigger zone on every moving platform.
Detect entities to them that stand on the moving platform.
Objects that are tilemaps or do not have Rigidbody2D's attached (such as GroundDetectors) do not get detected.
Technically, this script could work for things other than moving platforms, so maybe this script should be more generalized.

Documentation updated 12/17/2024
\author Alexander Art
*/

public class PlatformEntityDetector : MonoBehaviour
{
    /// List of all object standing on the moving platform that have Rigidbody2D's attached.
    public List<GameObject> entitiesInDetector;

    /// \brief Runs when a collider stands on the platform.
    /// Adds any valid entities that are touching the platform to the list.
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

    /// \brief Runs when a collider gets off of the platform.
    /// Removes any objects that are no longer touching the platform from the list.
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the collided object is in the list, then remove it.
        if (entitiesInDetector.Contains(collision.transform.gameObject) == true)
            entitiesInDetector.Remove(collision.transform.gameObject);
    }
}
