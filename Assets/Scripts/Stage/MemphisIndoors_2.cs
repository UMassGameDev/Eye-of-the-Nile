using UnityEngine;

/** \brief
This script is scene-specific to MemphisIndoors_2. It is used for making the sun shrine fall.

Documentation updated 3/25/2025
\author Alexander Art
*/
public class MemphisIndoors_2 : MonoBehaviour
{
    /// Reference to the sun shrine's Rigidbody2D component.
    [SerializeField] protected Rigidbody2D rb;
    /// Reference to the player's transform. Used for checking where the player is.
    [SerializeField] protected Transform player;
    /// Reference to the destructible wall. Gets removed immediately if the player enters from the StageWarp on the right (the exit).
    [SerializeField] protected GameObject destructibleWall;

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
        initialGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        /// Limit how far the sun shrine can fall.
        if (transform.position.y <= initialHeight - maxFallDistance)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(0f, 0f);
            transform.position = new Vector2(transform.position.x, initialHeight - maxFallDistance);
        }

        // If the player enters the scene from the exit door (the rightmost StageWarp), then the statue is already fallen.
        // This is in Update() because the player is not at the EntryPoint on Start().
        if (1.5f < player.position.x && player.position.x < 13f && 14.5f < player.position.y && player.position.y < 20.5)
        {
            GameObject.Destroy(destructibleWall);
            transform.position = new Vector2(transform.position.x, initialHeight - maxFallDistance);
        }
    }

    /// Used by InvokeOnInteract on the sun shrine to activate its gravity once the player interacts with it.
    public void StartFalling()
    {
        rb.gravityScale = initialGravityScale;
    }
}
