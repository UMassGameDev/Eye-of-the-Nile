using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour
{
    public Transform boulderPrefab;
    public Transform spawnPoint;
    float spawnCooldown = 3f;
    bool isSpawning = true;

    IEnumerator BoulderSpawnTimer()
    {
        while (isSpawning)
        {
            SpawnBoulder();
            yield return new WaitForSeconds(spawnCooldown);
        }
        
    }

    void SpawnBoulder()
    {
        Transform newBoulder = Instantiate(boulderPrefab,
            spawnPoint.position, Quaternion.identity);
        newBoulder.transform.up = transform.up;
    }

    void Awake()
    {
        spawnPoint = transform.Find("SpawnPoint");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BoulderSpawnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
