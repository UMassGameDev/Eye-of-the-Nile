/**************************************************
Handles opening the settings menu and the functionality of some of the buttons.

Documentation updated 12/5/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static bool SettingsOpen = false;

    /// Get settings menu game object. This is set in the inspector in Unity.
    public GameObject SettingsMenuUI;

    /// Reference to the data manager.
    DataManager dataManager;

    /// Get volume slider game object. This is set in the inspector in Unity.
    public Slider volumeSlider;

    /// Set reference to dataManager.
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    /// Set the value of the settings options to the data manager's copy of it when scenes are loaded.
    void Start()
    {
        volumeSlider.value = dataManager.GetVolume();
    }

    public void CloseSettings()
    {
        SettingsMenuUI.SetActive(false);
        SettingsOpen = false;
    }

    public void OpenSettings()
    {
        SettingsMenuUI.SetActive(true);
        SettingsOpen = true;
    }

    public void OnVolumeChanged()
    {
        dataManager.volume = volumeSlider.value;
    }

}
