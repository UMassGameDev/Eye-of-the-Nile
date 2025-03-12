using UnityEngine;

/** \brief
Functionality of ObjectHealth for breakable pots.
This script inherits from ObjectHealth. When the pot breaks, this script instantiates and launches a broken pot in its place.

Documentation updated 3/11/2025
\author Alexander Art
*/
public class PotHealth : ObjectHealth
{
    /// Prefab for the broken pot that spawns when the pot is broken.
    [SerializeField] Transform brokenPotPrefab;

    /// Random number generator, used for launching the shards.
    System.Random rng = new System.Random();

    /// When the pot takes damage, it breaks.
    public override void TakeDamage(Transform attacker, int damage)
    {
        AudioManager.instance.PlaySFX(deathSfxName);
        OnDeath?.Invoke();

        // Spawns the broken pot at the pot's position with the same rotation.
        Transform brokenPot = Instantiate(brokenPotPrefab, transform.position, transform.rotation);

        // Sets the scale of the broken pot to be the same as the original pot.
        brokenPot.localScale = transform.localScale;

        // Launch each shard of the broken pot a random amount in the direction away from the attacker.
        foreach (Transform shard in brokenPot.transform)
        {
            Rigidbody2D shardRigidbody = shard.GetComponent<Rigidbody2D>();
            shardRigidbody.velocity = (shard.position - attacker.position) * 5f * (float)rng.NextDouble();
        }

        Destroy(gameObject);
    }
}
