/**************************************************
Implements the inventory slots in the ability inventory found in the skyhub.
Adapted from InventorySlot.cs.

Documentation updated 4/2/2024
**************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityInventorySlot : MonoBehaviour, IDropHandler
{
    public string abilityName;
    [SerializeField] int slotNum = -1;
    [SerializeField] bool acceptsOnlyOneItem = true;  // if enabled, the user can only drop acceptedItem into this slot
    [SerializeField] AbilityInventoryItemData acceptedItem;

    public static event Action<string, int> receivedItem;

    void OnEnable()
    {
        receivedItem += checkDuplicateName;
    }

    void OnDisable()
    {
        receivedItem -= checkDuplicateName;
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
        abilityName = itemData.abilityName;
        receivedItem?.Invoke(abilityName, slotNum);
    }

    void checkDuplicateName(string thisName, int thisSlotNum)
    {
        if (thisName == abilityName && thisSlotNum != slotNum)
            abilityName = null;
    }

    public Vector2 GetPosition() { return gameObject.transform.position; }
}
