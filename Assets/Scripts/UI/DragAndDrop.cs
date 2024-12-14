/**************************************************
Attach this script to a UI item to allow it to be drag and dropped.

Documentation updated 12/14/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] float defaultOpacity = 1f;
    [SerializeField] float onDragOpacity = 0.6f;
    [SerializeField] Canvas canvas;
    // Set to true for items in the ability inventory (they depend on more conditions)
    [SerializeField] bool abilityInventoryItem = false;

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 startingPos;
    bool accepted = false;  // true if the item has been dragged into an inventory slot that accepts it

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (abilityInventoryItem == true) // If the dragged item is an ability inventory item
        {
            // Prevent empty abilities (the X's) from being dragged
            eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData); // Data of the dragged item
            if (itemData.abilityName == "EMPTY") { return; }

            // Disable raycasts for all ability icons when dragging
            foreach (Transform child in GameObject.Find("Ability Icons").transform) {
                child.GetComponent<Image>().raycastTarget = false;
            }
        }
        
        transform.SetSiblingIndex(100); // Bring the dragged item above its siblings
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = onDragOpacity;
        startingPos = transform.position;
        accepted = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (abilityInventoryItem == true) // If the dragged item is an ability inventory item
        {
            // Prevent empty abilities (the X's) from being dragged
            eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData); // Data of the dragged item
            if (itemData.abilityName == "EMPTY") { return; }
        }

        // Move item to where the cursor is (relative to the canvas)
        rectTransform.anchoredPosition = (eventData.position - canvas.pixelRect.size / 2) / canvas.scaleFactor;
        // (unused alternative) Move item by same amount that the cursor moved each frame (relative to the canvas)
        // rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (abilityInventoryItem == true) // If the dragged item is an ability inventory item
        {
            // Prevent empty abilities (the X's) from being dragged
            eventData.pointerDrag.TryGetComponent<AbilityInventoryItemData>(out var itemData); // Data of the dragged item
            if (itemData.abilityName == "EMPTY") { return; }

            // Re-enable raycasts for all ability icons when no longer dragging
            foreach (Transform child in GameObject.Find("Ability Icons").transform) {
                child.GetComponent<Image>().raycastTarget = true;
            }   
        }

        transform.SetSiblingIndex(0); // Move the dragged item back to the to back layer 
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = defaultOpacity;
        if (!accepted)  // Snap back to starting position if not accepted into a slot
            transform.position = startingPos;
        accepted = false; 
    }

    public void AcceptItem() { accepted = true; }
    public Vector2 GetStartingPos() { return startingPos; }
}
