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
    /// <param name="newTOD">0 = day time, 1 = night time, anything else = error.</param>
    public void setToD(int newTOD)
    {
        switch (newTOD)
        {
            case 0:
                DayBckgrnd.SetActive(true);
                NightBckgrnd.SetActive(false);
                break;
            case 1:
                DayBckgrnd.SetActive(false);
                NightBckgrnd.SetActive(true);
                break;
            default:
                Debug.Log("Failed to change time of day. Input: " + newTOD);
                break;
        }
    }
}
