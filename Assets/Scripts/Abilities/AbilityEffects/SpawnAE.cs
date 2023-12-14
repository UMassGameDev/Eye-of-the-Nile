using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAE : AbilityEffect
{
    public AbilityProjectile projectilePrefab;
    public int objectCount = 1;
    // some spawn pattern variable eventually, or maybe in function

    public override void Apply(AbilityOwner abilityOwner)
    {
        for (int i = 0; i < objectCount; i++)
        {
            AbilityProjectile spawnedProjectile = Instantiate(projectilePrefab,
            abilityOwner.OwnerTransform.position, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
