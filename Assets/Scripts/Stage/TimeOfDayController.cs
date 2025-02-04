using UnityEngine;

/** \brief
Handles changing the background image when the time of day is updated in the DataManager.

Documentation updated 1/29/2024
\author Stephen Nuttall
\todo Add support for different background in different levels.
\todo Add support for eclipses and blood moons.
*/
public class TimeOfDayController : MonoBehaviour
{
    /// Reference to the child object holding the sprite for the daytime background.
    public GameObject DayBckgrnd;
    /// Reference to the child object holding the sprite for the nighttime background.
    public GameObject NightBckgrnd;
    // public GameObject EclipseBckgrnd;
    // public GameObject BloodMoonBckgrnd;

    /// Disables all active background objects an enables the one corresponding to the set time of day.
    /// <param name="newTimeOfDay">0 = day time, 1 = night time, anything else = error.</param>
    public void SetTimeOfDay(TimeOfDay newTimeOfDay)
    {
        switch (newTimeOfDay)
        {
            case TimeOfDay.Day:
                DayBckgrnd.SetActive(true);
                NightBckgrnd.SetActive(false);
                break;
            case TimeOfDay.Night:
                DayBckgrnd.SetActive(false);
                NightBckgrnd.SetActive(true);
                break;
            default:
                Debug.Log("Failed to change time of day. Input: " + newTimeOfDay.ToString());
                break;
        }
    }
}
