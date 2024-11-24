using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/** \brief
Implements the inventory slots in the ability inventory found in the skyhub.
Adapted from InventorySlot.

Documentation updated 9/19/2024
\author Stephen Nuttall, Alexander Art
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
    /// If acceptsOnlyOneItem is true, this is the item this slot will accept and others will be rejected.
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
        // If an item is being dragged && that item has the required item data...
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData))
        {
            if (slotData == null || acceptsOnlyOneItem) { // If an item is being dragged into an empty slot or if the dragged item is the only valid item for that slot
                if (acceptsOnlyOneItem) {
                    if (itemData == acceptedItem)
                        itemAccepted(eventData, itemData);
                } else {
                    itemAccepted(eventData, itemData);
                }
            } else if (slotData != null && itemData.currentSlot != slotNum) { // If an item is being dragged into a full slot (other than its own slot)
                // This code works, but it is ugly, so it should probably be revised at some point.
                // fromSlot -> the slot that the dragged item came from
                // toSlot -> the slot that the dragged item is being dropped into (which is the slot that this code runs on)
                // fromItem -> the item that fromSlot initially holds (the item being dragged)
                // toItem -> the item that toSlot initially holds
                // holdSlot -> slot that is offscreen that holds one item so the items never overlap while swapping
                // Steps:
                // 1. Move fromItem from the fromSlot into the holdSlot
                // 2. Move toItem from the toSlot into the now empty fromSlot
                // 3. Move fromItem, now in the holdSlot, into the toSlot

                // TODO: If an item is being dragged from an inventory slot into a full active ability slot,
                // then put the active item back into its own inventory slot and let the dragged item go where it was trying to go.

                int fromSlotNum = itemData.currentSlot; // fromItem and the itemData variable are essentially the same thing
                // Get the fromSlot AbilityInventorySlot object
                AbilityInventorySlot fromSlot = null;
                foreach (Transform child in GameObject.Find("Inventory Slots").transform) {
                    if (child.GetComponent<AbilityInventorySlot>().slotNum == fromSlotNum) {
                        fromSlot = child.GetComponent<AbilityInventorySlot>();
                    }
                }
                foreach (Transform child in GameObject.Find("Active Ability Slots").transform) {
                    if (child.GetComponent<AbilityInventorySlot>().slotNum == fromSlotNum) {
                        fromSlot = child.GetComponent<AbilityInventorySlot>();
                    }
                }

                // As long as fromSlot and toSlot both accept more than one item, then swap the items
                if (!fromSlot.acceptsOnlyOneItem && !acceptsOnlyOneItem) {
                    // Put fromItem into holdSlot
                    AbilityInventorySlot holdSlot = GameObject.Find("Hold Slot").GetComponent<AbilityInventorySlot>();
                    eventData.pointerDrag.GetComponent<DragAndDrop>().AcceptItem();
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = holdSlot.GetComponent<RectTransform>().anchoredPosition;
                    receivedItem?.Invoke(itemData, holdSlot.slotNum);
                    eventData.pointerDrag.GetComponent<AbilityInventoryItemData>().currentSlot = holdSlot.slotNum;

                    // Put toItem into fromSlot
                    Transform toItem = null;
                    foreach (Transform child in GameObject.Find("Ability Icons").transform) {
                        if (child.GetComponent<AbilityInventoryItemData>().currentSlot == slotNum) {
                            toItem = child;
                        }
                    }
                    toItem.GetComponent<DragAndDrop>().AcceptItem();
                    toItem.GetComponent<RectTransform>().anchoredPosition = fromSlot.GetComponent<RectTransform>().anchoredPosition;
                    receivedItem?.Invoke(toItem.GetComponent<AbilityInventoryItemData>(), fromSlotNum);
                    toItem.GetComponent<AbilityInventoryItemData>().currentSlot = fromSlotNum;

                    // Put fromItem into toSlot
                    Transform fromItem = null;
                    foreach (Transform child in GameObject.Find("Ability Icons").transform) {
                        if (child.GetComponent<AbilityInventoryItemData>().currentSlot == holdSlot.slotNum) {
                            fromItem = child;
                        }
                    }
                    fromItem.GetComponent<DragAndDrop>().AcceptItem();
                    fromItem.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;;
                    receivedItem?.Invoke(fromItem.GetComponent<AbilityInventoryItemData>(), slotNum);
                    fromItem.GetComponent<AbilityInventoryItemData>().currentSlot = slotNum;
                    
                    // Update slotData & textboxes for the fromSlot and toSlot (updating holdSlot would be redundant)
                    slotData = fromItem.GetComponent<AbilityInventoryItemData>();
                    setTextboxes();
                    fromSlot.slotData = toItem.GetComponent<AbilityInventoryItemData>();
                    fromSlot.setTextboxes();
                }
            }
        }
    }

    /// Add the item to the slot, updating the slot's data and textboxes.
    void itemAccepted(PointerEventData eventData, AbilityInventoryItemData itemData)
    {
        // Basic functionality of an inventory slot from InventorySlot.cs
        eventData.pointerDrag.GetComponent<DragAndDrop>().AcceptItem();
        eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        // Update abilityName and check for duplicate ability names
        slotData = itemData;
        receivedItem?.Invoke(slotData, slotNum);

        // Update which slot the item thinks its in when it moves
        eventData.pointerDrag.GetComponent<AbilityInventoryItemData>().currentSlot = slotNum;

        // Update name and level text boxes
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

    // Returns the number of the slot.
    public int GetSlotNum() { return slotNum; }
}
