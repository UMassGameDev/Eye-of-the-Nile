using UnityEngine;

/** \brief
Script for the falling spike hazard.
When a player enters the trigger zone beneath the falling spike, it will unfreeze the spike's gravity.

Documentation updated 8/26/2024
\author Stephen Nuttall
*/
public class FallingSpike : MonoBehaviour
{
    /// <summary>
    /// When a player enters the trigger zone beneath the falling spike, unfreeze the spike's gravity.
    /// </summary>
    /// <param name="col">Represents the object that entered the trigger zone.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
