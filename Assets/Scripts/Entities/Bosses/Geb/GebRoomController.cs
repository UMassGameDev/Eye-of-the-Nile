using System;
using UnityEngine;
using Cinemachine;

/** \brief
This script is a work in progress. It will control most of Geb's bossroom (trigger zone, doors, cutscenes, healthbar visibility).
This script consists of:
- 6 functions that get called only once when Geb enters a new phase, one for each phase (except for the first one).
- 7 functions that get called every frame depending on Geb's phase, one for each phase.

Documentation updated 12/30/2024
\author Alexander Art
\todo Freeze the player during the cutscenes.
*/
public class GebRoomController : MonoBehaviour
{
    /// Reference to the Virtual Camera, used for zooming out.
    [SerializeField] protected CinemachineVirtualCamera virtualCamera;
    /// Reference to the boss healthbar HUD, used for making it appear and disappear at the start and end of the fight.
    [SerializeField] protected BossHealthbarHUD healthbar;
    /// Reference to Geb's phase controller.
    protected GebPhaseController phaseController;

    /// How far to zoom out the camera for Geb's bossfight.
    float fightZoom = 12f;

    /// Used for animating the cutscene when Geb is activated.
    private float cutsceneTimer = 0.0f;
    /// The zoom of the camera before the bossfight starts.
    private float defaultZoom;

    /// Set reference to Geb's phase controller.
    void Awake()
    {
        phaseController = GetComponent<GebPhaseController>();
    }

    /// Access the current zoom to set defaultZoom variable.
    void Start()
    {
        defaultZoom = virtualCamera.m_Lens.OrthographicSize; // It wasn't letting me do this during Awake().
    }

    /// Every frame, activate the logic for the current phase Geb is in.
    void Update()
    {
        switch (phaseController.phase)
        {
            case GebPhase.Inactive:
                InactiveState();
                break;
            case GebPhase.OpeningCutscene:
                OpeningCutsceneState();
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
            case GebPhase.ClosingCutscene:
                ClosingCutsceneState();
                break;
            case GebPhase.Defeated:
                DefeatedState();
                break;
        }
    }

    /// Called by GebPhaseController once when the opening cutscene starts.
    public void GebOpeningCutsceneStarted() {
        // Set Geb's healthbar to empty so the fill animation can be played.
        healthbar.SetHealthbarPercentage(0f);
    }
    /// Called by GebPhaseController once when the opening cutscene is over and the bossfight starts.
    public void GebPhase1Started() {
        // Reset the cutscene timer now that the opening cutscene is over.
        cutsceneTimer = 0f;
    }
    /// Called by GebPhaseController once when phase 2 starts.
    public void GebPhase2Started() {}
    /// Called by GebPhaseController once when phase 3 starts.
    public void GebPhase3Started() {}
    /// Called by GebPhaseController once when the closing cutscene starts.
    public void GebClosingCutsceneStarted() {
        // Hide Geb's healthbar.
        healthbar.SetHealthbarVisible(false);
    }
    /// Called by GebPhaseController once when the closing cutscene finishes.
    public void GebDefeated() {}

    /// Runs every frame when Geb is inactive.
    void InactiveState() {}
    /// Runs every frame when the opening cutscene is playing.
    void OpeningCutsceneState() {
        // Update the cutscene timer.
        cutsceneTimer += Time.deltaTime;

        // Zoom out the camera for 2 seconds when Geb is activated.
        virtualCamera.m_Lens.OrthographicSize = defaultZoom + (fightZoom - defaultZoom) * Math.Min(cutsceneTimer / 2f, 1f);

        // After 2 seconds, make Geb's healthbar visible.
        if (cutsceneTimer > 2f)
        {
            healthbar.SetHealthbarVisible(true);
        }

        // Animate the healthbar to fill from 0% to 100% when 2f < cutsceneTimer < 5f.
        healthbar.SetHealthbarPercentage(Math.Max(0f, Math.Min((cutsceneTimer - 2f) / 3f, 1f)));

        // After 5 seconds, end the cutscene and start Geb's phase 1.
        if (cutsceneTimer > 5f)
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
    /// Runs every frame when Geb is defeated and the closing cutscene is playing.
    void ClosingCutsceneState() {
        // Update the cutscene timer.
        cutsceneTimer += Time.deltaTime;

        // Zoom in the camera for 2 seconds when Geb is defeated.
        virtualCamera.m_Lens.OrthographicSize = fightZoom + (defaultZoom - fightZoom) * Math.Min(cutsceneTimer / 2f, 1f);

        // After 2 seconds, end the cutscene.
        if (cutsceneTimer > 2f)
        {
            phaseController.ClosingCutsceneEnded();
        }
    }
    /// Runs every frame when the closing cutscene is over.
    void DefeatedState() {}
}
