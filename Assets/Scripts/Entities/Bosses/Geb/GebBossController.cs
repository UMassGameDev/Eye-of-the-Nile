using UnityEngine;
using System;

/** \brief
This script is a work in progress. It will control Geb's movement and will trigger Geb's attacks.
This script mainly consists of:
- 6 functions that get called only once when Geb enters a new phase, one for each phase (except for the first one).
- 7 functions that get called every frame depending on Geb's phase, one for each phase.

Documentation updated 12/31/2024
\author Alexander Art
*/
public class GebBossController : MonoBehaviour
{
    /// Reference to Geb's health script, used for keeping Geb invincible before the bossfight starts.
    protected BossHealth bossHealth;
    /// Reference to Geb's Rigidbody 2D
    protected Rigidbody2D rb;
    /// Reference to Geb's PatrolZone (Bounds), the left end and right end of Geb's bossroom that he will move around in.
    [SerializeField] protected PatrolZone bounds;
    /// Reference to Geb's phase controller.
    protected GebPhaseController phaseController;
    /// Reference to the player object.
    protected GameObject player;
    /// Reference to the rock prefab for the rocks that Geb throws and have a chance to turn into a rock golem.
    [SerializeField] protected GameObject throwableRock;

    /// (Possibly temporary) radius around the boss that the player must be within for the bossfight to start.
    protected float bossActivationRadius = 11f;
    /// The speed at which Geb moves in phase 1.
    protected float flyingSpeed = 10f;

    /// True if Geb is in the middle of an attack animation.
    private bool attacking;
    /// The speed at which Geb's rocks are thrown in phase 1.
    private float throwSpeed = 20f;
    /// Create random number generator.
    private System.Random rng = new System.Random();
    /// The direction Geb moves in.
    private float horizontalDirection = 0f;
    /// Keep track of how long Geb has been doing whatever he's currently doing. Used for controlling movement behavior.
    private float currentActionTimer = 0.0f;
    /// The minimum horizontal distance that Geb will try to keep from the player in phase 1 when flying.
    private float minPlayerDistanceX = 10f;
    /// The maximum horizontal distance that Geb will try to keep from the player in phase 1 when flying.
    private float maxPlayerDistanceX = 20f;
    /// Either "LEFT" or "RIGHT", Geb will try to stay on this side of the player in phase 1 when flying.
    private string side;
    /// The x position that Geb is moving towards.
    private float targetPositionX;
    /// Used in phase 1 for keeping track of when Geb should stop moving.
    private bool notMoving;
    /// Amount of time Geb stays notMoving (in seconds).
    private float pauseDuration;

    /// Set references to player GameObject, Geb's BossHealth, and Geb's phase controller.
    void Awake()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
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
    public void GebOpeningCutsceneStarted() {}

    /// Called by GebPhaseController once when the opening cutscene is over and the bossfight starts.
    public void GebPhase1Started() {
        // Make Geb vincible once the bossfight starts.
        rb.simulated = true;

        // Geb flies in this phase.
        rb.gravityScale = 0f;

        // Geb hasn't done anything yet. Make sure attacking and currentActionTimer are reset.
        attacking = false;
        currentActionTimer = 0.0f;

        // Geb will start off moving.
        notMoving = false;

        // Initialize first movement cycle (this is very similar to the Phase1State() logic):
        // - Set Geb's direction to be away from the player.
        // - Calculate Geb's range of positions with minPlayerDistanceX and maxPlayerDistanceX.
        // - Make sure that Geb won't try to move out of bounds.
        // - Set Geb's target position based on the side of the player that Geb wants to stay on and the range of positions.

        // Geb initially moves away from the player.
        if (transform.position.x >= player.transform.position.x) // Player is not to the right of Geb.
        {
            side = "RIGHT";
        }
        else // Player is to the right of Geb.
        {
            side = "LEFT";
        }

        // The range of positions that Geb will try to be. [leftMin, leftMax) U [rightMin, rightMax)
        float leftMin = Math.Max(player.transform.position.x - maxPlayerDistanceX, bounds.LeftPoint().x); // Keeps leftmost point in bounds.
        float leftMax = player.transform.position.x - minPlayerDistanceX;
        float rightMin = player.transform.position.x + minPlayerDistanceX;
        float rightMax = Math.Min(player.transform.position.x + maxPlayerDistanceX, bounds.RightPoint().x); // Keeps rightmost point in bounds.

        // If Geb's left positions are all out of bounds then Geb must move right, and vice versa.
        if (leftMax <= bounds.LeftPoint().x)
        {
            side = "RIGHT";
        }
        else if (rightMax >= bounds.RightPoint().x)
        {
            side = "LEFT";
        }

        // Set Geb's target position using the calculated values.
        if (side == "LEFT")
        {
            targetPositionX = leftMin + (float)rng.NextDouble() * (leftMax - leftMin);
        }
        else if (side == "RIGHT")
        {
            targetPositionX = rightMin + (float)rng.NextDouble() * (rightMax - rightMin);
        }
    }

