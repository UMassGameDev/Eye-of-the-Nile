using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Copied & modified from EarthquakeZone.
// \todo Update documentation for this file.
// \todo Make the earthquake zone more visible (by making the particle spawn radius less random and more spread out).
public class GebEarthquakeZone : MonoBehaviour
{
    [SerializeField] GameObject particleEffect;

    [SerializeField] float particleSpawnRadius = 12f;
    [SerializeField] float timeBetweenParticles = 0.2f;
    [SerializeField] int damageAmount = 10;
    [SerializeField] float timeBetweenDamage = 0.2f;
    [SerializeField] float playerMoveVelocity = 6f;

    /// Create random number generator.
    private System.Random rng = new System.Random();
    float particleTimer = 0f;
    float damageTimer = 0f;
    List<PlayerHealth> objectsToDamage = new();
    float initialPlayerMoveVelocity;

    void Awake()
    {
        initialPlayerMoveVelocity = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().moveVelocity;
    }

    void Update()
    {
        particleTimer += Time.deltaTime;
        if (particleTimer > timeBetweenParticles)
        {
            particleTimer = 0;

            Vector3 spawnPos = transform.position + new Vector3(((float)rng.NextDouble() * 2f - 1f) * particleSpawnRadius, 0f, 0f);
            Instantiate(particleEffect, spawnPos, Quaternion.identity);
        }

        damageTimer += Time.deltaTime;
        if (damageTimer > timeBetweenDamage)
        {
            damageTimer = 0;

            foreach (PlayerHealth health in objectsToDamage)
            {
                health.TakeDamage(this.transform, damageAmount);
                if (health.IsDead)
                    objectsToDamage.Remove(health);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Reduce the player's move velocity when they enter the earthquake zone.
        if (col.TryGetComponent<PlayerMovement>(out var controller))
        {
            controller.moveVelocity = playerMoveVelocity;
        }

        if (col.CompareTag("Player") && col.TryGetComponent<PlayerHealth>(out var health) && !objectsToDamage.Contains(health))
        {
            objectsToDamage.Add(health);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Restore the player's move velocity when the exit the earthquake zone.
        if (col.TryGetComponent<PlayerMovement>(out var controller))
        {
            controller.moveVelocity = initialPlayerMoveVelocity;
        }

        if (col.CompareTag("Player") && col.TryGetComponent<PlayerHealth>(out var health) && objectsToDamage.Contains(health))
        {
            objectsToDamage.Remove(health);
        }
    }
}