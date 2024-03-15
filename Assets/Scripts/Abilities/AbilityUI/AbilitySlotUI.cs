/**************************************************
Handles displaying ability icons and swapping icons when drag and dropping.

Documentation updated 3/14/2024
**************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlotUI : MonoBehaviour, IDropHandler
{
    public delegate void DropEvent(int slotNumber);
    public event DropEvent DroppedUIEvent;

    public AbilityImageUI CurAbilityImageUI { get; set; }
    public int slotID = 0;

    public void SubscribeToDrop(TotalAbilityUI totalAbilityUI)
    {
        DroppedUIEvent += totalAbilityUI.SlotDropListener;
    }

    public void UnsubscribeFromDrop(TotalAbilityUI totalAbilityUI)
    {
        DroppedUIEvent -= totalAbilityUI.SlotDropListener;
    }

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
            Debug.Log(slotID);
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
            Debug.Log(previousSlot.slotID);
            previousSlot.CurAbilityImageUI = CurAbilityImageUI;
            CurAbilityImageUI.previousParent = previousSlot.transform;
            CurAbilityImageUI.nextParent = CurAbilityImageUI.previousParent;
            CurAbilityImageUI.transform.SetParent(CurAbilityImageUI.nextParent);
            previousSlot.DroppedUIEvent(previousSlot.slotID);

            abilityImageUI.nextParent = transform;
            CurAbilityImageUI = abilityImageUI;
            Debug.Log(slotID);
            DroppedUIEvent(slotID);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurAbilityImageUI = GetComponentInChildren<AbilityImageUI>();
    }
}
