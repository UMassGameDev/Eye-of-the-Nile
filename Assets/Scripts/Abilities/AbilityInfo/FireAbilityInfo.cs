using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FireAbilityInfo", menuName = "Abilities/Create New FireAbilityInfo")]
/*!<summary>
This is the info for the fire ability set made for the club fair build.
- The offense ability shoots bursts of fireballs.
- The defense ability spawns a tall fire on each side of the player.
- The utility ability makes the player immune to fire for a short time.
- The passive ability makes the player take less fire damage.

Documentation updated 8/14/2024
</summary>
\author Stephen Nuttall
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class FireAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    /** @name Offense Ability Info
    *  Information related to the offense ability in this set. The offense ability shoots bursts of fireballs.
    */
    ///@{
    /// \brief Reference to the projectile prefab this ability will instantiate.
    public GameObject projectilePrefab;
    /// \brief The amount of projectiles to spawn.
    public int numFireballs = 3;
    /// \brief How much time to wait between spawning projectiles.
    public float fireballDelay = 0.5f;
    ///@}

    [Header("Defense Ability Info")]
    /** @name Defense Ability Info
    *  Information related to the defense ability in this set. The defense ability spawns a tall fire on each side of the player.
    */
    ///@{
    /// \brief Reference to the firewall prefab this ability will instantiate.
    public GameObject firewallPrefab;
    /// \brief Offset on the x axis where the wall will spawn in relation to the player.
    public float firewallXOffset = 1.5f;  
    /// \brief Offset on the y axis where the wall will spawn in relation to the player.
    public float firewallYOffset = 0.5f;
    ///@}

    [Header("Utility Ability Info")]
    /** @name Utility Ability Info
    *  Information related to the utility ability in this set. The utility ability makes the player immune to fire for a short time.
    */
    ///@{
    /// \brief How long the fire immunity will last.
    public float fireImmunityTimer = 3f;
    ///@}

    [Header("Passive Ability Info")]
    /** @name Passive Ability Info
    *  Information related to the passive ability in this set. The passive ability makes the player take less fire damage.
    *  \remarks This system could've been a lot simpler if it wasn't designed to support multiple statMods at the same time.
    *  Since this ability will only grant fire resistance, this extra functionality was not necessary.
    */
    ///@{
    /// \brief List of references to the stat modifiers this ability will apply.
    public List<StatModifier> statMods;
    /// \brief Stat modifier currently in use.
    private string currentStat = "FireResistance";
    /// \brief Used to initialize _StatModDict.
    private Dictionary<string, StatModifier> _statModDictField;
    /// \brief Maps stat modifier names to their corresponding stat modifier objects.
    private Dictionary<string, StatModifier> _StatModDict
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
    ///@}

    /// <summary>
    /// Populates _StatModDict with the stat modifiers from statMods.
    /// </summary>
    private void StatModDictInitializer()
    {
        _statModDictField = new Dictionary<string, StatModifier>();
        foreach (StatModifier statMod in statMods)
        {
            // Debug.Log(statMod.TargetStat);
            _statModDictField.Add(statMod.TargetStat, statMod);
        }
    }

    /// <summary>
    /// Shoot a burst of fireballs.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerAttackManager>().ShootProjectileBurst(projectilePrefab, numFireballs, fireballDelay);
    }

    // Spawn a tall fire on each side of the player.
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Instantiate(firewallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x + firewallXOffset,
            abilityOwner.OwnerTransform.position.y + firewallYOffset), Quaternion.identity);

        Instantiate(firewallPrefab, new Vector2(
            abilityOwner.OwnerTransform.position.x - firewallXOffset,
            abilityOwner.OwnerTransform.position.y + firewallYOffset), Quaternion.identity);
    }

    /// <summary>
    /// Makes the player immune to fire for a short time.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().FireImmunity(fireImmunityTimer);
    }

    /// <summary>
    /// Makes the player take less fire damage.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
    }
}
