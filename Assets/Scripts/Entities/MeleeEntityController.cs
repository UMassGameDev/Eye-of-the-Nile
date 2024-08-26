using UnityEngine;

/** \brief
Functionality for basic entities with a melee attack.
Inherits from BaseEntityController.

Documentation updated 8/26/2024
*/
public class MeleeEntityController : BaseEntityController
{
    /// The size of the area the attack will search for enemies in (size of attack point).
    public float attackRange = 0.5f;  
    /// The strength of the knockback that the attack will apply.
    public float knockback = 75f;  
    /// If true, the melee attack will apply knockback to the attacked object.
    public bool knockbackEnabled = true;  

    /// Scan the attack range for enemies, and apply damage and knockback (if enabled)
    protected override void ActivateAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D hitObject in hitObjects)
        {
            hitObject.GetComponent<PlayerHealth>().TakeDamage(transform, attackDamage);
            if (knockbackEnabled && hitObject.TryGetComponent<KnockbackFeedback>(out var kb))
                kb.ApplyKnockback(gameObject, knockback);
        }
    }

    /// Trigger attacking animation
    protected override void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }
}
