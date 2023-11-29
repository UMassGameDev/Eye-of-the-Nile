using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnTrigger : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float spawnCooldown = 3f;
    public bool facingLeft = true;

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
            if (facingLeft)
                projectile.GetComponent<BasicProjectile>().FlipDirection();
        }

        cooldownTimer = 0;
    }
}
