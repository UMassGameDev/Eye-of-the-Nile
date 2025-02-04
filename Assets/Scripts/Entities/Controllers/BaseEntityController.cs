using System;
using System.Collections;
using UnityEngine;

/*! \brief
Basic functionality for any entities that are hostile towards other entities.
This script is abstract so it must be inherited by another script to be used.
Every script inheriting from this must override ActivateAttack().

- Patrol State (default): The entity will patrol an area (PatrolZone.cs) until an enemy (something on the enemyLayers) is within range.
- Chase State: The entity will chase the enemy it detected until it's close enough to attack.
- CloseAttack State: The entity will attack the enemy until it's no longer in range (either because it left or because it died)
- Dead State: This script won't do anything if the entity dies.

Documentation updated 11/9/2024
\author Roy Rapscual, Stephen Nuttal
*/
public abstract class BaseEntityController : MonoBehaviour
{
    /// Reference to the object responsible for managing the player's health
    protected ObjectHealth objectHealth;

    /// \brief Reference to the entity's attack point. It's a point in space that's a child of the entity, existing some distance in front of it.
    /// Projectiles spawn from the attack point, and melee attacks scan for enemies to damage from a certain radius around it.
    [SerializeField] protected Transform attackPoint;
    /// Amount of damage the entity's attack will deal.
    [SerializeField] protected int attackDamage = 30;
    /// The amount of time between attacks.
    [SerializeField] protected float attackCooldown = 0.8f;
    /// Amount of time until cooldown is over. Set to the current time + attackCooldown when the attack is triggered.
    protected float cooldownEndTime = 0f;

    /// \brief Objects on these layers will be considered an enemy of this entity, and if detected, this entity will seek to attack. 
    /// An object can be assigned to a layer in the Unity Editor from a drop down menu in the top right.
    [SerializeField] protected LayerMask enemyLayers;
    /// A patrol zone is an object that has two points the entity will walk between if it does not detect an enemy.
    [SerializeField] protected PatrolZone patrolZone;
    /// Current state the entity is in.
    public EntityState EState { get; protected set; } = EntityState.Patrol;
    /// Horizontal direction the entity is traveling in.
    [SerializeField] protected float horizontalDirection = 0f;
    /// How far away the entity will detect an enemy from and start chasing.
    [SerializeField] protected float detectionRange = 6f;
    /// How close the entity must be to an enemy to attack it.
    [SerializeField] protected float activateAttackRange = 3f;
    /// True if the entity has detected an enemy.
    protected bool hostileDetected = false;
    /// True if the entity is close enough to an enemy to attack it.
    protected bool hostileInCloseRange = false;

    /// Reference to the entity's rigidbody.
    protected Rigidbody2D rb;
    /// How fast the entity should move.
    [SerializeField] protected float moveVelocity = 6.0f;
    /// The precent of moveVelocity the entity should move at. Used for slowing the entity down.
    protected float speedModifier = 0f;
    /// Amount of drag to apply to the rigidbody.
    [SerializeField] protected float linearDrag = 1.0f;
    /// \deprecated Unused variable that was going to allow entities to jump, based on how player jumping used to work.
    /// This system has been replaced with a new system in PlayerMovement though, so that system should be implemented instead.
    /// \todo Implement jumping for entities.
    [SerializeField] protected float groundedRaycastLength = 1.8f;
    /// Reference to the entity's GroundDetector.
    protected GroundDetector groundDetector;
    /// Can be set to false to let the entity walk even when it is not on the ground.
    [SerializeField] protected bool groundNeeded = true;
    /// Used for disabling entity movement.
    protected bool canWalk = true;

    /// Reference to the entity's animator.
    protected Animator animator;

    /// Apply linear drag to the rigidbody.
    void Start()
    {
        rb.drag = linearDrag;
    }

    /// Activate the logic for the current state for this frame, based on EState.
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

    /// <summary>
    /// Triggered by an event in the attack animation
    /// (or you can override TriggerAttack() if there is not attack animation).
    /// This function muse be overridden by any inheriting class.
    /// </summary>
    protected abstract void ActivateAttack();

