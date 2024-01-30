/**************************************************
Handles all the attacks the player can perform, including both standard attacks and all ability attacks.
This script contains multiple overloads of some functions to provide more functionality if desired.
Abilities can access these functions via the "abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>()".

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    PlayerStatHolder playerStats;
    public Transform attackPoint;
    public LayerMask attackableLayers;
    public Animator animator;

    bool onCooldown = false;

    [Header("Default Melee Attack")]
    public KeyCode meleeKey = KeyCode.Mouse0;
    public string meleeAnimation = "Attack";
    public float meleeRange = 0.5f;
    public float meleeCooldown = 1f;
    public float meleeKnockback = 50f;
    float curRange;
    float curKnockback;

    [Header("Default Projectile Attack")]
    public KeyCode projectileKey = KeyCode.Mouse1;
    public GameObject defaultProjectilePrefab;
    public float ProjCooldown = 1f;

    void Awake()
    {
        playerStats = GetComponent<PlayerStatHolder>();
    }

    void Update()
    {
        if (Input.GetKeyDown(meleeKey))
            Melee(meleeAnimation, meleeCooldown);
        if (Input.GetKeyDown(projectileKey))
            ShootProjectile(defaultProjectilePrefab, ProjCooldown);
    }

    public void Melee(string animName, float cooldown)
    {
        if (onCooldown)
            return;

        animator.SetTrigger(animName);
        curRange = meleeRange;
        curKnockback = meleeKnockback;
        StartCoroutine(CooldownWait(cooldown));
    }

    public void Melee(string animName, float cooldown, float range, float kbStrength)
    {
        if (onCooldown)
            return;

        animator.SetTrigger(animName);
        curRange = range;
        curKnockback = kbStrength;
        StartCoroutine(CooldownWait(cooldown));
    }

    // no cooldown overload. This will not start a cooldown and will ignore the current cooldown
    public void ShootProjectile(GameObject projectilePrefab)
    {
        // create projectile object
        GameObject projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left (and this is a projectile object), flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0 && projectile.TryGetComponent<BasicProjectile>(out var basicProj)) {
            basicProj.FlipDirection();
        }
    }

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

    public void ShootProjectileBurst(GameObject projectilePrefab, int numProjectiles, float delay)
    {
        StartCoroutine(ProjectileBurst(projectilePrefab, numProjectiles, delay));
    }

    // This function is triggered by the animation (when the sword swings)
    void MeleeTrigger()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, curRange, attackableLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<ObjectHealth>().TakeDamage(transform, playerStats.GetValue("Damage"));
            if (enemy.TryGetComponent<KnockbackFeedback>(out var kb))
                kb.ApplyKnockback(gameObject, curKnockback);
        }
    }

    IEnumerator CooldownWait(float seconds)
    {
        onCooldown = true;
        yield return new WaitForSeconds(seconds);
        onCooldown = false;
    }

    IEnumerator ProjectileBurst(GameObject projectilePrefab, int numProjectiles, float delay)
    {
        for (int i = 0; i < numProjectiles; i++)
        {
            ShootProjectile(projectilePrefab);
            yield return new WaitForSeconds(delay);
        }
    }
}
