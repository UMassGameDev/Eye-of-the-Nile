using UnityEngine;
using System;

/** \brief
Functionality for Geb's rock golems.
Inherits from MeleeEntityController, which inherits from BaseEntityController.
What changed:
- The golems can also damage walls, not just player. (Modification to ActivateAttack())
- If a wall gets in the way of the golem chasing the player, the golem will attack the wall. (Modification to ChaseState())

Documentation updated 1/13/2025
\author Alexander Art
\copyright Lots of code from BaseEntityController and MeleeEntityController is used.
*/
public class RockGolemController : MeleeEntityController
{
    /// The rock golem does not chase objects on these layers, but will attack them if they are in their way of chasing.
    [SerializeField] protected LayerMask attackLayers;
    /// Particles the golem will spawn when it dies.
    public Transform boulderParticles;

    /// Similar to base.ChaseState except for attackLayers also detected.
    protected override void ChaseState()
    {
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite.
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            detectionRange,
            enemyLayers);

        hostileDetected = hitObject != null;
        hostileInCloseRange = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            activateAttackRange,
            enemyLayers) != null;

        // An offset of 1.4 is used to make it closer to upper body height.
        Collider2D objectRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                new Vector2(-transform.localScale.x, 0f).normalized,
                activateAttackRange,
                attackLayers).collider;

        bool objectVeryClose = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                new Vector2(-transform.localScale.x, 0f).normalized,
                1f,
                attackLayers).collider != null;

        bool objectInCloseRange = objectRay != null;

        // If detected enemy horizontal position is within the minimum amount (0.1f), don't switch directions.
        // This prevents the enemy from rapidly switching directions.
        // If detected enemy is to the left, go left.
        // If detected enemy is to the right, go right.
        if (hostileDetected && Mathf.Abs(hitObject.transform.position.x - transform.position.x) < 0.1f)
        {
            horizontalDirection = 0f;
        }
        else if (hostileDetected && hitObject.transform.position.x < transform.position.x)
        {
            horizontalDirection = -1f;
        }
        else if (hostileDetected && hitObject.transform.position.x > transform.position.x)
        {
            horizontalDirection = 1f;
        }
        else
        {
            horizontalDirection = 0f;
        }

        // If an object on attackLayers is in very close range, attack it. If it moves out of very close range, but is still somewhat close, attack it anyways.
        // If the object moves completely out of close range, then don't attack it.
        if ((objectVeryClose && Time.time >= cooldownEndTime) || (animator.GetBool("IsAttacking") && objectInCloseRange && Time.time >= cooldownEndTime))
        {
            horizontalDirection = 0f;
            cooldownEndTime = Time.time + attackCooldown;
            animator.SetBool("IsMoving", false);
            TriggerAttack();
        }
        else if (!objectInCloseRange)
            animator.SetBool("IsAttacking", false);

        // Flip the direction of the enemy depending on direction:
        // Facing right = -2 scale for x
        // Facing left = 2 scale for x
        if (horizontalDirection != 0f)
            transform.localScale = new Vector3(-2 * horizontalDirection,
                    transform.localScale.y,
                    transform.localScale.z);

        if (!animator.GetBool("IsAttacking"))
            animator.SetBool("IsMoving", horizontalDirection != 0f);

        // If the enemy is not on the ground, but needs the ground to walk, then don't let the enemy move.
        // When there is no groundDetector, the enemy is assumed to be on the ground.
        if ((groundDetector != null && !groundDetector.isGrounded) && groundNeeded)
            canWalk = false;
        else
            canWalk = true;

        if (!animator.GetBool("IsAttacking"))
            rb.velocity = new Vector2((moveVelocity + speedModifier) * horizontalDirection * Convert.ToInt32(canWalk), rb.velocity.y);

        // Prioritize death.
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (hostileInCloseRange)
        {
            EState = EntityState.CloseAttack;
        }
        else if (!hostileDetected)
            EState = EntityState.Patrol;
    }

    /// When the attack is activated, scan for the player and any walls. Then, apply damage (and knockback, if its the player). 
    protected override void ActivateAttack()
    {
        // Find all objects in hit range that are on enemyLayers.
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackLayers);

        foreach (Collider2D hitObject in hitObjects)
        {
            // If the object is a player, apply damage and knockback.
            // If the object is not a player, apply damage to the object's health.
            if (hitObject.tag == "Player")
            {
                hitObject.GetComponent<PlayerHealth>().TakeDamage(transform, attackDamage);
                if (knockbackEnabled && hitObject.TryGetComponent<KnockbackFeedback>(out var kb))
                    kb.ApplyKnockback(gameObject, knockback);
            }
            else
            {
                if (hitObject.TryGetComponent<ObjectHealth>(out var health))
                    health.TakeDamage(transform, attackDamage);
            }
        }
    }

    public void SpawnParticlesOnDeath()
    {
        Instantiate(boulderParticles,
            gameObject.transform.position,
            Quaternion.identity);
        Destroy(gameObject);
    }
}