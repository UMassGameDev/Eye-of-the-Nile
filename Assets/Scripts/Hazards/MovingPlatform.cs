using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** \brief
Moves the moving platforms.
Each moving platform has its own MovingPlatform script attached.

Documentation updated 12/17/2024
\author Alexander Art
\todo Prevent entities from clipping into walls when being pushed into them.
*/

public class MovingPlatform : MonoBehaviour
{
    /// Reference to the platform's rigidbody component.
    Rigidbody2D rb;
    /// A patrol zone is an object that has two points the platform will move between.
    [SerializeField] protected PatrolZone patrolZone;
    /// In a different script, this detects the entities that are standing on the platform.
    [SerializeField] protected PlatformEntityDetector platformEntityDetector;
    /// The speed at which the platform will move at.
    public float speed = 2.5f;
    /// The direction that the platform is moving towards. -1 for left, 1 for right.
    /// Setting this to anything else will make the platform forget where it moved last.
    private float direction = -1f;
    /// The amount of time that the platform will stop before changing directions.
    public float waitDelay = 1f;
    /// Keeps track of how long the platform has been stopped before changing directions.
    /// This is set to 0 when inactive, and will be equal to waitDelay once completed.
    private float delayCounter = 0f;
    /// Needed for calculating how far the platform has moved each frame when updating the position of entities standing atop.
    private Vector2 previousPosition;

    /// Set reference to the moving platform's rigidbody.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // If the stop timer has been started, then increment the timer.
        if (delayCounter != 0f)
        {
            delayCounter += Time.deltaTime;
        }

        // If the platform is done moving, change the direction and start the timer.
        if (direction == -1f && (Vector2)transform.position == patrolZone.LeftPoint()) 
        {
            direction = 1f;
            delayCounter += Time.deltaTime;
        }
        else if (direction == 1f && (Vector2)transform.position == patrolZone.RightPoint())
        {
            direction = -1f;
            delayCounter += Time.deltaTime;
        }

        // End the timer once enough time has passed.
        if (delayCounter >= waitDelay)
        {
            delayCounter = 0f;
        }

        // Keep track of where the platform was before it gets moved.
        previousPosition = transform.position;

        // If the timer is over, move the platform.
        if (direction == 1f && delayCounter == 0f)
        {
            delayCounter = 0f;
            transform.position = Vector2.MoveTowards(transform.position, patrolZone.RightPoint(), speed * Time.deltaTime);
        }
        else if (direction == -1f && delayCounter == 0f)
        {
            delayCounter = 0f;
            transform.position = Vector2.MoveTowards(transform.position, patrolZone.LeftPoint(), speed * Time.deltaTime);
        }

        // Move the entities that stand on the platform by the amount that the platform has moved.
        foreach (GameObject entity in platformEntityDetector.entitiesInDetector)
        {
            // Position keeps wanting to be a Vector3 so that's what's with all the typecasts.
            entity.transform.position += (Vector3) ((Vector2)transform.position - previousPosition);
        }
    }
}
