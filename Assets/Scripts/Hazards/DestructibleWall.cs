using UnityEngine;

/** \brief
Script for destructible walls that get broken when attacked.

Documentation updated 2/11/2025
\author Alexander Art
*/
public class DestructibleWall : MonoBehaviour
{
    /// Reference to the particle effect that is created when the wall is destroyed.
    [SerializeField] protected GameObject breakParticles;

    /// Whether or not this wall should destroy all other walls part of the same group (siblings) when destroyed.
    [SerializeField] protected bool destroyGroup = true;

    /// Called whenever a wall is broken. Breaks all sibling walls.
    public void WallDestroyed()
    {
        // Does not destroy siblings if 
        if (transform.parent != null && destroyGroup == true)
        {
            // Get all siblings (all GameObjects under the same parent) of the wall.
            foreach (Transform sibling in transform.parent)
            {
                // Get the wall component of the sibling (if it exists).
                DestructibleWall wall = sibling.GetComponent<DestructibleWall>();

                // Check if the sibling is not the self and if it is indeed a wall.
                if (sibling != transform && wall != null)
                {
                    // Destroy all sibling walls.
                    wall.SelfDestruct();
                }
            }
        }
    }

    /// <summary>
    /// An alternative method to destroy this wall (ObjectHealth.TakeDamage calls the OnDeath(), which was creating infinite loops).
    /// Called whenever a wall in the group gets destroyed.
    /// </summary>
    protected void SelfDestruct()
    {
        Instantiate(breakParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
