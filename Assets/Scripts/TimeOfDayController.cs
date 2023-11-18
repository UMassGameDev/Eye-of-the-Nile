using UnityEngine;
using UnityEngine.UI;

public class TimeOfDayController : MonoBehaviour
{
    public Image DayBckgrnd;
    public Image NightBckgrnd;
    // public Image EclipseBckgrnd;
    // public Image BloodMoonBckgrnd;

    public void setToD(int newTOD)
    {
        switch (newTOD)
        {
            case 0:
                DayBckgrnd.enabled = true;
                NightBckgrnd.enabled = false;
                break;
            case 1:
                DayBckgrnd.enabled = false;
                NightBckgrnd.enabled = true;
                break;
            default:
                Debug.Log("Failed to change time of day. Input: " + newTOD);
                break;
        }
    }
}
