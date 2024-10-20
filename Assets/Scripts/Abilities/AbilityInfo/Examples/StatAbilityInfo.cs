using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatAbilityInfo", menuName = "Abilities/Examples/Create New StatAbilityInfo")]
/*!<summary>
This is an example of an ability set that uses stat modifiers for each ability.
This script is useful for copy-pasting into your own ability info if you want the same functionality.

The initializing part at the top is only necessary if you expect to be applying multiple effects,
but do not want them applied at the same time. Honestly, it can probably be made to be much simpler.

For now, if you're looking to make your own ability, you can probably get away with copying the passive
ability in RockAbilityInfo. You can make the effect expire by using RemoveModifier() in AbilityDisable().

Documentation updated 8/14/2024
</summary>
\author Roy Pascual
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class StatAbilityInfo : BaseAbilityInfo
{
    [Header("Custom Ability Info")]
    public List<StatModifier> statMods;  // List of references to the stat modifiers these abilities will apply.
    private Dictionary<string, StatModifier> _statModDictField;  // Used to initialize _StatModDict.
    private Dictionary<string, StatModifier> _StatModDict  // Maps stat modifier names to their corresponding stat modifier objects.
    {
        get
        {
            if (_statModDictField != null)
                return _statModDictField;
            else
            {
                StatModDictInitializer();
                return _statModDictField;
            }
        }
        set { _statModDictField = value; }
    }
    private string currentStat = "Damage";  // Stat modifier currently in use.

    // Populates _StatModDict with the stat modifiers from statMods.
    private void StatModDictInitializer()
    {
        _statModDictField = new Dictionary<string, StatModifier>();
        foreach (StatModifier statMod in statMods)
        {
            // Debug.Log(statMod.TargetStat);
            _statModDictField.Add(statMod.TargetStat, statMod);
        }
    }

    // Runs StatModDictInitializer() as soon as the object is loaded.
    void Awake()
    {
        StatModDictInitializer();
    }

    // Increases player damage.
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Offense");

        // get transform and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        // remove any current stat modifier
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        // apply new stat modifier
        currentStat = "Damage";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

    }

    // Increases player's max health.
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Defense");

        // get transform, player health, and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        // debug messages
        Debug.Log(playerStats);
        Debug.Log(playerStats.GetStat(currentStat));
        Debug.Log(_StatModDict);
        Debug.Log(_StatModDict[currentStat]);

        // remove any current stat modifier
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        // apply new stat modifier
        currentStat = "MaxHealth";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

        // update player health
        playerHealth.InvokeHealthChange();
    }

    // Increases player speed.
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Utility.");

        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        currentStat = "Speed";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
    }

    // Increases player's max health.
    // this is the same code as the defense ability, but without the debug messages.
    protected override void AbilityPassiveEnable(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive Enable.");

        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        currentStat = "MaxHealth";
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);

        playerHealth.InvokeHealthChange();
    }

    /// removes the max health increase.
    protected override void AbilityPassiveDisable(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive Disable.");

        // get transform and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        if (currentStat == "MaxHealth")
        {
            // remove the max health stat modifier
            playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

            // since we changed the max health, update player health
            PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
            playerHealth.InvokeHealthChange();
        }
    }

    // Removes all active effects.
    public override void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        // get transform and stats
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();

        // remove any current stat modifier
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);

        // if we changed the max health, update player health
        if (currentStat == "MaxHealth")
        {
            PlayerHealth playerHealth = ownerTransform.GetComponent<PlayerHealth>();
            playerHealth.InvokeHealthChange();
        }

        // trigger the base version of AbilityDisable()
        base.AbilityDisable(abilityOwner, effectType);
    }
}
