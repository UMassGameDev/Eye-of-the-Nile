using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** \brief
Loads a given Unity scene when a player goes through a warp/door or dies.
This also handles the black fade transition.

Documentation updated 9/15/2024
\author Roy Pascual
*/
public class StageLoader : MonoBehaviour
{
    /// Dictionary linking each StageWarp object to its name.
    public Dictionary<string, StageWarp> StageWarps { get; set; }
    /// Reference to the animator responsible for the fate-to-black animation we want to play when loading a new scene.
    public Animator fadeAnimator;
    /// Reference to the DataManager.
    DataManager dataManager;

    /// Populate the dictionary with every StageWarp in the scene and their names.
    void InitializeWarps()
    {
        // Initialize dictionary by reserving memory for it
        StageWarps = new Dictionary<string, StageWarp>();
        // Make a list of StageWarps and fill it with every StageWarp in the scene
        StageWarp[] AllWarps = FindObjectsOfType(typeof(StageWarp)) as StageWarp[];

        // For each one of those StageWarps, add their name and a reference to them to the dictionary
        foreach (StageWarp sWarp in AllWarps)
        {
            // StageWarps.Add(sWarp.warpName, sWarp);
            StageWarps.Add(sWarp.srcWarpName, sWarp);
        }
    }

    /// \brief Starts the fade animation, waits one second for the animation to end, then loads the new scene.
    /// \todo Wait time for fade transition should not be hardcoded, but instead a variable.
    IEnumerator TransitionToNewStage(string newStageName)
    {
        // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newStageName);
        WarpInfo.CurrentlyWarping = true;
        fadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(newStageName);
        WarpInfo.CurrentlyWarping = false;
    }

    /// Parses the given stage name, changing it if it's a stand-in for another scene name ("this" = this scene, "RESPAWN" = scene of respawn point).
    /// Then, it runs a coroutine for TransitionToNewStage().
    public void LoadNewStage(string newStageName)
    {
        if (WarpInfo.CurrentlyWarping == true)
            return;

        if (newStageName == "this")
        {
            newStageName = SceneManager.GetActiveScene().name;
        }
        else if (newStageName == "RESPAWN")
        {
            newStageName = GameObject.Find("DataManager").GetComponent<DataManager>().GetRespawnSceneName();
            Debug.Log("Respawning in scene \"" + newStageName + '\"');
        }

        if (newStageName == null)
        {
            Debug.LogError("Can't load new stage. NewStageName is NULL");
            return;
        }

        StartCoroutine(TransitionToNewStage(newStageName));
    }

    /// Run InitializeWarps().
    void Awake()
    {
        InitializeWarps();

        // Set reference to dataManager.
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    /// 
    void Start()
    {
        if (WarpInfo.WarpName == "NONE")
        {
            // do nothing
            // possibly more
        }
        else if (WarpInfo.WarpName == "RESPAWN")
        {
            // If the warp name is RESPAWN, it should spawn the player to their respawn point
            // The respawn point is stored in the data manager
            GameObject player = GameObject.Find("Player");
            DataManager dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
            player.transform.position = dataManager.respawnPoint;
            Debug.Log("Spawning at position " + dataManager.respawnPoint);
        }
        else
        {
            // This occurs when this GameObject loads into the scene
            // It finds the name of the warp to spawn the player at
            // Then it uses the position of the entry point of that warp to spawn the player
            GameObject player = GameObject.Find("Player");
            player.transform.position = StageWarps[WarpInfo.WarpName].EntryPos;
        }

        // The player being reset to the opening scene is supposed to be a one time thing, so this line ensures that.
        if (dataManager.GetCurrSceneName() != "Skyhub")
        {
            dataManager.UnsetSkyhubToOpening();
        }
    }
}
