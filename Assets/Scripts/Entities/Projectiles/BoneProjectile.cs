using UnityEngine;

/** \brief
Script that powers \ref Prefabs_BoneProjectile that \ref Prefabs_SkeletonGuy uses.

Documentation updated 9/4/2024
\author Roy Pascual
\note Unlike most projectiles, this projectile's Collider2D component is set to be a trigger, so it uses OnTriggerEnterMethods().
However, OnCollisionEnterMethods() is also filled out with the same code. I'm not sure why that is but it doesn't seem necessary.
*/
public class BoneProjectile : BaseProjectile
{
    /// Reference to the rigidbody of the projectile.
    Rigidbody2D boneRb;
    /// The horizontal force applied to the projectile upon spawning in.
    public float initialXForce = 25f; // we're not getting out of Marvel with this one
    /// The vertical force applied to the projectile upon spawning in.
    public float initialYForce = 50f;

    /// \brief Sets reference to rigidbody, and disables damage to non-players
    /// \remarks Why is damageNonPlayers forced off here, instead of just disabling it in the Unity Editor? This should be changed.
    protected override void AwakeMethods()
    {
        boneRb = GetComponent<Rigidbody2D>();
        damageNonPlayers = false;
    }

    /// Apply initial force based on the direction the projectile is facing.
    protected override void StartMethods()
    {
        base.StartMethods();

        if (facingLeft)
        {
            boneRb.AddForce(new Vector2(-initialXForce, initialYForce), ForceMode2D.Impulse);
        }
        else
        {
            boneRb.AddForce(new Vector2(initialXForce, initialYForce), ForceMode2D.Impulse);
        }
    }

    /// Rotates the projectile.
    void Update()
    {
        if (facingLeft)
            transform.Rotate(0f, 0f, 500f * Time.deltaTime);
        else
            transform.Rotate(0f, 0f, -500f * Time.deltaTime);
    }

    /// Runs base.OnCollisionEnterMethods() but adapted to be compatable with OnTrigger (the parameter is slightly different).
    protected override void OnTriggerEnterMethods(Collider2D collisionInfo)
    {
        // if we collided with something we can damage, damage it
        if (collisionInfo.GetComponent<Collider2D>().CompareTag("DamagableByProjectile") && damageNonPlayers)
        {
            collisionInfo.GetComponent<Collider2D>().GetComponent<ObjectHealth>().TakeDamage(transform, damage);
        }
        else if (collisionInfo.GetComponent<Collider2D>().CompareTag("Player") && damagePlayers)
        {
            collisionInfo.GetComponent<Collider2D>().GetComponent<PlayerHealth>().TakeDamage(transform, damage);
        }

        // Destory the projectile
        if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Default") ||
            collisionInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(sprite);
            Destroy(gameObject);
        }
    }

    /// \brief Same as base.OnCollisionEnterMethods(). Not necessary because the projectile's Collider2D is a trigger, so this function will never run.
    /// Additionally, since this function doesn't override any of the behavior of the base version, it wouldn't need to be listed anyway.
    /// \todo Test if this function is secretly necessary. If not (likely), remove.
    protected override void OnCollisionEnterMethods(Collision2D collisionInfo)
    {
        // if we collided with something we can damage, damage it
        if (collisionInfo.collider.CompareTag("DamagableByProjectile") && damageNonPlayers)
        {
            collisionInfo.collider.GetComponent<ObjectHealth>().TakeDamage(transform, damage);
        }
        else if (collisionInfo.collider.CompareTag("Player") && damagePlayers)
        {
            collisionInfo.collider.GetComponent<PlayerHealth>().TakeDamage(transform, damage);
        }

        if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Default") ||
            collisionInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(sprite);
            Destroy(gameObject);
        }

    }


}
