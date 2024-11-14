using UnityEngine;

/** \brief
This script thrusts the object it's attached to in the given direction as soon as the object is created.
Used for some projectile movement. Eventually the boulder hazards will also use this script.

Documentation updated 11/13/2024
\author Stephen Nuttall
*/
public class InitialThrust : RudimentaryMovement
{
    /// Reference to the object's rigidbody.
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.AddForce(movementDirection, ForceMode2D.Impulse);
    }
}