    /// <summary>
    /// Triggers the entity's attack. This can be done through triggering an animation, spawning a projectile, or another method.
    /// By default, an "IsAttacking" animation is triggered.
    /// </summary>
    protected virtual void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }

    /// \brief When the entity hasn't detected any enemies...
    /// 1. Move towards one of the patrol zone's points. If the entity reaches one, go to the other one.
    /// 2. Flip the sprite to face the direction the entity is facing.
    /// 3. Run movement animation and set velocity to proper direction and speed.
    /// 4. Scan for enemies within the detection range.
    /// 5. Evaluate state
    ///     - If the entity has died, set state to Dead.
    ///     - If an enemy was detected, set state to Chase.
    ///     - Otherwise, don't change state. Allow Patrol state to continue.
    ///     
    /// \pre Active when there are no objects on enemyLayers within detectionRange.
    protected virtual void PatrolState()
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

        // If the enemy is not on the ground, but needs the ground to walk, then don't let the enemy move
        // When there is no groundDetector, the enemy is assumed to be on the ground
        if ((groundDetector != null && !groundDetector.isGrounded) && groundNeeded)
        {
            canWalk = false;
        }
        else
        {
            canWalk = true;
        }

        rb.velocity = new Vector2((moveVelocity + speedModifier) * horizontalDirection * Convert.ToInt32(canWalk), rb.velocity.y);

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

    /// <summary>
    /// If the entity has detected an enemy...
    /// 1. Scan for enemies within the detection range.
    /// 2. Scan for enemies close enough to attack.
    /// 3. Face the entity towards the enemy and flip the sprite to face the new direction.
    /// 4. Run movement animation and set velocity to proper direction and speed.
    /// 5. Evaluate state
    ///     - If the entity has died, set state to Dead.
    ///     - If an enemy is close to attack, set state to CloseAttack
    ///     - If there's no longer an enemy within the detection range, set state to Patrol
    ///     - Otherwise, don't change state. Allow Chase state to continue.
    /// </summary>
    /// \pre The entity has detected an enemy in its detection range.
    protected virtual void ChaseState()
    {
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            detectionRange,
            enemyLayers);

        hostileDetected = hitObject != null;
        hostileInCloseRange = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            activateAttackRange,
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

        // If the enemy is not on the ground, but needs the ground to walk, then don't let the enemy move
        // When there is no groundDetector, the enemy is assumed to be on the ground
        if ((groundDetector != null && !groundDetector.isGrounded) && groundNeeded)
        {
            canWalk = false;
        }
        else
        {
            canWalk = true;
        }

        rb.velocity = new Vector2((moveVelocity + speedModifier) * horizontalDirection * Convert.ToInt32(canWalk), rb.velocity.y);

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

    /// <summary>
    /// If the entity is close enough to an enemy to attack...
    /// 1. Scan for enemies close enough to attack.
    /// 2. If an enemy is close enough, face the entity towards it and trigger attack.
    /// 3. Stop movement animation if it's still happening.
    /// 4. Evaluate state
    ///     - If the entity has died, set state to Dead.
    ///     - If no enemies are close enough to attack, set state to Chase.
    ///     - Otherwise, don't change state. Allow CloseAttack state to continue.
    /// </summary>
    /// \pre The entity is close enough to an enemy to attack it.
    /// \todo Make 1.4 offest a variable rather than hard coded.
    protected virtual void CloseAttackState()
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

            if (enemyFrontRay.collider != null && Time.time >= cooldownEndTime)
            {
                horizontalDirection = 1f;
                cooldownEndTime = Time.time + attackCooldown;
                TriggerAttack();
            }
            else if (enemyBackRay.collider != null && Time.time >= cooldownEndTime)
            {
                horizontalDirection = -1f;
                cooldownEndTime = Time.time + attackCooldown;
                TriggerAttack();
            }
            else
            {
                horizontalDirection = 0f;
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

    /// \brief Displays radiuses of the detection and attack circles in the Unity Editor, specifically the scene view.
    /// This allows developers to see how far away an entity can see an enemy and how far way they will attack the enemy.
    /// \important Must be commented out or removed to export the game. Otherwise, Unity will throw compiler errors.
    /// \todo Make 1.3 offest a variable rather than hard coded.
    private void OnDrawGizmosSelected()
    {
        // Purely for debugging purposes
        // This displays the radius of the detection circle
        // And the radius of the attack circle
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 1.3f, 0f), detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 1.3f, 0f), activateAttackRange);
            
    }

    /// Set references to rigidbody, animator, object health, and GroundDetector.
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        objectHealth = GetComponent<ObjectHealth>();
        groundDetector = transform.GetComponentInChildren<GroundDetector>();
    }

    /// <summary>
    /// Speeds up or slows down the entity's movement speed. A negative value for speedChange will slow down the entity.
    /// </summary>
    /// \note If speedChange is less than negative moveVelocity (implying the entity should move backwards), it will be set to -moveVelocity.
    /// <param name="speedChange">The amount that will be added to speedModifier, which will be added to moveVelocity.</param>
    public void ChangeSpeed(float speedChange)
    {
        speedModifier += speedChange;
        if (speedModifier < -moveVelocity)
            speedModifier = -moveVelocity;
    }
    
    /// <summary>
    /// Temporarily speeds up or slows down the entity's movement speed. A negative value for speedChange will slow down the entity.
    /// </summary>
    /// \note If speedChange is less than negative moveVelocity (implying the entity should move backwards), it will be set to -moveVelocity.
    /// <param name="speedChange">The amount that will be added to speedModifier, which will be added to moveVelocity.</param>
    /// <param name="duration">The amount of time, in seconds, the speed change will last for.</param>
    public void ChangeSpeed(float speedChange, float duration)
    {
        StartCoroutine(ChangeSpeedClock(speedChange, duration));
    }

    /// Clock for ChangeSpeed(). See ChangeSpeed() for details.
    IEnumerator ChangeSpeedClock(float speedChange, float duration)
    {
        speedModifier += speedChange;
        if (moveVelocity + speedModifier < 0)
            speedModifier = 0;
            
        yield return new WaitForSeconds(duration);
        speedModifier -= speedChange;
    }

}

