/**************************************************
Implements the inventory slots in the ability inventory found in the skyhub.
Adapted from InventorySlot.cs.

Documentation updated 4/2/2024
**************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AbilityInventorySlot : MonoBehaviour, IDropHandler
{
    public AbilityInventoryItemData slotData;

    [SerializeField] int slotNum = -1;
    [SerializeField] bool enableDetailsButton = true;

    [SerializeField] bool acceptsOnlyOneItem = true;  // if enabled, the user can only drop acceptedItem into this slot
    [SerializeField] AbilityInventoryItemData acceptedItem;

    public static event Action<AbilityInventoryItemData, int> receivedItem;

    void OnEnable()
    {
        receivedItem += checkDuplicateName;
        AbilityInventoryUI.abilityInventorySlotInitialized += setTextboxes;
    }

    void OnDisable()
    {
        receivedItem -= checkDuplicateName;
        AbilityInventoryUI.abilityInventorySlotInitialized -= setTextboxes;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // if an item is being dragged && that item has the required item data...
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData))
        {
            if (acceptsOnlyOneItem) {
                if (itemData == acceptedItem)
                    itemAccepted(eventData, itemData);
            } else {
                itemAccepted(eventData, itemData);
            }
        }
    }

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

    void checkDuplicateName(AbilityInventoryItemData thisSlot, int thisSlotNum)
    {
        if (thisSlot == slotData && thisSlotNum != slotNum && !acceptsOnlyOneItem)
        {
            slotData = null;
            setTextboxes();
        }
    }

    public Vector2 GetPosition() { return gameObject.transform.position; }
}
