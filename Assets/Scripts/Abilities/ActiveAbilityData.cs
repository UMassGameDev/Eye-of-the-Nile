using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ActiveAbilityData", menuName = "Create New ActiveAbilityData")]
[Serializable]
/*!<summary>
Stores and manages the set of active ability sets. These are the 4 abilities on the hotbar the player has equipped and can use.
You can manage this in the unity editor with the ActiveAbilityData scriptable object.

Documentation updated 8/11/2024
</summary>
\author Roy Pascual*/
public class ActiveAbilityData : ScriptableObject
{
    /// <summary> Ability info representing the ability set in offense slot </summary>
    public BaseAbilityInfo offenseSlot;
    /// <summary> Ability info representing the ability set in defense slot </summary>
    public BaseAbilityInfo defenseSlot;
    /// <summary> Ability info representing the ability set in utility slot </summary>
    public BaseAbilityInfo utilitySlot;
    /// <summary> Ability info representing the ability set in passive slot </summary>
    public BaseAbilityInfo passiveSlot;

    /// <summary>
    /// True is a change has been made to the active abilities.
    /// This allows PlayerAbilityController.cs to know it should update the ability hotbar display in the bottom left.
    /// </summary>
    /// \note Might replace with an event someday.
    public bool QueueRefresh { get; set; } = false;
    /// <summary>
    /// Containers all slots that have been changed since the last QueueRefresh.
    /// This allows PlayerAbilityController.cs to know what slots it needs to update in the ability hotbar display.
    /// </summary>
    /// \note Might replace with an event someday.
    public List<int> RefreshSlots { get; set; } = new List<int>();

    /// <summary>
    /// returns the ability info at the given slot number.
    /// </summary>
    /// <param name="slotNumber"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Sets the ability info at the given slot number to the provided BaseAbilityInfo.
    /// The currentForm of the ability info is updated to reflect the slot it’s now in.
    /// </summary>
    /// <param name="slotNumber"></param>
    /// <param name="newAbilityInfo"></param>
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
