/**************************************************
Spawns boulders at a consistent rate in the game object's "up" direction.
Rotate the game object to change which direction "up" is to be to the left (90 degrees) or right (-90 degrees)

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
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
}
