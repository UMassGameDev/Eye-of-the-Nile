/**************************************************
Handles opening the settings menu and the functionality of some of the buttons.

Documentation updated 12/4/2024
**************************************************/
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static bool SettingsOpen = false;

    public GameObject SettingsMenuUI;

    void Update()
    {

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

}
