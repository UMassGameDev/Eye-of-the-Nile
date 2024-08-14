/**************************************************
This is the info for the fire ability set made for the club fair build.
The offense ability shoots bursts of fireballs.
The defense ability spawns a tall fire on each side of the player.
The utility ability makes the player immune to fire for a short time.
The passive ability makes the player take less fire damage.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/14/2024
**************************************************/
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FireAbilityInfo", menuName = "Abilities/Create New FireAbilityInfo")]
public class FireAbilityInfo : BaseAbilityInfo
{
    [Header("Offense Ability Info")]
    public GameObject projectilePrefab;  // Reference to the projectile prefab this ability will instantiate.
    public int numFireballs = 3;  // The amount of projectiles to spawn.
    public float fireballDelay = 0.5f;  // How much time to wait between spawning projectiles.

    [Header("Defense Ability Info")]
    public GameObject firewallPrefab;  // Reference to the firewall prefab this ability will instantiate.
    public float firewallXOffset = 1.5f;  // Offset on the x axis where the wall will spawn in relation to the player.
    public float firewallYOffset = 0.5f;  // Offset on the y axis where the wall will spawn in relation to the player.

    [Header("Utility Ability Info")]
    public float fireImmunityTimer = 3f;  // How long the fire immunity will last.

    // This system could've been a lot simpler if it wasn't designed to support multiple statMods at the same time.
    // Since this ability will only grant fire resistance, this extra functionality was not necessary.
    [Header("Passive Ability Info")]
    public List<StatModifier> statMods;  // List of references to the stat modifiers this ability will apply.
    private string currentStat = "FireResistance";  // Stat modifier currently in use.
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

    // Shoot a burst of fireballs.
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

    // Makes the player immune to fire for a short time.
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        abilityOwner.OwnerTransform.GetComponent<PlayerHealth>().FireImmunity(fireImmunityTimer);
    }

    // Makes the player take less fire damage.
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Transform ownerTransform = abilityOwner.OwnerTransform;
        PlayerStatHolder playerStats = ownerTransform.GetComponent<PlayerStatHolder>();
        playerStats.GetStat(currentStat).RemoveModifier(_StatModDict[currentStat]);
        playerStats.GetStat(currentStat).AddModifier(_StatModDict[currentStat]);
    }
}
