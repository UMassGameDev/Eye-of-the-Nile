using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// \brief
/// This is the script on the DataManager prefab, which is responsible for storing critical data that needs to persist across scene reloads.
/// The DataManager is persistent - it can never be destroyed, and there can only be one.
/// This way, if the player loads a new scene by going through a door, the player object can get its health from the DataManager,
/// thus “remembering what health it had.”
/// 
/// Documentation updated 9/15/2024
/// \author Stephen Nuttall
/// \todo Saving & auto-saving. Writing data to a file so it can restored after the game is closed.
public class DataManager : MonoBehaviour
{
    /// Possible values for the time of day. Eclipse and BloodMoon not yet implemented. 
    public enum TimeOfDay {Day, Night, Eclipse, BloodMoon};
    
    /** @name Reference Variables
    *  These are references to other objects this script needs to get information from.
    */
    ///@{
    /// To make the object persistent, it needs a reference to itself. 
    public static DataManager Instance;
    /// Reference to the TimeOfDayController. Used to update the time of day after a scene reload. 
    TimeOfDayController ToDController;
    /// Reference to the player’s health. Used to get the MaxHealth of the player on first load. 
    PlayerHealth playerObjHealth;
    /// Reference to the player itself. Used to set the initial respawn point to the player’s position. 
    GameObject player;
    ///@}

    /// \brief True if this is not the first time the game has been loaded. 
    /// This is used to determine if old data should be restored or if the default data should be used.
    static bool gameStarted = false;

    /// Default time of day when the game is first loaded. 
    public TimeOfDay defaultTimeOfDay = TimeOfDay.Day;
    /// Time of day that should be restored when a new scene is loaded. 
    static TimeOfDay currTimeOfDay;

    /// Player health that should be restored when a new scene is loaded. 
    int playerHealth = 100;
    /// Current amount of souls. 
    int souls = 0;
    /// Current amount of god souls. 
    int godSouls = 0;
    
    /// The index of the scene that is currently loaded.
    public static int currSceneIndex { get; private set; }
    /// The index of the scene that was previously loaded. 
    public static int prevSceneIndex { get; private set; }

    /// The name of the scene that is currently loaded.
    public static string currSceneName { get; private set; }
    /// The name of the scene that was previously loaded.
    public static string prevSceneName { get; private set; }

    /// The joke Anubis will tell when the player dies.
    public static string anubisDeathMessage { get; private set; }

    /// The name of the scene the player’s respawn point is in.
    public static string respawnSceneName { get; private set; }
    /// The coordinates of the player’s respawn point.
    public Vector2 respawnPoint {get; set;}

    /// When the amount of souls the player has changes, this event is invoked. 
    public static event Action<int> newSoulTotal;
    /// When the amount of god souls the player has changes, this event is invoked. 
    public static event Action<int> newGodSoulTotal;

    /// Value from 0 to 1, accessible from the settings menu, and this sets the default. (Does nothing yet! Eventually, it should control the volume.)
    public float volume = 1f;

    /******************************
    INTERNAL FUNCTIONALITY
    ******************************/
    /** @name Internal Functionality
    *  These functions handle the inner workings of the data manager, ensuring all the data is recorded and restored correctly after a scene reload.
    */
    ///@{
    
    /// Subscribe all functions that need to monitor other functionality to the corresponding events.
    void OnEnable()
    {
        PlayerHealth.onPlayerHealthChange += updatePlayerHealth;
        ObjectHealth.soulsDropped += AddSouls;
        ObjectHealth.godSoulsDropped += AddGodSouls;
        CurrencyWidget.onStart += invokeEvents;
        PlayerHealth.deathMessageChange += updateAnubisDeathMessage;
    }

    /// Unsubscribe from all events when this object or script is disabled.
    void OnDisable()
    {
        PlayerHealth.onPlayerHealthChange -= updatePlayerHealth;
        ObjectHealth.soulsDropped -= AddSouls;
        ObjectHealth.godSoulsDropped -= AddGodSouls;
        CurrencyWidget.onStart -= invokeEvents;
        PlayerHealth.deathMessageChange -= updateAnubisDeathMessage;
    }

    /// <summary>
    /// This is where most of the functionality of the DataManager happens, since Awake() is called right as the scene loads. Here are the steps that run when Awake() is called:
    /// First, we make sure this DataManager is the only DataManager, and it will not be destroyed in the next scene reload.
    /// Then, we find the references for the reference variables listed above.
    /// If the game hasn’t been loaded before (gameStarted == false), we will set the default values. Otherwise, the values we already have are the correct ones.
    /// Finally, these values are restored in their respective objects.
    /// We also check if respawnSceneName or respawnPoint are null. If so, they default to currentScene and the player’s current position.
    /// </summary>
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

