using System.Collections;
using UnityEngine;

/** \brief
Spawns boulders at a consistent rate at a given spawn point, facing upwards.

Documentation updated 8/26/2024
\author Roy Pascual
\note Boulders apply their initial force in their "up" direction, which is set to be this object's "up" direction. This will cause the boulder to
be thrown upwards, rather than to the side like anticipated. The intention is that you rotate the spawner object to change which direction "up" is,
and thus the direction the boulders will spawn from and be propelled towards. Usually you'd want to set it to face either to the left or right,
which is 90 degrees and -90 (or 270) degrees respectively.
*/
public class BoulderSpawner : MonoBehaviour
{
    /// Reference to the prefab of the boulder which we want to spawn.
    public Transform boulderPrefab;
    /// Reference to the point the boulder spawns at.
    public Transform spawnPoint;
    /// Time between each boulder spawn, in seconds.
    [SerializeField] float spawnCooldown = 3f;
    /// True if the spawner is currently spawning boulders.
    bool isSpawning = true;
    /// Time until object despawns, in seconds. -1 to use the boulder prefab's default despawn time.
    public float seconds = -1;

    /// Spawns a boulder every spawnCooldown seconds.
    IEnumerator BoulderSpawnTimer()
    {
        while (isSpawning)
        {
            SpawnBoulder();
            yield return new WaitForSeconds(spawnCooldown);
        }
        
    }

    /// Instantiates a boulder prefab and sets its up direction to this object's up direction. Also sets despawn time (if applicable).
    void SpawnBoulder()
    {
        Transform newBoulder = Instantiate(boulderPrefab,
            spawnPoint.position, Quaternion.identity);
        newBoulder.transform.up = transform.up;
        if (seconds != -1)
        {
            newBoulder.GetComponent<DespawnTimer>().seconds = seconds;
        }
    }

    /// Find the spawn point and set our reference to it.
    void Awake()
    {
        spawnPoint = transform.Find("SpawnPoint");
    }

    /// Starts the boulder spawner timer
    void Start()
    {
        StartCoroutine(BoulderSpawnTimer());
    }
}
