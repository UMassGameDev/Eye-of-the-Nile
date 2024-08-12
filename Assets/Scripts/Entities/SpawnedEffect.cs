/**************************************************
When a particle spawned, this script will despawn it after its duration is up.
Ideally, despawn timer should be used instead.

Documentation updated 8/12/2024
**************************************************/
using UnityEngine;

public class SpawnedEffect : MonoBehaviour
{
    ParticleSystem spawnedParticles;
    float effectDuration;

    void Awake()
    {
        spawnedParticles = GetComponent<ParticleSystem>();
        effectDuration = spawnedParticles.main.duration;
    }

    // timer before effect expires
    void Update()
    {
        if (spawnedParticles.totalTime >= effectDuration)
        {
            Destroy(gameObject);
        }
    }
}
