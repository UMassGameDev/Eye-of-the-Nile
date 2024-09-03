using UnityEngine;

/** \brief
This goes on day/night shrines, which are objects that allow the player to change the time of day when interacted with (press E).
If the player interacts with this object, it will tell the DataManager to update its record of the time of day.
The DataManager will then trigger an event that tells TimeOfDayController to change the background image.

Documentation updated 9/3/2024
\author Stephen Nuttall
*/
public class DayNight_Shrine : MonoBehaviour
{
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

    /// \brief Depending on the type of shrine this is, set the DataManager's record of the time of day to the corresponding time of day.
    /// This function is attached to an ObjectInteractable component to allow the player to trigger it by pressing E near the shrine.
    public void shrineTrigger()
    {
        switch (shrineType)
        {
            case ShrineType.Day:
                GameObject.Find("DataManager").GetComponent<DataManager>().SetTimeOfDay(DataManager.TimeOfDay.Day);
                break;
            case ShrineType.Night:
                GameObject.Find("DataManager").GetComponent<DataManager>().SetTimeOfDay(DataManager.TimeOfDay.Night);
                break;
        }
    }
}
