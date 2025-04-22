using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Possible values for the time of day. Eclipse and BloodMoon not yet implemented. 
public enum TimeOfDay { Day, Night, Eclipse, BloodMoon };

/// \brief
/// This is the script on the DataManager prefab, which is responsible for storing critical data that needs to persist across scene reloads.
/// Additionally, when the game is run through the Main Menu scene, the DataManager will load values from a file rather than using defaults.
/// It will also save these values back to the file while the game is running, and when the application quits.
/// The DataManager is persistent - it can never be destroyed, and there can only be one.
/// This way, if the player loads a new scene by going through a door, the player object can get its health from the DataManager,
/// thus “remembering what health it had.”
/// 
/// Documentation updated 2/3/2025
/// \author Stephen Nuttall
public class DataManager : MonoBehaviour
{
    /** @name Reference Variables
    *  These are references to other objects this script needs to get information from.
    */
    ///@{
    /// To make the object persistent, it needs a reference to itself. 
    public static DataManager Instance;
    /// Reference to the TimeOfDayController. Used to update the time of day after a scene reload. 
    TimeOfDayController ToDController;
    /// Reference to the player itself. Used to set the initial respawn point to the player’s position. 
    GameObject player;
    ///@}

    /** @name I/O attributes
    *  Variables and objects used to save and load the game from a file.
    */
    ///@{
    /// Name of the file to save the game to, and load the game from.
    [SerializeField] string saveFileName;
    /// Controls whether or not autosaving is enabled. Can be disabled mid-play in the Unity Editor, but not restarted.
    [SerializeField] bool autosaveEnabled = true;
    /// How often the game should run the autosave function.
    [SerializeField] float autosaveFrequency = 3f;
    /// Class used to read and write data to a file
    FileReadWrite fileReadWrite;
    /// True if no save file was found
    public bool noSaveFileFound { get; private set; }
    /// True if the saving system is enabled at all (false when any scene other than the MainMenu is loaded)
    bool readWriteEnabled = false;
    ///@}

    /** @name General Data
    *  Data related to the player, time of day, soul counts, and more.
    */
    ///@{
    /// Player health that should be restored when a new scene is loaded. 
    int playerHealth = 100;
    /// The amount of health potions the player currently has.
    int healthPotionCount = 0;
    /// Default time of day when the game is first loaded. 
    [SerializeField] TimeOfDay defaultTimeOfDay = TimeOfDay.Day;
    /// Time of day that should be restored when a new scene is loaded. 
    static TimeOfDay currTimeOfDay;
    /// Current amount of souls. 
    int souls = 0;
    /// Current amount of god souls. 
    int godSouls = 0;
    /// The joke Anubis will tell when the player dies.
    public static string anubisDeathMessage { get; private set; }
    /// Whether the ability hotbar is unlocked yet or not.
    public bool abilitiesUnlocked { get; private set; } = false;
    ///@}

    /** @name Scene Data
    *  Details about the current scene, previous scene, and the respawn scene.
    */
    ///@{
    /// The index of the scene that is currently loaded.
    public static int currSceneIndex { get; private set; }
    /// The index of the scene that was previously loaded. 
    public static int prevSceneIndex { get; private set; }
    /// The name of the scene that is currently loaded.
    public static string currSceneName { get; private set; }
    /// The name of the scene that was previously loaded.
    public static string prevSceneName { get; private set; }
    /// The name of the scene the player’s respawn point is in.
    public static string respawnSceneName { get; private set; }
    /// The coordinates of the player’s respawn point.
    public Vector2 respawnPoint { get; private set; }
    ///@}

    /** @name Volume Settings
    *  The volume gets multiplied by these values in AudioManager.cs (values ranging from 0 to 1, accessible from the settings menu).
    */
    ///@{
    /// The master volume gets multiplied by this values in AudioManager.cs (values ranging from 0 to 1).
    public float masterVolumeSetting = 1f;
    /// The music volume gets multiplied by this values in AudioManager.cs (values ranging from 0 to 1).
    public float musicVolumeSetting = 1f;
    /// The SFX volume gets multiplied by this values in AudioManager.cs (values ranging from 0 to 1).
    public float sfxVolumeSetting = 1f;
    ///@}

