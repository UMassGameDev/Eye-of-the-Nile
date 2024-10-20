using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExampleAbilityInfo", menuName = "Abilities/Examples/Create New ExampleAbilityInfo")]
/*!<summary>
This is an example ability with placeholder functionality.
Useful for understanding the syntax for the ability system.

Documentation updated 8/13/2024
</summary>
\author Roy Pascual
\note This is a scriptable object, meaning you can make an instance of it in the Unity Editor that exists in the file explorer.
*/
public class ExampleAbilityInfo : BaseAbilityInfo
{
    [Header("Custom Ability Info")]
    /** @name Custom Ability Info
    *  Customize the specifics of how this ability works by setting the references this ability set relies on.
    */
    ///@{
    /// \brief Reference to the prefab that each ability spawns. This is usually set to a test prefab with text the ability tries to change.
    public Transform projectilePrefab;
    /// \brief Prefab used by an unused function.
    public Transform effectPrefab;
    /// \brief Reference to a game object an ability has just instantiated.
    GameObject tempAbilitySpawn;
    ///@}

    /// Unused function that spawns an effect where the player is standing.
    private void PlaceholderEffect(AbilityOwner abilityOwner)
    {
        Instantiate(effectPrefab, abilityOwner.OwnerTransform.position, Quaternion.identity);
    }

    /// Spawns a red textbox where the player is standing that says “Offense.”
    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Offense");
        Transform ownerTransform = abilityOwner.OwnerTransform;

        // Placeholder Effect below
        if (tempAbilitySpawn != null)
            Destroy(tempAbilitySpawn);
        tempAbilitySpawn = Instantiate(projectilePrefab,
            ownerTransform.position + new Vector3(0f, 1f, 0f),
            Quaternion.identity).gameObject;
        tempAbilitySpawn.transform.Find("TestText").GetComponent<TextMeshPro>().text = "Offense";
    }

    /// Heals the player instantly by damage (from BaseAbilityInfo). Spawns a red textbox where the player is standing that says “Defense.”
    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Defense");
        PlayerHealth playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealInstant(damage);
        }

        Transform ownerTransform = abilityOwner.OwnerTransform;

        // Placeholder Effect below
        if (tempAbilitySpawn != null)
            Destroy(tempAbilitySpawn);
        tempAbilitySpawn = Instantiate(projectilePrefab,
            ownerTransform.position + new Vector3(0f, 1f, 0f),
            Quaternion.identity).gameObject;
        tempAbilitySpawn.transform.Find("TestText").GetComponent<TextMeshPro>().text = "Defense";
    }

    /// Spawns a red textbox where the player is standing that says “Utility.”
    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Utility");

        Transform ownerTransform = abilityOwner.OwnerTransform;

        // Placeholder Effect below
        if (tempAbilitySpawn != null)
            Destroy(tempAbilitySpawn);
        tempAbilitySpawn = Instantiate(projectilePrefab,
            ownerTransform.position + new Vector3(0f, 1f, 0f),
            Quaternion.identity).gameObject;
        tempAbilitySpawn.transform.Find("TestText").GetComponent<TextMeshPro>().text = "Utility";
    }

    /// Writes "Example Passive" to the console.
    protected override void AbilityPassiveEnable(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive Enable");
    }

    /// Placeholder AbilityPassiveDisable
    protected override void AbilityPassiveDisable(AbilityOwner abilityOwner) { Debug.Log("Example Passive Disable"); }

    /// Heals the player instantly by damage (from BaseAbilityInfo) repeatedly.
    public override void AbilityUpdate(AbilityOwner abilityOwner)
    {
        Debug.Log("Update : " + Time.time);
        PlayerHealth playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        Debug.Log("Health : " + playerHealth.GetHealth());
        if (playerHealth != null)
        {
            playerHealth.HealInstant(damage);
        }
    }

    /// Calls base version of this function (doesn’t need to be included)
    /// \deprecated Since this function just calls the base version of itself, it can just be removed.
    public override void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        base.AbilityDisable(abilityOwner, effectType);
    }
}
