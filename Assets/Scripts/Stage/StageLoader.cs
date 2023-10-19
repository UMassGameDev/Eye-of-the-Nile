using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public Dictionary<string, StageWarp> StageWarps { get; set; }

    public void LoadNewStage(string newStageName)
    {
        // TODO: Perform transition between scenes
        SceneManager.LoadScene(newStageName);
    }

    void Awake()
    {
        StageWarps = new Dictionary<string, StageWarp>();
        StageWarp[] AllWarps = FindObjectsOfType(typeof(StageWarp)) as StageWarp[];

        foreach (StageWarp sWarp in AllWarps)
        {
            StageWarps.Add(sWarp.warpName, sWarp);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (WarpInfo.WarpName == "NONE")
        {
            // do nothing
            // possibly more
        }
        else
        {
            // This occurs when this GameObject loads into the scene
            // It finds the name of the warp to spawn the player at
            // Then it uses the position of the entry point of that warp to spawn the player
            StageWarp currentWarp = StageWarps[WarpInfo.WarpName];
            GameObject player = GameObject.Find("Player");
            player.transform.position = StageWarps[WarpInfo.WarpName].EntryPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
