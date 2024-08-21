using UnityEngine;

[CreateAssetMenu(fileName = "New Buff AbilityEffect", menuName = "Ability Effects/Create New Buff AbilityEffect")]
/*!<summary>
Allows an ability to apply a permanent stat increase to the player.
Right now, this script only supports increasing the player's max health.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/13/2024
</summary>*/
public class BuffAE : AbilityEffect
{
    /// \brief Reference to the player’s health.
    PlayerHealth playerHealth;
    /// \brief The amount the player’s health should be buffed by.
    public int buffValue;

    /// <summary>
    /// Instantly heal the player by the buffValue.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public override void Apply(AbilityOwner abilityOwner)
    {
        if (playerHealth == null)
            playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        playerHealth.HealInstant(buffValue);
    }

    /// <summary>
    /// Set AbilityEffectType to continuous.
    /// </summary>
    void Awake()
    {
        AbilityEffectType = AbilityEffectType.Continuous;
    }
}
