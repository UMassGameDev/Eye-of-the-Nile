/**************************************************
Implements the inventory slots in the ability inventory found in the skyhub.
Adapted from InventorySlot.cs.

Documentation updated 4/2/2024
**************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityInventorySlot : MonoBehaviour, IDropHandler
{
    public string abilityName;
    [SerializeField] bool acceptsOnlyOneItem = true;  // if enabled, the user can only drop acceptedItem into this slot
    [SerializeField] AbilityInventoryItemData acceptedItem;

    public void OnDrop(PointerEventData eventData)
    {
        // if an item is being dragged && that item has the required item data...
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData))
        {
            if (acceptsOnlyOneItem) {
                if (itemData == acceptedItem)
                {
                    // basic functionality of an inventory slot from InventorySlot.cs
                    eventData.pointerDrag.GetComponent<DragAndDrop>().AcceptItem();
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                    // set this slot's ability key to the item's ability key
                    abilityName = itemData.abilityName;
                }
            } else {
                // basic functionality of an inventory slot from InventorySlot.cs
                eventData.pointerDrag.GetComponent<DragAndDrop>().AcceptItem();
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                // set this slot's ability key to the item's ability key
                abilityName = itemData.abilityName;
            }
        }
    }

    public Vector2 GetPosition() { return gameObject.transform.position; }
}
