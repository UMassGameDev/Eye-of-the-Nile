using UnityEngine;

/** \brief
When a particle spawned, this script will despawn it after its duration is up.

Documentation updated 8/26/2024
\deprecated Ideally, despawn timer should be used instead because it has the same functionality but implemented better.
*/
public class SpawnedEffect : MonoBehaviour
{
    /// Reference to the particles.
    ParticleSystem spawnedParticles;
    /// How long the particles will last before they despawn.
    float effectDuration;

    /// Set reference to particles and get effect duration from it.
    void Awake()
    {
        spawnedParticles = GetComponent<ParticleSystem>();
        effectDuration = spawnedParticles.main.duration;
    }

    /// Simple timer that destroys the effect object when it expires expires
    void Update()
    {
        if (spawnedParticles.totalTime >= effectDuration)
        {
            Destroy(gameObject);
        }
    }
}
