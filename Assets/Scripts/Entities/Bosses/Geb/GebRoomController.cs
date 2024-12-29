using UnityEngine;

/** \brief
This script is a work in progress. It will control most of Geb's bossroom (trigger zone, doors, cutscenes, healthbar visibility).
This script consists of:
- 5 functions that get only get called once when Geb enters a new phase, one for each phase.
- 5 functions that get called every frame depending on Geb's phase, one for each phase.

Documentation updated 12/28/2024
\author Alexander Art
*/
public class GebRoomController : MonoBehaviour
{
    /// Reference to the boss healthbar HUD, used for making it appear and disappear at the start and end of the fight.
    [SerializeField] protected BossHealthbarHUD healthbar;
    /// Reference to Geb's phase controller.
    protected GebPhaseController phaseController;
    /// Reference to the warp obelisk that will appear when Geb is defeated.
    [SerializeField] protected GameObject warpObelisk;

    /// Set reference to Geb's phase controller.
    void Awake()
    {
        phaseController = GetComponent<GebPhaseController>();
    }

    /// Every frame, activate the logic for the current phase Geb is in.
    void Update()
    {
        switch (phaseController.phase)
        {
            case GebPhase.Inactive:
                InactiveState();
                break;
            case GebPhase.Phase1:
                Phase1State();
                break;
            case GebPhase.Phase2:
                Phase2State();
                break;
            case GebPhase.Phase3:
                Phase3State();
                break;
            case GebPhase.Defeated:
                DefeatedState();
                break;
        }
    }

    /// Called by GebPhaseController once when the bossfight starts.
    public void GebBossfightStarted()
    {
        // Make Geb's healthbar visible.
        healthbar.SetHealthbarVisible(true);
    }
    /// Called by GebPhaseController once when phase 2 starts.
    public void GebPhase2Started() {}
    /// Called by GebPhaseController once when phase 3 starts.
    public void GebPhase3Started() {}
    /// Called by GebPhaseController once when Geb is defeated.
    public void GebDefeated() {
        // Hide Geb's healthbar.
        healthbar.SetHealthbarVisible(false);
        // Make the warp obelisk appear.
        warpObelisk.SetActive(true);
    }

    /// Runs every frame when Geb is inactive.
    void InactiveState() {}
    /// Runs every frame when Geb is in phase 1.
    void Phase1State() {}
    /// Runs every frame when Geb is in phase 2.
    void Phase2State() {}
    /// Runs every frame when Geb is in phase 3.
    void Phase3State() {}
    /// Runs every frame when Geb is defeated.
    void DefeatedState() {}
}
