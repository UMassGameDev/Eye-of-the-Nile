using System;
using UnityEngine;

/** \brief
This script controls the transparency of objects that are distant to the player.
This script affects the sprite renderer of the object that it is attached to.

Documentation updated 4/13/2025
\author Alexander Art
*/
public class FadeDistantObject : MonoBehaviour
{
    /// Reference to the SpriteRenderer to make transparent.
    protected SpriteRenderer spriteRenderer;
    /// Objects on this layer are detected as the player.
    [SerializeField] protected LayerMask playerLayer;
    
    /// The distance at which you want the object to be fully opaque.
    [SerializeField] protected float opaqueDistance = 8f;
    /// The distance at which you want the object to be fully transparent.
    [SerializeField] protected float transparentDistance = 10f;
    // The area between transparentDistance and opaqueDistance is where the object will be partially transparent (translucent).

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Detect the player if they are nearby.
        Collider2D player = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 0.5f, 0f),
            Math.Max(opaqueDistance, transparentDistance),
            playerLayer);

        // If the player is detected...
        if (player != null)
        {
            // Calculate the distance between the object this script is attached to and the player.
            float playerDistance = Vector2.Distance(transform.position, player.transform.position);

            if (opaqueDistance <= transparentDistance)
            {
                // If the player is inside the opaque range, make the object opaque.
                // If the player is between the opaque range and the transparent range, make the object translucent.
                // If the player is outside the transparent range, make the object transparent.
                if (playerDistance <= opaqueDistance)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                }
                else if (opaqueDistance < playerDistance && playerDistance < transparentDistance)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f - (playerDistance - opaqueDistance) / (transparentDistance - opaqueDistance));
                }
                else
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
                }
            }
            else if (opaqueDistance > transparentDistance)
            {
                // If the player is inside the transparent range, make the object transparent.
                // If the player is between the transparent range and the opaque range, make the object translucent.
                // If the player is outside the opaque range, make the object opaque.
                if (playerDistance <= transparentDistance)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
                }
                else if (transparentDistance < playerDistance && playerDistance < opaqueDistance)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, (playerDistance - transparentDistance) / (opaqueDistance - transparentDistance));
                }
                else
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                }
            }
        }
        else // If the player is not detected, then the player is most likely out of range.
        {
            // Set the object's transparency to what it should be when the player is far away.
            if (opaqueDistance <= transparentDistance)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
            }
            else if (opaqueDistance > transparentDistance)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
        }
    }
}
