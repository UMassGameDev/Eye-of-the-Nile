using UnityEngine;

// This is applies a stat modifier that occurs during the duration
[CreateAssetMenu(fileName = "New Stats AbilityEffect", menuName = "Ability Effects/Create New Stats AbilityEffect")]

/*!<summary>
Allows an ability to apply a stat change to the player.
Which stat to change and by how much can be changed in the editor.
This is a scriptable object, meaning you can make and instance of it in the editor.

Documentation updated 8/13/2024
</summary>*/
public class StatsAE : AbilityEffect
{
    // public string affectedStat;
    /// \brief Reference to the stat modifier we want to use.
    public StatModifier statMod;
    // public int magnitude;

    /// <summary>
    /// Get player stats holder and add statMod to it. Then invoke the health change.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public override void Apply(AbilityOwner abilityOwner)
    {
        PlayerStatHolder pStats = abilityOwner.OwnerTransform.GetComponent<PlayerStatHolder>();
        pStats.GetStat(statMod.TargetStat).AddModifier(statMod);
        if (statMod.TargetStat == "MaxHealth")
            abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().InvokeHealthChange();

        /*switch (affectedStat)
        {
            case "Damage":
                PlayerStatHolder pStats = abilityOwner.OwnerTransform.GetComponent<PlayerStatHolder>();
                pStats.GetStat(affectedStat).AddModifier(statMod);
                break;
            case "Speed":
                break;
            default:
                break;
        }*/
        // abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().HealInstant(magnitude);
    }

    /// <summary>
    /// Get player stats holder and remove statMod from it. Then invoke the health change.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public override void Disable(AbilityOwner abilityOwner)
    {
        PlayerStatHolder pStats = abilityOwner.OwnerTransform.GetComponent<PlayerStatHolder>();
        pStats.GetStat(statMod.TargetStat).RemoveModifier(statMod);
        if (statMod.TargetStat == "MaxHealth")
            abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().InvokeHealthChange();
        /*switch (affectedStat)
        {
            case "Damage":
                break;
            case "Speed":
                break;
            default:
                break;
        }*/
    }

    /// <summary>
    /// Set AbilityEffectType to Immediate.
    /// </summary>
    void Awake()
    {
        AbilityEffectType = AbilityEffectType.Immediate;
    }
}
