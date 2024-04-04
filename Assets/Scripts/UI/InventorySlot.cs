/**************************************************
Implements a slot in some inventory that an item can be drag and dropped into.
This is a generic version that can be inherited from if you want the slot to store data or only accept certain items.

Documentation updated 4/2/2024
**************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<DragAndDrop>().AcceptItem();
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
