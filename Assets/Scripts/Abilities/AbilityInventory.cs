/**************************************************
Stores all the abilities the player has unlocked.
You can manage this in the unity editor with the AbilityInventory scriptable object.

Documentation updated 3/13/2024
**************************************************/
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New AbilityInventory", menuName = "Create New AbilityInventory")]
[Serializable]
public class AbilityInventory : ScriptableObject
{
    [SerializeField] ActiveAbilityData activeAbilityData;
    [SerializeField] BaseAbilityInfo[] AbilitySets;  // All abilities the player can use are stored in here

    public void UpgradeAbility(int index)
    {
        if (index < AbilitySets.Length && index >= 0) {
            AbilitySets[index].UpgradeAbility();
        } else {
            Debug.LogError("Ability Index \"" + index + "\" is invalid.");
        }
    }

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

    public BaseAbilityInfo GetAbilitySet(int abilityKey) { return AbilitySets[abilityKey]; }
    public BaseAbilityInfo GetAbilitySet(string abilityName)
    {
        foreach (BaseAbilityInfo ability in AbilitySets)
        {
            if (ability.abilityName == abilityName)
            {
                return ability;
            }
        }
        return null;
    }

    public int GetNumAbilities() { return AbilitySets.Length; }
}
