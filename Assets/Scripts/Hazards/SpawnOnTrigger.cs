using Unity.VisualScripting;
using UnityEngine;

/** \brief
If something enters this object's trigger zone, this script will spawn an object or projectile.
This is useful for stage hazards, such as the arrow trap.

Documentation updated 8/26/2024
\author Stephen Nuttall
*/
public class SpawnOnTrigger : MonoBehaviour
{
    /// Projectile to spawn when an object enters the trigger zone.
    public GameObject projectilePrefab;
    /// Position where the projectile should be spawned.
    public Transform spawnPoint;
    /// How long, in seconds, until another projectile can be spawned.
    public float spawnCooldown = 3f;
    /// Should the projectile be facing left?
    public bool facingLeft = true;

    /// Seconds since cooldown was last started. When greater than spawnCooldown, cooldown is over.
    float cooldownTimer = 0f;

    /// Start cooldownTimer at spawnCooldown so the projectile can be spawned immediately.
    void Start()
    {
        cooldownTimer = spawnCooldown;
    }

    /// Increase cooldownTimer by the amount of time that's passed since the last frame.
    void Update()
    {
        cooldownTimer += Time.deltaTime;
    }

    /// <summary>
    /// If an object enters the trigger zone (and the cooldown is up), instantiate a new instance of projectilePrefab.
    /// Make sure it's facing the right direction (if projectilePrefab is actually a BasicProjectile), and reset the cooldown timer.
    /// </summary>
    /// <param name="col">Represents the object that's entered the trigger zone.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (cooldownTimer >= spawnCooldown && col.tag != "Projectile")
        {
            GameObject projectile = Instantiate(projectilePrefab, new Vector2(spawnPoint.position.x, spawnPoint.position.y), Quaternion.identity);
            
            // if object is a projectile and facing left, flip the projectile's direction
            if (facingLeft && projectile.TryGetComponent<BasicProjectile>(out var bp))
                bp.FlipDirection();
        }

        cooldownTimer = 0;
    }
}
