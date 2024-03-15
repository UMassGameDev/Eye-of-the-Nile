/**************************************************
Basic projectile functionality that every other projectile type must inherit from to be compatible with other systems (like the attack manager).
You can also put this script onto any object or prefab to give it normal projectile functionality.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public GameObject sprite;
    public float speed = 0.3f;
    public int damage = 30;
    public bool facingLeft = false;
    public bool damagePlayers = true;
    public bool damageNonPlayers = true;
    public string spawnSFX = "bullet_fire";

    Vector3 spriteScaleLeft;
    Vector3 spriteScaleRight;

    void Awake()
    {
        AwakeMethods();
    }

    protected virtual void AwakeMethods()
    {
        // determine ahead of time how to display the sprite depending on the direction the projectile is facing
        spriteScaleRight = sprite.transform.localScale;
        spriteScaleLeft = new Vector3(-sprite.transform.localScale.x, sprite.transform.localScale.y, sprite.transform.localScale.z);
    }

    void Start()
    {
        StartMethods();
    }

    protected virtual void StartMethods()
    {
        AudioManager.Instance.PlaySFX(spawnSFX);
    }

    void Update()
    {
        UpdateMethods();
    }

    protected virtual void UpdateMethods()
    {
        // move in the direction the projectile is set to face, and ensure the sprite is facing that direction
        if (facingLeft)
        {
            // move projectile to the left by [speed]
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            sprite.transform.localScale = spriteScaleLeft;
        }
        else
        {
            // move projectile to the right by [speed]
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
            sprite.transform.localScale = spriteScaleRight;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnterMethods(collision);
    }

    protected virtual void OnTriggerEnterMethods(Collider2D collision)
    {

    }

    // if projectile collides with something...
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        OnCollisionEnterMethods(collisionInfo);
    }

    protected virtual void OnCollisionEnterMethods(Collision2D collisionInfo)
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

        // Destory the projectile
        Destroy(sprite);
        Destroy(gameObject);
    }

    public void FlipDirection()
    {
        facingLeft = !facingLeft;
    }
}
