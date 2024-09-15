/**
This class remembers the warp the player last entered, and if the player is currently being warped.

Documentation updated 9/15/2024
\author Roy Pascual
\note This class does not inhert from monobehavior, so it does not have access to unity functions such as Start() or Update().
*/
public static class WarpInfo
{
    /// \brief This is the name of the warp to spawn the player at.
    /// When a StageWarp's exit zone is triggered, this is set to the name of the corresponding StageWarp.
    public static string WarpName { get; set; } = "NONE";
    /// True if the player is currently warping (typically true during the fade-to-black animation the StageLoader plays).
    public static bool CurrentlyWarping { get; set; } = false;
}
