using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New AbilityInventory", menuName = "Create New AbilityInventory")]
[Serializable]

/*!<summary>
Stores all the abilities the player has unlocked.
You can manage this in the unity editor with the AbilityInventory scriptable object.

Documentation updated 8/11/2024
</summary>
\author Stephen Nuttall*/
public class AbilityInventory : ScriptableObject
{
    [SerializeField] ActiveAbilityData activeAbilityData;  // Reference to the ActiveAbilityData scriptable object, containing the 4 in use ability info objects.
    [SerializeField] BaseAbilityInfo[] AbilitySets;  // All abilities the player can use are stored in here.

    // Tells the given ability to run its upgrade function, assuming the given index is in range.
    public void UpgradeAbility(int index)
    {
        if (index < AbilitySets.Length && index >= 0) {
            AbilitySets[index].UpgradeAbility();
        } else {
            Debug.LogError("Ability Index \"" + index + "\" is invalid.");
        }
    }

    // Sets the ability at slotNumber in the ActiveAbilityData to ability at this index, assuming the ability is unlocked and the given index is in range.
    // Example: putting Geb's ability in the third slot of your ability hotbar (represented by activeAbilityData).
    public void EquipAbility(int slotNumber, int index)
    {
        if (index < AbilitySets.Length && index >= 0) {
            if (AbilitySets[index].abilityLevel > 0) {
                activeAbilityData.SetAbilityAt(slotNumber, AbilitySets[index]);
            } else {
                Debug.Log("Ability is not unlocked!");
            }
        } else {
            Debug.LogError("Ability Index \"" + index + "\" is invalid.");
        }
    }

    // Returns the ability info at that index in the AbilitySets array (get by key).
    public BaseAbilityInfo GetAbilitySet(int abilityKey) { return AbilitySets[abilityKey]; }

    // Returns the ability info using that name (get by name).
    // It goes through the AbilitySets array to find an ability using that name. If not found, the function returns null.
    public BaseAbilityInfo GetAbilitySet(string abilityName)
    {
        foreach (BaseAbilityInfo ability in AbilitySets)
        {
            if (ability != null && ability.abilityName == abilityName)
            {
                return ability;
            }
        }
        return null;
    }

    // Get the number of abilities (the length of AbilitySets)
    public int GetNumAbilities() { return AbilitySets.Length; }
}
