using UnityEngine;

/** \brief 
Script for a boulder that the player can throw as a projectile.
Adapted from BoulderHazard. Inherits from BasicProjectile.

Documentation updated 9/7/2024
\author Stephen Nuttall
*/
public class BoulderProjectile : BaseProjectile
{
    /// Reference to the boulder's rigidbody.
    Rigidbody2D boulderBody;
    /// Reference to the particles to spawn when the boulder breaks.
    [SerializeField] Transform boulderParticles;
    /// Initial force applied to the boulder when spawned in.
    [SerializeField] float initialForce = 100f;
    /// Objects on these layers will break the boulder and possibly get damaged if the boulder collides with them.
    public LayerMask collisionLayers {get; private set;}

    /// Breaks the boulder by spawning particles and destroying this object.
    public void BreakBoulder()
    {
        Collider2D thisCollider = GetComponent<Collider2D>();
        Instantiate(boulderParticles,
            thisCollider.bounds.center,
            Quaternion.identity);
        Destroy(gameObject);
    }

    /// Get reference to the boulder's rigidbody.
    protected override void AwakeMethods()
    {
        boulderBody = GetComponent<Rigidbody2D>();
    }

    /// Apply initial force based on the direction the boulder is facing.
    protected override void StartMethods()
    {
        base.StartMethods();

        if (facingLeft) {
            boulderBody.AddForce(new Vector2(-initialForce, initialForce), ForceMode2D.Impulse);
        } else {
            boulderBody.AddForce(new Vector2(initialForce, initialForce), ForceMode2D.Impulse);
        }
    }

    /// Empty. The collision is handled by the boulder's hithox, so we want the functionality of the base OnCollisionEnterMethods() to be ignored.
    protected override void OnCollisionEnterMethods(Collision2D collisionInfo)
    {
        // Collision is handled by the hitbox!
    }
}
