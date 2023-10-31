using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    ObjectHealth objectHealth;

    // This LayerMask includes the Player's layer so the enemy is alerted
    public LayerMask enemyLayers;
    public PatrolZone patrolZone;
    public EntityState EState { get; set; } = EntityState.Patrol;
    public float horizontalDirection = 0f;
    public float detectionRange = 5f;
    bool hostileInRange = false;

    Rigidbody2D rb;
    public float moveVelocity = 6.0f;
    public float linearDrag = 1.0f;
    public float groundedRaycastLength = 1.8f;

    Animator animator;

    void Patrol()
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

        Collider2D hitObject = Physics2D.OverlapCircle(transform.position,
            detectionRange,
            enemyLayers);

        hostileInRange = hitObject != null;

        // Prioritize death
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (hostileInRange)
            EState = EntityState.Chase;
    }

    void Chase()
    {
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position,
            detectionRange,
            enemyLayers);

        hostileInRange = hitObject != null;

        // If detected enemy is to the left, go left
        // If detected enemy is to the right, go right
        if (hostileInRange && hitObject.transform.position.x < transform.position.x)
        {
            horizontalDirection = -1f;
        }
        else if (hostileInRange && hitObject.transform.position.x > transform.position.x)
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
        else if (!hostileInRange)
            EState = EntityState.Patrol;
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
                Patrol();
                break;
            case EntityState.Chase:
                Chase();
                break;
            case EntityState.Dead:
                break;
            default:
                Patrol();
                break;
        }
    }
}
