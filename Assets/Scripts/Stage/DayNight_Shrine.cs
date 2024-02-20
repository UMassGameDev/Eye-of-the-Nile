/**************************************************
This goes on day/night shrines.
If the player interacts with this object, it will tell the DataManager to update the time of day.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class DayNight_Shrine : MonoBehaviour
{
    public enum ShrineType { Day, Night };
    public ShrineType shrineType = ShrineType.Day;

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
