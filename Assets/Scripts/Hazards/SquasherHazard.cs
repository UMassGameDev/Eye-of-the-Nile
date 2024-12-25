using UnityEngine;

/** \brief
Script for the squasher hazard.
When the player enters the squasher's trigger zone, it falls under gravity.
When the squasher hits the ground, it stays there for a moment before returning.
This script goes on the trigger zone of the squasher hazard.

Documentation updated 12/24/2024
\author Alexander Art
*/
public class SquasherHazard : MonoBehaviour
{
    /// The squasher hazard that falls when it detects a player.
    [SerializeField] private GameObject squasher;
    /// Hitbox on the bottom of the squasher that will only be active when the squasher is falling.
    [SerializeField] private GameObject fallingDamageHitbox;
    /// Hitbox within the squasher that can only hurt the player if the player clips into the squasher. Always active, but usually only affects the player when the player gets pushed into the ceiling by the squasher.
    [SerializeField] private GameObject crushingHitbox;
    /// The point that the squasher will return to after falling.
    [SerializeField] private GameObject returnPoint;

    /// The gravity scale at which the squasher will fall.
    public float fallSpeed = 1f;
    /// The speed at which the squasher returns to its starting position.
    public float returnSpeed = 3f;
    /// Set to true to let the squasher stop falling the moment player exits its trigger zone. If false, it can't stop falling until it hits the ground.
    public bool allowFallCancel = false;
    /// Set to true to allow the squasher start falling while it's returning to its starting position. If false, it can't start falling until it's at its starting position.
    public bool allowReturnCancel = false;
    /// When the squasher hits the ground, this is how long it stays stuck to the ground (measured in seconds).
    public float groundTime = 1.5f;

    /// Keeps track of how long the squasher has been on the ground (measured in seconds).
    private float groundTimeCounter = 0f;
    /// The four states the squasher can be in are "IDLE", "FALLING", "GROUNDED", and "RETURNING". It will typically cycle through these four states.
    private string state = "IDLE";

    /// <summary>
    /// Handle the logic for the four states that the squasher can be in:
    /// - "IDLE":
    ///     - Keep the squasher still, prevent it from dealing falling damage, and wait for the player to enter its trigger zone.
    /// - "FALLING":
    ///     - Let the squasher fall, let it deal falling damage, and wait for it be on the ground.
    /// - "GROUNDED":
    ///     - Let the squasher fall if at all possible, but prevent it from dealing falling damage.
    ///     - Once some time passes, set the squasher's state to "RETURNING".
    /// - "RETURNING":
    ///     - Move the squasher upwards and prevent it from dealing falling damage.
    ///     - Once the squasher is in its starting position, set its state to "IDLE".
    /// </summary>
    void Update()
    {
        switch (state)
        {
            // In the "IDLE" state, the squasher is in its starting position and is waiting for the player.
            case "IDLE":
                // Prevent the squasher from falling and remove any velocity.
                squasher.GetComponent<Rigidbody2D>().gravityScale = 0f;
                squasher.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);

                // Prevent the squasher from dealing falling damage.
                fallingDamageHitbox.SetActive(false);
                
                break;
            // In the "FALLING" state, the squasher is falling and can hurt the player.
            case "FALLING":
                // Let the squasher fall.
                squasher.GetComponent<Rigidbody2D>().gravityScale = fallSpeed;

                // Let the squasher deal falling damage.
                fallingDamageHitbox.SetActive(true);
                
                // If the squasher has no y-velocity, it is considered on the ground.
                // However, the state is not set to grounded right away to give the squasher time to accelerate from rest.
                if (squasher.GetComponent<Rigidbody2D>().velocity.y == 0f)
                {
                    groundTimeCounter += Time.deltaTime;
                }

                // Once the squasher has been on the ground for 0.1 seconds, set the state to grounded.
                if (groundTimeCounter >= 0.1f)
                {
                    groundTimeCounter = 0f;
                    state = "GROUNDED";
                }

                break;
            // In the "GROUNDED" state, the squasher has hit the ground and it will stay there for a moment.
            case "GROUNDED":
                // Let the squasher fall (it is on the ground, so it probably won't).
                squasher.GetComponent<Rigidbody2D>().gravityScale = fallSpeed;

                // Prevent the squasher from dealing falling damage (it is on the ground, so it probably wouldn't anyways).
                fallingDamageHitbox.SetActive(false);

                // Stay grounded for groundTime duration, then return.
                groundTimeCounter += Time.deltaTime;
                if (groundTimeCounter >= groundTime)
                {
                    groundTimeCounter = 0f;
                    state = "RETURNING";
                }

                break;
            // In the "RETURNING" state, the squasher goes up to its starting position.
            case "RETURNING":
                // Prevent the squasher from falling and remove any velocity.
                squasher.GetComponent<Rigidbody2D>().gravityScale = 0f;
                squasher.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);

                // Prevent the squasher from dealing falling damage.
                fallingDamageHitbox.SetActive(false);

                // Move the squasher towards the starting position.
                squasher.transform.position = Vector2.MoveTowards(squasher.transform.position, returnPoint.transform.position, returnSpeed * Time.deltaTime);

                // If the squasher is back in its return position, it goes back to idling.
                if (squasher.transform.position == returnPoint.transform.position)
                {
                    state = "IDLE";
                }

                break;
        }
    }

    /// <summary>
    /// When the player enters the squasher's trigger zone and the squasher is ready to fall, make it fall.
    /// The squasher is considered ready to fall when:
    ///     - It is idle (at its starting position)
    ///     OR
    ///     - it is returning to its starting position and is allowed to cancel its returning state.
    /// </summary>
    /// <param name="col">Represents the object that entered the trigger zone.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && (state == "IDLE" || (state == "RETURNING" && allowReturnCancel)))
        {
            state = "FALLING";
        }
    }

    /// <summary>
    /// If the squasher is allowed to cancel its falling state, then when the player leaves the trigger zone,
    /// let it go back to its starting position (sets state to "RETURNING").
    /// </summary>
    /// <param name="col">Represents the object that entered the trigger zone.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" && (state == "FALLING" && allowFallCancel))
        {
            state = "RETURNING";
        }
    }
}
