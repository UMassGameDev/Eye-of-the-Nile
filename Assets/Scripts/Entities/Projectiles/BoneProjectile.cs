using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneProjectile : BasicProjectile
{
    Rigidbody2D boneRb;
    public float initialXForce = 25f; // we're not getting out of Marvel with this one
    public float initialYForce = 50f;

    protected override void AwakeMethods()
    {
        boneRb = GetComponent<Rigidbody2D>();
        damageNonPlayers = false;
    }

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

    protected override void UpdateMethods()
    {
        if (facingLeft)
            transform.Rotate(0f, 0f, 500f * Time.deltaTime);
        else
            transform.Rotate(0f, 0f, -500f * Time.deltaTime);
    }

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
