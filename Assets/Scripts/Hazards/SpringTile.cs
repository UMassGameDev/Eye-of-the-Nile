using UnityEngine;

/** \brief
Moves entities when they jump on a spring tile.
Change the spring's mass (and other properties) to change how bouncy the spring appears.
To adjust the height of the spring tile, place it 1 tile above the floor, then set the "Distance"
on the Spring Joint 2D to be 1 greater than where you would like the spring to end up.

Documentation updated 12/25/2024
\author Alexander Art
\todo Make the amount that the spring launches the entities based on the mass of the entity and how compressed the spring is.
\todo Give the spring itself some positive velocity when launching.
\todo Prevent the spring from getting stuck in the floor.
\todo Make the support beam for the spring tile extend and retract as the spring tile moves up and down, rather than having it always be as long as possibly necessary.
*/
public class SpringTile : MonoBehaviour
{
    /// Reference to the spring tile's rigidbody component.
    Rigidbody2D rb;
    /// Detects the entities that are standing on the platform (controlled by a different script).
    [SerializeField] protected MovableEntityDetector entityDetector;
    
    /// The speed at which the spring tile will launch entities at.
    public float launchSpeed = 50f;

    /// Used for making sure the spring only launches when it's compressed.
    private bool compressed;

    /// Set reference to the moving platform's rigidbody and initial target point.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// If the spring was not compressed, but an entity is on the spring and the spring is moving downwards, then it is compressed.
    /// If the spring is compressed, wait for it to stop compressing, then launch the spring.
    /// </summary>
    void Update()
    {
        if (compressed == false && entityDetector.entitiesInDetector.Count > 0 && rb.velocity.y < 0)
        {
            compressed = true;
        }
        else if (compressed == true && rb.velocity.y >= 0)
        {
            LaunchSpring();
            compressed = false;
        }        
    }

    /// <summary>
    /// Launch the entities on the spring upwards by adding launchSpeed to their y velocities.
    /// </summary>
    public void LaunchSpring()
    {
        foreach (GameObject entity in entityDetector.entitiesInDetector)
        {
            Rigidbody2D entityRigidbody = entity.transform.GetComponent<Rigidbody2D>();
            entityRigidbody.velocity += new Vector2(0f, launchSpeed);
        }
    }
}
