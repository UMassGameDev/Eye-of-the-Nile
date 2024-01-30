/**************************************************
--- DEPRECATED ---
Please use PlayerAttackManager.cs

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class PlayerBasicProjectileAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform attackPoint;
    public float attackCooldown = 1f;
    float cooldownTimer = 0f;

    public PlayerAttackManager playerAttackManager;

    void Update()
    {
        cooldownTimer += Time.deltaTime; // update attack cooldown timer

        // if right click is pressed and the cooldown timer has expired...
        if (Input.GetKeyDown(KeyCode.Mouse1) && cooldownTimer >= attackCooldown)
        {
            playerAttackManager.ShootProjectile(projectilePrefab);

            // reset cooldown timer
            cooldownTimer = 0;
        }
    }
}