    /// Called by GebPhaseController once when phase 2 starts.
    public void GebPhase2Started() {
        // Geb stops flying at this phase.
        rb.gravityScale = 1f;
    }

    /// Called by GebPhaseController once when phase 3 starts.
    public void GebPhase3Started() {}
    /// Called by GebPhaseController once when the closing cutscene starts.
    public void GebClosingCutsceneStarted() {}
    /// Called by GebPhaseController once when the closing cutscene finishes.
    public void GebDefeated() {}

    /// Runs every frame when Geb is inactive.
    void InactiveState()
    {
        // Make Geb invincible before the bossfight starts.
        rb.simulated = false;

        // Wait for the player to get close to start the opening cutscene.
        if (Vector2.Distance(transform.position, player.transform.position) < bossActivationRadius)
        {
            phaseController.StartGebOpeningCutscene();
        }

        // Floating animation.
        transform.position = new Vector3(transform.position.x, 16f + (float)Math.Sin(2f * Time.time) / 2f, transform.position.z);
    }

    /// Runs every frame when the opening cutscene is playing.
    void OpeningCutsceneState() {
        // Make Geb invincible before the bossfight starts.
        rb.simulated = false;

        // Floating animation.
        transform.position = new Vector3(transform.position.x, 16f + (float)Math.Sin(2f * Time.time) / 2f, transform.position.z);
    }

