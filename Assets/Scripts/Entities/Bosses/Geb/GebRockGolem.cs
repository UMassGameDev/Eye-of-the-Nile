using UnityEngine;

/** \brief
Script for the rock golems in Geb's bossroom.
All it does is decrement the rock golem count in Geb's bossroom when the rock golems die.

Documentation updated 1/2/2025
\author Alexander Art
*/
public class GebRockGolem : MonoBehaviour
{
    /// Reference to Geb's room controller.
    protected GebRoomController gebRoomController;

    void Awake()
    {
        gebRoomController = GameObject.Find("Geb").GetComponent<GebRoomController>();
    }

    /// Subtracts 1 from Geb's bossroom's rock golem count. Called by the rock golem's ObjectHealth when it dies.
    public void DecrementRockGolemCount()
    {
        // Update rockGolemCount.
        gebRoomController.rockGolemCount--;
    }
}
