/**************************************************
This is where crucial data that needs to be saved is stored.
The DataManager object will not be destroyed when a new Unity scene is loaded, such as when going through a door.

Documentation updated 1/29/2024
**************************************************/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public enum TimeOfDay {Day, Night, Eclipse, BloodMoon};

    TimeOfDayController ToDController;
    PlayerHealth playerObjHealth;
    GameObject player;

    static bool gameStarted = false;

    public TimeOfDay defaultTimeOfDay = TimeOfDay.Day;
    static TimeOfDay currTimeOfDay;

    int playerHealth = 100;
    int souls = 0;
    int godSouls = 0;
    
    public static int currSceneIndex { get; private set; }
    public static int prevSceneIndex { get; private set; }

    public static string currSceneName { get; private set; }
    public static string prevSceneName { get; private set; }
    public static string anubisDeathMessage { get; private set; }

    public static string respawnSceneName { get; private set; }
    public Vector2 respawnPoint {get; set;}

    public static event Action<int> newSoulTotal;
    public static event Action<int> newGodSoulTotal;

    /******************************
    SUBSCRIBE/UNSUBSCRIBE FROM EVENTS
    ******************************/
    void OnEnable()
    {
        PlayerHealth.onPlayerHealthChange += updatePlayerHealth;
        ObjectHealth.soulsDropped += AddSouls;
        ObjectHealth.godSoulsDropped += AddGodSouls;
        CurrencyWidget.onStart += invokeEvents;
        PlayerHealth.deathMessageChange += updateAnubisDeathMessage;
    }

    // unsubscribe from all events when this object or script is disabled
    void OnDisable()
    {
        PlayerHealth.onPlayerHealthChange -= updatePlayerHealth;
        ObjectHealth.soulsDropped -= AddSouls;
        ObjectHealth.godSoulsDropped -= AddGodSouls;
        CurrencyWidget.onStart -= invokeEvents;
        PlayerHealth.deathMessageChange -= updateAnubisDeathMessage;
    }

    /******************************
    INTERNAL FUNCTIONALITY
    ******************************/

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

        // find components of other game objects we need to acccess
        player = GameObject.Find("Player");
        ToDController = GameObject.Find("BackgroundCanvas").GetComponent<TimeOfDayController>();
        playerObjHealth = player.GetComponent<PlayerHealth>();

        // set default values. Only do this once!
        if (!gameStarted)
        {
            // playerHealth = playerObjHealth.maxHealth;
            playerHealth = playerObjHealth.MaxHealth;
            currTimeOfDay = defaultTimeOfDay;
            anubisDeathMessage = "[DEFAULT]";
            
            gameStarted = true;
        }

        // Things to do when a new scene loads
        invokeEvents();
        updateSceneName();
        updateSceneIndex();
        SetTimeOfDay(currTimeOfDay);

        if (respawnSceneName == null) {
            respawnSceneName = currSceneName;
            respawnPoint = player.transform.position;
            Debug.Log("RespawnSceneName is null! Defaulting to \"" + respawnSceneName + "\" at " + respawnPoint);
        } else if (respawnPoint == null) {
            respawnPoint = player.transform.position;
        }
    }

    void invokeEvents()
    {
        newSoulTotal?.Invoke(souls);
        newGodSoulTotal?.Invoke(godSouls);
    }

    /******************************
    PRIVATE UPDATE FUNCTIONS
    ******************************/

    void updatePlayerHealth(int newHealth) { playerHealth = newHealth; }

    void updateSceneIndex()
    {
        if (currSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            prevSceneIndex = currSceneIndex;
            currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
        // Debug.Log("Current Scene Index: " + currSceneIndex + ", Previous Scene Index: " + prevSceneIndex);
    }

    void updateSceneName()
    {
        if (currSceneName != SceneManager.GetActiveScene().name)
        {
            prevSceneName = currSceneName;
            currSceneName = SceneManager.GetActiveScene().name;
        }
    }

    void updateAnubisDeathMessage(string newDeathMessage) { anubisDeathMessage = newDeathMessage; }

    /******************************
    PUBLIC GETTERS
    ******************************/

    public TimeOfDay GetTimeOfDay() { return currTimeOfDay; }

    public int GetPlayerHealth() { return playerHealth; }

    public int GetSouls() { return souls; }

    public int GetGodSouls() { return godSouls; }

    public int GetPrevSceneIndex() { return prevSceneIndex; }

    public int GetCurrSceneIndex() { return currSceneIndex; }

    public string GetPrevSceneName() { return prevSceneName; }

    public string GetCurrSceneName() { return currSceneName; }
    
    public string GetRespawnSceneName() { return respawnSceneName; }

    public string GetAnubisDeathMessage() { return anubisDeathMessage; }

    /******************************
    PUBLIC UPDATE FUNCTIONS
    ******************************/

    public void SetTimeOfDay(TimeOfDay newTime)
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

    public void UpdateRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        respawnSceneName = currSceneName;
        Debug.Log("New Respawn Point: " + respawnPoint + " in \"" + respawnSceneName + '\"');
    }

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
