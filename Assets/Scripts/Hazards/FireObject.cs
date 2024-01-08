using System.Collections;
using System.Collections.Generic;
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
        ObjectHealth health = col.GetComponent<ObjectHealth>();

        if (health != null)
        {
            if (!col.CompareTag("Player") && damageNonPlayers) {
                StartCoroutine(health.SetOnFire(damageCount, damageSpeed, damage));
            } else if (col.CompareTag("Player") && damagePlayer) {
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