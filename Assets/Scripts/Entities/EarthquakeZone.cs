using System.Collections.Generic;
using UnityEngine;

public class EarthquakeZone : MonoBehaviour
{
    /// Reference to the particles that will spawn every time the earthquake damage occurs.
    [SerializeField] Transform particles;
    /// The amount of damage the earthquake deals.
    [SerializeField] int damageAmount = 10;
    /// How many time the earthquake zone will damage entities inside it.
    [SerializeField] int damageCount = 3;
    /// The time between each attempt at damaging entities.
    [SerializeField] float timeBetweenDamage = 1f;
    /// How much entities are slowed down when inside the earthquake zone.
    [SerializeField] float entitySpeedModifier = 3f;
    /// The amount of time after the last damage attempt the particles should linger for.
    [SerializeField] float particleLingerTime = 1f;
    /// Destroys the earthquake zone when it does all damageCount attempts at damaging entities.
    [SerializeField] bool destroyWhenDoneDamaging = true;
    /// How many attempts at damaging entities have passed.
    int damageCounter = 0;
    /// How much time since the last attempt at damaging entities.
    float damageTimer = 0f;

    /// The list of object health components to apply damage to.
    List<ObjectHealth> objectsToDamage = new();

    /// Set the size of the area particles can spawn in, set how long they'll spawn for, and create the particle spawner
    void Awake()
    {
        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();

        ParticleSystem.ShapeModule particleShape = particleSystem.shape;
        particleShape.scale = GetComponent<Collider2D>().bounds.size;

        ParticleSystem.MainModule particleMain = particleSystem.main;
        particleMain.duration = timeBetweenDamage * (damageCount + particleLingerTime);

        Instantiate(particles, transform.position, Quaternion.identity);
    }

    /// \brief Every frame, check if it's time to attempt to damage an entity.
    /// If so, damage each entity in the objectsToDamage list.
    /// If destroyWhenDoneDamaging is enabled, check if the damageCounter has passed damageCount and if so, destroy this object.
    void Update()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer > timeBetweenDamage && damageCounter < damageCount)
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

        if (destroyWhenDoneDamaging && damageCounter >= damageCount)
        {
            Destroy(gameObject);
        }
    }

    /// When an object enters the earthquake zone, attempt to slow it down and add it's object health to the objecctsToDamage.
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

    /// When an object enters the earthquake zone, attempt to restore it's speed and remove it's object health from the objecctsToDamage.
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