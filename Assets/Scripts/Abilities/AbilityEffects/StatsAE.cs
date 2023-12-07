using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats AbilityEffect", menuName = "Abilities/Create New Stats AbilityEffect")]
public class StatsAE : AbilityEffect
{
    public string affectedStat;
    public int magnitude;

    public override void Apply(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().HealInstant(magnitude);
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
