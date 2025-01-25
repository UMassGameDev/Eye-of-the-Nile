using System.Collections;
using UnityEngine;

/** \brief
Script for a tornado projectile that deals no damage, instead pushing targets away with its hitbox.
Inherits from BasicProjectile.

Documentation updated 9/7/2024
\author Stephen Nuttall
*/
public class TornadoProjectile : BaseProjectile
{
    /// Seconds until the projectile despawns.
    public float despawnTime = 5f;

    /// Start the despawn timer, as well as the base functionality of playing the spawning sound effect.
    protected override void StartMethods()
    {
        AudioManager.instance.PlaySFX(spawnSFX);
        StartCoroutine(DespawnTimer());
    }

    /// Functionality of the DespawnTimer script, but was written before it was created.
    IEnumerator DespawnTimer()
    {
        /// Wait for despawn time to be up,
        yield return new WaitForSeconds(despawnTime);

        /// then destory the projectile.
        Destroy(gameObject);
    }

    /// Do nothing. Ignore base functionality.
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        /// Don't do anything on impact.
    }
}
