/**************************************************
This is the script on the DataManager prefab, which is responsible for storing critical data that needs to persist across scene reloads.
The DataManager is persistent - it can never be destroyed, and there can only be one.
This way, if the player loads a new scene by going through a door, the player object can get its health from the DataManager,
thus “remembering what health it had.”

Documentation updated 8/12/2024
**************************************************/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;  // To make the object persistent, it needs a reference to itself.

    public enum TimeOfDay {Day, Night, Eclipse, BloodMoon};

    TimeOfDayController ToDController;  // Reference to the TimeOfDayController. Used to update the time of day after a scene reload.
    PlayerHealth playerObjHealth;  // Reference to the player’s health. Used to get the MaxHealth of the player on first load.
    GameObject player;  // Reference to the player itself. Used to set the initial respawn point to the player’s position.

    // true if this is not the first time the game has been loaded.
    // This is used to determine if old data should be restored or if the default data should be used.
    static bool gameStarted = false;

    public TimeOfDay defaultTimeOfDay = TimeOfDay.Day;  // Default time of day when the game is first loaded.
    static TimeOfDay currTimeOfDay;  // Time of day that should be restored when a new scene is loaded.

    int playerHealth = 100;  // Player health that should be restored when a new scene is loaded.
    int souls = 0;  // Current amount of souls.
    int godSouls = 0;  // Current amount of god souls.
    
    public static int currSceneIndex { get; private set; }  // The index of the scene that is currently loaded.
    public static int prevSceneIndex { get; private set; }  // The index of the scene that was previously loaded.

    public static string currSceneName { get; private set; }  // The name of the scene that is currently loaded.
    public static string prevSceneName { get; private set; }  // The name of the scene that was previously loaded.

    public static string anubisDeathMessage { get; private set; }  // The joke Anubis will tell when the player dies.

    public static string respawnSceneName { get; private set; }  // The name of the scene the player’s respawn point is in.
    public Vector2 respawnPoint {get; set;}  // The coordinates of the player’s respawn point.

    public static event Action<int> newSoulTotal;  // When the amount of souls the player has changes, this event is invoked.
    public static event Action<int> newGodSoulTotal;  // When the amount of god souls the player has changes, this event is invoked.

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

    // Invokes all events the DataManager has. Usually used when a new scene loads and all these values are updated.
    // Right now, DataManager only has events related to the soul counts,
    // which the display uses to know what number to put on the screen (that’s why it needs to be called when loading a new scene).
    void invokeEvents()
    {
        newSoulTotal?.Invoke(souls);
        newGodSoulTotal?.Invoke(godSouls);
    }

    /******************************
    PRIVATE UPDATE FUNCTIONS
    ******************************/

    // Updates the DataManager’s record of player health. Subscribed to the onPlayerHealthChange event.
    void updatePlayerHealth(int newHealth) { playerHealth = newHealth; }

    // If the currSceneIndex no longer matches the index of the actual current scene, update currSceneIndex and prevSceneIndex.
    void updateSceneIndex()
    {
        if (currSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            prevSceneIndex = currSceneIndex;
            currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
        // Debug.Log("Current Scene Index: " + currSceneIndex + ", Previous Scene Index: " + prevSceneIndex);
    }

    // If the currSceneName no longer matches the index of the actual current scene, update currSceneName and prevSceneName.
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

    // Set the time of day to be either Day, Night, Eclipse, or BloodMoon. (Eclipse and BloodMoon not yet implemented).
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
