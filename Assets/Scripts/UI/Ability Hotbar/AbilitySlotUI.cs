using UnityEngine;
using UnityEngine.EventSystems;

/** \brief
This script goes on the slots of the ability hotbar and handles displaying ability icons and swapping icons when drag and dropping.

Documentation updated 9/18/2024
\author Roy Pascual
\note This is a temporary feature and in the future you will not be able to drag the icons around without going to the Skyhub.
*/
public class AbilitySlotUI : MonoBehaviour, IDropHandler
{
    /// Delegate used to create DroppedUIEvent
    public delegate void DropEvent(int slotNumber);
    /// Triggered when totalAbilityUI.SlotDropListener() is run.
    public event DropEvent DroppedUIEvent;

    /// Reference to the AbilityImageUI object that is in this slot, which represents the ability's icon that can be dragged and dropped.
    public AbilityImageUI CurAbilityImageUI { get; set; }
    /// The ID number of this slot. 0 - offense slot, 1 - defense slot, 2 - utility slot, 3 - passive slot.
    public int slotID = 0;

    /// \brief Subscribes totalAbilityUI.SlotDropListener() to DroppedUIEvent.
    /// Run by the TotalAbilityUI object that governs the ability hotbar system.
    public void SubscribeToDrop(TotalAbilityUI totalAbilityUI)
    {
        DroppedUIEvent += totalAbilityUI.SlotDropListener;
    }

    /// \brief Unsubscribes totalAbilityUI.SlotDropListener() from DroppedUIEvent. 
    /// Run by the TotalAbilityUI object that governs the ability hotbar system.
    public void UnsubscribeFromDrop(TotalAbilityUI totalAbilityUI)
    {
        DroppedUIEvent -= totalAbilityUI.SlotDropListener;
    }

    /// \brief Runs when the user drags and drops an icon above this slot. 
    /// Adds the icon (AbilityImageUI object) dropped on this slot to this slot.
    /// If there's a icon already in this slot, put it in the slot the dropped icon came from.
    public void OnDrop(PointerEventData eventData)
    {
        AbilityImageUI abilityImageUI = eventData.pointerDrag.GetComponent<AbilityImageUI>();
        Debug.Log("Here.");

        if (abilityImageUI != null && CurAbilityImageUI == null)
        {
            abilityImageUI.nextParent = transform;
            CurAbilityImageUI = abilityImageUI;
            AbilitySlotUI previousSlot = abilityImageUI.previousParent.GetComponent<AbilitySlotUI>();
            previousSlot.CurAbilityImageUI = null;
            // Debug.Log(slotID);
            previousSlot.DroppedUIEvent(previousSlot.slotID);
            DroppedUIEvent(slotID); 
        }
        else if (abilityImageUI != null && CurAbilityImageUI != null)
        {
            // Put the current ability slot's ability image in the new ability image's old ability slot
            // In other words, swap images between slots
            /*AbilitySlotUI previousSlot = abilityImageUI.previousParent.GetComponent<AbilitySlotUI>();
            CurAbilityImageUI.nextParent = previousSlot.transform;
            previousSlot.CurAbilityImageUI = CurAbilityImageUI;
            CurAbilityImageUI.transform.SetParent(CurAbilityImageUI.nextParent);
            previousSlot.DroppedUIEvent(previousSlot.slotID);*/
            AbilitySlotUI previousSlot = abilityImageUI.previousParent.GetComponent<AbilitySlotUI>();
            // Debug.Log(previousSlot.slotID);
            previousSlot.CurAbilityImageUI = CurAbilityImageUI;
            CurAbilityImageUI.previousParent = previousSlot.transform;
            CurAbilityImageUI.nextParent = CurAbilityImageUI.previousParent;
            CurAbilityImageUI.transform.SetParent(CurAbilityImageUI.nextParent);
            previousSlot.DroppedUIEvent(previousSlot.slotID);

            abilityImageUI.nextParent = transform;
            CurAbilityImageUI = abilityImageUI;
            // Debug.Log(slotID);
            DroppedUIEvent(slotID);
        }
    }

    /// Set reference to AbilityImageUI object that's in this slot.
    void Start()
    {
        CurAbilityImageUI = GetComponentInChildren<AbilityImageUI>();
    }
}
