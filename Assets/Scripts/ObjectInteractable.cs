using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Put this script on an object you want the player to be able to interact with.
/// Put the function you want triggered when the player interacts in the InvokeOnInteract section in the Unity Editor.
/// If you want the function to trigger after the interact button has been held for a certain amount of time, use the InvokeOnLongPress section.
/// If you want the function to trigger when the player melee attacks the object, use the InvokeOnMelee section.
/// 
/// Documentation updated 1/21/2024
/// </summary>
/// \author Stephen Nuttall
/// \note for InvokeOnMelee to work, the object needs a Collider2D AND be on the interactable layer (set in the top right corner of the inspector).
public class ObjectInteractable : MonoBehaviour
{
    /// <summary> Invokes any functions added to the InvokeOnInteract event. </summary>
    public UnityEvent InvokeOnInteract;
    /// <summary> Invokes any functions added to the InvokeOnLongPress event. </summary>
    public UnityEvent InvokeOnLongPress;
    /// <summary> Invokes any functions added to the InvokeOnMelee event. </summary>
    public UnityEvent InvokeOnMelee;

    /// <summary>
    /// Add the function you want to run when the player interacts with this object to this event in the Unity Editor.
    /// </summary>
    public void triggerInteraction()
    {
        InvokeOnInteract?.Invoke();
    }

    /// <summary>
    /// If you want the function to run after a long press, add it to this event in the Unity Editor.
    /// </summary>
    public void triggerLongPress()
    {
        InvokeOnLongPress?.Invoke();
    }

    /// <summary>
    /// If you want the function to run after the object is meleed, add it to this event in the Unity Editor
    /// </summary>
    public void triggerMelee()
    {
        InvokeOnMelee?.Invoke();
    }
}