    /// Runs every frame when Geb is in phase 1.
    // Steps:
    // - Update currentActionTimer and horizontalDirection.
    // - Start a new movement cycle. Geb will either stand still, set a new target position, or summon a rock golem.
    //      - There are two regions on either side of the player, a left region and a right region.
    //        The size of these regions are defined by minPlayerDistanceX and maxPlayerDistanceX.
    //        Geb's target positions stay within one of these regions until he decides to move to the other side of the player.
    // - Update Geb's velocity.
    // - Flip Geb depending on which region his target position is in.
    void Phase1State() {
        // Update currentActionTimer.
        currentActionTimer += Time.deltaTime;

        // Set Geb to move towards the target position.
        if (transform.position.x < targetPositionX) // Geb is to the left of the target position.
        {
            horizontalDirection = 1f;
        }
        else if (transform.position.x > targetPositionX) // Geb is to the right of the target position.
        {
            horizontalDirection = -1f;
        }

        // Make sure Geb doesn't move when notMoving is true.
        if (notMoving == true)
        {
            horizontalDirection = 0f;
        }
        
        // If Geb is done standing still and is within 1 unit of the target position,
        // then either make geb stand still again or set a new target position.
        if ((notMoving == false || currentActionTimer > pauseDuration) && (targetPositionX - 1 < transform.position.x && transform.position.x < targetPositionX + 1))
        {
            // Movement cycle complete. Reset currentActionTimer.
            currentActionTimer = 0.0f;

            // In phase 1, Geb only has one attack, and that is to throw rocks/summon rock golems.
            // The attack is started once pauseDuration runs out, so that's why the code for this is placed here.
            if (attacking)
            {
                // Summon rock.
                GameObject rock = Instantiate(throwableRock, transform.position, transform.rotation);
                
                // Throw rock foward.
                if (side == "LEFT")
                {
                    rock.GetComponent<Rigidbody2D>().velocity = new Vector2(throwSpeed, 0f);
                }
                else if (side == "RIGHT")
                {
                    rock.GetComponent<Rigidbody2D>().velocity = new Vector2(-throwSpeed, 0f);
                }

                // Complete attack.
                attacking = false;

                // Geb is done standing still.
                notMoving = false;
            }

            // 50% chance for Geb to stop moving for pauseDuration seconds, unless if Geb was already paused.
            // 30% chance to Set a new target position:
            // - Geb will have a 10% chance to switch which side of the player he was on.
            // - Calculate Geb's range of positions with minPlayerDistanceX and maxPlayerDistanceX.
            // - Make sure that Geb won't try to move out of bounds.
            // - Set Geb's target position based on the side of the player that Geb wants to stay on and the range of positions.
            // 20% chance for Geb to stop moving and throw a rock.

            double randomNumber = rng.NextDouble();

            if (randomNumber < 0.5 && notMoving == false) // 50% chance for Geb to stop moving for pauseDuration seconds, unless if Geb was already paused.
            {
                // How long Geb should stop moving.
                pauseDuration = 0.5f;

                // Make Geb not move until pauseDuration runs out.
                notMoving = true;
            }
            else if (randomNumber < 0.8) // 30% chance to Set a new target position.
            {
                // Geb will be moving.
                notMoving = false;

                // Geb initially moves away from the player.
                if (side == "LEFT")
                {
                    // 10% chance to change direction.
                    if (rng.NextDouble() < 0.1)
                        side = "RIGHT";
                    else { side = "LEFT"; }
                }
                else if (side == "RIGHT")
                {
                    // 10% chance to change direction.
                    if (rng.NextDouble() < 0.1)
                        side = "LEFT";
                    else { side = "RIGHT"; }
                }

                // The range of positions that Geb will try to be. [leftMin, leftMax) U [rightMin, rightMax)
                float leftMin = Math.Max(player.transform.position.x - maxPlayerDistanceX, bounds.LeftPoint().x); // Keeps leftmost point in bounds.
                float leftMax = player.transform.position.x - minPlayerDistanceX;
                float rightMin = player.transform.position.x + minPlayerDistanceX;
                float rightMax = Math.Min(player.transform.position.x + maxPlayerDistanceX, bounds.RightPoint().x); // Keeps rightmost point in bounds.

                // If Geb's left positions are all out of bounds then Geb must move right, and vice versa.
                if (leftMax <= bounds.LeftPoint().x)
                {
                    side = "RIGHT";
                }
                else if (rightMax >= bounds.RightPoint().x)
                {
                    side = "LEFT";
                }

                // Set Geb's target position using the calculated values.
                if (side == "LEFT")
                {
                    targetPositionX = leftMin + (float)rng.NextDouble() * (leftMax - leftMin);
                }
                else if (side == "RIGHT")
                {
                    targetPositionX = rightMin + (float)rng.NextDouble() * (rightMax - rightMin);
                }
            }
            else // 20% chance to throw rock.
            {
                // How long Geb should stop moving before throwing the rock.
                pauseDuration = 0.5f;

                // Make Geb not move until pauseDuration runs out.
                notMoving = true;

                // Make Geb throw a rock once pauseDuration runs out.
                attacking = true;
            }
        }

        // Update Geb's velocity.
        rb.velocity = new Vector2(horizontalDirection * flyingSpeed, rb.velocity.y);

        // Flip Geb depending on which region the target position is in.
        if (side == "LEFT")
        {
            transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
        }
        else if (side == "RIGHT")
        {
            transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
        }
    }

    /// Runs every frame when Geb is in phase 2.
    void Phase2State() {}
    /// Runs every frame when Geb is in phase 3.
    void Phase3State() {}
    /// Runs every frame when Geb is defeated and the closing cutscene is playing.
    void ClosingCutsceneState() {}
    /// Runs every frame when the closing cutscene is over.
    void DefeatedState() {}

    // Flip Geb to face the player.
    void FacePlayer()
    {
        // Face the player.
        if (transform.position.x > player.transform.position.x) // Player is on the left.
        {
            transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
        }
        else if (transform.position.x < player.transform.position.x) // Player is on the right.
        {
            transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
        }
    }
}
