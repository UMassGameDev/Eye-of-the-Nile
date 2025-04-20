using System;
using UnityEngine;
using Cinemachine;

/** \brief
This script is a work in progress. It will control most of Geb's bossroom (trigger zone, doors, cutscenes, healthbar visibility).
This script mainly consists of:
- 6 functions that get called only once when Geb enters a new phase, one for each phase (except for the first one).
- 7 functions that get called every frame depending on Geb's phase, one for each phase.

Documentation updated 4/1/2025
\author Alexander Art
\todo Freeze the player during the cutscenes.
\todo Make the distribution of the falling rocks less RNG-based.
\todo Keep track of the camera's default offset and return the camera to the default offset when Geb is defeated.
*/
public class GebRoomController : MonoBehaviour
{
    /// Reference to the Virtual Camera. Needed for camera control during the cutscenes.
    [SerializeField] protected CinemachineVirtualCamera virtualCamera;
    /// Reference to the Virtual Camera's transposer. Needed for extra camera control.
    protected CinemachineFramingTransposer framingTransposer;
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
    /// Reference to the rock (prefab) that Geb throws during the opening cutscene.
    [SerializeField] protected GameObject cutsceneRock;
    /// Reference to the part of the door in the scene that Geb closes off in the opening cutscene.
    [SerializeField] protected GameObject doorBlocker;
    /// Reference to the rocks (prefab) that fall from the sky during Geb's earthquake attack in phase 3.
    [SerializeField] protected GameObject fallingSkyRocks;
    /// Reference to the defeat message that appears after Geb is defeated.
    [SerializeField] protected GameObject defeatedMessage;
    /// Reference to the interactable zone for when the player needs to press E after Geb is defeated.
    [SerializeField] protected GameObject interactableZone;
    /// Reference to the warp obelisk that appears after Geb is defeated.
    [SerializeField] protected GameObject warpObelisk;
    /// Reference to the shattered version of Geb that is instantiated after Geb is defeated.
    [SerializeField] protected GameObject shatteredGeb;

    /// How far to zoom out the camera for Geb's bossfight.
    float fightZoom = 11f;
    /// How often a rock will fall from the sky during Geb's earthquake attack.
    float fallingRockSpawnPeriod = 0.075f;

    /// Create random number generator.
    private System.Random rng = new System.Random();
    /// Number of rock golems currently in the room.
    public int rockGolemCount = 0;
    /// Maximum number of rock golems that can be present in the room before they stop getting spawned.
    public int maxRockGolems = 4;
    /// The zoom of the camera before the bossfight starts.
    private float defaultZoom;
    /// Used for keeping track of when the last rock fell from the sky.
    private float fallingRockSpawnTimer = 0.0f;
    /// The minimum x position that the player can have. Calculated using the player's width and the bounds of the room.
    private float minPlayerPosX;
    /// The maximum x position that the player can have. Calculated using the player's width and the bounds of the room.
    private float maxPlayerPosX;
    /// Reference to the rock that is spawned during the opening cutscene.
    private GameObject summonedRock;
    /// Used for making sure only one rock is spawned during the opening cutscene.
    private bool rockSummoned = false;

    /// Set references.
    void Awake()
    {
        player = GameObject.Find("Player");
        phaseController = GetComponent<GebPhaseController>();
        bossController = GetComponent<GebBossController>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    /// Access the current zoom to set defaultZoom variable. Also calculate the minimum and maximum x position for the player.
    void Start()
    {
        defaultZoom = virtualCamera.m_Lens.OrthographicSize; // It wasn't letting me do this during Awake().
    
        // Get the width of the player.
        float playerWidth = player.GetComponent<BoxCollider2D>().bounds.size.x;
        // Calculate the minimum x position for the player, factoring in the player's collider's width.
        minPlayerPosX = bounds.LeftPoint().x + playerWidth / 2;
        // Calculate the maximum x position for the player, factoring in the player's collider's width.
        maxPlayerPosX = bounds.RightPoint().x - playerWidth / 2;
    }

    /// <summary>
    /// Every frame, activate the logic for the current phase Geb is in.
    /// Regardless of phase, keep the player within the bounds of the bossroom.
    /// </summary>
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

        if (phaseController.phase != GebPhase.Inactive)
        {
            // If the player is past the left boundary, move them right.
            // If the player is past the right boundary, move them left.
            if (minPlayerPosX > player.transform.position.x)
            {
                player.transform.position = new Vector2(minPlayerPosX, player.transform.position.y);
            }
            else if (maxPlayerPosX < player.transform.position.x)
            {
                player.transform.position = new Vector2(maxPlayerPosX, player.transform.position.y);
            }
        }
    }

