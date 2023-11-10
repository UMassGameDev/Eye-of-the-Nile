using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public GameObject sprite;
    public GameObject thisProjectile;
    public float speed = 0.3f;
    public int damage = 30;
    public bool facingLeft = false;

    Vector3 spriteScaleLeft;
    Vector3 spriteScaleRight;
    void Awake()
    {
        spriteScaleRight = sprite.transform.localScale;
        spriteScaleLeft = new Vector3(-sprite.transform.localScale.x, sprite.transform.localScale.y, sprite.transform.localScale.z);
    }

    void Update()
    {
        if (facingLeft) {
            // move projectile to the left by [speed]
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            sprite.transform.localScale = spriteScaleLeft;
        } else {
            // move projectile to the right by [speed]
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
            sprite.transform.localScale = spriteScaleRight;
        }
    }

    // if projectile collides with something...
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        // if we collided with something we can damage, damage it
        if (collisionInfo.collider.tag == "DamagableByProjectile") {
            collisionInfo.collider.GetComponent<ObjectHealth>().TakeDamage(transform, damage);
        }
        
        // Destory the projectile
        Destroy(sprite);
        Destroy(thisProjectile);
    }

    public void FlipDirection()
    {
        facingLeft = !facingLeft;
    }
}
