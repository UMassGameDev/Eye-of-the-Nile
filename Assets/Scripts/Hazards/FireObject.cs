using System.Collections;
using FMODUnity;
using UnityEngine;

/** \brief
Script for a fire object, such a big fire, wall of fire, anything that's fire.
If burnout enabled, the fire will despawn after burnoutTime has passed.
If an entity collides with the fire, it will be set on fire and take fire damage.

Documentation updated 8/26/2024
\author Stephen Nuttall
\todo Replace damageNonPlayers and damagePlayer bools with one layer mask that allows for more choices in what is damaged.
*/
public class FireObject : MonoBehaviour
{
    /// How many times damage should be dealt after being set on fire by this object.
    [SerializeField] int damageCount = 6;
    /// How quickly damage should be dealt after being set on fire by this object.
    [SerializeField] float damageSpeed = 0.5f;
    /// Damage being set on fire from this object will do (every damageSpeed seconds).
    [SerializeField] int damage = 5;
    /// Seconds until fire despawns (if enabled).
    [SerializeField] float burnoutTime = 10f;

    /// If burnout enabled, the fire will despawn after burnoutTime has passed.
    [SerializeField] bool burnout = false;
    /// If true, entities that are not the player will be set on fire when stepping into this object.
    [SerializeField] bool damageNonPlayers = true;
    /// If true, the player will be set on fire when stepping into this object.
    [SerializeField] bool damagePlayer = true;
    /// Sound effect that plays when the object first spawns in.
    [SerializeField] EventReference spawnSFX;
    /// Play a set-on-fire sound effect when spawned.
    [SerializeField] bool playSFXOnSpawn = false;

    /// Play set on fire sound effect and start burnout timer (if those are enabled).
    void Start()
    {
        if (playSFXOnSpawn)
            AudioManager.instance.PlaySFX(spawnSFX);
        if (burnout)
            StartCoroutine(BurnoutTimer());
    }

    /// <summary>
    /// If an object entered this fire, set it on fire (unless damageNonPlayers or damagePlayer dictates otherwise).
    /// </summary>
    /// <param name="col">Represents the object that entered the fire.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        // if the object in the fire has health...
        if (col.TryGetComponent<ObjectHealth>(out var health))
        {
            if (!col.CompareTag("Player") && damageNonPlayers)
            {  // if it's not a player (and damageNonPlayers is enabled), set it on fire.
                StartCoroutine(health.SetOnFire(damageCount, damageSpeed, damage));
            }
            else if (col.CompareTag("Player") && damagePlayer)
            {  // if it is a player (and damagePlayer is enabled), set it on fire.
                StartCoroutine(health.SetOnFire(damageCount, damageSpeed, damage));
            }
        }
    }

    /// Wait burnoutTime seconds, then destroy this fire.
    IEnumerator BurnoutTimer()
    {
        yield return new WaitForSeconds(burnoutTime);
        Destroy(gameObject);
    }
}