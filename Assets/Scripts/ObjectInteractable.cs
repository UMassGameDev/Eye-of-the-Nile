/**************************************************
Put this script on an object you want the player to be able to interact with.
Put the function you want triggered when the player interacts in the InvokeOnInteract section in the editor.
If you want the function to trigger after the interact button has been held for a certain amount of time, use the InvokeOnLongPress section.
If you want the function to trigger when the player melee attacks the object, use the InvokeOnMelee section.

Note: for InvokeOnMelee to work, the object needs a Collider2D.

Documentation updated 1/21/2024
**************************************************/
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteractable : MonoBehaviour
{
    public UnityEvent InvokeOnInteract;
    public UnityEvent InvokeOnLongPress;
    public UnityEvent InvokeOnMelee;

    public void triggerInteraction()
    {
        InvokeOnInteract?.Invoke();
    }

    public void triggerLongPress()
    {
        InvokeOnLongPress?.Invoke();
    }

    public void triggerMelee()
    {
        InvokeOnMelee?.Invoke();
    }
}
