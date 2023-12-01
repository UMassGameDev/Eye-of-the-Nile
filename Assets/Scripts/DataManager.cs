using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public enum TimeOfDay {Day, Night, Eclipse, BloodMoon};

    TimeOfDayController ToDController;
    PlayerHealth playerObjHealth;

    static bool gameStarted = false;

    public TimeOfDay defaultTimeOfDay = TimeOfDay.Day;
    static TimeOfDay currTimeOfDay;
    int playerHealth = 100;

    void Awake()
    {
        // Make sure this instance of the DataManager persists across scenes 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        ToDController = GameObject.Find("BackgroundCanvas").GetComponent<TimeOfDayController>();
        playerObjHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        // set default values. Only do this once!
        if (!gameStarted)
        {
            playerHealth = playerObjHealth.maxHealth;
            currTimeOfDay = defaultTimeOfDay;
            gameStarted = true;
        }

        setTimeOfDay(currTimeOfDay);
    }
    
    public void setTimeOfDay(TimeOfDay newTime)
    {
        // For some reason, the ToDController sometimes gets unassigned. This will ensure that won't happen
        if (ToDController == null)
            ToDController = GameObject.Find("BackgroundCanvas").GetComponent<TimeOfDayController>();

        currTimeOfDay = newTime;
        switch (newTime)
        {
            case TimeOfDay.Day:
                ToDController.setToD(0);
                currTimeOfDay = newTime;
                break;
            case TimeOfDay.Night:
                ToDController.setToD(1);
                currTimeOfDay = newTime;
                break;
        }
    }

    public TimeOfDay GetTimeOfDay()
    {
        return currTimeOfDay;
    }

    public void savePlayerHealth(int newHealth)
    {
        playerHealth = newHealth;
    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }

}
