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
                GameObject.Find("DataManager").GetComponent<DataManager>().setTimeOfDay(DataManager.TimeOfDay.Day);
                break;
            case ShrineType.Night:
                GameObject.Find("DataManager").GetComponent<DataManager>().setTimeOfDay(DataManager.TimeOfDay.Night);
                break;
        }
    }
}
