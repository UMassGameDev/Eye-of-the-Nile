using UnityEngine;
using System;

/** \brief
This script is a work in progress. It will control Geb's movement and will trigger Geb's attacks.
This script mainly consists of:
- 6 functions that get called only once when Geb enters a new phase, one for each phase (except for the first one).
- 7 functions that get called every frame depending on Geb's phase, one for each phase.
- An enum for the actions that Geb will use in phases 1-3.

Documentation updated 1/4/2025
\author Alexander Art
*/

/// <summary>
/// The actions that Geb will use in phases 1-3. Phases Inactive, OpeningCutscene, ClosingCutscene, and Defeated DO NOT use this.
/// New actions unlock cumulatively with each additional phase until the fight is over.
/// </summary>
public enum GebAction
{
    /*! Unlocked in phase 1. On Idle, Geb stops moving and does nothing (usually brief).*/
    Idle,
    /*! Unlocked in phase 1. On Moving, Geb moves until reaching a target position.*/
    Moving,
    /*! Unlocked in phase 1. On RockThrowAttack, Geb stops moving, prepares to throw a rock, and ends by throwing it.*/
    RockThrowAttack
    /*! Phase 2 and 3 coming soon...*/
}

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
    /// The maximum number of attack actions that can happen in a row.
    protected int maxAttackChain = 3;
    /// <summary>
    /// The minimum number of attacks that can happen in a row, assuming there has been at least 1 attack.
    /// Values less than or equal to 1 do nothing.
    /// </summary> 
    protected int minAttackChain = -1;
    /// The maximum number of non-attack actions that can happen in a row.
    protected int maxNonAttackChain = 3;
    /// <summary>
    /// The minimum number of non-attack actions that can happen in a row, assuming there has been at least 1 non-attack.
    /// Values less than or equal to 1 do nothing.
    /// </summary>
    protected int minNonAttackChain = -1;
    /// (Phase 1) The speed at which Geb moves/flies.
    protected float flyingSpeed = 15f;
    /// (Phase 1) The duration of the rock throw animation before the rock entity is spawned (in seconds).
    protected float throwDuration = 1f;
    /// (Phase 1) Limit Geb's rock throwing speed.
    protected float maxThrowSpeed = 100f;
    /// (Phase 1) Each time Geb stops moving (Idle), this is the duration it will last (in seconds).
    protected float idleDuration = 1f;
    /// (Phase 1) The minimum horizontal distance that Geb will try to keep from the player when moving.
    protected float minPlayerDistanceX = 10f;
    /// (Phase 1) The maximum horizontal distance that Geb will try to keep from the player when moving.
    protected float maxPlayerDistanceX = 20f;

    /// The current action that Geb is taking.
    private GebAction currentAction;
    /// Create random number generator.
    private System.Random rng = new System.Random();
    /// The number of times in a row Geb has attacked. Negative values count how many times in a row Geb doesn't attack.
    private int attackCount;
    /// The direction Geb moves in.
    private float horizontalDirection = 0f;
    /// How long Geb has been doing his current action (in seconds).
    private float currentActionTimer = 0.0f;
    /// How long the current action should last (in seconds).
    private float currentActionDuration;
    /// The x position that Geb is moving towards.
    private float targetPositionX;
    /// <summary>
    /// Either "LEFT" or "RIGHT", Geb will try to stay on this side of the player in phase 1 when moving.
    /// This is done by always picking targetPositionX to be on this side of the player.
    /// This also determines which direction Geb faces. Geb always moves backwards in phase 1.
    /// </summary>    
    private string side;

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

        // Geb initially moves away from the player.
        if (transform.position.x >= player.transform.position.x) // Player is not to the right of Geb.
        {
            side = "RIGHT";
        }
        else // Player is to the right of Geb.
        {
            side = "LEFT";
        }

        // Start Geb as Moving and pick a target position for Geb to move to.
        Phase1StartMoving();
    }

    /// Called by GebPhaseController once when phase 2 starts.
    public void GebPhase2Started() {
        // Geb stops flying at this phase.
        rb.gravityScale = 1f;
        
        // Disable linear drag.
        rb.drag = 0f;
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

    /// <summary>
    /// Runs every frame when Geb is in phase 1.
    // Steps:
    // - Update currentActionTimer.
    // - Perform logic depending on Geb's current action.
    //     - Idle:
    //         - Don't move and wait for time to pass.
    //         - Once the time passes, start a different action.
    //     - Moving:
    //         - Update Geb's x velocity.
    //         - Check if Geb has gotten within 1 unit of the target position. If he has, then:
    //             - Check if the player has gotten close to being behind Geb. If so, then Geb has a chance to change direction.
    //             - Otherwise, start a different action.
    //     - RockThrowAttack:
    //         - Don't move and wait for the (missing) rock throwing animation to finish.
    //         - There is a temporary throwing animation for this attack. It looks like Geb wiggles.
    //         - Once the (missing) animation finishes, instantiate and launch the rock projectile.
    //         - Start a new action, either idle or throw another rock projectile.
    // - Update Geb's y velocity/position to never let the player get above Geb.
    // - Flip Geb to match the "side" variable.
    /// </summary>
    void Phase1State() {
        // Update currentActionTimer.
        currentActionTimer += Time.deltaTime;

        // Run the logic that depends on Geb's current action.
        switch (currentAction)
        {
            case GebAction.Idle:
                // Geb is not moving.
                horizontalDirection = 0f;

                // Once Geb has been idle for long enough, start a new action.
                if (currentActionTimer > currentActionDuration)
                {
                    // A new action is going to be started, so currentActionTimer can be reset.
                    currentActionTimer = 0.0f;

                    // Get a random number [0, 1) to be used for randomly picking the next action.
                    double randomNumber = rng.NextDouble();

                    // There is a minimum number of non-attacks that can happen in a row.
                    // Also limit the number of non-attacks that can happen in a row.
                    // Override randomNumber if necessary.
                    if (attackCount > -minNonAttackChain) // The min number of non-attacks has not been reached yet.
                    {
                        randomNumber = 0.4; // Start Moving.
                    }
                    else if (attackCount <= -maxNonAttackChain) // The max number of non-attacks have occurred in a row.
                    {
                        randomNumber = 0.9; // Start a RockThrowAttack.
                    }

                    // Unless overridden,
                    // 80% chance to start Moving.
                    // - 10% chance to change side.
                    // 20% chance to start a RockThrowAttack.
                    if (randomNumber >= 0.0 && randomNumber < 0.8)
                    {
                        // 10% chance to change side.
                        if (side == "LEFT")
                        {   
                            if (rng.NextDouble() < 0.1) { side = "RIGHT"; }
                            else { side = "LEFT"; }
                        }
                        else if (side == "RIGHT")
                        {
                            if (rng.NextDouble() < 0.1) { side = "LEFT"; }
                            else { side = "RIGHT"; }
                        }

                        // Start moving action.
                        Phase1StartMoving();
                        // Update attackCount.
                        if (attackCount > 0)
                            attackCount = -1;
                        else
                            attackCount--;
                    }
                    else if (randomNumber >= 0.8 && randomNumber < 1)
                    {
                        // Face the player.
                        FacePlayer();
                        // Start rock throw attack.
                        currentAction = GebAction.RockThrowAttack;
                        // Make sure that the action lasts for the appropriate amount of time.
                        currentActionDuration = throwDuration;
                        // Update attackCount.
                        if (attackCount < 0)
                            attackCount = 1;
                        else
                            attackCount++;
                    }
                }
                break;

            case GebAction.Moving:
                // If Geb is to the left of the target position, go right.
                // If Geb is to the right of the target position, go left.
                if (transform.position.x < targetPositionX) 
                {
                    horizontalDirection = 1f;
                }
                else if (transform.position.x > targetPositionX)
                {
                    horizontalDirection = -1f;
                }

                // Update Geb's x velocity.
                rb.velocity = new Vector2(horizontalDirection * flyingSpeed, rb.velocity.y);

                // If Geb is within 1 unit of his target position, start a new action.
                if (targetPositionX - 1 < transform.position.x && transform.position.x < targetPositionX + 1)
                {
                    // A new action is going to be started, so currentActionTimer can be reset.
                    currentActionTimer = 0.0f;

                    // Get a random number [0, 1) to be used for randomly picking the next action.
                    double randomNumber = rng.NextDouble();

                    // Limit the number of non-attacks that can happen in a row.
                    // Override randomNumber if necessary.
                    // There is also a minimum number of non-attacks that can happen in a row (don't override for this yet).
                    if (attackCount <= -maxNonAttackChain) // The max number of non-attacks have occurred in a row.
                    {
                        randomNumber = 0.75; // Geb will not change side.
                    }

                    // Unless overridden,
                    // If the player is within 1 unit of being behind Geb, then there is a 50% for Geb to change side.
                    // If the player did not get close to behind Geb or the 50% chance failed, then:
                    // - 30% chance to start being Idle.
                    // - 70% chance to start a RockThrowAttack.
                    if (side == "LEFT" && transform.position.x > player.transform.position.x - 1 && randomNumber < 0.5)
                    {
                        // Change the side.
                        side = "RIGHT";
                        // Reset the Moving action.
                        Phase1StartMoving();
                        // Update attackCount.
                        if (attackCount > 0)
                            attackCount = -1;
                        else
                            attackCount--;
                    }
                    else if (side == "RIGHT" && transform.position.x < player.transform.position.x + 1 && randomNumber < 0.5)
                    {
                        // Change the side.
                        side = "LEFT";
                        // Reset the Moving action.
                        Phase1StartMoving();
                        // Update attackCount.
                        if (attackCount > 0)
                            attackCount = -1;
                        else
                            attackCount--;
                    }
                    else
                    {
                        // Pick a new random number [0, 1).
                        randomNumber = rng.NextDouble();

                        // There is a minimum number of non-attacks that can happen in a row.
                        // Also limit the number of non-attacks that can happen in a row.
                        // Override randomNumber if necessary.
                        if (attackCount > -minNonAttackChain) // The min number of non-attacks has not been reached yet.
                        {
                            randomNumber = 0.15; // Start being Idle.
                        }
                        else if (attackCount <= -maxNonAttackChain) // The max number of non-attacks have occurred in a row.
                        {
                            randomNumber = 0.65; // Start a RockThrowAttack.
                        }

                        // Unless overridden,
                        // 30% chance to start being Idle.
                        // 70% chance to start a RockThrowAttack.
                        if (randomNumber >= 0.0 && randomNumber < 0.3)
                        {
                            // Face the player.
                            FacePlayer();
                            // Start idling.
                            currentAction = GebAction.Idle;
                            // Make sure that the action lasts for the appropriate amount of time.
                            currentActionDuration = idleDuration;
                            // Update attackCount.
                            if (attackCount > 0)
                                attackCount = -1;
                            else
                                attackCount--;
                        }
                        else if (randomNumber >= 0.3 && randomNumber < 1)
                        {
                            // Face the player.
                            FacePlayer();
                            // Start rock throw attack.
                            currentAction = GebAction.RockThrowAttack;
                            // Make sure that the action lasts for the appropriate amount of time.
                            currentActionDuration = throwDuration;
                            // Update attackCount.
                            if (attackCount < 0)
                                attackCount = 1;
                            else
                                attackCount++;
                        }
                    }                    
                }
                break;
            
            case GebAction.RockThrowAttack:
                // Geb is not moving.
                horizontalDirection = 0f;

                // TEMPORARY Wiggle animation.
                transform.position = new Vector3(transform.position.x + (float)Math.Cos(50f * Time.time) / 100f * currentActionTimer / currentActionDuration, transform.position.y + (float)Math.Sin(50f * Time.time) / 100f * currentActionTimer / currentActionDuration, transform.position.z);

                // Before currentActionTimer is greater than currentActionDuration,
                // this time should be used for Geb to prepare a rock (grabbing rock off back animation and throwing animation).
                // Once Geb is done doing that, summon and launch the rock. Then, start a new action.
                // This condition can be replaced with some other condition (or function) that'll be easier to time with the animation.
                if (currentActionTimer > currentActionDuration)
                {
                    // Summon rock and get its Rigidbody2D component.
                    Rigidbody2D rock = Instantiate(throwableRock, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
                    
                    // Calculate the speed the rock should be thrown at in order to hit the player.
                    // Get the rock's gravity.
                    float rockAccelerationY = Math.Abs(Physics2D.gravity.y * rock.gravityScale);
                    // Calculate Geb's relative position to the player
                    float deltaX = Math.Abs(transform.position.x - player.transform.position.x);
                    float deltaY = Math.Abs(transform.position.y - player.transform.position.y);
                    // The predicted amount of time the rock will take to reach the player if they don't move.
                    float throwTime = (float)Math.Sqrt(2f * deltaY / rockAccelerationY);
                    // Get the player's gravity.
                    float playerAccelerationY = Math.Abs(Physics2D.gravity.y * player.GetComponent<Rigidbody2D>().gravityScale);
                    // How much the player's current instantaneous velocity is taken into consideration when throwing the rock.
                    double predictionPercentage = Math.Sqrt(rng.NextDouble());
                    // Predict where the rock will be by the time it reaches the player.
                    deltaX = Math.Abs(transform.position.x - player.transform.position.x - player.GetComponent<Rigidbody2D>().velocity.x * throwTime * (float)predictionPercentage);
                    if (player.GetComponent<Animator>().GetBool("IsGliding") == true) // Do not account for gravity.
                        deltaY = Math.Abs(transform.position.y - player.transform.position.y - player.GetComponent<Rigidbody2D>().velocity.y * throwTime * (float)predictionPercentage);
                    else // Account for gravity.
                        deltaY = Math.Abs(transform.position.y - player.transform.position.y - player.GetComponent<Rigidbody2D>().velocity.y * throwTime * (float)predictionPercentage + 0.5f * playerAccelerationY * throwTime * throwTime * (float)predictionPercentage);
                    deltaY = (float)Math.Min(deltaY, transform.position.y - 5.879); // The player does not go through the floor, so a lower bound is set.
                    // Recalculate how long it will take the rock to reach the player.
                    // In theory, this and the previous step could be repeated to increase accuracy, but the prediction is still accurate without it.
                    throwTime = (float)Math.Sqrt(2f * deltaY / rockAccelerationY);
                    // Calculate the throw speed, but cap it.
                    float throwSpeedX = Math.Min(deltaX / throwTime, maxThrowSpeed);

                    // Face the player before throwing the rock.
                    // This might end up interfering with the throwing animation, in which case this line can be removed.
                    FacePlayer();

                    // Launch rock foward.
                    if (side == "LEFT")
                    {
                        rock.velocity = new Vector2(throwSpeedX, 0f);
                    }
                    else if (side == "RIGHT")
                    {
                        rock.velocity = new Vector2(-throwSpeedX, 0f);
                    }

                    // A new action is going to be started, so currentActionTimer can be reset.
                    currentActionTimer = 0.0f;

                    // Get a random number [0, 1) to be used for randomly picking the next action.
                    double randomNumber = rng.NextDouble();

                    // Limit the number of attacks that can happen in a row.
                    // There is also a minimum number of attacks that can happen in a row.
                    // Override randomNumber if necessary.
                    if (attackCount >= maxAttackChain) // The max number of attacks have occurred in a row.
                    {
                        randomNumber = 0.4; // Start being Idle.
                    }
                    else if (attackCount < minAttackChain) // The min number of attacks has not been reached yet.
                    {
                        randomNumber = 0.9; // Start a RockThrowAttack.
                    }

                    // Unless overridden,
                    // 80% chance to start being Idle.
                    // 20% chance to do another RockThrowAttack.
                    if (randomNumber >= 0.0 && randomNumber < 0.8)
                    {
                        // Face the player.
                        FacePlayer();
                        // Start idling.
                        currentAction = GebAction.Idle;
                        // Make sure that the action lasts for the appropriate amount of time.
                        currentActionDuration = idleDuration;
                        // Update attackCount.
                        if (attackCount > 0)
                            attackCount = -1;
                        else
                            attackCount--;
                    }
                    else if (randomNumber >= 0.8 && randomNumber < 1)
                    {
                        // Face the player.
                        FacePlayer();
                        // Start rock throw attack.
                        currentAction = GebAction.RockThrowAttack;
                        // Make sure that the action lasts for the appropriate amount of time.
                        currentActionDuration = throwDuration;
                        // Update attackCount.
                        if (attackCount < 0)
                            attackCount = 1;
                        else
                            attackCount++;
                    }
                }
                break;

            default:
                Debug.Log("An invalid action for this phase has been activated.");
                break;
        }

        // If the player is above Geb, match Geb's y velocity to the player's y velocity.
        // If the player is not above Geb, and Geb is too high up, lower Geb.
        // If the player is not above Geb, and Geb is low down, raise Geb.
        if (transform.position.y < player.transform.position.y)
        {
            rb.velocity = new Vector2(rb.velocity.x, player.GetComponent<Rigidbody2D>().velocity.y);
        }
        else if (transform.position.y > 16f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -1f);
        }
        else if (transform.position.y < 15f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 1f);
        }

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

    // Change Geb's side to face the player (for phase 1).
    void FacePlayer()
    {
        if (transform.position.x > player.transform.position.x) // Geb is to the right of the player.
        {
            side = "RIGHT";
        }
        else if (transform.position.x < player.transform.position.x) // Geb is to the left of the player.
        {
            side = "LEFT";
        }
    }

    // Flip Geb to face the player.
    void FlipToFacePlayer()
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

    // Used within phase 1 to set Geb's current action to Moving and to calculate a new target position for Geb to move towards.
    // Variable "side" is used to determine whether Geb will pick a target position to the left or to the right of the player.
    private void Phase1StartMoving()
    {
        // This counts as a new action, reset currentActionTimer.
        currentActionTimer = 0.0f;

        // Set Geb's current action to Moving.
        currentAction = GebAction.Moving;

        // The range of positions that Geb will try to be. [leftMin, leftMax) U [rightMin, rightMax)
        float leftMin = Math.Max(player.transform.position.x - maxPlayerDistanceX, bounds.LeftPoint().x); // Keeps leftmost point in bounds.
        float leftMax = player.transform.position.x - minPlayerDistanceX;
        float rightMin = player.transform.position.x + minPlayerDistanceX;
        float rightMax = Math.Min(player.transform.position.x + maxPlayerDistanceX, bounds.RightPoint().x); // Keeps rightmost point in bounds.

        // If all of Geb's left positions are out of bounds then Geb must move right, and vice versa.
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
}