    /// Called by GebPhaseController once when the opening cutscene starts.
    public void GebOpeningCutsceneStarted() {
        // Set Geb's healthbar to empty so the fill animation can be played.
        healthbar.SetHealthbarPercentage(0f);
    }
    /// Called by GebPhaseController once when the opening cutscene is over and the bossfight starts.
    public void GebPhase1Started() {}
    /// Called by GebPhaseController once when phase 2 starts.
    public void GebPhase2Started() {}
    /// Called by GebPhaseController once when phase 3 starts.
    public void GebPhase3Started() {}
    /// Called by GebPhaseController once when the closing cutscene starts.
    public void GebClosingCutsceneStarted()
    {
        // Hide Geb's healthbar.
        healthbar.SetHealthbarVisible(false);

        // Stop all of Geb's rocks from dealing damage (thrown rocks, wall debris, and falling rocks) after Geb is defeated.
        GebRock[] gebRocks = GameObject.FindObjectsOfType<GebRock>();
        foreach (GebRock gebRock in gebRocks)
        {
            Destroy(gebRock.transform.GetComponent<DamageOnTrigger>());
        }

        // Stop spawning rock golems.
        maxRockGolems = 0;
    }
    /// Called by GebPhaseController once when the closing cutscene finishes.
    public void GebDefeated()
    {
        /// Move the defeated message to Geb.
        defeatedMessage.transform.position = transform.position;
        /// Make the defeated message visible.
        defeatedMessage.SetActive(true);

        /// Move the interactable to Geb.
        interactableZone.transform.position = transform.position;
        
        /// Move the warp obelisk to Geb (y - 1.75f to bring to the ground).
        warpObelisk.transform.position = new Vector3(transform.position.x, transform.position.y - 1.75f, transform.position.z);
    }

