using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeMeleeRangedEntity : PrototypeBaseEntity
{
    public BasicProjectile projectilePrefab;

    public virtual void TestActivate()
    {
        ActivateAttack();
    }

    protected override void ActivateAttack()
    {
        base.ActivateAttack();

        BasicProjectile projectile = Instantiate(projectilePrefab, new Vector2(attackPoint.position.x, attackPoint.position.y), Quaternion.identity);

        // if we're facing left, flip the direction (projectile faces right by default)
        if (transform.localScale.x > 0)
        {
            projectile.GetComponent<BasicProjectile>().FlipDirection();
        }
    }
}
