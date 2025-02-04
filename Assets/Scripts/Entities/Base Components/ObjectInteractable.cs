using UnityEngine;
using UnityEngine.Events;

/// \brief
/// Put this script on an object you want the player to be able to interact with.
/// Put the function you want triggered when the player interacts in the InvokeOnInteract section in the Unity Editor.
/// If you want the function to trigger after the interact button has been held for a certain amount of time, use the InvokeOnLongPress section.
/// If you want the function to trigger when the player melee attacks the object, use the InvokeOnMelee section.
/// 
/// Documentation updated 9/15/2024
/// \author Stephen Nuttall
/// \note for InvokeOnMelee to work, the object needs a Collider2D AND be on the interactable layer (set in the top right corner of the inspector).
public class ObjectInteractable : MonoBehaviour
{
    /// \brief Invokes any functions added to the InvokeOnInteract event.
    public UnityEvent InvokeOnInteract;
    /// \brief Invokes any functions added to the InvokeOnLongPress event.
    public UnityEvent InvokeOnLongPress;
    /// \brief Invokes any functions added to the InvokeOnMelee event.
    public UnityEvent InvokeOnMelee;

    /// \brief Add the function you want to run when the player interacts with this object to this event in the Unity Editor.
    public void triggerInteraction()
    {
        InvokeOnInteract?.Invoke();
    }

    /// \brief If you want the function to run after a long press, add it to this event in the Unity Editor.
    public void triggerLongPress()
    {
        InvokeOnLongPress?.Invoke();
    }

    /// \brief If you want the function to run after the object is meleed, add it to this event in the Unity Editor
    public void triggerMelee()
    {
        InvokeOnMelee?.Invoke();
    }
}
