using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEntityController : MonoBehaviour
{
    ObjectHealth objectHealth;

    public Transform attackPoint;
    public GameObject projectilePrefab;
    public int attackDamage = 30;
    public float attackCooldown = 1f;
    float cooldownEndTime = 0f;

    // This LayerMask includes the Player's layer so the enemy is alerted
    public LayerMask enemyLayers;
    public PatrolZone patrolZone;
    public EntityState EState { get; set; } = EntityState.Patrol;
    public float horizontalDirection = 0f;
    public float detectionRange = 9f;
    public float attackRange = 7f;
    bool hostileDetected = false;
    bool hostileInCloseRange = false;

    Rigidbody2D rb;
    public float moveVelocity = 6.0f;
    public float linearDrag = 1.0f;
    public float groundedRaycastLength = 1.8f;

    Animator animator;

    // Triggered by attack animation
    void ActivateAttack()
    {
        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left, flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0) {
            projectile.GetComponent<BasicProjectile>().FlipDirection();
        }
    }

    void RangedAttack()
    {
        // Here the attack animation would play if there was one
        // Because there's not, we'll just trigger the attack manually for now

        // animator.SetBool("IsAttacking", true);
        ActivateAttack();
    }

    void PatrolState()
    {
        if (horizontalDirection == 0f)
            horizontalDirection = 1f;

        // If to the left of the left point, go right
        // If to the right of the right point, go left
        if (transform.position.x <= patrolZone.LeftPoint().x)
        {
            horizontalDirection = 1f;
        }
        else if (transform.position.x >= patrolZone.RightPoint().x)
        {
            horizontalDirection = -1f;
        }

        // Flip the direction of the enemy depending on direction
        // Facing right = -2 scale for x
        // Facing left = 2 scale for x
        if (horizontalDirection != 0f)
            transform.localScale = new Vector3(-2 * horizontalDirection,
                    transform.localScale.y,
                    transform.localScale.z);

        animator.SetBool("IsMoving", horizontalDirection != 0f);

        rb.velocity = new Vector2(moveVelocity * horizontalDirection, rb.velocity.y);

        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            detectionRange,
            enemyLayers);

        hostileDetected = hitObject != null;

        // Prioritize death
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (hostileDetected)
            EState = EntityState.Chase;
    }

    void ChaseState()
    {
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            detectionRange,
            enemyLayers);

        hostileDetected = hitObject != null;
        hostileInCloseRange = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            attackRange,
            enemyLayers) != null;

        // If detected enemy horizontal position is within the minimum amount (0.1f), don't switch directions
        // This prevents the enemy from rapidly switching directions
        // If detected enemy is to the left, go left
        // If detected enemy is to the right, go right
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

        // Flip the direction of the enemy depending on direction
        // Facing right = -2 scale for x
        // Facing left = 2 scale for x
        if (horizontalDirection != 0f)
            transform.localScale = new Vector3(-2 * horizontalDirection,
                    transform.localScale.y,
                    transform.localScale.z);

        animator.SetBool("IsMoving", horizontalDirection != 0f);

        rb.velocity = new Vector2(moveVelocity * horizontalDirection, rb.velocity.y);

        // Prioritize death
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

    void CloseAttackState()
    {
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            attackRange,
            enemyLayers);

        hostileInCloseRange = hitObject != null;

        // I use an offset of 1.4 to make it closer to upper body height
        if (hostileInCloseRange)
        {
            Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right * attackRange,
                Color.cyan);

            Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.left * attackRange,
                Color.cyan);

            RaycastHit2D enemyFrontRay, enemyBackRay;

            enemyFrontRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right,
                attackRange,
                enemyLayers);

            enemyBackRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.left,
                attackRange,
                enemyLayers);

            horizontalDirection = 0f;
            if (enemyFrontRay.collider != null && Time.time >= cooldownEndTime)
            {
                transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
                horizontalDirection = 0f;
                cooldownEndTime = Time.time + attackCooldown;
                RangedAttack();
            }
            else if (enemyBackRay.collider != null && Time.time >= cooldownEndTime)
            {
                transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
                cooldownEndTime = Time.time + attackCooldown;
                RangedAttack();
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

    private void OnDrawGizmosSelected()
    {
        // Purely for debugging purposes
        // This displays the radius of the detection circle
        // And the radius of the attack circle
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 1.3f, 0f), detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 1.3f, 0f), attackRange);
            
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        objectHealth = GetComponent<ObjectHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.drag = linearDrag;
    }

    // Update is called once per frame
    void Update()
    {
        switch (EState)
        {
            case EntityState.Patrol:
                PatrolState();
                break;
            case EntityState.Chase:
                ChaseState();
                break;
            case EntityState.CloseAttack:
                CloseAttackState();
                break;
            case EntityState.Dead:
                break;
            default:
                PatrolState();
                break;
        }
    }
}
