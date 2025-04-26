using UnityEngine;

/**
Handles the functionality of the start and quit buttons in the main menu.

Documentation updated 2/3/2025
*/
public class MainMenu : MonoBehaviour
{
    [SerializeField] string defaultStartScene = "Opening";

    DataManager dataManager;
    StageLoader stageLoader;

    void Awake()
    {
        dataManager = FindObjectOfType<DataManager>();
        stageLoader = FindObjectOfType<StageLoader>();
    }

    public void StartGame()
    {
        if (dataManager.noSaveFileFound)
        {
            stageLoader.LoadNewStage(defaultStartScene);
        }
        else
        {
            WarpInfo.WarpName = "RESPAWN";
            stageLoader.LoadNewStage("RESPAWN");
        }
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("---------- Game Closed ----------");
    }

}
