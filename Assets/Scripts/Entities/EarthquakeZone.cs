using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EarthquakeZone : MonoBehaviour
{
    [SerializeField] int damageAmount = 10;
    [SerializeField] int damageCount = 3;
    [SerializeField] float timeBetweenDamage = 1f;
    [SerializeField] float entitySpeedModifier = 3f;
    [SerializeField] bool destroyWhenDoneDamaging = true;
    int damageCounter = 0;
    float damageTimer = 0f;

    List<ObjectHealth> objectsToDamage = new();

    void Update()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer > timeBetweenDamage && damageCounter <= damageCount)
        {
            foreach (ObjectHealth health in objectsToDamage)
            {
                health.TakeDamage(damageAmount);
                if (health.IsDead)
                    objectsToDamage.Remove(health);
            }

            damageTimer = 0;
            damageCounter++;
        }

        if (destroyWhenDoneDamaging && damageCounter > damageCount)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<BaseEntityController>(out var controller))
        {
            controller.ChangeSpeed(-entitySpeedModifier);
        }

        if (!col.CompareTag("Player") && col.TryGetComponent<ObjectHealth>(out var health) && !objectsToDamage.Contains(health))
        {
            objectsToDamage.Add(health);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.TryGetComponent<BaseEntityController>(out var controller))
        {
            controller.ChangeSpeed(entitySpeedModifier);
        }

        if (!col.CompareTag("Player") && col.TryGetComponent<ObjectHealth>(out var health) && objectsToDamage.Contains(health))
        {
            objectsToDamage.Remove(health);
        }
    }
}