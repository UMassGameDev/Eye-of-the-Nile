using System;
using UnityEngine;

/** \brief
Warp Obelisks are interactable objects throughout levels. The ObjectInteractable script triggers its public functions.
The player can interact with a \ref Prefabs_WarpObelisk to go to \ref Scenes_Skyhub or to set their spawnpoint.

Documentation updated 3/18/2025
\author Stephen Nuttall, Alexander Art
*/
public class WarpObelisk : MonoBehaviour
{
    /// \brief Reference to the child object holding the sprite for the unactivated warp obelisk.
    /// An unactivated warp obelisk is not the player's current respawn point.
    GameObject unactivated;
    /// \brief Reference to the child object holding the sprite for the activated warp obelisk.
    /// An activated warp obelisk is the player's current respawn point.
    GameObject activated;
    /// The coordinates the player will respawn at if they die after activating this warp obelisk.
    Vector2 respawnPoint;
    /// Reference to the DataManager. When the warp obelisk is activated, it tells the DataManager update the respawn point.
    DataManager dataManager;
    /// Reference to the StageLoader. The StageLoader is used to load \ref Scenes_Skyhub scene in WarpToSkyhub().
    StageLoader stageLoader;

    /// \brief True if the warp obelisk can be activated, thus setting the player's spawnpoint.
    /// The only warp obelisk that has this set to false is the one in \ref Scenes_Skyhub itself, which is used to return to 
    /// the warp obelisk the player used to get to there.
    public bool canSetSpawn = true;

    /// True if the warp obelisk is active, false otherwise.
    protected bool isActive = false;

    /// Triggers when any warp obelisk is activated.
    public static event Action onObeliskActivate;

    /// Sets references to unactivated, activated, respawnPoint, DataManager, and StageLoader.
    /// If the current spawnpoint in the DataManager is this warp obelisk's spawnpoint, set this warp obelisk to active.
    /// isActive is updated and the visual state of the warp obelisk is set.
    void Awake()
    {
        unactivated = transform.GetChild(0).gameObject;
        activated = transform.GetChild(1).gameObject;
        respawnPoint = transform.GetChild(2).position;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        stageLoader = GameObject.Find("StageLoader").GetComponent<StageLoader>();
        
        // Update state of obelisk on scene load.
        if (respawnPoint == dataManager.respawnPoint) {
            isActive = true;
            unactivated.SetActive(false);
            activated.SetActive(true);
        } else {
            isActive = false;
            unactivated.SetActive(true);
            activated.SetActive(false);
        }
    }

    /// Subscribes to the onObeliskActivate event.
    void OnEnable()
    {
        onObeliskActivate += UpdateActiveState;
    }

    /// Unsubscribes from the onObeliskActivate event.
    void OnDisable()
    {
        onObeliskActivate -= UpdateActiveState;
    }

    /// Triggered by ObjectInteractable. If enabled by canSetSpawn, set the DataManager's copy of spawnpoint to this spawnpoint, and activate the warp obelisk.
    public void SetSpawnpoint()
    {
        if (!canSetSpawn)
            return;

        dataManager.UpdateRespawnPoint(respawnPoint);
        onObeliskActivate?.Invoke();
    }

    /// Triggered by ObjectInteractable. If the current scene is \ref Scenes_Skyhub, return to the previous scene. If not, load \ref Scenes_Skyhub.
    public void WarpToSkyhub()
    {
        if (dataManager.GetCurrSceneName() == "Skyhub")
        {
            if (dataManager.skyhubExited == true)
            {            
                stageLoader.LoadNewStage(dataManager.GetPrevSceneName());
            } else { // Special condition for the player exiting the Skyhub for the first time.
                stageLoader.LoadNewStage("Opening");
            }
        } else {
            stageLoader.LoadNewStage("Skyhub");
        }
    }

    /// Triggered by ObjectInteractable. This will only warp the player to the Skyhub if it's unlocked.
    public void ConditionalWarpToSkyhub()
    {
        if (dataManager.GetCurrSceneName() == "Skyhub")
        {
            stageLoader.LoadNewStage(dataManager.GetPrevSceneName());
        } else {
            if (dataManager.skyhubUnlocked == true)
            {
                stageLoader.LoadNewStage("Skyhub");
            } else {
                Debug.Log("The player attempted to warp to the Skyhub, but it is not unlocked!");
            }
        }
    }

    /// If the warp obelisk needs to change its visual state, then this will update it.
    public void UpdateActiveState()
    {
        if (respawnPoint == dataManager.respawnPoint)
        {
            if (isActive == false)
            {
                unactivated.SetActive(false);
                activated.SetActive(true);

                isActive = true;
            }
        }
        else
        {
            if (isActive == true)
            {
                unactivated.SetActive(true);
                activated.SetActive(false);

                isActive = false;
            }
        }
    }
}
