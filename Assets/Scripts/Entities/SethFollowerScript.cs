using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(distance < distanceBetween)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // damages player
        if(collision.gameObject == player)
        {
            playerHealth.TakeDamage(transform, damage);
        }
    }
}
