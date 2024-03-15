using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeSpawnerEntity : PrototypeBaseEntity
{
    public PrototypeMinionEntity minionPrefab;
    List<PrototypeMinionEntity> minionPool = new List<PrototypeMinionEntity>();
    Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);
    float minionLimit = 4;
    public float LivingMinions { get; set; } = 0;
    public float spawnCooldown = 3f;
    float spawnCooldownEndTime = 0f;

    protected virtual void SpawnEnemy()
    {
        if (minionPool.Count < minionLimit)
        {
            PrototypeMinionEntity newMinion = Instantiate(minionPrefab,
                transform.position + spawnOffset,
                Quaternion.identity);
            newMinion.ParentEntity = this;
            minionPool.Add(newMinion);
            LivingMinions++;
        }
        else
        {
            // Take dead minion from pool
            // Reactivate
            // Reset values
            PrototypeMinionEntity deadMinion = null;
            foreach (PrototypeMinionEntity minion in minionPool)
            {
                if (minion.objectHealth.IsDead)
                {
                    deadMinion = minion;
                    break;
                }
            }

            if (deadMinion != null)
            {
                deadMinion.ResetValues();
                deadMinion.transform.position = transform.position + spawnOffset;
                deadMinion.gameObject.SetActive(true);
                LivingMinions++;
            }
        }
    }

    protected override void ChaseState()
    {
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        DetectHostile();

        // Stand still and spawn enemies
        StandStill();

        if (LivingMinions < minionLimit && Time.time > spawnCooldownEndTime)
        {
            spawnCooldownEndTime = Time.time + spawnCooldown;
            SpawnEnemy();
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
    }
}
