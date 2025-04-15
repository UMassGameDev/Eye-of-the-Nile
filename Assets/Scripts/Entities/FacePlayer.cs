using UnityEngine;

/** \brief
This script constantly updates the X Flip of the SpriteRenderer of the object that this script is attached to,
so the object constantly faces the player.
If this script makes the object face the opposite direction that you want, change the object's SpriteRenderer's default X Flip value.

Documentation updated 4/14/2025
\author Alexander Art
*/
public class FacePlayer : MonoBehaviour
{
    // Refernece to the object's sprite renderer
    private SpriteRenderer spriteRenderer;
    // Reference to the player
    private GameObject player;

    // Horizontal radius in which the object will not flip to face the player.
    private float deadZone = 1f;

    // Default X Flip value of the spriteRenderer
    protected bool objectDefaulFlipX;

    // Set references and default values
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        objectDefaulFlipX = spriteRenderer.flipX;
    }

    // Update the facing of the object every frame based on the player's position.
    void Update()
    {
        if (player.transform.position.x + deadZone < transform.position.x) // When the player is to the left
        {
            spriteRenderer.flipX = objectDefaulFlipX;
        }
        else if (transform.position.x < player.transform.position.x - deadZone) // When the player is to the right
        {
            spriteRenderer.flipX = !objectDefaulFlipX;
        }
    }
}
