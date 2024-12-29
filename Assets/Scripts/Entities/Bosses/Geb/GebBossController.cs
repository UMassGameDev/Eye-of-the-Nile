using UnityEngine;

/** \brief
This script is a work in progress. It will control Geb's movement and will trigger Geb's attacks.
This script consists of:
- 5 functions that get only get called once when Geb enters a new phase, one for each phase.
- 5 functions that get called every frame depending on Geb's phase, one for each phase.

Documentation updated 12/28/2024
\author Alexander Art
*/
public class GebBossController : MonoBehaviour
{
    /// Reference to Geb's phase controller.
    protected GebPhaseController phaseController;

    /// Reference to the player object.
    protected GameObject player;

    /// Set references to player GameObject and Geb's phase controller.
    void Awake()
    {
        player = GameObject.Find("Player");
        phaseController = GetComponent<GebPhaseController>();
    }

    /// (Probably temporary) radius around the boss that the player must be within for the bossfight to start.
    /// If a trigger zone is ever used to start the bossfight, this script could go on that trigger zone instead of having this radius.
    protected float bossActivationRadius = 20f;

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
    public void GebBossfightStarted() {}
    /// Called by GebPhaseController once when phase 2 starts.
    public void GebPhase2Started() {}
    /// Called by GebPhaseController once when phase 3 starts.
    public void GebPhase3Started() {}
    /// Called by GebPhaseController once when Geb is defeated.
    public void GebDefeated()
    {
        // Hide Geb when defeated.
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    /// Runs every frame when Geb is inactive.
    void InactiveState()
    {
        // Wait for the player to get close to start the fight.
        if (Vector2.Distance(transform.position, player.transform.position) < bossActivationRadius)
        {
            phaseController.StartGebBossfight();
        }
    }
    /// Runs every frame when Geb is in phase 1.
    void Phase1State() {}
    /// Runs every frame when Geb is in phase 2.
    void Phase2State() {}
    /// Runs every frame when Geb is in phase 3.
    void Phase3State() {}
    /// Runs every frame when Geb is defeated.
    void DefeatedState() {}
}
