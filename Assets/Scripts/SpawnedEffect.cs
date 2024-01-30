/**************************************************
When a particle spawned, this script will despawn it after its duration is up.

Documentation updated 1/29/2024
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
