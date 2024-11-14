using UnityEngine;

/** \brief
A projectile used in Geb's ability set that spawns an earthquake where it lands.

Documentation updated 11/13/2024
\author Stephen Nuttall
*/
public class EarthquakeProjectile : BaseProjectile
{
    /// Reference to the particles to spawn when the boulder breaks.
    [SerializeField] Transform boulderParticles;
    [SerializeField] GameObject earthquakeZone;

    /// Same execution as the base function, but also spawns particles and earthquake zone.
    protected override void OnCollisionEnterMethods(Collision2D collisionInfo)
    {
        base.OnCollisionEnterMethods(collisionInfo);

        Collider2D thisCollider = GetComponent<Collider2D>();
        Instantiate(boulderParticles, thisCollider.bounds.center, Quaternion.identity);

        Instantiate(earthquakeZone, thisCollider.bounds.center, Quaternion.identity);
    }
}
