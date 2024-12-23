using UnityEngine;

/** \brief
Moves the moving platforms.
Each moving platform has its own MovingPlatform script attached.

Documentation updated 12/23/2024
\author Alexander Art
\todo Prevent entities from clipping into walls when being pushed into them.
\todo Prevent colliding moving platforms from sticking together (or just never place them in each other's path).
*/

public class MovingPlatform : MonoBehaviour
{
    /// Reference to the platform's rigidbody component.
    Rigidbody2D rb;
    /// Detects the entities that are standing on the platform (controlled by a different script).
    [SerializeField] protected PlatformEntityDetector platformEntityDetector;
    /// The object with children as the points that the platform will move between.
    [SerializeField] protected Transform path;
    
    /// The speed at which the platform will move at.
    public float moveSpeed = 2.5f;
    /// The amount of time the platform will stop between each point (in seconds).
    public float pauseDuration = 1f;

    /// Keeps track of how long the platform has been stopped before going to a new point (in seconds).
    private float pauseCounter = 0f;
    /// The point that the platform should be moving towards.
    private Transform targetPoint;
    /// Keeps track of which point the platform should be moving towards.
    private int targetPointIndex = 0;
    /// Needed for calculating how far the platform has moved each frame when updating the position of entities standing atop.
    private Vector2 previousPosition;

    /// Set reference to the moving platform's rigidbody and initial target point.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPoint = path.GetChild(targetPointIndex);
    }

    void Update()
    {
        // Keep track of where the platform was before it gets moved.
        previousPosition = transform.position;

        // If the platform has reached its target destination, wait for pauseDuration, then start moving to the next point.
        if (transform.position == targetPoint.position)
        {
            pauseCounter = pauseCounter + Time.deltaTime;
            if (pauseCounter >= pauseDuration)
            {
                pauseCounter = 0f;
                targetPointIndex++;
                targetPointIndex = targetPointIndex % path.childCount; // Loops between points.
                targetPoint = path.GetChild(targetPointIndex);
            }
        }

        // Move the platform.
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Move all entities that stand on the platform by the same amount that the platform moved.
        foreach (GameObject entity in platformEntityDetector.entitiesInDetector)
        {
            // Typecasts are used because position keeps being Vector2 or Vector3.
            entity.transform.position += (Vector3) ((Vector2)transform.position - previousPosition);
        }
    }
}
