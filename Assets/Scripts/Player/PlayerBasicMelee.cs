/**************************************************
--- DEPRECATED ---
Please use PlayerAttackManager.cs

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class PlayerBasicMelee : MonoBehaviour
{
    PlayerStatHolder playerStats;
    public Transform attackPoint;
    public LayerMask attackableLayers;
    public Animator animator;

    public float attackRange = 0.5f;
    // public int attackDamage = 20;
    public float attackCooldown = 1f;
    
    float cooldownTimer = 0f;

    void Awake()
    {
        playerStats = GetComponent<PlayerStatHolder>();
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime; // update attack cooldown timer

        // if left click is pressed and the cooldown timer has expired...
        if (Input.GetKeyDown(KeyCode.Mouse0) && cooldownTimer >= attackCooldown)
        {
            Attack();
            cooldownTimer = 0; // reset cooldown timer
        }
    }

    void ActivateAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackableLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<ObjectHealth>().TakeDamage(transform, playerStats.GetValue("Damage"));
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
    }

    // show a wire sphere in the edit for the attack range
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
