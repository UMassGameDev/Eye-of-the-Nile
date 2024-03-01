using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeMinionEntity : PrototypeBaseEntity
{
    public PrototypeSpawnerEntity ParentEntity { get; set; }
    bool checkDeadFlag = false;
    
    public virtual void ResetValues()
    {
        checkDeadFlag = false;
        objectHealth.ResetHealth();
        animator.SetBool("IsDead", false);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;
        objectHealth.enabled = true;
        EState = EntityState.Patrol;
        animator.Play("Mushroom_Idle");
    }

    protected override void AwakeMethods()
    {
        base.AwakeMethods();
    }

    protected override void StartMethods()
    {
        base.StartMethods();
    }

    protected override void UpdateMethods()
    {
        base.UpdateMethods();
        if (objectHealth.IsDead && !checkDeadFlag)
        {
            checkDeadFlag = true;
            ParentEntity.LivingMinions--;
        }
    }
}
