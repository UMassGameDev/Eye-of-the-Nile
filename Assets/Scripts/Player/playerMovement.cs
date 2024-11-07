using UnityEngine;

/** \brief
Handles the player movement, such as walking and jumping, when the user presses the corresponding keys.

Documentation updated 10/8/2024
\author Stephen Nuttall, Roy Pascual, Alexander Art
\todo Implement features that make movement more smooth.
\todo Add animation trigger for when the playing is falling (don't know if it should be its own animation or keep the last frame of the jump animation).
*/
public class PlayerMovement : MonoBehaviour
{
    /** @name Basic movement configuration
    *  Information that can be configured in the Unity Editor to customize how the player moves.
    * 
    *  \todo This goes for nearly every script, but public variables like these that are only public so they can be edited in the Unity Editor
    *  should be changed to [SerializeField]. By replacing public with [SerializeField], the variable is private and cannot be edited arbitrarily by
    *  another script, but can still be viewed and changed in the Unity Editor.
    *  Example: under no circumstances do we expect the jumpForce to be changed by code. It makes more sense to use SpecialJump() or to add a new function
    *  that can properly handle whatever you're trying to do, rather than add potentially conflicting functionality in another script. What if you
    *  accidentally change jumpForce in the middle of a jump? This could create unexpected behavior that could break things.
    *  (If you're new to comp sci, you'll learn more about public and private variables and when to use/not use them in Computing III and IV).
    */
    ///@{
    /// The speed the player should walk at (how fast does the player walk).
    public float moveVelocity = 12.0f;
    /// The amount of force the player should jump with (how high does the player jump).
    public float jumpForce = 10.0f;
    /// The amount of linear drag that should be applied to the rigidbody.
    public float linearDrag = 1.0f;
    /// The amount of time the player has held the vertical input during a jump. Value is 0 if the player is not jumping.
    [SerializeField]
    private float jumpHeldDuration = 0.0f;
    /// How long the player is able to hold jump button before the jump stops increasing in power
    public float maxJumpDuration = 0.2f;
    /// The maximum velocity that the player can fall when vertical input is positive (gliding).
    /// More negative = faster fall.
    public float glideSpeed = -3.5f;
    /// If the player walks off an edge, coyote time is the amount of time after leaving the ground that they can still jump.
    /// This prevents jumps from having to be frame perfect. The user gets a little bit of leniency with the timing of their jumps.
    public float coyoteTime = 0.5f;
    /// Used to prevent multiple coyote jumps before the player returns to the ground.
    [SerializeField]
    private bool coyoteJumpAvailable = false;
    /// The amount of time since the player was last on the ground.
    public float airTime = 0.0f;
    /// True if the player is currently warping to another area through a StageWarp, such as a door or some other exit.
    /// \todo Make this an event rather than a public bool. This is the more "proper" way to do this as it prevents OnWarp from being changed arbitrarily.
    public bool OnWarp {get; set;} = false;
    ///@}

    /** @name Double Jumping
    *  Information related to jump chains (double jumping, tripple jumping, etc) and their functionality.
    */
    ///@{
    /// True if the player is currently falling, meaning they're in the air but their height in the world is decreasing.
    /// \note Stays true until the player jumps again, even if the player lands, stops decreasing their height or even increases it without jumping.
    /// Maybe this variable should be called "wasFalling" but I think that would be more confusing.
    /// \deprecated No longer used and will likely be removed soon.
    bool isFalling = false;
    /// \brief Maximum amount of times the player can jump without touching the ground.
    /// \note 0 would be a normal jump, 1 allows for double jumping, 2 for triple jumping, etc.
    public int maxJumpChain = 1;
    /// Amount of jumps still available in the jump chain before the player must touch the ground again to jump.
    int jumpsAvailable;
    ///@}

    /** @name Reference Variables
    *  These variables are references to other components on the player this script needs to communicate with.
    */
    ///@{
    /// Reference to the player's rigidbody component.
    Rigidbody2D rb;
    /// Reference to the player's animator component.
    public Animator animator;
    /// References to the player's PlayerHealth component.
    public PlayerHealth objectHealth;
    /// Reference to the player's GroundDetector - a small trigger zone beneath the player's feet that detects if the player is on the ground.
    GroundDetector groundDetector;
    ///@}

