/**************************************************
Handles changing the background image when the time of day is updated in the DataManager.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class TimeOfDayController : MonoBehaviour
{
    public GameObject DayBckgrnd;
    public GameObject NightBckgrnd;
    // public GameObject EclipseBckgrnd;
    // public GameObject BloodMoonBckgrnd;

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
