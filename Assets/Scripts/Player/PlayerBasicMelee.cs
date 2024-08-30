using UnityEngine;

/** \deprecated
This was the original script that handled the player's basic melee attack. Now, this is handled by PlayerAttackManager.
Please use PlayerAttackManager instead of this script. It has been disconnected from the player object and will not work.

Documentation updated 8/30/2024
\author Stephen Nuttall
*/
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
