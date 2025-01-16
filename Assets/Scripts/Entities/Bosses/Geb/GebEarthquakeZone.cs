using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Copied & modified from EarthquakeZone.
public class GebEarthquakeZone : MonoBehaviour
{
    [SerializeField] int damageAmount = 10;
    [SerializeField] float timeBetweenDamage = 1f;
    [SerializeField] float playerMoveVelocity = 6f;
    float damageTimer = 0f;

    List<PlayerHealth> objectsToDamage = new();
    float initialPlayerMoveVelocity;

    void Awake()
    {
        initialPlayerMoveVelocity = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().moveVelocity;
    }

    void Update()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer > timeBetweenDamage)
        {
            foreach (PlayerHealth health in objectsToDamage)
            {
                health.TakeDamage(this.transform, damageAmount);
                Debug.Log("heck");
                if (health.IsDead)
                    objectsToDamage.Remove(health);
            }

            damageTimer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
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