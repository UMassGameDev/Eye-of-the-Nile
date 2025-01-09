using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/** \brief
This script is a work in progress. It will control Geb's movement and will trigger Geb's attacks.
This script mainly consists of:
- 6 functions that get called only once when Geb enters a new phase, one for each phase (except for the first one).
- 7 functions that get called every frame depending on Geb's phase, one for each phase.
- An enum for the actions that Geb will use in phases 1-3.

Documentation updated 1/7/2025
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
    RockThrowAttack,
    /*! Unlocked in phase 2. On SummoningWall, Geb stops moving and raises a wall. (Counts as an attack, but does no damage.)*/
    SummoningWall,
    /*! Unlocked in phase 2. On ChargeAttack, Geb runs towards the player, deals damage, and breaks any walls in his path.*/
    ChargeAttack
    /*! Phase 3 coming soon...*/
}

public class GebBossController : MonoBehaviour
{
    /// Reference to Geb's health script, used for keeping Geb invincible before the bossfight starts.
    protected BossHealth bossHealth;
    /// Reference to Geb's Rigidbody 2D.
    protected Rigidbody2D rb;
    /// Reference to Geb's Box Collider 2D.
    protected BoxCollider2D bc;
    /// Reference to the bossroom's Bounds (PatrolZone), which has the left and right ends of the bossroom that Geb must stay in.
    [SerializeField] protected PatrolZone bounds;
    /// Reference to Geb's phase controller.
    protected GebPhaseController phaseController;
    /// Reference to the player object.
    protected GameObject player;
    /// Reference to the rock prefab for the rocks that Geb throws and have a chance to turn into a rock golem.
    [SerializeField] protected GameObject throwableRock;
    /// The image to use for Geb in phase 2 once he grows legs.
    [SerializeField] protected Sprite phase2Sprite;

