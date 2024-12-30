using UnityEngine;

/** \brief
Moves entities when they jump on a spring tile.
Change the spring's mass (and other properties) to change how bouncy the spring appears.
To adjust the height of the spring tile, place it 1 tile above the floor, then set the "Distance"
on the Spring Joint 2D to be where you would like the spring to end up.

Documentation updated 12/29/2024
\author Alexander Art
\todo Make the amount that the spring launches the entities based on the mass of the entity and how compressed the spring is.
\todo Give the spring itself some positive velocity when launching.
\todo Prevent the spring from getting stuck in the floor.
*/
public class SpringTile : MonoBehaviour
{
    /// Reference to the spring tile's rigidbody component.
    Rigidbody2D rb;
    /// Detects the entities that are standing on the platform (controlled by a different script).
    [SerializeField] protected MovableEntityDetector entityDetector;
    /// Object of the texture under the spring tile that makes it visually connect to the ground.
    [SerializeField] protected GameObject supportTexture;
    /// The point under the spring tile that the Spring Joint 2D connects to.
    [SerializeField] protected GameObject anchor;
    
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
    /// 
    /// Additionally, scale the height of the support texture to match the height of the spring.
    /// The height is precisely the difference between the anchor point and the bottom of the platform.
    /// </summary>
    void Update()
    {
        // Handle compression.
        if (compressed == false && entityDetector.entitiesInDetector.Count > 0 && rb.velocity.y < 0)
        {
            compressed = true;
        }
        else if (compressed == true && rb.velocity.y >= 0)
        {
            LaunchSpring();
            compressed = false;
        }

        // Change the height (y scale) of the support texture.
        float targetHeight = (transform.position.y - anchor.transform.position.y - transform.parent.localScale.y * transform.localScale.y / 2) / transform.localScale.y / transform.parent.localScale.y;
        supportTexture.transform.localScale = new Vector2(supportTexture.transform.localScale.x, targetHeight);
        // Adjust the position of the support texture to match the new height.
        float targetPosition = -0.5f - targetHeight / 2;
        supportTexture.transform.localPosition = new Vector2(supportTexture.transform.localPosition.x, targetPosition);
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
