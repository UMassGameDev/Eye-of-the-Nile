using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    ObjectHealth objectHealth;

    public PatrolZone patrolZone;
    public EntityState EState { get; set; } = EntityState.Patrol;
    public float horizontalDirection = 0f;

    Rigidbody2D rb;
    public float moveVelocity = 8.0f;
    public float linearDrag = 1.0f;
    public float groundedRaycastLength = 1.8f;
    bool hostileInRange = false;

    Animator animator;

    void Patrol()
    {
        if (horizontalDirection == 0f)
            horizontalDirection = 1f;

        animator.SetBool("IsMoving", horizontalDirection != 0f);

        rb.velocity = new Vector2(moveVelocity * horizontalDirection, rb.velocity.y);

        if (transform.position.x <= patrolZone.LeftPoint().x)
        {
            horizontalDirection = 1f;
            transform.localScale = new Vector3(-2,
                transform.localScale.y,
                transform.localScale.z);
        }
        else if (transform.position.x >= patrolZone.RightPoint().x)
        {
            horizontalDirection = -1f;
            transform.localScale = new Vector3(2,
                transform.localScale.y,
                transform.localScale.z);
        }

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


        if (!hostileInRange)
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
