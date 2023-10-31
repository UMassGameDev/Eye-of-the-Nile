using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public GameObject sprite;
    public GameObject thisProjectile;
    public float speed = 100f;
    public int damage = 30;
    public bool facingLeft = false;

    Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (facingLeft) {
            rb.AddForce(new Vector2(-speed * Time.deltaTime, 0));
        } else {
            rb.AddForce(new Vector2(speed * Time.deltaTime, 0));
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo.collider.tag == "DamagableByProjectile") {
            collisionInfo.collider.GetComponent<ObjectHealth>().TakeDamage(damage);
        }
        Destroy(sprite);
        Destroy(thisProjectile);
    }
}
