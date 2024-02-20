/**************************************************
Loads a given Unity scene when a player goes through a warp/door or dies.
This also handles the black fade transition.

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public Dictionary<string, StageWarp> StageWarps { get; set; }
    public Animator fadeAnimator;

    void InitializeWarps()
    {
        StageWarps = new Dictionary<string, StageWarp>();
        StageWarp[] AllWarps = FindObjectsOfType(typeof(StageWarp)) as StageWarp[];

        foreach (StageWarp sWarp in AllWarps)
        {
            // StageWarps.Add(sWarp.warpName, sWarp);
            StageWarps.Add(sWarp.srcWarpName, sWarp);
        }
    }

    IEnumerator TransitionToNewStage(string newStageName)
    {
        // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newStageName);
        WarpInfo.CurrentlyWarping = true;
        fadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(newStageName);
        WarpInfo.CurrentlyWarping = false;
    }

    public void LoadNewStage(string newStageName)
    {
        if (WarpInfo.CurrentlyWarping == true)
            return;

        if (newStageName == "this") {
            newStageName = SceneManager.GetActiveScene().name;
        } else if (newStageName == "RESPAWN") {
            newStageName = GameObject.Find("DataManager").GetComponent<DataManager>().GetRespawnSceneName();
            Debug.Log("Respawning in scene \"" + newStageName + '\"');
        }
        
        if (newStageName == null) {
            Debug.LogError("Can't load new stage. NewStageName is NULL");
            return;
        }

        StartCoroutine(TransitionToNewStage(newStageName));
    }

    void Awake()
    {
        InitializeWarps();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (WarpInfo.WarpName == "NONE") {
            // do nothing
            // possibly more
        } else if (WarpInfo.WarpName == "RESPAWN") {
            // If the warp name is RESPAWN, it should spawn the player to their respawn point
            // The respawn point is stored in the data manager
            GameObject player = GameObject.Find("Player");
            DataManager dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
            player.transform.position = dataManager.respawnPoint;
            Debug.Log("Spawning at position " + dataManager.respawnPoint);
        } else {
            // This occurs when this GameObject loads into the scene
            // It finds the name of the warp to spawn the player at
            // Then it uses the position of the entry point of that warp to spawn the player
            GameObject player = GameObject.Find("Player");
            player.transform.position = StageWarps[WarpInfo.WarpName].EntryPos;
        }
    }
}
