using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff AbilityEffect", menuName = "Ability Effects/Create New Buff AbilityEffect")]
public class BuffAE : AbilityEffect
{
    PlayerHealth playerHealth;
    public int buffValue;

    public override void Apply(AbilityOwner abilityOwner)
    {
        if (playerHealth == null)
            playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        playerHealth.HealInstant(buffValue);
    }

    void Awake()
    {
        AbilityEffectType = AbilityEffectType.Continuous;
    }
}