    /// Runs every frame when Geb is inactive.
    void InactiveState() {}
    /// Runs every frame when the opening cutscene is playing.
    void OpeningCutsceneState() {
        // Pan to Geb for two seconds.
        if (phaseController.phaseTime < 2f)
        {
            float deltaX = transform.position.x - player.transform.position.x;
            float deltaY = transform.position.y - player.transform.position.y;
            framingTransposer.m_TrackedObjectOffset = new Vector3(deltaX * phaseController.phaseTime / 2f, 1.4f * (1f - phaseController.phaseTime / 2f) + deltaY * phaseController.phaseTime / 2f, 0f);
        }

        if (2f < phaseController.phaseTime && phaseController.phaseTime < 3f)
        {
            // TEMPORARY Wiggle animation.
            transform.position = new Vector3(transform.position.x + (float)Math.Cos(50f * Time.time) / 50f * phaseController.phaseTime / 2f, transform.position.y + (float)Math.Sin(50f * Time.time) / 100f * phaseController.phaseTime / 2f, transform.position.z);
        }

        if (!rockSummoned && phaseController.phaseTime > 3f)
        {
            // Summon rock and get its Rigidbody2D component.
            summonedRock = Instantiate(cutsceneRock, transform.position, transform.rotation);

            // The speed and angle the rock should be thrown at in order to reach the door.
            // Get the rock's gravity.
            float rockAccelerationY = Math.Abs(Physics2D.gravity.y * summonedRock.GetComponent<Rigidbody2D>().gravityScale);
            // Get door's relative position to Geb.
            float deltaX = doorBlocker.transform.position.x - transform.position.x;
            float deltaY = doorBlocker.transform.position.y - transform.position.y;
            // Velocity to throw the rock at.
            Vector2 initialVelocity = bossController.GetMinInitialThrowVelocity(deltaX, deltaY, rockAccelerationY);

            summonedRock.GetComponent<Rigidbody2D>().velocity = initialVelocity;

            rockSummoned = true;
        }

        if (3f < phaseController.phaseTime && phaseController.phaseTime < 4f)
        {
            // Pan to door for one second.
            float gebDeltaX = transform.position.x - player.transform.position.x;
            float gebDeltaY = transform.position.y - player.transform.position.y;
            float gebPercentage = 1f - (phaseController.phaseTime - 3f);
            float doorDeltaX = doorBlocker.transform.position.x - player.transform.position.x;
            float doorDeltaY = doorBlocker.transform.position.y - player.transform.position.y;
            float doorPercentage = (phaseController.phaseTime - 3f) * 2f;
            framingTransposer.m_TrackedObjectOffset = new Vector3((gebDeltaX * gebPercentage + doorDeltaX * doorPercentage) / 2f, (gebDeltaY * gebPercentage + doorDeltaY * doorPercentage) / 2f, 0f);
        }

        // Block the door when the thrown rock crashes into it.
        if (rockSummoned && summonedRock == null)
        {
            doorBlocker.SetActive(true);
        }
        
        // Return camera to player (with some vertical offset to see Geb).
        if (5f < phaseController.phaseTime && phaseController.phaseTime < 6f)
        {
            float doorDeltaX = doorBlocker.transform.position.x - player.transform.position.x;
            float doorDeltaY = doorBlocker.transform.position.y - player.transform.position.y;
            float doorPercentage = 1f - (phaseController.phaseTime - 5f);
            float playerPercentage = phaseController.phaseTime - 5f;
            framingTransposer.m_TrackedObjectOffset = new Vector3(doorDeltaX * doorPercentage, doorDeltaY * doorPercentage + 5f * playerPercentage, 0f);
        }

        // Zoom the camera out when 7f < phaseController.phaseTime < 9f.
        virtualCamera.m_Lens.OrthographicSize = defaultZoom + (fightZoom - defaultZoom) * Math.Max(0f, Math.Min((phaseController.phaseTime - 7f) / 2f, 1f));

        // After 9 seconds, make Geb's healthbar visible.
        if (phaseController.phaseTime > 9f)
        {
            healthbar.SetHealthbarVisible(true);
        }

        // Animate the healthbar to fill from 0% to 100% when 9f < phaseController.phaseTime < 11f.
        healthbar.SetHealthbarPercentage(Math.Max(0f, Math.Min((phaseController.phaseTime - 9f) / 2f, 1f)));

        // After 10 seconds, end the cutscene and start Geb's phase 1.
        if (phaseController.phaseTime > 11f)
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
            // The rock has a bossController.GetCurrentActionPercentage() chance of spawning. This means that at first,
            // the rocks will have a 0% chance to spawn, but by the time the attack finishes, they have a 100% spawn chance.
            fallingRockSpawnTimer += Time.deltaTime;
            if (fallingRockSpawnTimer >= fallingRockSpawnPeriod && rng.NextDouble() >= bossController.GetCurrentActionPercentage())
            {
                fallingRockSpawnTimer = 0f;

                // Determine where to instantiate the rock, then instantiate it.
                float minSpawnX = bounds.LeftPoint().x - 25;
                float maxSpawnX = bounds.RightPoint().x + 25;
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
        // Zoom in the camera for 2 seconds when Geb is defeated.
        virtualCamera.m_Lens.OrthographicSize = fightZoom + (defaultZoom - fightZoom) * Math.Min(phaseController.phaseTime / 2f, 1f);

        // After 2 seconds, end the cutscene.
        if (phaseController.phaseTime > 2f)
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

    /// Runs when the player interacts with the Geb interactable.
    public void GebInteracted()
    {
        if (phaseController.phase == GebPhase.Defeated)
        {
            // Hide the defeated message and disable the interactability.
            defeatedMessage.SetActive(false);
            interactableZone.SetActive(false);

            // Make the warp obelisk visible and interactable.
            warpObelisk.SetActive(true);

            // Shatter Geb like a pot.
            Instantiate(shatteredGeb, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
