/**************************************************
If something enters this object's trigger zone, this script will spawn an object or projectile.
This is useful for stage hazards, such as the arrow trap.

Documentation updated 1/29/2024
**************************************************/
using Unity.VisualScripting;
using UnityEngine;

public class SpawnOnTrigger : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float spawnCooldown = 3f;
    public bool facingLeft = true;  // on required if the object is a projectile

    float cooldownTimer = 0f;

    void Start()
    {
        cooldownTimer = spawnCooldown;
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime; // update cooldown timer
    }

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
