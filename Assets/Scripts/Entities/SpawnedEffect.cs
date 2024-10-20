using UnityEngine;

/** \brief
When a particle spawned, this script will despawn it after its duration is up.

Documentation updated 8/26/2024
\author Roy Pascual
\note Formerly marked as deprecated, but in practice it's actually simplier to use this script over DespawnTimer
because it already knows how long it should last for without it having to be specified in the Unity Editor.
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
