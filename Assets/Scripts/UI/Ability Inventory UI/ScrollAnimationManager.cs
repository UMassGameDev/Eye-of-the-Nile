using UnityEngine;
using UnityEngine.UI;

/** \brief
Plays the scroll open and close animation for the Ability Inventory in the Skyhub,
as well as the events that then render the inventory elements.

Documentation updated 4/17/2025
\author Stephen Nuttall
*/
public class ScrollAnimationManager : MonoBehaviour
{
    /// Reference to the scroll background image itself.
    [SerializeField] Image scrollBackground;
    /// Reference to the animator for the inventory background, which animates the opening and closing of the scroll.
    [SerializeField] Animator scrollAnimator;
    /// Reference to the ability inventory UI script, which handles the display and functionality of the ability inventory UI.
    [SerializeField] AbilityInventoryUI abilityInventoryUI;
    /// True if an animation is already playing
    bool animationPlaying = false;

    /// Plays the animation of the scroll opening.
    public void PlayOpenAnimation()
    {
        if (!animationPlaying)
        {
            scrollBackground.enabled = true;
            scrollAnimator.SetTrigger("Open");
            animationPlaying = true;
        }
    }

    /// Plays the animation of the scroll closing.
    public void PlayCloseAnimation()
    {
        if (!animationPlaying)
        {
            scrollAnimator.SetTrigger("Close");
            animationPlaying = true;
        }
    }

    /// Displays the widgets of the ability inventory UI.
    public void OpenInventory()
    {
        animationPlaying = false;
        abilityInventoryUI.OpenInventory();
    }

    /// Hides the widgets of the ability inventory UI.
    public void CloseInventory()
    {
        animationPlaying = false;
        scrollBackground.enabled = false;
    }
}
