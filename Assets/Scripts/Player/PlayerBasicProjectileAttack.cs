using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicProjectileAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform attackPoint;
    public float attackCooldown = 1f;
    
    float cooldownTimer = 0f;

    void Update()
    {
        cooldownTimer += Time.deltaTime; // update attack cooldown timer

        // if right click is pressed and the cooldown timer has expired...
        if (Input.GetKeyDown(KeyCode.Mouse1) && cooldownTimer >= attackCooldown)
        {
            // create projectile object
            GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

            // if we're facing left, flip the direction (projectile faces right by default)
            if (transform.localScale.x > 0) {
                projectile.GetComponent<BasicProjectile>().FlipDirection();
            }

            // reset cooldown timer
            cooldownTimer = 0;
        }
    }
}
