using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the script on the DataManager prefab, which is responsible for storing critical data that needs to persist across scene reloads.
/// The DataManager is persistent - it can never be destroyed, and there can only be one.
/// This way, if the player loads a new scene by going through a door, the player object can get its health from the DataManager,
/// thus “remembering what health it had.”
/// 
/// Documentation updated 8/12/2024
/// </summary>
/// \author Stephen Nuttall
/// \todo Saving & auto-saving. Writing data to a file so it can restored after the game is closed.
public class DataManager : MonoBehaviour
{
    /// <summary> Possible values for the time of day. Eclipse and BloodMoon not yet implemented. </summary>
    public enum TimeOfDay {Day, Night, Eclipse, BloodMoon};
    
    /** @name Reference Variables
    *  These are references to other objects this script needs to get information from.
    */
    ///@{
    /// <summary> To make the object persistent, it needs a reference to itself. </summary>
    public static DataManager Instance;
    /// <summary> Reference to the TimeOfDayController. Used to update the time of day after a scene reload. </summary>
    TimeOfDayController ToDController;
    /// <summary> Reference to the player’s health. Used to get the MaxHealth of the player on first load. </summary>
    PlayerHealth playerObjHealth;
    /// <summary> Reference to the player itself. Used to set the initial respawn point to the player’s position. </summary>
    GameObject player;
    ///@}

    /// <summary>
    /// True if this is not the first time the game has been loaded. <summary>
    /// This is used to determine if old data should be restored or if the default data should be used.
    /// <summary>
    static bool gameStarted = false;

    /// <summary> Default time of day when the game is first loaded. <summary>
    public TimeOfDay defaultTimeOfDay = TimeOfDay.Day;
    /// <summary> Time of day that should be restored when a new scene is loaded. <summary>
    static TimeOfDay currTimeOfDay;

    /// <summary> Player health that should be restored when a new scene is loaded. <summary>
    int playerHealth = 100;
    /// <summary> Current amount of souls. <summary>
    int souls = 0;
    /// <summary> Current amount of god souls. <summary>
    int godSouls = 0;
    
    /// <summary> The index of the scene that is currently loaded. <summary>
    /// 
    public static int currSceneIndex { get; private set; }
    /// <summary> The index of the scene that was previously loaded. <summary>
    /// <value></value> 
    public static int prevSceneIndex { get; private set; }

    /// <summary> The name of the scene that is currently loaded. <summary>
    /// <value></value>
    public static string currSceneName { get; private set; }
    /// <summary> The name of the scene that was previously loaded. <summary>
    /// <value></value>
    public static string prevSceneName { get; private set; }

    /// <summary> The joke Anubis will tell when the player dies. </summary>
    /// <value></value>
    public static string anubisDeathMessage { get; private set; }

    /// <summary> The name of the scene the player’s respawn point is in. </summary>
    /// <value></value>
    public static string respawnSceneName { get; private set; }
    /// <summary> The coordinates of the player’s respawn point. <summary>
    /// <value></value>
    public Vector2 respawnPoint {get; set;}

    /// <summary> When the amount of souls the player has changes, this event is invoked. <summary>
    public static event Action<int> newSoulTotal;
    /// <summary> When the amount of god souls the player has changes, this event is invoked. <summary>
    public static event Action<int> newGodSoulTotal;

    /******************************
    INTERNAL FUNCTIONALITY
    ******************************/
    /** @name Internal Functionality
    *  These functions handle the inner workings of the data manager, ensuring all the data is recorded and restored correctly after a scene reload.
    */
    ///@{
    
    /// <summary>
    /// Subscribe all functions that need to monitor other functionality to the corresponding events.
    /// </summary>
    void OnEnable()
    {
        PlayerHealth.onPlayerHealthChange += updatePlayerHealth;
        ObjectHealth.soulsDropped += AddSouls;
        ObjectHealth.godSoulsDropped += AddGodSouls;
        CurrencyWidget.onStart += invokeEvents;
        PlayerHealth.deathMessageChange += updateAnubisDeathMessage;
    }

    /// <summary>
    /// Unsubscribe from all events when this object or script is disabled.
    /// </summary>
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
        /// Make sure this instance of the DataManager persists across scenes 
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

    /// <summary>
    /// Updates the DataManager’s record of player health. Subscribed to the onPlayerHealthChange event.
    /// </summary>
    /// <param name="newHealth"></param>
    void updatePlayerHealth(int newHealth) { playerHealth = newHealth; }

