using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ActiveAbilityData", menuName = "Create New ActiveAbilityData")]
[Serializable]
/*!<summary>
Stores and manages the set of active ability sets. These are the 4 abilities on the hotbar the player has equipped and can use.
You can manage this in the unity editor with the ActiveAbilityData scriptable object.

Documentation updated 8/11/2024
</summary>*/
public class ActiveAbilityData : ScriptableObject
{
    public BaseAbilityInfo offenseSlot;
    public BaseAbilityInfo defenseSlot;
    public BaseAbilityInfo utilitySlot;
    public BaseAbilityInfo passiveSlot;
    public bool QueueRefresh { get; set; } = false;
    public List<int> RefreshSlots { get; set; } = new List<int>();

    // returns the ability info at the given slot number.
    public BaseAbilityInfo AbilityAt(int slotNumber)
    {
        switch (slotNumber)
        {
            case 0: return offenseSlot;
            case 1: return defenseSlot;
            case 2: return utilitySlot;
            case 3: return passiveSlot;
            default: return null;
        }
    }

    // Sets the ability info at the given slot number to the provided BaseAbilityInfo.
    // The currentForm of the ability info is updated to reflect the slot it’s now in.
    public void SetAbilityAt(int slotNumber, BaseAbilityInfo newAbilityInfo)
    {
        switch (slotNumber)
        {
            case 0:
                if (newAbilityInfo != null)
                    newAbilityInfo.currentForm = AbilityForm.Offense;
                offenseSlot = newAbilityInfo;
                break;
            case 1:
                if (newAbilityInfo != null)
                    newAbilityInfo.currentForm = AbilityForm.Defense;
                defenseSlot = newAbilityInfo;
                break;
            case 2:
                if (newAbilityInfo != null)
                    newAbilityInfo.currentForm = AbilityForm.Utility;
                utilitySlot = newAbilityInfo;
                break;
            case 3:
                if (newAbilityInfo != null)
                    newAbilityInfo.currentForm = AbilityForm.Passive;
                passiveSlot = newAbilityInfo;
                break;
            default:
                Debug.LogError("slotNumber is out of range (0 - 3). Cannot set ability. slotNumber provided: " + slotNumber);
                return;  // do nothing if out of range
        }
        QueueRefresh = true;
        RefreshSlots.Add(slotNumber);
    }
}
