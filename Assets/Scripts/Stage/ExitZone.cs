using UnityEngine;
using UnityEngine.Events;

/** \brief
If the player is inside a warp zone (such as a door or some form of exit), this script will warp them to the corresponding exit door.
In other words, if you go through a door, this script spawns the player on the other side.

Documentation updated 9/3/2024
\author Roy Pascual
*/
public class ExitZone : MonoBehaviour
{
    /// \brief Unity event triggered when a player begins to warp.
    /// \note LoadNewStage() in StageLoader needs to be subscribed to this. This will tell StageLoader to load the warp's destination and spawn
    /// the player there (as well as the face to black transition).
    public UnityEvent<string> collidedEvent;
    /// Reference to the StageWarp component, which contains basic information about the warp.
    public StageWarp stageWarp;
    /// \brief Layers which object collisions will be searched for. Set to "Player."
    /// \note Should only contain the player. This is a way of singling out the player from any other object that might enter the warp.
    private string _collisionLayer = "Player";

    /// Updates WarpInfo and invokes collidedEvent, which hopefully LoadNewStage() in StageLoader is subscribed to (thus loading the destination).
    void WarpToZone()
    {
        WarpInfo.WarpName = stageWarp.destWarpName;
        collidedEvent.Invoke(stageWarp.sceneToWarpTo);
    }

    /// Runs when an object is inside the warp zone. If that object is the player, warp the player
    /// (unless the warp is classified as a door rather than an exit, in which case wait for the player to press W while inside the warp zone).
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer(_collisionLayer))
        {
            collider.gameObject.GetComponent<PlayerMovement>().OnWarp = true;
            switch(stageWarp.warpType)
            {
                case StageWarpType.DirectExit:
                    WarpToZone();
                    break;
                case StageWarpType.DoorExit:
                    // Will have to change this later and create some relation to a Player script
                    // For example, don't want the Player jumping when the door is entered
                    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                        WarpToZone();
                    break;
            }
            
        }
    }

    /// Runs when an object leaves the warp zone. If that object is the player, tell the player it's not warping anymore.
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer(_collisionLayer))
        {
            collider.gameObject.GetComponent<PlayerMovement>().OnWarp = false;
        }
    }

    /// Set reference to the StageWarp component.
    void Awake()
    {
        if (stageWarp == null)
        {
            stageWarp = transform.parent.GetComponent<StageWarp>();
        }
    }

}
