/**************************************************
This is an example ability with placeholder functionality.
Useful for understanding the syntax for the ability system.
This is a scriptable object, meaning you can make an instance of it in the editor.

Documentation updated 8/13/2024
**************************************************/
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExampleAbilityInfo", menuName = "Abilities/Create New ExampleAbilityInfo")]
public class ExampleAbilityInfo : BaseAbilityInfo
{
    [Header("Custom Ability Info")]
    public Transform projectilePrefab;  // projectile that each a
    public Transform effectPrefab;
    GameObject tempAbilitySpawn;

    // Unused function that spawns an effect where the player is standing.
    private void PlaceholderEffect(AbilityOwner abilityOwner)
    {
        Instantiate(effectPrefab, abilityOwner.OwnerTransform.position, Quaternion.identity);
    }

    // Spawns a red textbox where the player is standing that says “Offense.”
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

    // Heals the player instantly by damage (from BaseAbilityInfo). Spawns a red textbox where the player is standing that says “Defense.”
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

    // Spawns a red textbox where the player is standing that says “Utility.”
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

    // Writes "Example Passive" to the console.
    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive");
    }

    // Heals the player instantly by damage (from BaseAbilityInfo) repeatedly.
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

    // Calls base version of this function (doesn’t need to be included)
    public override void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        base.AbilityDisable(abilityOwner, effectType);
    }
}
