using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public enum TimeOfDay {Day, Night, Eclipse, BloodMoon};

    TimeOfDayController ToDController;
    PlayerHealth playerObjHealth;

    static bool gameStarted = false;

    TimeOfDay currTimeOfDay = TimeOfDay.Day;
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
            gameStarted = true;
        }

        setTimeOfDay(currTimeOfDay);
    }
    
    public void setTimeOfDay(TimeOfDay newTime)
    {
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