    /// (Possibly temporary) radius around the boss that the player must be within for the bossfight to start.
    protected float bossActivationRadius = 11f;
    /// The maximum number of attack actions that can happen in a row.
    protected int maxAttackChain = 3;
    /// <summary>
    /// The minimum number of attacks that can happen in a row whenever there has already been at least 1 attack.
    /// Values less than or equal to 1 do nothing.
    /// </summary> 
    protected int minAttackChain = -1;
    /// The maximum number of non-attack actions that can happen in a row.
    protected int maxNonAttackChain = 3;
    /// <summary>
    /// The minimum number of non-attack actions that can happen in a row whenever there has already been at least 1 non-attack.
    /// Values less than or equal to 1 do nothing.
    /// </summary>
    protected int minNonAttackChain = 2;
    /// (Phase 1) The speed at which Geb moves horizontally.
    protected float flySpeedX = 15f;
    /// (Phase 1) The duration of the rock throw animation before the rock entity is spawned (in seconds).
    protected float throwDuration = 1f;
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
    /// The minimum x position that Geb can have. Calculated using Geb's width and the bounds of the room.
    private float minPosX;
    /// The maximum x position that Geb can have. Calculated using Geb's width and the bounds of the room.
    private float maxPosX;
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
        bc = GetComponent<BoxCollider2D>();
        phaseController = GetComponent<GebPhaseController>();
    }

    /// Calculate the minimum and maximum x position for Geb using the bounds and his collider's width.
    void Start()
    {
        // Get the width of Geb.
        float gebWidth = bc.bounds.size.x;
        // Calculate the minimum x position for Geb, factoring in his collider's width.
        minPosX = bounds.LeftPoint().x + gebWidth / 2;
        // Calculate the maximum x position for Geb, factoring in his collider's width.
        maxPosX = bounds.RightPoint().x - gebWidth / 2;
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
        // Disable linear drag. Friction with the floor occur instead.
        rb.drag = 0f;

        // Start the phase Idle.
        currentAction = GebAction.Idle;
        // Reset currentActionTimer and attackCount.
        currentActionTimer = 0.0f;
        attackCount = -1;
        // Make sure that the action lasts for the appropriate amount of time.
        currentActionDuration = idleDuration;
        
        // Switch Geb's sprite to the image of Geb with legs.
        GetComponent<SpriteRenderer>().sprite = phase2Sprite;
        // To adjust to the size of the new image, decrease Geb's x and y scale.
        transform.localScale = new Vector3(transform.localScale.x / 1.7f, transform.localScale.y / 1.7f, transform.localScale.z);
        // Increase the size of the BoxCollider2D to roughly match the new scale.
        bc.size = new Vector2(bc.size.x * 1.9f, bc.size.y * 1.9f);
        
        // Flip Geb to face the player.
        FlipToFacePlayer();
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
                rb.velocity = new Vector2(horizontalDirection * flySpeedX, rb.velocity.y);

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

                    // The speed and angle the rock should be thrown at in order to hit the player if they don't move.
                    // Get the rock's gravity.
                    float rockAccelerationY = Math.Abs(Physics2D.gravity.y * rock.gravityScale);
                    // Get player's relative position to Geb.
                    float deltaX = player.transform.position.x - transform.position.x;
                    float deltaY = player.transform.position.y - transform.position.y;
                    // Velocity to throw the rock at.
                    Vector2 initialVelocity = GetMinInitialThrowVelocity(deltaX, deltaY, rockAccelerationY);

                    // Predict where the player will end up by the time the rock reaches them.
                    // The rock is thrown somewhere between this predicted location and the player's current position.
                    // The prediction is based on the rock's calculated airtime, but the result of the prediction changes the airtime.
                    // So, this prediction isn't perfect, but repeating it several times could improve accuracy.
                    // Get the player's gravity.
                    float playerAccelerationY = Math.Abs(Physics2D.gravity.y * player.GetComponent<Rigidbody2D>().gravityScale);
                    // What percentage of the player's current motion is taken into consideration when predicting the new location.
                    double predictionPercentage = Math.Sqrt(rng.NextDouble());
                    // Approximate how long it will take the rock to reach the player
                    float airtime = deltaX / initialVelocity.x;
                    // Predict where the player will be.
                    Vector2 predictedPlayerPosition;                        
                    if (player.GetComponent<Animator>().GetBool("IsGliding") == true) // Do not account for gravity.
                        predictedPlayerPosition = PredictLocation(player.transform.position, player.GetComponent<Rigidbody2D>().velocity, 0, airtime);
                    else // Account for gravity.
                        predictedPlayerPosition = PredictLocation(player.transform.position, player.GetComponent<Rigidbody2D>().velocity, playerAccelerationY, airtime);
                    float predictedDeltaX = predictedPlayerPosition.x - transform.position.x;
                    float predictedDeltaY = predictedPlayerPosition.y - transform.position.y;
                    // Use the prediction percentage.
                    predictedDeltaX = deltaX + (predictedDeltaX - deltaX) * (float)predictionPercentage;
                    predictedDeltaY = deltaY + (predictedDeltaY - deltaY) * (float)predictionPercentage; 
                    // Recalculate which velocity to throw the rock at.
                    initialVelocity = GetMinInitialThrowVelocity(predictedDeltaX, predictedDeltaY, rockAccelerationY);

                    // Face the player before throwing the rock.
                    // If this ends up interfering with the throwing animation, this line can be removed.
                    FacePlayer();

                    // Launch the rock.
                    rock.velocity = initialVelocity;

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

        // Flip Geb depending on which region the target position is in.
        if (side == "LEFT")
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (side == "RIGHT")
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// Runs every frame when Geb is in phase 2.
    void Phase2State()
    {   
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
                rb.velocity = new Vector2(horizontalDirection * flySpeedX, rb.velocity.y);

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
                transform.position = new Vector3(transform.position.x + (float)Math.Cos(50f * Time.time) / 50f * currentActionTimer / currentActionDuration, transform.position.y + (float)Math.Sin(50f * Time.time) / 100f * currentActionTimer / currentActionDuration, transform.position.z);

                // Before currentActionTimer is greater than currentActionDuration,
                // this time should be used for Geb to prepare a rock (grabbing rock off back animation and throwing animation).
                // Once Geb is done doing that, summon and launch the rock. Then, start a new action.
                // This condition can be replaced with some other condition (or function) that'll be easier to time with the animation.
                if (currentActionTimer > currentActionDuration)
                {
                    // Summon rock and get its Rigidbody2D component.
                    Rigidbody2D rock = Instantiate(throwableRock, transform.position, transform.rotation).GetComponent<Rigidbody2D>();

                    // The speed and angle the rock should be thrown at in order to hit the player if they don't move.
                    // Get the rock's gravity.
                    float rockAccelerationY = Math.Abs(Physics2D.gravity.y * rock.gravityScale);
                    // Get player's relative position to Geb.
                    float deltaX = player.transform.position.x - transform.position.x;
                    float deltaY = player.transform.position.y - transform.position.y;
                    // Velocity to throw the rock at.
                    Vector2 initialVelocity = GetMinInitialThrowVelocity(deltaX, deltaY, rockAccelerationY);

                    // Predict where the player will end up by the time the rock reaches them.
                    // The rock is thrown somewhere between this predicted location and the player's current position.
                    // The prediction is based on the rock's calculated airtime, but the result of the prediction changes the airtime.
                    // So, this prediction isn't perfect, but repeating it several times could improve accuracy.
                    // Get the player's gravity.
                    float playerAccelerationY = Math.Abs(Physics2D.gravity.y * player.GetComponent<Rigidbody2D>().gravityScale);
                    // What percentage of the player's current motion is taken into consideration when predicting the new location.
                    double predictionPercentage = Math.Sqrt(rng.NextDouble());
                    // Approximate how long it will take the rock to reach the player
                    float airtime = deltaX / initialVelocity.x;
                    // Predict where the player will be.
                    Vector2 predictedPlayerPosition;                        
                    if (player.GetComponent<Animator>().GetBool("IsGliding") == true) // Do not account for gravity.
                        predictedPlayerPosition = PredictLocation(player.transform.position, player.GetComponent<Rigidbody2D>().velocity, 0, airtime);
                    else // Account for gravity.
                        predictedPlayerPosition = PredictLocation(player.transform.position, player.GetComponent<Rigidbody2D>().velocity, playerAccelerationY, airtime);
                    float predictedDeltaX = predictedPlayerPosition.x - transform.position.x;
                    float predictedDeltaY = predictedPlayerPosition.y - transform.position.y;
                    // Use the prediction percentage.
                    predictedDeltaX = deltaX + (predictedDeltaX - deltaX) * (float)predictionPercentage;
                    predictedDeltaY = deltaY + (predictedDeltaY - deltaY) * (float)predictionPercentage; 
                    // Recalculate which velocity to throw the rock at.
                    initialVelocity = GetMinInitialThrowVelocity(predictedDeltaX, predictedDeltaY, rockAccelerationY);

                    // Face the player before throwing the rock.
                    // If this ends up interfering with the throwing animation, this line can be removed.
                    FacePlayer();

                    // Launch the rock.
                    rock.velocity = initialVelocity;

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
            
            case GebAction.SummoningWall:
                Debug.Log("Geb's wall summon has not been implemented yet!");
                break;
            
            case GebAction.ChargeAttack:
                Debug.Log("Geb's charge attack has not been implemented yet!");
                break;

            default:
                Debug.Log("An invalid action for this phase has been activated.");
                break;
        }

        // Flip Geb depending on which region the target position is in.
        if (side == "LEFT")
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (side == "RIGHT")
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
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
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (transform.position.x < player.transform.position.x) // Player is on the right.
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Used within phase 1 to set Geb's current action to Moving and to calculate a new target position for Geb to move towards.
    // Variable "side" is used to determine whether Geb will pick a target position to the left or to the right of the player.
    // This is also used within phase 2 (temporary).
    private void Phase1StartMoving()
    {
        // This counts as a new action, reset currentActionTimer.
        currentActionTimer = 0.0f;

        // Set Geb's current action to Moving.
        currentAction = GebAction.Moving;

        // The range of positions that Geb will try to be. [leftMin, leftMax) U [rightMin, rightMax)
        float leftMin = Math.Max(player.transform.position.x - maxPlayerDistanceX, minPosX); // Keeps leftmost point in bounds.
        float leftMax = player.transform.position.x - minPlayerDistanceX;
        float rightMin = player.transform.position.x + minPlayerDistanceX;
        float rightMax = Math.Min(player.transform.position.x + maxPlayerDistanceX, maxPosX); // Keeps rightmost point in bounds.

        // If all of Geb's left positions are out of bounds then Geb must move right, and vice versa.
        if (leftMax <= minPosX)
        {
            side = "RIGHT";
        }
        else if (rightMax >= maxPosX)
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

    /// <summary>
    /// Find the velocity vector with the minimum possible magnitude
    /// that can reach a given target location when launching a projectile.
    /// </summary>
    /// <param name="deltaX">The x distance to the target location.</param>
    /// <param name="deltaY">The y distance to the target location.</param>
    /// <param name="gravity">The magnitude of the gravity of the projectile (direction assumed downwards).</param>
    /// <returns>A vector2 that is to be used as the velocity of the projectile.</returns>
    Vector2 GetMinInitialThrowVelocity(float deltaX, float deltaY, float gravity)
    {
        // The only condition where the math breaks is if deltaX and deltaY are both 0, but the solution for that is easy.
        if (deltaX == 0f && deltaY == 0f)
            return new Vector2(0f, 0f);
        // Calculate terms that will simplify the equation.
        double n = gravity * deltaX;
        double m = 2 * gravity * deltaY;
        // This equation calculates the y component of the velocity vector with the minimum possible magnitude that will reach the player.
        // I got to this equation through a combination of physics, calculus, graphing (Desmos), and a solver (WolframAlpha).
        double initialVelocityY = Math.Sqrt(
                                      (Math.Pow(m, 3))         / (          Math.Pow(m, 2) + 4 * Math.Pow(n, 2))
                                    + (Math.Pow(m, 2))         / (Math.Sqrt(Math.Pow(m, 2) + 4 * Math.Pow(n, 2)))
                                    + (4 * Math.Pow(n, 2) * m) / (          Math.Pow(m, 2) + 4 * Math.Pow(n, 2))
                                    + (2 * Math.Pow(n, 2))     / (Math.Sqrt(Math.Pow(m, 2) + 4 * Math.Pow(n, 2)))
                                  ) / Math.Sqrt(2);
        // The x component can be related to the y component and the terms.
        double initialVelocityX = n / (initialVelocityY + Math.Sqrt(Math.Pow(initialVelocityY, 2) - m));
        // Return the vector.
        return new Vector2((float)initialVelocityX, (float)initialVelocityY);
    }

    /// <summary>
    /// Predicts where something will be in a given amount of time based on its current position, velocity, and gravity.
    /// Limits are placed that excludes positions out of bounds.
    /// </summary>
    /// <param name="currentPos">Current position of the entity.</param>
    /// <param name="currentVel">Current velocity of the entity.</param>
    /// <param name="gravity">Magnitude of gravity on the entity (assumed downwards).</param>
    /// <param name="time">Number of seconds in the future this prediction will be made for.</param>
    /// <returns>A vector2 of the predicted position.</returns>
    Vector2 PredictLocation(Vector2 currentPos, Vector2 currentVel, float gravity, float time)
    {
        // Initialize the variables.
        float predictedPosX, predictedPosY;
        // Predict the x position of the entity if it stays at a constant velocity.
        predictedPosX = currentPos.x + currentVel.x * (float)time;
        // Limit the predicion to always stay in bounds.
        predictedPosX = (float)Math.Max(predictedPosX, bounds.LeftPoint().x);
        predictedPosX = (float)Math.Min(predictedPosX, bounds.RightPoint().x);
        // Predict the x position of the entity if it stays at a constant velocity and under constant gravity.
        predictedPosY = currentPos.y + currentVel.y * (float)time - 0.5f * gravity * (float)Math.Pow(time, 2);
        // Limit the prediction to always stay in bounds.
        predictedPosY = (float)Math.Max(predictedPosY, 5.879);
        // Return the position.
        return new Vector2(predictedPosX, predictedPosY);
    }
}