    /// <summary>
    /// Invokes all events the DataManager has. Usually used when a new scene loads and all these values are updated.
    /// Right now, DataManager only has events related to the soul counts,
    /// which the display uses to know what number to put on the screen (that’s why it needs to be called when loading a new scene).
    /// </summary>
    void invokeEvents()
    {
        newSoulTotal?.Invoke(souls);
        newGodSoulTotal?.Invoke(godSouls);
    }
    ///@}

    /******************************
    PRIVATE UPDATE FUNCTIONS
    ******************************/
    /** @name Private Update Functions
    *  These functions are helper functions that update data when called by Awake().
    */
    ///@{

    /// Updates the DataManager’s record of player health. Subscribed to the onPlayerHealthChange event.
    void updatePlayerHealth(int newHealth) { playerHealth = newHealth; }

    /// If the currSceneIndex no longer matches the index of the actual current scene, update currSceneIndex and prevSceneIndex.
    void updateSceneIndex()
    {
        if (currSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            prevSceneIndex = currSceneIndex;
            currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
        // Debug.Log("Current Scene Index: " + currSceneIndex + ", Previous Scene Index: " + prevSceneIndex);
    }

    /// If the currSceneName no longer matches the index of the actual current scene, update currSceneName and prevSceneName.
    void updateSceneName()
    {
        if (currSceneName != SceneManager.GetActiveScene().name)
        {
            prevSceneName = currSceneName;
            currSceneName = SceneManager.GetActiveScene().name;
        }
    }

    /// Sets the joke Anubis will tell to the given string. Subscribes to the PlayerHealth.deathMessageChange event.
    /// <param name="newDeathMessage">Joke that Anubis will tell.</param>
    void updateAnubisDeathMessage(string newDeathMessage) { anubisDeathMessage = newDeathMessage; }
    ///@}

    /******************************
    PUBLIC GETTERS
    ******************************/
    /** @name Public Getters
    *  These functions allow other scripts to access (but not change) the data stored in the data manager.
    */
    ///@{

    /// Returns current time of day as a TimeOfDay enum.
    public TimeOfDay GetTimeOfDay() { return currTimeOfDay; }

    /// Returns the player's health.
    public int GetPlayerHealth() { return playerHealth; }

    /// Returns number of souls the player has.
    public int GetSouls() { return souls; }

    /// Returns number of god souls the player has.
    public int GetGodSouls() { return godSouls; }

    /// Returns the index of the previous scene.
    public int GetPrevSceneIndex() { return prevSceneIndex; }

    /// Returns the index of the current scene.
    public int GetCurrSceneIndex() { return currSceneIndex; }

    /// Returns the name of the previous scene.
    public string GetPrevSceneName() { return prevSceneName; }

    /// Returns the name of the current scene.
    public string GetCurrSceneName() { return currSceneName; }
    
    /// Returns the name of the scene with the player's respawn point.
    public string GetRespawnSceneName() { return respawnSceneName; }

    /// Returns the joke Anubis will tell when the player dies.
    public string GetAnubisDeathMessage() { return anubisDeathMessage; }

    /// Returns the current volume setting.
    public float GetVolume() { return volume; }
    ///@}

    /******************************
    PUBLIC UPDATE FUNCTIONS
    ******************************/
    /** @name Public Update Functions
    *  These functions allow other scripts to update data stored in the data manager.
    */
    ///@{

    /// Set the time of day to be either Day, Night, Eclipse, or BloodMoon. (Eclipse and BloodMoon not yet implemented).
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

    /// Sets the player’s respawn point the given position in the current scene.
    /// <param name="newRespawnPoint">The coordinates in the respawn scene the respawn point is located.</param>
    public void UpdateRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        respawnSceneName = currSceneName;
        Debug.Log("New Respawn Point: " + respawnPoint + " in \"" + respawnSceneName + '\"');
    }

    /// Adds the given amount of souls. Subscribes to the ObjectHealth.soulsDropped event.
    public void AddSouls(int numSouls)
    {
        souls += numSouls;
        newSoulTotal?.Invoke(souls);
    }

    /// Subtracts the given amount of souls.
    public void SubtractSouls(int numSouls)
    {
        souls -= numSouls;
        newSoulTotal?.Invoke(souls);
    }

    /// Adds the given amount of god souls. Subscribes to the ObjectHealth.godSoulsDropped event.
    public void AddGodSouls(int numSouls)
    {
        godSouls += numSouls;
        newGodSoulTotal?.Invoke(godSouls);
    }

    /// Subtracts the given amount of god souls.
    public void SubtractGodSouls(int numSouls)
    {
        godSouls -= numSouls;
        newGodSoulTotal?.Invoke(godSouls);
    }
    ///@}
}
