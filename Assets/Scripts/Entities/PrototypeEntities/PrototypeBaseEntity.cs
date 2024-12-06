using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

public class PrototypeBaseEntity : MonoBehaviour
{
    // Base info of the Entity stored in a ScriptableObject
    public PrototypeEntityInfo baseInfo;
    public EntityState EState { get; set; } = EntityState.Patrol;
    public PatrolZoneProto patrolZonePrefab;
    public PatrolZoneProto CurrentPatrolZone { get; set; }
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    public Transform attackPoint;

    // Entities (for initialization)
    public ObjectHealth objectHealth;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected float horizontalDirection;

    protected Collider2D currentHitObject;

    protected PrototypeEntityChecks entityBools;
    // protected AttackDeadzone currentDeadzone;
    public int attackVariant = 0;
    float cooldownEndTime = 0f;

    // Must work on new NPC AI system
    // For now, the goal for this prototype is to
    // Have the mushroom guy spawn smaller variants of himself
    // He will also attack if the player gets close
    // To reiterate, this is the expected behavior
    // Movement:
    // Mushroom stands still
    // Attack:
    // Mushroom attacks when player enters range
    // Spawning:
    // Mushroom spawns smaller mushrooms when player is in sight

    //
    // DETECTION
    //

    protected virtual void DetectHostile()
    {
        // The 1.3f offset moves this to the center
        currentHitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            baseInfo.detectionRange,
            baseInfo.enemyLayers);

        entityBools.hostileDetected = currentHitObject != null;

