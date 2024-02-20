/**************************************************
Warp Obelisks are interactable objects throughout levels.
The player can interact with them to go to the Skyhub or to set their spawnpoint.

Documentation updated 2/19/2024
**************************************************/
using UnityEngine;

public class WarpObelisk : MonoBehaviour
{
    GameObject unactivated;
    GameObject activated;
    Vector2 respawnPoint;
    DataManager dataManager;

    void Awake()
    {
        unactivated = transform.GetChild(0).gameObject;
        activated = transform.GetChild(1).gameObject;
        respawnPoint = transform.GetChild(2).position;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        if (respawnPoint == dataManager.respawnPoint) {
            unactivated.SetActive(false);
            activated.SetActive(true);
        } else {
            unactivated.SetActive(true);
            activated.SetActive(false);
        }
    }

    public void SetSpawnpoint()
    {
        dataManager.UpdateRespawnPoint(respawnPoint);
        unactivated.SetActive(false);
        activated.SetActive(true);
    }

    public void WarpToSkyhub()
    {
        Debug.Log("Warping to Skyhub");
    }
}
