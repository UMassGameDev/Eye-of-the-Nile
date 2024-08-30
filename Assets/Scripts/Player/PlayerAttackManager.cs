using System.Collections;
using UnityEngine;

/** \brief
Handles all the attacks the player can perform, including both standard attacks and all ability attacks.
This script contains multiple overloads of some functions to provide more functionality if desired.

Documentation updated 8/30/2024
\author Stephen Nuttall
\note If you're writing an ability info and you want to access one of these functions, you can reach them with
"abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().[function name goes here]"
*/
public class PlayerAttackManager : MonoBehaviour
{
    /// Reference to the PlayerStatsHolder, which is responsible for managing the player's modifiable stats.
    PlayerStatHolder playerStats;
    /// \brief Reference to the entity's attack point. It's a point in space that's a child of the entity, existing some distance in
    /// front of it. Projectiles spawn from the attack point, and melee attacks scan for enemies to damage from a certain radius around it.
    public Transform attackPoint;
    /// Objects on these layers which can be hit from the player's melee attacks.
    public LayerMask attackableLayers;
    /// Reference the the player's animator.
    public Animator animator;

    /// True if attacking is on cooldown, preventing another attack from being used.
    bool onCooldown = false;

    [Header("Default Melee Attack")]
    /** @name Default Melee Attack
    *  Information related to the player's default melee attack.
    */
    ///@{
    /// Key the user must press to activate this attack.
    public KeyCode meleeKey = KeyCode.Mouse0;
    /// Name of the animation to play 
    public string meleeAnimation = "Attack";
    /// The default distance from the attack point that enemies will be hit by the melee attack (size of attack point).
    public float meleeRange = 0.5f;
    /// Cooldown time for all attacks must wait after the melee attack is used.
    public float meleeCooldown = 1f;
    /// The default strength of the knockback the melee attack applies.
    public float meleeKnockback = 50f;
    /// \brief The melee range that will be used for the next attack.
    /// If no range parameter is given to Melee(), this will be set to meleeRange by default.
    /// \note The reason there need to be a separate variable is because TriggerMelee() is run by the animation at the peak of the
    /// sword swing, rather than being directly called. Therefore, it cannot have parameters passed to it like a normal function call.
    float curRange;
    /// \brief The melee knockback that will be used for the next attack.
    /// If no range parameter is given to Melee(), this will be set to meleeKnockback by default.
    /// \note The reason there need to be a separate variable is because TriggerMelee() is run by the animation at the peak of the
    /// sword swing, rather than being directly called. Therefore, it cannot have parameters passed to it like a normal function call.
    float curKnockback;
    ///@}

    [Header("Default Projectile Attack")]
    /** @name Default Projectile Attack
    *  Information related to the player's default projectile attack.
    */
    ///@{
    /// Key the user must press to activate this attack.
    public KeyCode projectileKey = KeyCode.Mouse1;
    /// Reference to the projectile this attack will instantiate.
    public GameObject defaultProjectilePrefab;
    /// Cooldown time for all attacks must wait after the projectile attack is used.
    public float ProjCooldown = 1f;
    ///@}

    /// Set reference to the PlayerStatsHolder.
    void Awake()
    {
        playerStats = GetComponent<PlayerStatHolder>();
    }

    /// Every frame, check if the user is pressing the melee key or projectile key.
    void Update()
    {
        if (Input.GetKeyDown(meleeKey))
            Melee(meleeAnimation, meleeCooldown);
        if (Input.GetKeyDown(projectileKey))
            ShootProjectile(defaultProjectilePrefab, ProjCooldown);
    }

    /// <summary>
    /// Sets up the variables for TriggerMelee(), and then triggers the attack animation and cooldown.
    /// </summary>
    /// <param name="animName">Name of the attack animation.</param>
    /// <param name="cooldown">Length of the attack cooldown after this ability is triggered.</param>
    public void Melee(string animName, float cooldown)
    {
        if (onCooldown)
            return;

        animator.SetTrigger(animName);
        curRange = meleeRange;
        curKnockback = meleeKnockback;
        StartCoroutine(CooldownWait(cooldown));
    }

