/**************************************************
If the player is inside a warp/door, this script will warp them to the corresponding exit door.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;
using UnityEngine.Events;

public class ExitZone : MonoBehaviour
{
    public UnityEvent<string> collidedEvent;
    public StageWarp stageWarp;
    private string _collisionLayer = "Player";

    void WarpToZone()
    {
        // WarpInfo.WarpName = stageWarp.warpName;
        WarpInfo.WarpName = stageWarp.destWarpName;
        collidedEvent.Invoke(stageWarp.sceneToWarpTo);
    }

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

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer(_collisionLayer))
        {
            collider.gameObject.GetComponent<PlayerMovement>().OnWarp = false;
        }
    }

    void Awake()
    {
        if (stageWarp == null)
        {
            stageWarp = transform.parent.GetComponent<StageWarp>();
        }
    }

}
