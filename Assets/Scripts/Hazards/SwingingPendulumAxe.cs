using System;
using UnityEngine;

/** \brief
Handles the swinging pendulum axe traps.

Documentation updated 12/21/2024
\author Alexander Art
*/

public class SwingingPendulumAxe : MonoBehaviour
{
    /// The point that the trap will rotate about. This should be set to the "Pivot" which is part of the trap's prefab.
    public GameObject pivotPoint;

    /// How much time it takes the pendulum to swing one cycle back and forth.
    public float rotationSpeed = 0.4f;
    /// In degrees, the maximum angle the pendulum will reach above the vertical, either side.
    public float maxAngle = 90f;

    void Update()
    {
        var targetRotation = maxAngle * Math.Cos(Time.time * 2 * Math.PI * rotationSpeed);
        var rotationDifference = targetRotation - transform.eulerAngles.z;
        transform.RotateAround(pivotPoint.transform.position, Vector3.forward, (float)rotationDifference);
    }
}
