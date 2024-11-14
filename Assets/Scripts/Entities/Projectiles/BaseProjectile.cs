using UnityEngine;

/** \brief
Basic projectile functionality that every other projectile type must inherit from to be compatible with other systems (like the attack manager).
You can also put this script onto any object or prefab to give it normal projectile functionality.

Documentation updated 11/13/2024
\author Stephen Nuttall, Roy Pascual
\note This script no longer handles projectile movement on its own. Instead, it supports any script that inherits from RudimentaryMovement,
which must be attached separately. This is to make BaseProjectile a more universal solution to projectiles, since each projectile moves differently.
\todo Replace damagePlayers and damageNonPlayers bools with one layer mask that allows for more choices in what is damaged.
*/
public class BaseProjectile : MonoBehaviour
{
    /// \brief Reference to the game object that holds the sprite renderer of the projectile.
    /// Often this is just the projectile itself, but sometimes there's a child with the sprite. This allows compatability with both systems.
    [SerializeField] protected GameObject sprite;

    /// The amount of damage the projectile will apply if it comes in contact with something it can damage.
    [SerializeField] protected int damage = 30;
    /// A mirror of damage that allows for other objects to read its value without changing it, and also allowing damage to appear in the Unity Editor.
    public int refDamage {get => damage; private set => damage = value;}

    /// If true, the projectile can damage the player.
    [SerializeField] protected bool damagePlayers = true;
    /// If true, the projectile can damage objects that aren't the player (assuming they have an ObjectHealth component).
    [SerializeField] protected bool damageNonPlayers = true;
    /// If true, the projectile will destroy itself when it hits something (regardless of if it can damage it or now).
    [SerializeField] protected bool destroyOnImpact = true;

    /// Sound effect that plays when the projectile is spawned in.
    [SerializeField] protected string spawnSFX = "bullet_fire";

    /// When facingLeft is true, the sprite will be flipped. Usually used to make the sprite "face the way the projectile is moving."
    [SerializeField] protected bool flipSprite = true;
    /// True if the projectile is facing (and thus moving) to the left. Likewise, false if the projectile is facing to the right.
    public bool facingLeft {get; private set;} = false;

    /// Updates the direction the projectile is facing and runs AwakeMethods(), which can be changed in scripts that inherit from BaseProjectile.
    void Awake()
    {
        UpdateFacing();
        AwakeMethods();
    }

    /// Runs when Awake() is called.
    protected virtual void AwakeMethods()
    {
        /// Currently does nothing and exists so inheriting functions can use it.
    }

    /// Runs StartMethods(), which can be changed in scripts that inherit from BaseProjectile.
    void Start()
    {
        StartMethods();
    }

    /// Plays the spawn sound effect.
    protected virtual void StartMethods()
    {
        AudioManager.Instance.PlaySFX(spawnSFX);
    }

    /// <summary>
    /// Runs OnTriggerEnterMethods(), which can be changed in scripts that inherit from BaseProjectile.
    /// This is required if your projectile's Collider2D component is set to be a trigger.
    /// </summary>
    /// <param name="collision">Represents the object the projectile collided with.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnterMethods(collision);
    }

    /// \brief Runs if the projectile collides with an object (and it's a trigger zone). Does nothing by default.
    /// \note This function only called if the projectile's Collider2D component is set to be a trigger. This is an option that makes the object have no
    /// collision, but sets off OnTriggerEnter2D() if anything steps into it (usually for something like a finish line in a race, or something similar).
    /// The basic projectile is not set to be a trigger, so this function isn't used in BaseProjectile. A script inheriting from it for a projectile that is
    /// a trigger though (maybe a laser that goes through multiple enemies) could use this.
    /// <param name="collision">Represents the object the projectile collided with.</param>
    protected virtual void OnTriggerEnterMethods(Collider2D collision)
    {
        
    }

    /// <summary>
    /// Runs OnCollisionEnterMethods(), which can be changed in scripts that inherit from BaseProjectile.
    /// Afterwards, destroys the projectile, if enabled.
    /// </summary>
    /// <param name="collisionInfo">Represents the object the projectile collided with.</param>
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        OnCollisionEnterMethods(collisionInfo);

        if (destroyOnImpact)
        {
            Destroy(sprite);
            Destroy(gameObject);
        }
    }

    /// \brief Runs if the projectile collides with an object. Damages the object it collided with if possible, then destroys the projectile.
    /// <param name="collisionInfo">Represents the object the projectile collided with.</param>
    protected virtual void OnCollisionEnterMethods(Collision2D collisionInfo)
    {
        /// Steps:
        /// If the object we collided with has an ObjectHealth AND at least one of the following is true:
        ///  1. both damageNonPlayers AND damagePlayers are true
        ///  2. the object is NOT the player AND damangeNonPlayers is true
        ///  3. the object is the player AND damangePlayers is true,
        /// then apply damage to the object.
        if (collisionInfo.collider.TryGetComponent<ObjectHealth>(out var objHealth) && (// Has ObjectHealth
                damageNonPlayers && damagePlayers ||                                    // #1
                (!collisionInfo.collider.CompareTag("Player") && damageNonPlayers) ||   // #2
                (collisionInfo.collider.CompareTag("Player") && damagePlayers)          // #3
            )
        )
        {
            objHealth.TakeDamage(transform, damage);
        }
    }

    /// Flip the direction the projectile is facing (and thus moving).
    /// Switches facingLeft to be the opposite of what it currently is.
    public void FlipDirection()
    {
        facingLeft = !facingLeft;
        UpdateFacing();
    }

    /// \brief Flips the sprite and movement direction (if a compatable component for projectile movement is attached)
    /// depending on if the projectile is currently facing left or right.
    protected void UpdateFacing()
    {
        // Flip sprite (if enabled)
        if (flipSprite)
        {
            if (facingLeft)
            {
                sprite.transform.localScale = new Vector3(
                    -Mathf.Abs(-sprite.transform.localScale.x),
                    sprite.transform.localScale.y,
                    sprite.transform.localScale.z
                );
            }
            else
            {
                sprite.transform.localScale = new Vector3(
                    Mathf.Abs(-sprite.transform.localScale.x),
                    sprite.transform.localScale.y,
                    sprite.transform.localScale.z
                );
            }
        }
        
        // change movement direction of MoveInDirection component (if there is one)
        if (TryGetComponent<RudimentaryMovement>(out var rm))
        {
            rm.HorizontalDirectionChange(facingLeft);
        }
    }
}
