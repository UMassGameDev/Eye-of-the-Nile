using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public GameObject sprite;  // OPTIONAL
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
        spriteScaleRight = sprite.transform.localScale;
        spriteScaleLeft = new Vector3(-sprite.transform.localScale.x, sprite.transform.localScale.y, sprite.transform.localScale.z);
    }

    void Start()
    {
        AudioManager.Instance.PlaySFX(spawnSFX);
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
        if (collisionInfo.collider.CompareTag("DamagableByProjectile") && damageNonPlayers) {
            collisionInfo.collider.GetComponent<ObjectHealth>().TakeDamage(transform, damage);
        } else if (collisionInfo.collider.CompareTag("Player") && damagePlayers) {
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
