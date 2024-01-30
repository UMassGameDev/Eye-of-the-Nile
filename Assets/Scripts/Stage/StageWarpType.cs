/**************************************************
Enum to differentiate between warps that teleport you on collision vs when interacted with ("doors").

Documentation updated 1/29/2024
**************************************************/
public enum StageWarpType
{
    DirectExit, // Touching the ExitZone immediately warps the player
    DoorExit // Requires the player to input a key
}
