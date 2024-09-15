using UnityEngine;

/** \brief
Basic component of \ref Prefabs_StageWarp that stores basic information about the it, including the
warp name, destination name, and warp type.

Documentation updated 9/15/2024
\author Roy Pascual
*/
public class StageWarp : MonoBehaviour
{
    /// Name of this warp. Used to identify it when connecting another warp to this one as its destination.
    public string srcWarpName = "DEFAULT_SOURCE";
    /// Name of the destination warp. This is the warp the player will be teleported to after traveling through this one.
    public string destWarpName = "DEFAULT_DESTINATION";
    /// \brief The type of warp functionality this warp will have.
    /// A DirectExit will immediately warp the player upon entering the warp zone, while DoorExit requires the player to give upwards input.
    public StageWarpType warpType = StageWarpType.DirectExit;

    /// Name of the scene the destination warp is in. If it's in the same scene, use "this".
    public string sceneToWarpTo;
    /// The coordinates of this StageWarp's entry point, which is just entryPoint.position.
    public Vector2 EntryPos { get { return entryPoint.position; } }
    /// \brief Reference to the StageWarp's entry point, which is a transform and a child of \ref Prefabs_StageWarp. 
    /// It's the position the player will spawn at when it warps to this StageWarp (the "spawnpoint").
    protected Transform entryPoint;

    /// Set refernce to the entry point.
    void Awake()
    {
        entryPoint = transform.Find("EntryPoint");
    }

}