    /// Set references
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetector = transform.GetComponentInChildren<GroundDetector>();
    }

    /// Set linear drag in rigidbody to linearDrag, and jumpsAvailable to maxJumpChain.
    void Start()
    {
        rb.drag = linearDrag;
        jumpsAvailable = maxJumpChain;
    }

    /// <summary>
    /// Determines how the player's movement should be processed this frame. See detailed view for steps.
    /// 
    /// Steps:
    /// - Get horizontal and vertical input.
    /// - If the player is not currently warping (using a door or going through an exit) update their velocity to match the horizontal input.
    /// - Flip the player to face the direction they are moving in.
    /// - Check if the player is falling.
    /// - Disable the option for coyote time jumps if the player is moving up.
    /// - If the conditions are met, let the player jump.
    ///     - Conditions to initialize a jump:
    ///         - Vertical input is positive (the user is pressing either, W or the up arrow) or the user is pressing the space bar. This also gets reffered to as "the jump button."
    ///         - The player is not currently warping (using a door or going through an exit).
    ///         - One of these three scenarios are true:
    ///             1. The player is on the ground (on ground scenario).
    ///             2. The player has recently been on the ground and has not already used a coyote time jump since last grounded (coyote time scenario).
    ///             3. There is at least one jump available for a jump chain (double jump scenario).
    ///     - What do to when a jump is initialized:
    ///         - Count the number of double jumps the player has left.
    ///         - Force the player upwards and update isFalling (by using the Jump() function).
    ///         - Play jumping animation and sound.
    ///         - Count how long the jump has been held.
    /// - The jump button can be held to make a jump last slightly longer and go slightly higher.
    ///     - Conditions to sustain a jump:
    ///         - A jump must have already been initialized (see above).
    ///         - The jump must not have already been sustained for too long.
    ///     - When the jump button is held:
    ///         - Continue to force the player upwards and update isFalling.
    ///         - Continue to count how long the jump button has been held.
    /// </summary>
    void Update()
    {
        // This is a combo of a/d inputs and left/right inputs
        // Negative = Left
        // Positive = Right
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (!WarpInfo.CurrentlyWarping && Time.timeScale > 0.0f)
            animator.SetFloat("horizontalInput", Mathf.Abs(horizontalInput));

        // Likewise, this is w/s and up/down
        // Negative = Down
        // Positive = Up
        float verticalInput = Input.GetAxisRaw("Vertical");

        // If the player is warping, reject horizontal inputs and slow the player down. Otherwise, let them move horizontally as usual.
        if (WarpInfo.CurrentlyWarping)
            rb.velocity = new Vector2(rb.velocity.x * 0.985f, rb.velocity.y);
        else
            rb.velocity = new Vector2(moveVelocity * horizontalInput, rb.velocity.y);

        // Flip the player to face the direction it is going
        if (horizontalInput > 0 && objectHealth.IsDead == false && !WarpInfo.CurrentlyWarping && Time.timeScale > 0.0f)
            transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < 0 && objectHealth.IsDead == false && !WarpInfo.CurrentlyWarping && Time.timeScale > 0.0f)
            transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
        
        // Player must be coming down from previous jump before jumping again
        if (rb.velocity.y < 0f)
            isFalling = true;
        else if (rb.velocity.y > 0.1f)
            coyoteJumpAvailable = false; // Disable coyote time jump availability after jump (when player is moving up).

        if (groundDetector.isGrounded)
        {
            airTime = 0.0f; // Reset airTime when grounded
            coyoteJumpAvailable = true; // Enable coyote time jump availability when grounded
        }
        else
            airTime += Time.deltaTime; // Airtime increases when the player is not on the ground

        if ((verticalInput > 0 || Input.GetKey(KeyCode.Space)) && !OnWarp && !WarpInfo.CurrentlyWarping && Time.timeScale > 0.0f)
        {
            if (groundDetector.isGrounded && jumpHeldDuration == 0) // On ground scenario
            {
                jumpsAvailable = maxJumpChain;
                Jump();
                jumpHeldDuration += Time.deltaTime;
                animator.SetTrigger("Jump");
                AudioManager.Instance.PlaySFX("jump");
            }
            else if ((airTime < coyoteTime) && coyoteJumpAvailable && jumpHeldDuration == 0f) // Coyote time scenario
            {
                jumpsAvailable = maxJumpChain;
                Jump();
                jumpHeldDuration += Time.deltaTime;
                animator.SetTrigger("Jump");
                AudioManager.Instance.PlaySFX("jump");
            }
            else if (jumpsAvailable != 0 && jumpHeldDuration == 0f) // Double jump scenario
            {
                jumpsAvailable--;
                Jump();
                jumpHeldDuration += Time.deltaTime;
                animator.SetTrigger("DoubleJump");
                AudioManager.Instance.PlaySFX("jump");
            }
            else if (jumpHeldDuration != 0f && jumpHeldDuration < maxJumpDuration) // Continues the jump when the jump button is held
            {
                Jump();
                jumpHeldDuration += Time.deltaTime;
            }

            // Gliding - This makes the player fall slower when the vertical input is positive (jump button held) and the player is not warping.
            if (rb.velocity.y < glideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, glideSpeed);
            }

            animator.SetBool("IsGliding", true);

            if (groundDetector.isGrounded)
            {
                animator.SetBool("IsGliding", false);
            }
        }
        else
        {
            jumpHeldDuration = 0.0f; // The jump ends when the jump button is no longer pressed (or when the player is warping).
            animator.SetBool("IsGliding", false);
        }
    }

    /// Makes the player jump by canceling current velocity, adding upwards force, and setting isFalling to false.
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
        rb.AddRelativeForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
        animator.SetBool("IsGliding", false);
        isFalling = false;
    }

    /// <summary>
    /// Makes the player jump by canceling current velocity, adding upwards force, triggering the animation and sound effect, and setting isFalling to false.
    /// This jump allows for the amount of force applied to be specified.
    /// </summary>
    /// <param name="newJumpForce">Amount of force to apply to the jump.</param>
    public void Jump(float newJumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
        rb.AddRelativeForce(new Vector2(0.0f, newJumpForce), ForceMode2D.Impulse);
        animator.SetTrigger("Jump");
        animator.SetBool("IsGliding", false);
        AudioManager.Instance.PlaySFX("jump");
        isFalling = false;
    }
}
