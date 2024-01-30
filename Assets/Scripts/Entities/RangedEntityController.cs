/**************************************************
Functionality for entities with a ranged attack.
Inherits from BaseEntityController.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class RangedEntityController : BaseEntityController
{
    public GameObject projectilePrefab;

    // Triggered by attack animation
    protected override void ActivateAttack()
    {
        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left, flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0) {
            projectile.GetComponent<BasicProjectile>().FlipDirection();
        }
    }

    protected override void TriggerAttack()
    {
        // Here the attack animation would play if there was one
        // Because there's not, we'll just trigger the attack manually for now

        // animator.SetBool("IsAttacking", true);
        ActivateAttack();
    }
    
    // modified so entity flips direction if target moves behind it
    protected override void CloseAttackState()
    {
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            activateAttackRange,
            enemyLayers);

        hostileInCloseRange = hitObject != null;

        // I use an offset of 1.4 to make it closer to upper body height
        if (hostileInCloseRange)
        {
            Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right * activateAttackRange,
                Color.cyan);

            Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.left * activateAttackRange,
                Color.cyan);

            RaycastHit2D enemyFrontRay, enemyBackRay;

            enemyFrontRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right,
                activateAttackRange,
                enemyLayers);

            enemyBackRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.left,
                activateAttackRange,
                enemyLayers);

            horizontalDirection = 0f;
            if (enemyFrontRay.collider != null && Time.time >= cooldownEndTime)
            {
                transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
                horizontalDirection = 0f;
                cooldownEndTime = Time.time + attackCooldown;
                TriggerAttack();
            }
            else if (enemyBackRay.collider != null && Time.time >= cooldownEndTime)
            {
                transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
                cooldownEndTime = Time.time + attackCooldown;
                TriggerAttack();
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }

            if (horizontalDirection != 0f)
                transform.localScale = new Vector3(-2 * horizontalDirection,
                        transform.localScale.y,
                        transform.localScale.z);

        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }

        animator.SetBool("IsMoving", false);

        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (!hostileInCloseRange && Time.time >= cooldownEndTime)
            EState = EntityState.Chase;
    }
}
