using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WarpInfo
{
    // This is the name of the warp to spawn the player at
    // When a StageWarp's exit zone is triggered,
    // this is set to the name of the corresponding StageWarp
    public static string WarpName { get; set; } = "NONE";
    public static bool CurrentlyWarping { get; set; } = false;
}
