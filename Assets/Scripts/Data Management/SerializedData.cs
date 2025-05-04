using System.Collections.Generic;

/** \brief
This takes data from the DataManager (given in the constructor) and packages into a class that can be serialized.
FileReadWrite can then write this class to a file, or create a new one after reading from a pre-existing file.
In short, this class is an intermediate format between the DataManager and data saved to a file, allowing FileReadWrite to translate between the two.

Documentation updated 2/3/2025
\author Stephen Nuttall
*/
[System.Serializable]
public class SerializedData
{
    /** @name General Data
    *  Data related to the player, time of day, soul counts, and more.
    */
    ///@{
    /// Player health that should be restored when a new scene is loaded. 
    public int playerHealth { get; private set; }
    /// The amount of health potions the player currently has.
    public int healthPotionCount { get; private set; }
    /// Time of day that should be restored when a new scene is loaded. 0 = day, 1 = night, 2 = eclipse, and 3 = blood moon.
    public TimeOfDay currTimeOfDay { get; private set; }
    /// Current amount of souls. 
    public int souls { get; private set; }
    /// Current amount of god souls. 
    public int godSouls { get; private set; }
    /// Whether the ability hotbar is unlocked yet or not.
    public bool abilitiesUnlocked { get; private set; }
    /// True if warp obelisks should allow the player to warp to the Skyhub. Unlocks after Geb is defeated.
    public bool skyhubUnlocked { get; private set; }
    /// If Ma'at has never been talked to before, the welcome message should be displayed.
    public bool maatTalked { get; private set; }
    /// If the player has ever exited the Skyhub.
    public bool skyhubExited { get; private set; }
    /// Set to true and the Skyhub will spawn the player back at the Opening scene.
    public bool skyhubLeadsToOpening { get; private set; }
    ///@}

    /** @name Scene Data
    *  Details about the current scene, previous scene, and the respawn scene.
    */
    ///@{
    /// The index of the scene that is currently loaded.
    public int currSceneIndex { get; private set; }
    /// The index of the scene that was previously loaded. 
    public int prevSceneIndex { get; private set; }
    /// The name of the scene that is currently loaded.
    public string currSceneName { get; private set; }
    /// The name of the scene that was previously loaded.
    public string prevSceneName { get; private set; }
    /// The name of the scene the player’s respawn point is in.
    public string respawnSceneName { get; private set; }
    /// The X coordinate of the player’s respawn point.
    public float respawnPoint_X { get; private set; }
    /// The Y coordinate of the player’s respawn point.
    public float respawnPoint_Y { get; private set; }
    ///@}

    /** @name Volume Settings
    *  The volume gets multiplied by these values in AudioManager.cs (values ranging from 0 to 1, accessible from the settings menu).
    */
    ///@{
    /// The master volume gets multiplied by this values in AudioManager.cs (values ranging from 0 to 1).
    public float masterVolumeSetting { get; private set; }
    /// The music volume gets multiplied by this values in AudioManager.cs (values ranging from 0 to 1).
    public float musicVolumeSetting { get; private set; }
    /// The SFX volume gets multiplied by this values in AudioManager.cs (values ranging from 0 to 1).
    public float sfxVolumeSetting { get; private set; }
    ///@}

    /// Constructor. Sets all variables to the values in the DataManager.
    public SerializedData(DataManager dataManager)
    {
        playerHealth = dataManager.GetPlayerHealth();
        healthPotionCount = dataManager.GetHealthPotionCount();
        currTimeOfDay = dataManager.GetTimeOfDay();
        souls = dataManager.GetSouls();
        godSouls = dataManager.GetGodSouls();
        abilitiesUnlocked = dataManager.abilitiesUnlocked;
        skyhubUnlocked = dataManager.skyhubUnlocked;
        maatTalked = dataManager.maatTalked;
        skyhubExited = dataManager.skyhubExited;
        skyhubLeadsToOpening = dataManager.skyhubLeadsToOpening;

        currSceneIndex = dataManager.GetCurrSceneIndex();
        currSceneName = dataManager.GetCurrSceneName();
        prevSceneIndex = dataManager.GetPrevSceneIndex();
        prevSceneName = dataManager.GetPrevSceneName();
        respawnSceneName = dataManager.GetRespawnSceneName();
        respawnPoint_X = dataManager.GetRespawnPoint().x;
        respawnPoint_Y = dataManager.GetRespawnPoint().y;

        masterVolumeSetting = dataManager.GetMasterVolume();
        musicVolumeSetting = dataManager.GetMusicVolume();
        sfxVolumeSetting = dataManager.GetSFXVolume();
    }
}
