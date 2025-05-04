using UnityEngine;

/** \brief
Functionality for entities with a ranged attack.
Inherits from BaseEntityController.

Documentation updated 8/26/2024
\author Roy Pascual
*/
public class RangedEntityController : BaseEntityController
{
    /// The projectile this entity will shoot.
    public GameObject projectilePrefab;

    /// Instantiates a new projectile and flips it to face the right direction.
    protected override void ActivateAttack()
    {
        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left, flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0)
        {
            projectile.GetComponent<BaseProjectile>().FlipDirection();
        }
    }

    /// Plays the attack animation and runs ActivateAttack().
    /// \note The attack animation for the Seth Follower fires the arrow immediately, so just calling ActivateAttack() immediately is ok.
    protected override void TriggerAttack()
    {
        print("Attack!");
        animator.SetTrigger("Attack");
        ActivateAttack();
    }

    /// Modified from the base version so entity flips direction if target moves behind it
    /// \todo Make entityCenter offest a variable rather than hard coded.
    protected override void CloseAttackState()
    {
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, entityCenter, 0f),
            activateAttackRange,
            enemyLayers);

        hostileInCloseRange = hitObject != null;

        if (hostileInCloseRange)
        {
            Debug.DrawRay(transform.position + new Vector3(0f, entityCenter, 0f),
                Vector2.right * activateAttackRange,
                Color.cyan);

            Debug.DrawRay(transform.position + new Vector3(0f, entityCenter, 0f),
                Vector2.left * activateAttackRange,
                Color.cyan);

            RaycastHit2D enemyFrontRay, enemyBackRay;

            enemyFrontRay = Physics2D.Raycast(transform.position + new Vector3(0f, entityCenter, 0f),
                Vector2.right,
                activateAttackRange,
                enemyLayers);

            enemyBackRay = Physics2D.Raycast(transform.position + new Vector3(0f, entityCenter, 0f),
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
