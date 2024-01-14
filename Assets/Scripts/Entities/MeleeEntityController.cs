using UnityEngine;

public class MeleeEntityController : BaseEntityController
{
    public float attackRange = 0.5f;  // range which melee attack does damage (size of attack point)
    public float knockback = 75f;  // knockback that the attack will apply
    public bool knockbackEnabled = true;  // apply knockback

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

    protected override void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }
}
