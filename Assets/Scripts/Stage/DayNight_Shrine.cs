using UnityEngine;

/** \brief
This goes on day/night shrines, which are objects that allow the player to change the time of day when interacted with (press E).
If the player interacts with this object, it will tell the DataManager to update its record of the time of day.
The DataManager will then trigger an event that tells TimeOfDayController to change the background image.
This script is also used for controlling whether or not the shrine should be animated when the player gets in range.

Documentation updated 4/7/2025
\author Stephen Nuttall
*/
public class DayNight_Shrine : MonoBehaviour
{
    /// Referene to the DataManager.
    private DataManager dataManager;
    /// Reference to the shrine's animator.
    private Animator animator;
    
    /// Possible types of shrines.
    public enum ShrineType
    {
        /// Day shrine (Shrine of Ra) allows the player to change the time of day to day time.
        Day,
        /// Night shrine (Shrine of Thoth) allows the player to change the time of day to night time.
        Night
    };
    /// The type of shrine this shrine is.
    public ShrineType shrineType = ShrineType.Day;

    /// Set reference to DataManager and the Animator.
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        animator = GetComponent<Animator>();
    }

    /// \brief Depending on the type of shrine this is, set the DataManager's record of the time of day to the corresponding time of day.
    /// This function is attached to an ObjectInteractable component to allow the player to trigger it by pressing E near the shrine.
    public void shrineTrigger()
    {
        switch (shrineType)
        {
            case ShrineType.Day:
                dataManager.SetTimeOfDay(TimeOfDay.Day);
                animator.SetBool("InRange", false);
                break;
            case ShrineType.Night:
                dataManager.SetTimeOfDay(TimeOfDay.Night);
                animator.SetBool("InRange", false);
                break;
        }
    }

    /// \brief If the player is within close proximity to the shrine, make it animated only if the time of day is right.
    public void playerNearShrine()
    {
        if (shrineType == ShrineType.Day && dataManager.GetTimeOfDay() == TimeOfDay.Night)
        {
            animator.SetBool("InRange", true);
        }
        else if (shrineType == ShrineType.Night && dataManager.GetTimeOfDay() == TimeOfDay.Day)
        {
            animator.SetBool("InRange", true);
        }
    }

    /// \brief If the player leaves the shrine's range, make it not animated.
    public void playerAwayFromShrine()
    {
        animator.SetBool("InRange", false);
    }
}
