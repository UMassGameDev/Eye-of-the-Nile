using System;
using UnityEngine;
using Cinemachine;

/** \brief
This script is a work in progress. It will control most of Geb's bossroom (trigger zone, doors, cutscenes, healthbar visibility).
This script mainly consists of:
- 6 functions that get called only once when Geb enters a new phase, one for each phase (except for the first one).
- 7 functions that get called every frame depending on Geb's phase, one for each phase.

Documentation updated 1/18/2025
\author Alexander Art
\todo Freeze the player during the cutscenes.
*/
public class GebRoomController : MonoBehaviour
{
    /// Reference to the Virtual Camera, used for zooming out.
    [SerializeField] protected CinemachineVirtualCamera virtualCamera;
    /// Reference to the boss healthbar HUD, used for making it appear and disappear at the start and end of the fight.
    [SerializeField] protected BossHealthbarHUD healthbar;
    /// Reference to Geb's PatrolZone (Bounds), the left end and right end of Geb's bossroom that the rock golems will move around in.
    [SerializeField] public PatrolZone bounds;
    /// Reference to Geb's phase controller.
    protected GebPhaseController phaseController;
    /// Reference to Geb's boss controller.
    protected GebBossController bossController;
    /// Reference to the player object.
    protected GameObject player;
    /// Reference to the rocks that fall from the sky during Geb's earthquake attack in phase 3.
    [SerializeField] protected GameObject fallingSkyRocks;

    /// How far to zoom out the camera for Geb's bossfight.
    float fightZoom = 11f;
    /// How often a rock will fall from the sky during Geb's earthquake attack.
    float fallingRockSpawnPeriod = 0.1f;

    /// Create random number generator.
    private System.Random rng = new System.Random();
    /// Number of rock golems currently in the room.
    public int rockGolemCount = 0;
    /// Maximum number of rock golems that can be present in the room before they stop getting spawned.
    public int maxRockGolems = 4;
    /// Used for animating the cutscene when Geb is activated.
    private float cutsceneTimer = 0.0f;
    /// The zoom of the camera before the bossfight starts.
    private float defaultZoom;
    /// Used for keeping track of when the last rock fell from the sky.
    private float fallingRockSpawnTimer = 0.0f;

    /// Set references.
    void Awake()
    {
        player = GameObject.Find("Player");
        phaseController = GetComponent<GebPhaseController>();
        bossController = GetComponent<GebBossController>();
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

        // Animate the healthbar to fill from 0% to 100% when 2f < cutsceneTimer < 4f.
        healthbar.SetHealthbarPercentage(Math.Max(0f, Math.Min((cutsceneTimer - 2f) / 2f, 1f)));

        // After 4 seconds, end the cutscene and start Geb's phase 1.
        if (cutsceneTimer > 4f)
        {
            phaseController.StartGebBossfight();
        }
    }
    /// Runs every frame when Geb is in phase 1.
    void Phase1State() {}
    /// Runs every frame when Geb is in phase 2.
    void Phase2State() {}

    /// Runs every frame when Geb is in phase 3.
    void Phase3State()
    {
        // If Geb is quaking the ground, rocks will fall from the sky.
        if (bossController.GetCurrentAction() == GebAction.Earthquake)
        {
            // Every fallingRockSpawnPeriod seconds, a rock will spawn at a random position above the player and fall.
            fallingRockSpawnTimer += Time.deltaTime;
            if (fallingRockSpawnTimer >= fallingRockSpawnPeriod)
            {
                fallingRockSpawnTimer = 0f;

                // Determine where to instantiate the rock, then instantiate it.
                float minSpawnX = bounds.LeftPoint().x + 1;
                float maxSpawnX = bounds.RightPoint().x - 1;
                // The player's position can be taken into account when determining where to spawn the rock,
                // but this doesn't take into account the user's screen size/aspect ratio.
                //float minSpawnX = Math.Max(bounds.LeftPoint().x + 1, player.transform.position.x - 30f);
                //float maxSpawnX = Math.Min(bounds.RightPoint().x - 1, player.transform.position.x + 30f);
                // The x spawn position is picked randomly between the minimum and maximum x. 
                float spawnX = minSpawnX + (float)rng.NextDouble() * (maxSpawnX - minSpawnX);
                // The y spawn position is a fixed distance above the player. This should also scale with aspect ratio, but it does not.
                float spawnY = player.transform.position.y + 20f;
                // Instantiate rock.
                GameObject rock = Instantiate(fallingSkyRocks, new Vector2(spawnX, spawnY), Quaternion.identity);
                
                // Scale rock.
                float scale = (float)rng.NextDouble() + 1f;
                rock.transform.localScale = new Vector3(scale, scale, 1f);
                
                // Have the rock move downwards at a constant velocity.
                rock.GetComponent<MoveInDirection>().movementDirection = new Vector2((float)rng.NextDouble() * 10f - 5f, -10f - (float)rng.NextDouble() * 5f);
            }
        }
    }

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

    /// Called by the rock golems when they die. Subtracts 1 from the rock golem count.
    public void DecrementRockGolemCount()
    {
        rockGolemCount--;
    }
}
