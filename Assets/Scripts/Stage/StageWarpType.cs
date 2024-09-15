/** \brief 
Enum to differentiate between warps that teleport you immediately on collision vs when interacted with ("doors").

Documentation updated 9/15/2024
\author Roy Pascual
*/
public enum StageWarpType
{
    DirectExit, /// Touching the ExitZone immediately warps the player.
    DoorExit /// Requires the player to give vertical input (W, UpArrow, or SpaceBar).
}
