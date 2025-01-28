using UnityEngine;
using UnityEngine.UI;

/** \brief
Handles opening the settings menu and the functionality of the buttons.

Documentation updated 1/27/2025
\author Stephen Nuttall
*/
public class SettingsMenu : MonoBehaviour
{
    public static bool SettingsOpen = false;

    /// Get settings menu game object. This is set in the inspector in Unity.
    public GameObject SettingsMenuUI;

    /// Reference to the data manager.
    /// All values from the settings menu should go to the DataManager so they can be saved between scenes!
    DataManager dataManager;

    /// Get settings menu UI options game objects. These are set in the inspector in Unity.
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    /// Set reference to dataManager.
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    /// Set the value of the settings options to the data manager's copy of it when scenes are loaded.
    void Start()
    {
        masterVolumeSlider.value = dataManager.GetMasterVolumeSetting();
        musicVolumeSlider.value = dataManager.GetMusicVolumeSetting();
        sfxVolumeSlider.value = dataManager.GetSfxVolumeSetting();
    }

    public void CloseSettings()
    {
        SettingsMenuUI.SetActive(false);
        SettingsOpen = false;
    }

    public void OpenSettings()
    {
        SwitchTab("Video Tab"); // Have the settings menu always be open on the video tab by default
        SettingsMenuUI.SetActive(true);
        SettingsOpen = true;
    }

    public void OnMasterVolumeChanged()
    {
        // Update the master volume value in the DataManager
        dataManager.masterVolumeSetting = masterVolumeSlider.value;
        // Tell the AudioManager to update the volume of the currently playing music
        AudioManager.instance.VolumeChanged();
    }

    public void OnMusicVolumeChanged()
    {
        // Update the music volume value in the DataManager
        dataManager.musicVolumeSetting = musicVolumeSlider.value;
        // Tell the AudioManager to update the volume of the currently playing music
        AudioManager.instance.VolumeChanged();
    }

    public void OnSfxVolumeChanged()
    {
        // Update the sound effects volume value in the DataManager
        dataManager.sfxVolumeSetting = sfxVolumeSlider.value;
        // Tell the AudioManager to update the volume of the currently playing music (not needed here)
        AudioManager.instance.VolumeChanged();
    }

    public void SwitchTab(string newTab)
    {
        // Find the transform of the GameObject that contains all of the different settings menu tabs
        Transform tabs = SettingsMenuUI.transform.Find("Tabs");

        // Find the transform of the tab that is trying to be opened
        Transform targetTab = tabs.Find(newTab);

        if (targetTab == null)
        {
            Debug.Log("Attempted to open tab \"" + newTab + "\" in the settings menu, but that tab does not exist!");
        }
        else
        {
            // Disable all tabs
            foreach (Transform child in tabs)
            {
                child.gameObject.SetActive(false);
            }

            // Activate the tab that has been switched to
            targetTab.gameObject.SetActive(true);
        }
    }

}
