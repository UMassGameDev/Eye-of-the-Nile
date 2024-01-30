/**************************************************
Script for a fire object, such a big fire, wall of fire, anything that's fire.
If enabled, the fire will "burnout" (despawn) after burnoutTime has passed.
If an entity collides with the fire, it will be set on fire and take fire damage.

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;

public class FireObject : MonoBehaviour
{
    public int damageCount = 6;
    public float damageSpeed = 0.5f;
    public int damage = 5;
    public float burnoutTime = 10f;  // seconds until fire despawns (if enabled)

    public bool burnout = false;  // despawn after burnout time is up
    public bool damageNonPlayers = true;
    public bool damagePlayer = true;
    public bool playSFXOnSpawn = false;

    void Start()
    {
        if (playSFXOnSpawn)
            AudioManager.Instance.PlaySFX("set_on_fire");
        if (burnout)
            StartCoroutine(BurnoutTimer());
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        // if the object in the fire has health...
        if (col.TryGetComponent<ObjectHealth>(out var health))
        {
            if (!col.CompareTag("Player") && damageNonPlayers) {  // if it's not a player (and damageNonPlayers is enabled), set it on fire.
                StartCoroutine(health.SetOnFire(damageCount, damageSpeed, damage));
            } else if (col.CompareTag("Player") && damagePlayer) {  // if it is a player (and damagePlayer is enabled), set it on fire.
                StartCoroutine(health.SetOnFire(damageCount, damageSpeed, damage));
            }
        }
    }

    IEnumerator BurnoutTimer()
    {
        yield return new WaitForSeconds(burnoutTime);
        Destroy(gameObject);
    }
}