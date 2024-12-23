using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/** \brief
Used by pressure plates to detect entities standing on it and to activate/deactivate a connected object.

Documentation updated 12/22/2024
\author Alexander Art
\todo Make this script part of a pressure plate prefab that is more functional and versatile.
*/

public class PressurePlate : MonoBehaviour
{
    /// The object that deactivates when touched by the pressure plate.
    public GameObject affectedObject;

    /// List of entities standing on the pressure plate.
    public List<GameObject> entitiesOnPlate;

    void Update()
    {
        // Deactivate the pressure plate's object when something stands on it.
        if (entitiesOnPlate.Count >= 1)
        {
            affectedObject.SetActive(false);
        }
        else
        {
            affectedObject.SetActive(true);
        }
    }

    /// \brief Runs when a collider touches the pressure plate.
    /// If it is a valid entity, add it to the list.
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collided object has a Rigidbody2D and is not a tilemap...
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null && collision.transform.gameObject.GetComponent<Tilemap>() == null)
        {
            // And the entity is not already in the detected entity list, then add it.
            if (entitiesOnPlate.Contains(collision.transform.gameObject) == false)
            {
                entitiesOnPlate.Add(collision.transform.gameObject);
            }
        }
    }

    /// \brief Runs when a collider steps off of the pressure plate.
    /// Removes objects that are no longer on the pressure plate from the list.
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the collided object is in the list, then remove it.
        if (entitiesOnPlate.Contains(collision.transform.gameObject) == true)
            entitiesOnPlate.Remove(collision.transform.gameObject);
    }
}
