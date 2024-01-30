/**************************************************
Stores and manages the set of active ability sets.
You can manage this in the unity editor with the ActiveAbilityData scriptable object.

Documentation updated 1/29/2024
**************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ActiveAbilityData", menuName = "Create New ActiveAbilityData")]
[Serializable]
public class ActiveAbilityData : ScriptableObject
{
    public BaseAbilityInfo offenseSlot;
    public BaseAbilityInfo defenseSlot;
    public BaseAbilityInfo utilitySlot;
    public BaseAbilityInfo passiveSlot;
    public bool QueueRefresh { get; set; } = false;
    public List<int> RefreshSlots { get; set; } = new List<int>();

    public BaseAbilityInfo AbilityAt(int slotNumber)
    {
        switch (slotNumber)
        {
            case 0: return offenseSlot;
            case 1: return defenseSlot;
            case 2: return utilitySlot;
            case 3: return passiveSlot;
            default: return offenseSlot;
        }
    }

    public void SetAbilityAt(int slotNumber, BaseAbilityInfo baseAbilityInfo)
    {
        switch (slotNumber)
        {
            case 0:
                if (baseAbilityInfo != null)
                    baseAbilityInfo.currentForm = AbilityForm.Offense;
                offenseSlot = baseAbilityInfo; break;
            case 1:
                if (baseAbilityInfo != null)
                    baseAbilityInfo.currentForm = AbilityForm.Defense;
                defenseSlot = baseAbilityInfo; break;
            case 2:
                if (baseAbilityInfo != null)
                    baseAbilityInfo.currentForm = AbilityForm.Utility;
                utilitySlot = baseAbilityInfo; break;
            case 3:
                if (baseAbilityInfo != null)
                    baseAbilityInfo.currentForm = AbilityForm.Passive;
                passiveSlot = baseAbilityInfo; break;
            default: offenseSlot = baseAbilityInfo; break;
        }
        QueueRefresh = true;
        RefreshSlots.Add(slotNumber);
    }
}
