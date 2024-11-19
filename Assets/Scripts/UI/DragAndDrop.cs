/**************************************************
Attach this script to a UI item to allow it to be drag and dropped.

Documentation updated 4/2/2024
**************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] float defaultOpacity = 1f;
    [SerializeField] float onDragOpacity = 0.6f;
    [SerializeField] Canvas canvas;

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
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = onDragOpacity;
        startingPos = transform.position;
    }

    // Move item to where the cursor is (relative to the canvas)
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = (eventData.position - canvas.pixelRect.size / 2) / canvas.scaleFactor;
        // rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // (unused) Move item by same amount that the cursor moved each frame (relative to the canvas)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = defaultOpacity;
        if (!accepted)  // snap back to starting position if not accepted into a slot
            transform.position = startingPos;
        accepted = false;
    }

    public void AcceptItem() { accepted = true; }
}
