/**************************************************
Basic information for a warp or door that can then be used by other scripts.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class StageWarp : MonoBehaviour
{
    public string srcWarpName = "DEFAULT_SOURCE";
    public string destWarpName = "DEFAULT_DESTINATION";
    public StageWarpType warpType = StageWarpType.DirectExit;

    // Edit this string to the name of the destination scene
    public string sceneToWarpTo;
    // Every StageWarp has an "EntryPoint" as a child which serves as a spawn point
    public Vector2 EntryPos { get { return entryPoint.position; } }
    private Transform entryPoint;

    void Awake()
    {
        entryPoint = transform.Find("EntryPoint");
    }

}
