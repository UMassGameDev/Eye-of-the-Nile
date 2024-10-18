using System.Collections;
using UnityEngine;

/** \brief
Script for a tornado projectile that deals no damage, instead pushing targets away with its hitbox.
Inherits from BasicProjectile.

Documentation updated 9/7/2024
\author Stephen Nuttall
*/
public class TornadoProjectile : BasicProjectile
{
    /// Seconds until the projectile despawns.
    public float despawnTime = 5f;

    /// Start the despawn timer, as well as the base functionality of playing the spawning sound effect.
    protected override void StartMethods()
    {
        AudioManager.Instance.PlaySFX(spawnSFX);
        StartCoroutine(DespawnTimer());
    }

    /// Same functionality as the base function, except the sprite doesn't flip.
    protected override void UpdateMethods()
    {
        // move projectile to the left/right by (speed * Time.deltaTime), accounting for the amount of time that has past since the last frame
        if (facingLeft) {
            transform.position = new Vector2(transform.position.x - (speed * Time.deltaTime), transform.position.y);
        } else {
            transform.position = new Vector2(transform.position.x + (speed * Time.deltaTime), transform.position.y);
        }
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