    /// <summary>
    /// If the currSceneIndex no longer matches the index of the actual current scene, update currSceneIndex and prevSceneIndex.
    /// </summary>
    void updateSceneIndex()
    {
        if (currSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            prevSceneIndex = currSceneIndex;
            currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
        // Debug.Log("Current Scene Index: " + currSceneIndex + ", Previous Scene Index: " + prevSceneIndex);
    }

    /// <summary>
    /// If the currSceneName no longer matches the index of the actual current scene, update currSceneName and prevSceneName.
    /// </summary>
    void updateSceneName()
    {
        if (currSceneName != SceneManager.GetActiveScene().name)
        {
            prevSceneName = currSceneName;
            currSceneName = SceneManager.GetActiveScene().name;
        }
    }

    /// <summary>
    /// Sets the joke Anubis will tell to the given string. Subscribes to the PlayerHealth.deathMessageChange event.
    /// </summary>
    /// <param name="newDeathMessage"></param> <summary>
    /// Joke that Anubis will tell.
    /// </summary>
    /// <param name="newDeathMessage"></param>
    void updateAnubisDeathMessage(string newDeathMessage) { anubisDeathMessage = newDeathMessage; }
    ///@}

    /******************************
    PUBLIC GETTERS
    ******************************/
    /** @name Public Getters
    *  These functions allow other scripts to access (but not change) the data stored in the data manager.
    */
    ///@{

    /// <summary>
    /// Returns current time of day as a TimeOfDay enum.
    /// </summary>
    public TimeOfDay GetTimeOfDay() { return currTimeOfDay; }

    /// <summary>
    /// Returns the player's health.
    /// </summary>
    public int GetPlayerHealth() { return playerHealth; }

    /// <summary>
    /// Returns number of souls the player has.
    /// </summary>
    public int GetSouls() { return souls; }

    /// <summary>
    /// Returns number of god souls the player has.
    /// </summary>
    public int GetGodSouls() { return godSouls; }

    /// <summary>
    /// Returns the index of the previous scene.
    /// </summary>
    public int GetPrevSceneIndex() { return prevSceneIndex; }

    /// <summary>
    /// Returns the index of the current scene.
    /// </summary>
    public int GetCurrSceneIndex() { return currSceneIndex; }

    /// <summary>
    /// Returns the name of the previous scene.
    /// </summary>
    public string GetPrevSceneName() { return prevSceneName; }

    /// <summary>
    /// Returns the name of the current scene.
    /// </summary>
    public string GetCurrSceneName() { return currSceneName; }
    
    /// <summary>
    /// Returns the name of the scene with the player's respawn point.
    /// </summary>
    public string GetRespawnSceneName() { return respawnSceneName; }

    /// <summary>
    /// Returns the joke Anubis will tell when the player dies.
    /// </summary>
    public string GetAnubisDeathMessage() { return anubisDeathMessage; }
    ///@}

    /******************************
    PUBLIC UPDATE FUNCTIONS
    ******************************/
    /** @name Public Update Functions
    *  These functions allow other scripts to update data stored in the data manager.
    */
    ///@{

    /// <summary>
    /// Set the time of day to be either Day, Night, Eclipse, or BloodMoon. (Eclipse and BloodMoon not yet implemented).
    /// </summary>
    /// <param name="newTime"></param>
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

    /// <summary>
    /// Sets the player’s respawn point the given position in the current scene.
    /// </summary>
    /// <param name="newRespawnPoint"></param>
    public void UpdateRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        respawnSceneName = currSceneName;
        Debug.Log("New Respawn Point: " + respawnPoint + " in \"" + respawnSceneName + '\"');
    }

    /// <summary>
    /// Adds the given amount of souls. Subscribes to the ObjectHealth.soulsDropped event.
    /// </summary>
    /// <param name="numSouls"></param>
    public void AddSouls(int numSouls)
    {
        souls += numSouls;
        newSoulTotal?.Invoke(souls);
    }

    /// <summary>
    /// Subtracts the given amount of souls.
    /// </summary>
    /// <param name="numSouls"></param>
    public void SubtractSouls(int numSouls)
    {
        souls -= numSouls;
        newSoulTotal?.Invoke(souls);
    }

    /// <summary>
    /// Adds the given amount of god souls. Subscribes to the ObjectHealth.godSoulsDropped event.
    /// </summary>
    /// <param name="numSouls"></param>
    public void AddGodSouls(int numSouls)
    {
        godSouls += numSouls;
        newGodSoulTotal?.Invoke(godSouls);
    }

    /// <summary>
    /// Subtracts the given amount of god souls.
    /// </summary>
    /// <param name="numSouls"></param>
    public void SubtractGodSouls(int numSouls)
    {
        godSouls -= numSouls;
        newGodSoulTotal?.Invoke(godSouls);
    }
    ///@}
}
