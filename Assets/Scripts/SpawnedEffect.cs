using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedParticles.totalTime >= effectDuration)
        {
            Destroy(gameObject);
        }
    }
}
