using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/** \brief
This is the script for Geb's earthquake zone.
This script summons particles, scales the earthquake zone size, can damage the player, and can slow the player down.

Documentation updated 1/30/2025
\author Alexander Art
\copyright Copied & modified from EarthquakeZone.cs.
\todo Finish documentation for this file.
*/
public class GebEarthquakeZone : MonoBehaviour
{
    /// Reference to the particle object that will be repeatedly instantiated at the earthquake.
    [SerializeField] GameObject particleEffect;
    /// Reference to Geb's boss controller.
    protected GebBossController gebBossController;

    [SerializeField] float maxEarthquakeSize = 25f;
    [SerializeField] float timeBetweenParticles = 0.1f;
    [SerializeField] int damageAmount = 10;
    [SerializeField] float timeBetweenDamage = 0.2f;
    [SerializeField] float playerMoveVelocity = 6f;

    /// Create random number generator.
    private System.Random rng = new System.Random();
    float particleTimer = 0f;
    float damageTimer = 0f;
    List<PlayerHealth> objectsToDamage = new();
    float initialPlayerMoveVelocity;
    float initialParticleEffectSizeX;

    void Awake()
    {
        gebBossController = transform.parent.GetComponent<GebBossController>();
        initialPlayerMoveVelocity = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().moveVelocity;
        initialParticleEffectSizeX = particleEffect.GetComponent<ParticleSystem>().shape.scale.x;
    }

    void Update()
    {
        // Make the earthquake zone (the part that damages the player) get larger as the earthquake action progresses.
        transform.localScale = new Vector3(gebBossController.GetCurrentActionPercentage() * maxEarthquakeSize, transform.localScale.y, transform.localScale.z);

        particleTimer += Time.deltaTime;
        if (particleTimer > timeBetweenParticles)
        {
            particleTimer = 0;

            // Instantiate the particle effect and get the instance's shape module at the same time.
            ParticleSystem.ShapeModule particleInstance = Instantiate(particleEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().shape;
            // Make the particles get larger as the earthquake action progresses (matches the earthquake zone).
            particleInstance.scale = new Vector3(initialParticleEffectSizeX * gebBossController.GetCurrentActionPercentage() / 15f * maxEarthquakeSize, particleInstance.scale.y, particleInstance.scale.z);
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