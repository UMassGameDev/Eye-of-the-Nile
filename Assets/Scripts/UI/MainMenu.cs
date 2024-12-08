/**************************************************
Handles pausing the game and the functionality of the buttons in the pause menu.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Play Button Clicked");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("---------- Game Closed ----------");
    }

}