    /** @name Events
    *  Events that trigger when certain values are updated.
    */
    ///@{
    /// When the amount of souls the player has changes, this event is invoked. 
    public static event Action<int> newSoulTotal;
    /// When the amount of god souls the player has changes, this event is invoked. 
    public static event Action<int> newGodSoulTotal;
    ///@}

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
        PlayerItemHolder.potionCountChanged += updatePotionCount;
    }

    /// Unsubscribe from all events when this object or script is disabled.
    void OnDisable()
    {
        PlayerHealth.onPlayerHealthChange -= updatePlayerHealth;
        ObjectHealth.soulsDropped -= AddSouls;
        ObjectHealth.godSoulsDropped -= AddGodSouls;
        CurrencyWidget.onStart -= invokeEvents;
        PlayerHealth.deathMessageChange -= updateAnubisDeathMessage;
        PlayerItemHolder.potionCountChanged -= updatePotionCount;
    }

    /// \brief This is where most of the functionality of the DataManager happens, since Awake() is called right as the scene loads. Here are the steps that run when Awake() is called:
    /// First, we make sure this DataManager is the only DataManager, and it will not be destroyed in the next scene reload.
    /// If we're in the main menu scene (build index 0), enable the read/write system and read the save from file.
    /// Otherwise, start by finding the references for the reference variables listed above.
    /// Finally, these values are restored in their respective objects.
    /// We also check if respawnSceneName or respawnPoint are null. If so, they default to currentScene and the player’s current position.
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

        // initialize FileReadWrite object.
        fileReadWrite = new(saveFileName);

        // if we're in the Main Menu...
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            readWriteEnabled = true;
            readSaveFile();
        }
        else // otherwise...
        {
            // find components of other game objects we need to acccess
            player = GameObject.Find("Player");
            ToDController = GameObject.Find("BackgroundCanvas").GetComponent<TimeOfDayController>();

            // Things to do when a new scene loads
            invokeEvents();
            updateSceneName();
            updateSceneIndex();
            SetTimeOfDay(currTimeOfDay);

            if (respawnSceneName == null)
            {
                respawnSceneName = currSceneName;
                respawnPoint = player.transform.position;
                Debug.Log("RespawnSceneName is null! Defaulting to \"" + respawnSceneName + "\" at " + respawnPoint);
            }
            else if (respawnPoint == null)
            {
                respawnPoint = player.transform.position;
            }
        }
    }

    /// Start the autosave loop and update the volume levels.
    void Start()
    {
        StartCoroutine(autosaveLoop());
        AudioManager.instance.VolumeChanged();
    }

    /// \brief Invokes all events the DataManager has. Usually used when a new scene loads and all these values are updated.
    /// Right now, DataManager only has events related to the soul counts,
    /// which the display uses to know what number to put on the screen (that’s why it needs to be called when loading a new scene).
    void invokeEvents()
    {
        newSoulTotal?.Invoke(souls);
        newGodSoulTotal?.Invoke(godSouls);
    }
    ///@}

    /******************************
    SAVING AND LOADING
    ******************************/
    /** @name Saving and Loading
    *  These functions are used to save and load the game from a file.
    */
    ///@{

    /// Read from the save file and restore those values. If there is no save file, load the default values.
    void readSaveFile()
    {
        SerializedData saveData = fileReadWrite.ReadData();

        if (saveData != null)
        {
            playerHealth = saveData.playerHealth;
            healthPotionCount = saveData.healthPotionCount;
            currTimeOfDay = saveData.currTimeOfDay;
            souls = saveData.souls;
            godSouls = saveData.godSouls;

            currSceneIndex = saveData.currSceneIndex;
            currSceneName = saveData.currSceneName;
            prevSceneIndex = saveData.prevSceneIndex;
            prevSceneName = saveData.prevSceneName;
            respawnSceneName = saveData.respawnSceneName;
            respawnPoint = new Vector2(saveData.respawnPoint_X, saveData.respawnPoint_Y);

            masterVolumeSetting = saveData.masterVolumeSetting;
            musicVolumeSetting = saveData.musicVolumeSetting;
            sfxVolumeSetting = saveData.sfxVolumeSetting;

            noSaveFileFound = false;
        }
        else
        {
            currTimeOfDay = defaultTimeOfDay;
            anubisDeathMessage = "[DEFAULT]";

            noSaveFileFound = true;
        }
    }

    /// Write various values to the save file.
    void writeSaveFile()
    {
        if (readWriteEnabled)
            fileReadWrite.WriteData(this);
    }

    IEnumerator autosaveLoop()
    {
        while (autosaveEnabled && readWriteEnabled)
        {
            writeSaveFile();
            yield return autosaveFrequency;
        }
    }

    void OnApplicationQuit()
    {
        writeSaveFile();
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

    void updatePotionCount(int potionCount) { healthPotionCount = potionCount; }

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

    /// Returns the name of the scene with the player's respawn point.
    public Vector2 GetRespawnPoint() { return respawnPoint; }

    /// Returns the joke Anubis will tell when the player dies.
    public string GetAnubisDeathMessage() { return anubisDeathMessage; }

    /// Returns the current master volume setting.
    public float GetMasterVolume() { return masterVolumeSetting; }

    /// Returns the current music volume setting.
    public float GetMusicVolume() { return musicVolumeSetting; }

    /// Returns the current SFX volume setting.
    public float GetSFXVolume() { return sfxVolumeSetting; }

    /// Returns the current number of health potions the player has.
    public int GetHealthPotionCount() { return healthPotionCount; }
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

        ToDController.SetTimeOfDay(newTime);
        currTimeOfDay = newTime;
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

    /// Unlocks the ability hotbar. (This depends on a scene reload)
    public void UnlockAbilities()
    {
        abilitiesUnlocked = true;
    }
    ///@}
}
