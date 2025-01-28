using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** \brief
Simple enemy script designed for a "Seth Follower" enemy.
This script is separate from the existing entity systems.

Documentation updated 1/27/2025
\author Michael Leahy
\deprecated Please use the more developed entity systems to make new enemies.
*/
[System.Obsolete("Please use the more developed entity systems to make new enemies")]
public class SethFollowerScript : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public PlayerHealth playerHealth;
    // public Transform transform;

    [Header("Basic Attributes")]
    public float speed;

    [Header("Attack Attributes")]
    public int damage;
    public float distanceBetween;
    private float distance;

    void Update()
    {
        // checks distance between player and enemy
        distance = Vector2.Distance(transform.position, player.transform.position);

        // if the player is near the enemy then chase the player
        if (distance < distanceBetween)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // damages player
        if (collision.gameObject == player)
        {
            playerHealth.TakeDamage(transform, damage);
        }
    }
}
