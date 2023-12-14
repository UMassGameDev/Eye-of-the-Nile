using System;
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
    int souls = 0;
    int godSouls = 0;

    public static event Action<int> newSoulTotal;
    public static event Action<int> newGodSoulTotal;

    void OnEnable()
    {
        PlayerHealth.onPlayerHealthChange += updatePlayerHealth;
        ObjectHealth.soulsDropped += AddSouls;
        ObjectHealth.godSoulsDropped += AddGodSouls;
        CurrencyWidget.onStart += invokeEvents;
    }

    void OnDisable()
    {
        PlayerHealth.onPlayerHealthChange -= updatePlayerHealth;
        ObjectHealth.soulsDropped -= AddSouls;
        ObjectHealth.godSoulsDropped -= AddGodSouls;
        CurrencyWidget.onStart -= invokeEvents;
    }

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
            // playerHealth = playerObjHealth.maxHealth;
            playerHealth = playerObjHealth.MaxHealth;
            currTimeOfDay = defaultTimeOfDay;
            gameStarted = true;
        }

        invokeEvents();
        setTimeOfDay(currTimeOfDay);
    }

    void Start()
    {
        
    }

    void invokeEvents()
    {
        newSoulTotal?.Invoke(souls);
        newGodSoulTotal?.Invoke(godSouls);
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

    void updatePlayerHealth(int newHealth) { playerHealth = newHealth; }

    public TimeOfDay GetTimeOfDay() { return currTimeOfDay; }

    public int GetPlayerHealth() { return playerHealth; }

    public int GetSouls() { return souls; }

    public int GetGodSouls() { return godSouls; }

    public void AddSouls(int numSouls)
    {
        souls += numSouls;
        newSoulTotal?.Invoke(souls);
    }

    public void SubtractSouls(int numSouls)
    {
        souls -= numSouls;
        newSoulTotal?.Invoke(souls);
    }

    public void AddGodSouls(int numSouls)
    {
        godSouls += numSouls;
        newGodSoulTotal?.Invoke(godSouls);
    }

    public void SubtractGodSouls(int numSouls)
    {
        godSouls -= numSouls;
        newGodSoulTotal?.Invoke(godSouls);
    }
}
