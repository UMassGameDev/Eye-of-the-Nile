using UnityEngine;

/** \brief
Script for the walls that Geb summons.
They are raised from the ground. This spawning animation can be replaced with anything once the idea is finalized.


Documentation updated 1/10/2025
\author Alexander Art
\todo Make the walls spawn debris when broken.
*/
public class ProtectiveWall : MonoBehaviour
{
    // The amount of time that the spawning animation lasts.
    protected float spawnDuration = 1f;

    // Keeps track of when this object was started. Used for calculating how long it has been around.
    private float startTime = 0f;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // Raise the wall for spawnDuration seconds.
        if (Time.time - startTime < spawnDuration)
        {
            transform.position += new Vector3(0f, 7f * Time.deltaTime, 0f);
        }
    }

    // To do: replace with debris.
    public void DestroyWall()
    {
        Destroy(gameObject);
    }
}
