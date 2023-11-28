using UnityEngine;
using UnityEngine.UI;

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
