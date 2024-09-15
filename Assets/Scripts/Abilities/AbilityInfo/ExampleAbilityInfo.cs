using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExampleAbilityInfo", menuName = "Abilities/Create New ExampleAbilityInfo")]
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

    /// <summary>
    /// Unused function that spawns an effect where the player is standing.
    /// </summary>
    /// <param name="abilityOwner"></param>
    private void PlaceholderEffect(AbilityOwner abilityOwner)
    {
        Instantiate(effectPrefab, abilityOwner.OwnerTransform.position, Quaternion.identity);
    }

    /// <summary>
    /// Spawns a red textbox where the player is standing that says “Offense.”
    /// </summary>
    /// <param name="abilityOwner"></param>
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

    /// <summary>
    /// Heals the player instantly by damage (from BaseAbilityInfo). Spawns a red textbox where the player is standing that says “Defense.”
    /// </summary>
    /// <param name="abilityOwner"></param>
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

    /// <summary>
    /// Spawns a red textbox where the player is standing that says “Utility.”
    /// </summary>
    /// <param name="abilityOwner"></param>
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

    /// <summary>
    /// Writes "Example Passive" to the console.
    /// </summary>
    /// <param name="abilityOwner"></param>
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive");
    }

    /// <summary>
    /// Heals the player instantly by damage (from BaseAbilityInfo) repeatedly.
    /// </summary>
    /// <param name="abilityOwner"></param>
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

    /// <summary>
    /// Calls base version of this function (doesn’t need to be included)
    /// </summary>
    /// <param name="abilityOwner"></param>
    /// <param name="effectType"></param>
    public override void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        base.AbilityDisable(abilityOwner, effectType);
    }
}
