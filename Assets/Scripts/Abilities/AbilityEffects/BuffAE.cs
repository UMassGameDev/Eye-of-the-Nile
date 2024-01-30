/**************************************************
Allows an ability to apply a permanent stat increase to the player.
Right now, this script only supports increasing the player's max health.
This is a scriptable object, meaning you can make and instance of it in the editor.

Documentation updated 1/29/2024
**************************************************/
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
