/**************************************************
Put this script on an object you want the player to be able to interact with.
Put the function you want triggered when the player interacts in InvokeOnInteract in the editor.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteractable : MonoBehaviour
{
    public UnityEvent InvokeOnInteract;

    public void triggerInteraction()
    {
        InvokeOnInteract?.Invoke();
    }
}