        if (entityBools.hostileDetected)
            entityBools.hostileInCloseRange =
                Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
                baseInfo.activateAttackRange,
                baseInfo.enemyLayers) != null;
        else
            entityBools.hostileInCloseRange = false;
    }

    protected virtual void ScanAttackRange()
    {
        Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right * baseInfo.activateAttackRange,
                Color.cyan);

        Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
            Vector2.left * baseInfo.activateAttackRange,
            Color.cyan);

        RaycastHit2D enemyFrontRay, enemyBackRay;

        enemyFrontRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
            Vector2.right,
            baseInfo.activateAttackRange,
            baseInfo.enemyLayers);

        enemyBackRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
            Vector2.left,
            baseInfo.activateAttackRange,
            baseInfo.enemyLayers);

        entityBools.enemyIsForward = enemyFrontRay.collider != null;
        entityBools.enemyIsBackward = enemyBackRay.collider != null;
        // Debug.Log(entityBools.enemyIsForward);
        // Debug.Log(entityBools.enemyIsBackward);
    }

    // TODO: need to fix this in some way
    protected virtual void ScanAttackRangeMultiple()
    {
        Vector2 pos2D = transform.position;

        //bool inRange = false;

        for (int i = 0; i < baseInfo.attackRanges.Count; i++)
        {
            if (Mathf.Abs(currentHitObject.transform.position.x - pos2D.x) <=
                baseInfo.attackRanges[i])
            {
                attackVariant = i;
                //inRange = true;
                break;
            }
        }
    }

    /*protected virtual void ScanAttackRangeMultiple()
    {
        Vector2 pos2D = transform.position;

        bool inDeadzone = false;
        AttackDeadzone foundDeadzone = null;

        foreach (AttackDeadzone aD in baseInfo.attackDeadZones)
        {
            inDeadzone = ScanDeadzone(pos2D, aD);
            if (inDeadzone)
            {
                foundDeadzone = aD;
                break;
            }
        }

        entityBools.hostileInDeadzone = inDeadzone;
        currentDeadzone = foundDeadzone;
        
    }*/

    /*protected virtual bool ScanDeadzone(Vector2 pos2D, AttackDeadzone aD)
    {
        if ( Mathf.Abs(currentHitObject.transform.position.x - pos2D.x) <= aD.upperBound &&
            aD.lowerBound <= Mathf.Abs(currentHitObject.transform.position.x - pos2D.x) )
        {
            return true;
        }
        return false;
    }*/

    //
    // MOVEMENT AND POSITIONING
    //

    protected virtual void FlipSprite(float flipDirection)
    {
        transform.localScale = new Vector3(-1f * flipDirection * baseInfo.xScale,
                    transform.localScale.y,
                    transform.localScale.z);
    }

    protected virtual void MoveLeft()
    {
        horizontalDirection = -1f;
        FlipSprite(horizontalDirection);
        rb.velocity = new Vector2(baseInfo.moveVelocity * horizontalDirection, rb.velocity.y);
        animator.SetBool("IsMoving", true);
    }

    protected virtual void MoveRight()
    {
        horizontalDirection = 1f;
        FlipSprite(horizontalDirection);
        rb.velocity = new Vector2(baseInfo.moveVelocity * horizontalDirection, rb.velocity.y);
        animator.SetBool("IsMoving", true);
    }

    protected virtual void MaintainMovement()
    {
        FlipSprite(horizontalDirection);
        rb.velocity = new Vector2(baseInfo.moveVelocity * horizontalDirection, rb.velocity.y);
        animator.SetBool("IsMoving", true);
    }

    protected virtual void StandStill()
    {
        horizontalDirection = 0f;
        rb.velocity = new Vector2(baseInfo.moveVelocity * horizontalDirection, rb.velocity.y);
        animator.SetBool("IsMoving", false);
    }

    //
    // ATTACKING
    //

    protected virtual void ActivateAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, baseInfo.attackSize, baseInfo.enemyLayers);

        foreach (Collider2D hitObject in hitObjects)
        {
            hitObject.GetComponent<PlayerHealth>().TakeDamage(transform, baseInfo.attackDamage);
        }
    }

    protected virtual void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }



    protected virtual void StateCheck(EntityState eState)
    {
        switch(eState)
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

    protected virtual void InitializePatrolZone()
    {
        CurrentPatrolZone = Instantiate(patrolZonePrefab, transform.position, Quaternion.identity);
        CurrentPatrolZone.transform.parent = transform;
        CurrentPatrolZone.SpawnBasePos = transform.position;
        CurrentPatrolZone.leftEndOffset = leftOffset;
        CurrentPatrolZone.rightEndOffset = rightOffset;
        CurrentPatrolZone.spawnedIn = true;
    }

    protected virtual void PatrolState()
    {
        if (horizontalDirection == 0f)
            horizontalDirection = 1f;

        // If to the left of the left point, go right
        // If to the right of the right point, go left

        if (transform.position.x <= CurrentPatrolZone.LeftPoint().x)
        {
            MoveRight();
        }
        else if (transform.position.x >= CurrentPatrolZone.RightPoint().x)
        {
            MoveLeft();
        }
        else
        {
            MaintainMovement();
        }

        DetectHostile();

        // Prioritize death
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (entityBools.hostileDetected)
            EState = EntityState.Chase;
    }

    protected virtual void ChaseState()
    {
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        DetectHostile();

        // If detected enemy horizontal position is within the minimum amount (0.1f), don't switch directions
        // This prevents the enemy from rapidly switching directions
        // If detected enemy is to the left, go left
        // If detected enemy is to the right, go right
        if (entityBools.hostileDetected && Mathf.Abs(currentHitObject.transform.position.x - transform.position.x) < 0.1f)
        {
            StandStill();
        }
        else if (entityBools.hostileDetected && currentHitObject.transform.position.x < transform.position.x)
        {
            MoveLeft();
        }
        else if (entityBools.hostileDetected && currentHitObject.transform.position.x > transform.position.x)
        {
            MoveRight();
        }
        else
        {
            StandStill();
        }

        // Prioritize death
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (entityBools.hostileInCloseRange)
        {
            EState = EntityState.CloseAttack;
        }
        else if (!entityBools.hostileDetected)
            EState = EntityState.Patrol;
    }
    
    
    protected virtual void CloseAttackState()
    {
        DetectHostile();

        // I use an offset of 1.4 to make it closer to upper body height
        if (entityBools.hostileInCloseRange)
        {
            ScanAttackRange();

            if (entityBools.enemyIsForward && Time.time >= cooldownEndTime)
            {
                horizontalDirection = 1f;
                cooldownEndTime = Time.time + baseInfo.attackCooldown;
                TriggerAttack();
            }
            else if (entityBools.enemyIsBackward && Time.time >= cooldownEndTime)
            {
                horizontalDirection = -1f;
                cooldownEndTime = Time.time + baseInfo.attackCooldown;
                TriggerAttack();
            }
            else if (Time.time >= cooldownEndTime)
            {
                horizontalDirection = 0f;
                animator.SetBool("IsAttacking", false);
            }

            if (horizontalDirection != 0f)
                FlipSprite(horizontalDirection);

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
        else if (!entityBools.hostileInCloseRange && Time.time >= cooldownEndTime)
            EState = EntityState.Chase;
    }

    protected virtual void ProtoCloseAttackState()
    {
        DetectHostile();

        // I use an offset of 1.4 to make it closer to upper body height
        if (entityBools.hostileDetected)
        {
            ScanAttackRange();
            //ScanAttackRangeMultiple();

            if (!entityBools.hostileInDeadzone)
            {
                // TODO: manage movement with deadzones in consideration
            }

            if (horizontalDirection != 0f)
                FlipSprite(horizontalDirection);

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
        else if (!entityBools.hostileInCloseRange && Time.time >= cooldownEndTime)
            EState = EntityState.Chase;
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
            if (Selection.activeTransform == transform && CurrentPatrolZone == null)
            {
                Vector2 basePos = transform.position;
                Gizmos.DrawIcon(basePos + leftOffset, "sv_icon_dot14_pix16_gizmo", true, Color.green);
                Gizmos.DrawIcon(basePos + rightOffset, "sv_icon_dot14_pix16_gizmo", true, Color.green);
            }
        #endif
    }

    // To avoid repetition of methods in Awake, Start, Update,
    // Child classes need only override the corresponding
    // [Awake/Start/Update]Methods method

    private void Awake()
    {
        AwakeMethods();
    }

    protected virtual void AwakeMethods()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        objectHealth = GetComponent<ObjectHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMethods();
    }

    protected virtual void StartMethods()
    {
        entityBools = new PrototypeEntityChecks();
        if (CurrentPatrolZone == null && patrolZonePrefab != null)
        {
            InitializePatrolZone();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMethods();
    }

    protected virtual void UpdateMethods()
    {
        StateCheck(EState);
    }
}

public class PrototypeEntityChecks
{
    /*public Dictionary<string, bool> ChecksDict { get; set; } = new Dictionary<string, bool>();

    public PrototypeEntityChecks(List<string> conditionList)
    {
        foreach (string conditionName in conditionList)
        {
            ChecksDict[conditionName] = false;
        }
    }

    public bool CheckCondition(string conditionName)
    {
        return ChecksDict[conditionName];
    }

    public void SetCondition(string conditionName, bool conditionBool)
    {
        ChecksDict[conditionName] = conditionBool;
    }*/

    public bool hostileDetected = false;
    public bool hostileInCloseRange = false;
    public bool hostileInDeadzone = false;
    public bool enemyIsForward = false;
    public bool enemyIsBackward = false;
}

/*public class PrototypeStateManager
{
    public Dictionary<EntityState, List<EntityState>> connectedStates = new Dictionary<EntityState, List<EntityState>>
    {
        { EntityState.Patrol, new List<EntityState> { EntityState.Patrol, EntityState.Dead }  },
        { EntityState.Chase, new List<EntityState> { EntityState.Patrol, EntityState.CloseAttack } },
        { EntityState.CloseAttack, new List<EntityState> { EntityState. } },
        { EntityState.Dead, new List<EntityState> {  }  }
    }

    // these conditions must be true to make the switch
    public Dictionary<EntityState, List<string>> trueConditions = new Dictionary<EntityState, List<string>>
    {
        { EntityState.Patrol, new List<string> { "hostileDetected" }  },
        { EntityState.Chase, new List<string> { "hostileDetected" } },
        { EntityState.CloseAttack, new List<string> { "hostileDetected" } },
        { EntityState.Dead, new List<string> { "hostileDetected" }  }
    };
    public Dictionary<EntityState, List<string>> falseConditions = new Dictionary<EntityState, List<string>>
    {
        { EntityState.Patrol, new List<string> { "hostileDetected" }  },
        { EntityState.Chase, new List<string> { "hostileDetected" } },
        { EntityState.CloseAttack, new List<string> { "hostileDetected" } },
        { EntityState.Dead, new List<string> { "hostileDetected" }  }
    };
}*/