    /// <summary>
    /// Sets up the variables for TriggerMelee(), and then triggers the attack animation and cooldown.
    /// This overload provides two extra parameters to customize the range and knockback.
    /// </summary>
    /// <param name="animName">Name of the attack animation.</param>
    /// <param name="cooldown">Length of the attack cooldown after this ability is triggered.</param>
    /// <param name="range">The distance from the attack point that enemies will be hit by the melee attack (size of attack point).</param>
    /// <param name="kbStrength">The strength of the knockback the melee attack will apply.</param>
    public void Melee(string animName, float cooldown, float range, float kbStrength)
    {
        if (onCooldown)
            return;

        animator.SetTrigger(animName);
        curRange = range;
        curKnockback = kbStrength;
        StartCoroutine(CooldownWait(cooldown));
    }

    /// This function is triggered by the melee animation at the peak of the sword swing. At that moment, the actual melee attack happens.
    void MeleeTrigger()
    {
        /// Scan for enemies in curRange radius of the attack point.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, curRange, attackableLayers);

        /// For each one found:
        foreach (Collider2D enemy in hitEnemies)
        {
            /// - Deal the amount of damage dictated by the PlayerStatsHolder (if it has an ObjectHealth component),
            if (enemy.TryGetComponent<ObjectHealth>(out var objHealth))
                objHealth.TakeDamage(transform, playerStats.GetValue("Damage"));

            /// - Apply knockback with curKnockback strength (if it has a KnockbackFeedback component),
            if (enemy.TryGetComponent<KnockbackFeedback>(out var kb))
                kb.ApplyKnockback(gameObject, curKnockback);
            
            /// - Trigger it's melee interaction (if it has a ObjectInteractable component).
            if (enemy.TryGetComponent<ObjectInteractable>(out var objInteractable))
                objInteractable.triggerMelee();
        }
    }

    /// <summary>
    /// Spawn a projectile from the attack point and set it facing the correct direction.
    /// No cooldown overload. This will not start a cooldown and will ignore the current cooldown
    /// </summary>
    /// <param name="projectilePrefab">Reference the projectile the attack will instantiate.</param>
    public void ShootProjectile(GameObject projectilePrefab)
    {
        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left (and this is a projectile object), flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0 && projectile.TryGetComponent<BasicProjectile>(out var basicProj)) {
            basicProj.FlipDirection();
        }
    }

    /// <summary>
    /// Spawn a projectile from the attack point and set it facing the correct direction.
    /// Then start the attack cooldown.
    /// </summary>
    /// <param name="projectilePrefab">Reference the projectile the attack will instantiate.</param>
    /// <param name="cooldown">Length of the attack cooldown after this ability is triggered.</param>
    public void ShootProjectile(GameObject projectilePrefab, float cooldown)
    {
        if (onCooldown)
            return;

        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left (and this is a projectile object), flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0 && projectile.TryGetComponent<BasicProjectile>(out var basicProj)) {
            basicProj.FlipDirection();
        }

        // start cooldown
        StartCoroutine(CooldownWait(cooldown));
    }

    /// <summary>
    /// Will start a coroutine for ProjectileBurst, which F\fires a projectile numProjectiles times, wtih delay seconds in between.
    /// </summary>
    /// <param name="projectilePrefab">Reference the projectile the attack will instantiate.</param>
    /// <param name="numProjectiles">Number of projectiles to spawn.</param>
    /// <param name="delay">How much time between each projectile spawning, in seconds.</param>
    public void ShootProjectileBurst(GameObject projectilePrefab, int numProjectiles, float delay)
    {
        StartCoroutine(ProjectileBurst(projectilePrefab, numProjectiles, delay));
    }

    /// <summary>
    /// Prevents any attacks that observe the attack cooldown from triggering for the given amount of seconds.
    /// </summary>
    /// <param name="seconds">How long the cooldown should last, in seconds.</param>
    IEnumerator CooldownWait(float seconds)
    {
        onCooldown = true;
        yield return new WaitForSeconds(seconds);
        onCooldown = false;
    }

    /// <summary>
    /// Fires a projectile numProjectiles times, wtih delay seconds in between.
    /// </summary>
    /// <param name="projectilePrefab">Reference the projectile the attack will instantiate.</param>
    /// <param name="numProjectiles">Number of projectiles to spawn.</param>
    /// <param name="delay">How much time between each projectile spawning, in seconds.</param>
    IEnumerator ProjectileBurst(GameObject projectilePrefab, int numProjectiles, float delay)
    {
        for (int i = 0; i < numProjectiles; i++)
        {
            ShootProjectile(projectilePrefab);
            yield return new WaitForSeconds(delay);
        }
    }
}
