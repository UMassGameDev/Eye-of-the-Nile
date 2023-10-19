using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ExitZone : MonoBehaviour
{
    public UnityEvent<string> collidedEvent;
    public StageWarp stageWarp;
    private string _collisionLayer = "Player";

    void WarpToZone()
    {
        WarpInfo.WarpName = stageWarp.warpName;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
