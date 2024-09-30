using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/** \brief
This script goes on (as a component) the icons for the abilities on the ability hotbar and handles the drag and drop functionality.

Documentation updated 9/18/2024
\author Roy Pascaul
\note This is a temporary feature and in the future you will not be able to drag the icons around without going to the Skyhub.
*/
public class AbilityImageUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// Reference the the ability info of the ability this icon represents.
    public BaseAbilityInfo CurAbilityInfo { get; set; }
    /// Reference to the image used as the icon for the ability.
    public Image CurImage { get; set; }

    /// The transform of the previous slot this icon was under.
    [HideInInspector] public Transform previousParent;
    /// \brief The transform of the next slot this icon will be under.
    /// This will be set to the correct value by AbilitySlotUI when it's time to drop the icon.
    [HideInInspector] public Transform nextParent;

    /// \brief Runs when the user begins to drag the icon. Prevents further mouse input to prevent conflicts (raycastTarget),
    /// remembers the now previous parent slot, and removes the icon as a child of the icon it was under.
    public void OnBeginDrag(PointerEventData eventData)
    {
        CurImage.raycastTarget = false;
        transform.Find("AbilitySubIconUI").GetComponent<Image>().raycastTarget = false;
        previousParent = transform.parent;
        nextParent = previousParent;
        transform.SetParent(transform.root);
    }

    /// Runs every frame the user is dragging the icon. Sets the position of the icon to match the location of the cursor.
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    /// \brief Runs when the user stops dragging the icon and drops it.
    /// Re-enables mouse inputs (raycastTarget) and sets itself as a child of it's next parent.
    public void OnEndDrag(PointerEventData eventData)
    {
        CurImage.raycastTarget = true;
        transform.Find("AbilitySubIconUI").GetComponent<Image>().raycastTarget = true;
        transform.SetParent(nextParent);
    }

    /// Set the reference to the image used as the icon for this ability.
    void Awake()
    {
        CurImage = GetComponent<Image>();
    }
}
