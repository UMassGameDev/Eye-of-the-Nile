using System;
using UnityEngine;

/** \brief
Handles the rotation of the swinging pendulum axe traps.

Documentation updated 12/22/2024
\author Alexander Art
*/

public class SwingingPendulumAxe : MonoBehaviour
{
    /// The point that the trap will rotate about. This should be set to the "Pivot" which is part of the trap's prefab.
    public GameObject pivotPoint;

    /// How much time it takes the pendulum to swing one cycle back and forth.
    public float rotationSpeed = 0.5f;
    /// In degrees, the maximum angle the pendulum will reach above the vertical. Symmetrical either side.
    public float maxAngle = 90f;

    void Update()
    {
        // Calculate what the rotation of the axe should be based on game time the axe's properties.
        var targetRotation = maxAngle * Math.Cos(Time.time * 2 * Math.PI * rotationSpeed);
        // How far the axe needs to rotate this frame to match where it should be.
        var rotationDifference = targetRotation - transform.eulerAngles.z;
        // Rotate about the pivot point on the z-axis by the calculated amount.
        transform.RotateAround(pivotPoint.transform.position, Vector3.forward, (float)rotationDifference);
    }
}
