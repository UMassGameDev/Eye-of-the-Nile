using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/** \brief
Implements the inventory slots in the ability inventory found in the skyhub.
Adapted from InventorySlot.

Documentation updated 9/19/2024
\author Stephen Nuttall
*/
public class AbilityInventorySlot : MonoBehaviour, IDropHandler
{
    /// The ability item that this slot currently holds.
    public AbilityInventoryItemData slotData;

    /// The ID number of this slot. -1 is a default value to represent an error.
    [SerializeField] int slotNum = -1;
    /// \brief If true, the details button will be displayed beneath the slot that displays extra info about the ability.
    /// Disabled for the 4 active ability slots at the bottom of the ability inventory.
    [SerializeField] bool enableDetailsButton = true;

    /// If enabled, the user can only drop acceptedItem into this slot. Enabled for the 9 inventory slots at the top.
    [SerializeField] bool acceptsOnlyOneItem = true;
    /// Of acceptsOnlyOneItem is true, this is the item this slot will accept and others will be rejected.
    [SerializeField] AbilityInventoryItemData acceptedItem;

    /// Invoked when the slot receives an item. Contains the item data and slot ID number.
    public static event Action<AbilityInventoryItemData, int> receivedItem;

    /// \brief Shortly after the object is created or re-enabled, subscribe checkDuplicateName() to receivedItem
    /// and setTextboxes() to AbilityInventoryUI.abilityInventorySlotInitialized.
    void OnEnable()
    {
        receivedItem += checkDuplicateName;
        AbilityInventoryUI.abilityInventorySlotInitialized += setTextboxes;
    }

    /// When the object is disabled, unsubscribe from all events.
    void OnDisable()
    {
        receivedItem -= checkDuplicateName;
        AbilityInventoryUI.abilityInventorySlotInitialized -= setTextboxes;
    }

    /// When the user drops an item into this slot, run itemAccept() (unless this slot is full or only accepts a different item).
    public void OnDrop(PointerEventData eventData)
    {
        // If an item is being dragged && that item has the required item data && it is being dropped into an empty slot...
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData))
        {
            if (slotData == null || acceptsOnlyOneItem) {
                if (acceptsOnlyOneItem) {
                    if (itemData == acceptedItem)
                        itemAccepted(eventData, itemData);
                } else {
                    itemAccepted(eventData, itemData);
                }
            } // TODO: swap the position of the items when the slot is full
        }
    }

    /// Add the item to the slot, updating the slot's data and textboxes.
    void itemAccepted(PointerEventData eventData, AbilityInventoryItemData itemData)
    {
        // basic functionality of an inventory slot from InventorySlot.cs
        eventData.pointerDrag.GetComponent<DragAndDrop>().AcceptItem();
        eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        // update abilityName and check for duplicate ability names
        slotData = itemData;
        receivedItem?.Invoke(slotData, slotNum);

        // update name and level text boxes
        setTextboxes();
    }

    /// Displays details button (if enabled), and the name and level of the ability in this slot.
    void setTextboxes()
    {
        if (enableDetailsButton)
            transform.GetChild(2).gameObject.SetActive(true);

        if (slotData == null || slotData.abilityName == "EMPTY" || slotData.abilityName == "") {
            transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            transform.GetChild(2).gameObject.SetActive(false);
        } else {
            transform.GetChild(0).GetComponent<TMP_Text>().text = slotData.abilityName;
            if (slotData.abilityLevel == 0) {
                transform.GetChild(1).GetComponent<TMP_Text>().text = "Locked";
            } else {
                transform.GetChild(1).GetComponent<TMP_Text>().text = "Level " + slotData.abilityLevel;
            }
        }
    }

    /// \brief Run when the slot receives an item. If the name of the item dropped into this slot is the same as
    /// an item in another slot, remove the duplicate item.
    void checkDuplicateName(AbilityInventoryItemData thisSlot, int thisSlotNum)
    {
        if (thisSlot == slotData && thisSlotNum != slotNum && !acceptsOnlyOneItem)
        {
            slotData = null;
            setTextboxes();
        }
    }

    /// Returns the position of the slot.
    public Vector2 GetPosition() { return gameObject.transform.position; }
}
