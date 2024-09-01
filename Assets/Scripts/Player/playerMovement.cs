using UnityEngine;

/** \brief
Handles the player movement, such as walking and jumping, when the user presses the corresponding keys.

Documentation updated 9/1/2024
\author Stephen Nuttall, Roy Pascual
\todo Implement features that make movement more smooth, such as coyote time.
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
    /// Unimplemented feature.
    /// If the player walks off an edge, coyote time is the amount of time after leaving the ground that they can still jump.
    /// This prevents jumps from having to be frame perfect. The user gets a little bit of leniency with the timing of their jumps.
    /// \todo Implement coyote time.
    public float coyoteTime = 0.5f;
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
    /// - Check if the player is falling
    /// - If the conditions are met, let the player jump.
    ///     - Conditions for a jump:
    ///         - Vertical input is positive (the user is pressing either, W or the up arrow) or the user is pressing the space bar.
    ///         - The player is not currently warping (using a door or going through an exit).
    ///         - One of these two scenarios are true:
    ///             1. The player is on the ground or not moving, and the is falling/has fallen (on ground scenario).
    ///             2. The player is falling/has fallen, and there is at least one jump available for a jump chain (double jump scenario).
    ///     - What do to when these conditions are met:
    ///         - Add upwards force to the player.
    ///         - Play jumping animation and sound.
    ///         - Update isFalling and jumpsAvailable.
    /// </summary>
    void Update()
    {
        // This is a combo of a/d inputs and left/right inputs
        // Negative = Left
        // Positive = Right
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("horizontalInput", Mathf.Abs(horizontalInput));

        // Likewise, this is w/s and up/down
        // Negative = Down
        // Positive = Up
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (!WarpInfo.CurrentlyWarping)
            rb.velocity = new Vector2(moveVelocity * horizontalInput, rb.velocity.y);

        // Flip the player to face the direction it is going
        if (horizontalInput > 0 && objectHealth.IsDead == false)
            transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < 0 && objectHealth.IsDead == false)
            transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
        
        // Player must be coming down from previous jump before jumping again
        if (rb.velocity.y < 0f)
            isFalling = true;

        if ((verticalInput > 0 || Input.GetKey(KeyCode.Space)) && !OnWarp)
        {
            if ((groundDetector.isGrounded || (rb.velocity.x == 0 && rb.velocity.y == 0)) && isFalling)
            {
                rb.AddRelativeForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
                AudioManager.Instance.PlaySFX("jump");
                isFalling = false;
                jumpsAvailable = maxJumpChain;
            } else if (isFalling && jumpsAvailable != 0) {
                rb.AddRelativeForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
                AudioManager.Instance.PlaySFX("jump");
                isFalling = false;
                jumpsAvailable--;
            }
        }
    }

    /// <summary>
    /// A special jump can be triggered regardless of the normal jump requirements
    /// </summary>
    /// <param name="jumpForce">Amount of force to apply to the jump.</param>
    public void SpecialJump(float jumpForce)
    {
        rb.AddRelativeForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
        animator.SetTrigger("Jump");
        AudioManager.Instance.PlaySFX("jump");
        isFalling = false;
    }
}
