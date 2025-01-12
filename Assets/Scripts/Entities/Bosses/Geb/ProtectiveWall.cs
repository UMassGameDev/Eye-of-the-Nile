using UnityEngine;

/** \brief
Script for the walls that Geb summons.
They are raised from the ground. This spawning animation can be replaced with anything once the idea is finalized.
When the wall is broken, debris spawn in its place.

Documentation updated 1/11/2025
\author Alexander Art
\todo Prevent walls from overlapping (being summoned on top of each other).
*/
public class ProtectiveWall : MonoBehaviour
{
    /// Reference to the wall debris prefab for the debris that falls when the wall breaks.
    [SerializeField] protected GameObject wallDebris;

    /// The amount of time that the spawning animation lasts.
    protected float spawnDuration = 1f;

    /// Create random number generator (for randomly displacing debris).
    private System.Random rng = new System.Random();
    /// Keeps track of when this object was started. Used for calculating how long it has been around.
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
            transform.position += new Vector3(0f, 12f * Time.deltaTime, 0f);
        }
    }

    /// Called whenever the wall is broken. Debris spawn in place of the wall.
    public void DestroyWall()
    {
        // Instantiate the debris.
        for (int y=-7; y<12; y++)
        {
            Rigidbody2D debris = Instantiate(wallDebris, transform.position + new Vector3((float)rng.NextDouble()*4f-2f, y, 0f), transform.rotation).GetComponent<Rigidbody2D>();
            debris.velocity = new Vector2((float)rng.NextDouble()*4f-2f, (float)rng.NextDouble());
        }

        // Get rid of the wall object.
        Destroy(gameObject);
    }
}
