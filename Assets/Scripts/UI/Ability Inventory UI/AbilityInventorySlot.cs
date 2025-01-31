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

    /// This is the slot number of the hold slot, a slot that this script utilizes to make it easier to move items around.
    /// The hold slot should never be visible in-game.
    /// This variable exists only to make it easier to change which slot is considered the hold slot, if ever necessary.
    private int holdSlotNum = 200;

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
            if (itemData.abilityName != "EMPTY" && (slotData == null || acceptsOnlyOneItem)) { // If an item is being dragged into an empty slot or a slot that only accepts one item
                if (acceptsOnlyOneItem) {
                    if (itemData == acceptedItem) { // If the dragged item is the only valid item for the slot
                        itemAccepted(eventData, itemData);
                    } else { // If the dragged item is not the valid item for the slot
                        // Find the slot that only accepts the item data of the item being dragged
                        int returnSlotNum = -1;
                        foreach (Transform child in GameObject.Find("Inventory Slots").transform) {
                            if (child.GetComponent<AbilityInventorySlot>().acceptedItem == itemData) {
                                returnSlotNum = child.GetComponent<AbilityInventorySlot>().slotNum;
                            }
                        }

                        // Find the slot that the dragged item came from
                        int fromSlotNum = itemData.currentSlot;
                        AbilityInventorySlot fromSlot = null;
                        foreach (Transform child in GameObject.Find("Inventory Slots").transform) {
                            if (child.GetComponent<AbilityInventorySlot>().slotNum == fromSlotNum) {
                                fromSlot = child.GetComponent<AbilityInventorySlot>();
                            }
                        }

                        // Send the dragged item to its inventory slot
                        sendItem(fromSlotNum, returnSlotNum);
                    }
                } else {
                    itemAccepted(eventData, itemData);
                }
            } else if (itemData.abilityName != "EMPTY" && slotData != null && itemData.currentSlot != slotNum) { // If an item is being dragged into a full slot (other than its own slot)
                // fromSlot -> the slot that the dragged item came from.
                // toSlot -> the slot that the dragged item is being dropped into. (The slot that this code runs on.)
                // fromItem -> the item that fromSlot initially holds. (The item being dragged.)
                // toItem -> the item that toSlot initially holds.
                // hold slot -> slot that is offscreen that holds one item so the items never overlap while swapping.

                // Steps:
                // - Find fromSlot and its slot number.
                // - Find toSlot and its slot number.
                // - If both slots accept more than one item, swap fromItem's and toItem's positions. This utilizes the hold slot.
                //     - Steps:
                //         1. Move fromItem from the fromSlot into the hold slot.
                //         2. Move toItem from the toSlot into the now empty fromSlot.
                //         3. Move fromItem, now in the hold slot, into the toSlot.
                // - If the toSlot accepts more than one item but the fromSlot accepts only one item, then send the toItem
                //   back to its inactive inventory slot and let the fromItem go where it was being dragged to.
                //     - Steps:
                //         1. Find which slot is the slot that only accepts the toItem.
                //         2. Move the toItem into that slot.
                //         3. Move the fromItem into the now empty toSlot.

                // Find the slot that the dragged item came from
                int fromSlotNum = itemData.currentSlot;
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
                if (GameObject.Find("Hold Slot").GetComponent<AbilityInventorySlot>().slotNum == fromSlotNum) {
                    fromSlot = GameObject.Find("Hold Slot").GetComponent<AbilityInventorySlot>();
                }

                // The slot that the item is being dragged to
                int toSlotNum = this.slotNum;
                AbilityInventorySlot toSlot = this;

                // If fromSlot and toSlot both accept more than one item, then swap the items
                if (!fromSlot.acceptsOnlyOneItem && !toSlot.acceptsOnlyOneItem) {
                    // Send the item in the fromSlot into the hold slot
                    sendItem(fromSlotNum, holdSlotNum);

                    // Send the item in the toSlot into the fromSlot
                    sendItem(toSlotNum, fromSlotNum);

                    // Send the item in the hold slot (which now holds the fromItem) into the toSlot (which is now empty)
                    sendItem(holdSlotNum, toSlotNum);
                }

                // If toSlot, but not fromSlot, accepts more than one item, then...
                if (fromSlot.acceptsOnlyOneItem && !toSlot.acceptsOnlyOneItem) {
                    // Find the inventory slot that the toItem belongs to
                    AbilityInventoryItemData toItemData = null; // Find the item data of the item in the toSlot
                    foreach (Transform child in GameObject.Find("Ability Icons").transform) {
                        if (child.GetComponent<AbilityInventoryItemData>().currentSlot == toSlotNum) {
                            toItemData = child.GetComponent<AbilityInventoryItemData>();
                        }
                    }
                    int returnSlotNum = -1; // Find which slot accepts the item data of the toItem
                    foreach (Transform child in GameObject.Find("Inventory Slots").transform) {
                        if (child.GetComponent<AbilityInventorySlot>().acceptedItem == toItemData) {
                            returnSlotNum = child.GetComponent<AbilityInventorySlot>().slotNum;
                        }
                    }

                    // Send the item in the toSlot into the slot that it belongs to
                    sendItem(toSlotNum, returnSlotNum);

                    // Send the item in the fromSlot into the toSlot
                    sendItem(fromSlotNum, toSlotNum);
                }
            }
        }
    }

    /// Move item from one slot to another slot in the ability inventory menu.
    /// This will replace the data in the destination slot, so use the hold slot (slot #200) to swap item spots.
    void sendItem(int fromSlotNum, int toSlotNum)
    {
        // fromSlot -> the slot with the item being moved.
        // fromItem -> the item being moved.
        // toSlot -> the slot that the item will end up in.

        // Find the item in the fromSlot
        Transform fromItem = null;
        foreach (Transform child in GameObject.Find("Ability Icons").transform) {
            if (child.GetComponent<AbilityInventoryItemData>().currentSlot == fromSlotNum) {
                fromItem = child;
            }
        }

        // Find the AbilityInventorySlot object for the toSlot
        AbilityInventorySlot toSlot = null;
        foreach (Transform child in GameObject.Find("Inventory Slots").transform) {
            if (child.GetComponent<AbilityInventorySlot>().slotNum == toSlotNum) {
                toSlot = child.GetComponent<AbilityInventorySlot>();
            }
        }
        foreach (Transform child in GameObject.Find("Active Ability Slots").transform) {
            if (child.GetComponent<AbilityInventorySlot>().slotNum == toSlotNum) {
                toSlot = child.GetComponent<AbilityInventorySlot>();
            }
        }
        if (GameObject.Find("Hold Slot").GetComponent<AbilityInventorySlot>().slotNum == toSlotNum) {
            toSlot = GameObject.Find("Hold Slot").GetComponent<AbilityInventorySlot>();
        }

        // Send the item in the fromSlot to the toSlot (this part is basically the same as itemAccepted)
        // Basic functionality of an inventory slot from InventorySlot.cs
        fromItem.GetComponent<DragAndDrop>().AcceptItem();
        fromItem.GetComponent<RectTransform>().anchoredPosition = toSlot.GetComponent<RectTransform>().anchoredPosition;

        // Update abilityName and check for duplicate ability names
        toSlot.slotData = fromItem.GetComponent<AbilityInventoryItemData>();
        receivedItem?.Invoke(toSlot.slotData, toSlotNum);

        // Update which slot the item thinks it's in when it moves
        fromItem.GetComponent<AbilityInventoryItemData>().currentSlot = toSlotNum;

        // Update name and level text boxes
        toSlot.setTextboxes();
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

        // Update which slot the item thinks it's in when it moves
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
