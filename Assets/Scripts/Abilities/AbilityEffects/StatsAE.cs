/**************************************************
ALlows an ability to apply a stat change to the player.
Which stat to change and by how much can be changed in the editor.
This is a scriptable object, meaning you can make and instance of it in the editor.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

// This is applies a stat modifier that occurs during the duration
[CreateAssetMenu(fileName = "New Stats AbilityEffect", menuName = "Ability Effects/Create New Stats AbilityEffect")]
public class StatsAE : AbilityEffect
{
    // public string affectedStat;
    public StatModifier statMod;
    // public int magnitude;

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

    void Awake()
    {
        AbilityEffectType = AbilityEffectType.Immediate;
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
