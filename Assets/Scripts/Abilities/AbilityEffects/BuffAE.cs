/**************************************************
Allows an ability to apply a permanent stat increase to the player.
Right now, this script only supports increasing the player's max health.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/13/2024
**************************************************/
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff AbilityEffect", menuName = "Ability Effects/Create New Buff AbilityEffect")]
public class BuffAE : AbilityEffect
{
    PlayerHealth playerHealth;  // Reference to the player’s health.
    public int buffValue;  // The amount the player’s health should be buffed by.

    // Instantly heal the player by the buffValue.
    public override void Apply(AbilityOwner abilityOwner)
    {
        if (playerHealth == null)
            playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        playerHealth.HealInstant(buffValue);
    }

    // Set AbilityEffectType to continuous.
    void Awake()
    {
        AbilityEffectType = AbilityEffectType.Continuous;
    }
}
