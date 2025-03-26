using UnityEngine;

/** \brief
This script is scene-specific to MemphisIndoors_2. It is used for making the sun shrine fall.

Documentation updated 3/25/2025
\author Alexander Art
*/
public class MemphisIndoors_2 : MonoBehaviour
{
    /// Reference to the sun shrine's Rigidbody2D component.
    [SerializeField] protected new Rigidbody2D rigidbody;

    /// The sun shrine's collision is an illusion. The statue only stops falling once this distance has been reached.
    [SerializeField] protected float maxFallDistance = 23f;

    /// Initial y-position of the sun shrine.
    protected float initialHeight;
    /// Initial gravity scale of the sun shrine.
    protected float initialGravityScale;

    /// Initialize the state of the sun shrine.
    void Start()
    {
        initialHeight = transform.position.y;
        initialGravityScale = rigidbody.gravityScale;
        rigidbody.gravityScale = 0f;
    }

    void Update()
    {
        /// Limit how far the sun shrine can fall.
        if (transform.position.y <= initialHeight - maxFallDistance)
        {
            rigidbody.gravityScale = 0f;
            rigidbody.velocity = new Vector2(0f, 0f);
            transform.position = new Vector2(transform.position.x, initialHeight - maxFallDistance);
        }
    }

    /// Used by InvokeOnInteract on the sun shrine to activate its gravity once the player interacts with it.
    public void StartFalling()
    {
        rigidbody.gravityScale = initialGravityScale;
    }
}
