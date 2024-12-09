/**************************************************
Handles the functionality of the quit button in the main menu.

Documentation updated 12/8/2024
**************************************************/
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
        Debug.Log("---------- Game Closed ----------");
    }

}
