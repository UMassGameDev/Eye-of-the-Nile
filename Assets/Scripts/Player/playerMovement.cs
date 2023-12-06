using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveVelocity = 12.0f;
    public float jumpForce = 10.0f;
    public float linearDrag = 1.0f;
    public bool OnWarp {get; set;} = false;

    bool canJump = false;

    public Animator animator;
    public PlayerHealth objectHealth;
    GroundDetector groundDetector;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundDetector = transform.GetComponentInChildren<GroundDetector>();
    }
    void Start()
    {
        rb.drag = linearDrag;
    }

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
            canJump = true;

        if ((verticalInput > 0 || Input.GetKey("space")) && !OnWarp)
        {
            if (groundDetector.isGrounded && canJump)
            {
                rb.AddRelativeForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
                AudioManager.Instance.PlaySFX("jump");
                canJump = false;
            }
        }
    }
}
