using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/** \brief
Info icons can be placed in UI menus to tell information to the user when hovered over.

Documentation updated 12/14/2024
\author Alexander Art
*/

public class InfoIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// GameObject of the message to appear when the info icon is hovered over
    [SerializeField] private GameObject infoMessage;

    // Show the message when the info icon is hovered over
    public void OnPointerEnter(PointerEventData eventData)
    {
        infoMessage.SetActive(true);
    }

    // Hide the message when the mouse leaves the info icon
    public void OnPointerExit(PointerEventData eventData)
    {
        infoMessage.SetActive(false);
    }
}