/**************************************************
Script for a boulder that the player can throw as a projectile.
Inherits from BasicProjectile.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class BoulderProjectile : BasicProjectile
{
    Rigidbody2D boulderBody;
    public Transform boulderParticles;
    public float initialForce = 100f;
    public LayerMask collisionLayers;

    public void BreakBoulder()
    {
        Collider2D thisCollider = GetComponent<Collider2D>();
        Instantiate(boulderParticles,
            thisCollider.bounds.center,
            Quaternion.identity);
        Destroy(gameObject);
    }

    void Awake()
    {
        boulderBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (facingLeft) {
            boulderBody.AddForce(new Vector2(-initialForce, initialForce), ForceMode2D.Impulse);
        } else {
            boulderBody.AddForce(new Vector2(initialForce, initialForce), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        // Collision is handled by the hitbox!
    }

    void Update()
    {
        // Don't propell the projectile continuously!
    }
}
