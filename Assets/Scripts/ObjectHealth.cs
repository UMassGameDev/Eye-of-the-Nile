using System.Collections;
using UnityEngine;
using System;

/// \brief
/// Handles health, damage, and death of an object/entity.
/// If you want an object to have health, take damage, and die, put this script on it.
/// 
/// Documentation updated 9/15/2024
/// \author Stephen Nuttall
public class ObjectHealth : MonoBehaviour
{
    /// Reference to the object’s sprite renderer
    public SpriteRenderer sRenderer;
    /// Reference to the object’s animator
    public Animator animator;
    /// Reference to the particle effects the object should spawn when taking damage
    public Transform hurtEffect;  
    /// Reference to the sprite that should be overlaid the object’s sprite when the object is on fire
    public SpriteRenderer onFireSprite;  
    /// Reference to the sound effect the object should play when it dies
    public string deathSfxName;  

    /// The object is dead if the currentHealth falls to 0
    public bool IsDead { get { return currentHealth <= 0; } }  
    /// If enabled, the object will spawn the given hurtEffect particles when taking damage
    public bool enableDamageParticles = true;  

    /// The maximum health the object can have
    public int maxHealth = 100;  
    /// The amount of health the object currently has
    public int currentHealth { get; protected set; }  

    /// The amount of souls the object should grant the player upon death. To disable, set to <= 0
    public int soulsDroppedOnDeath = 0;  
    /// The amount of god souls the object should grant the player upon death. To disable, set to <= 0
    public int godSoulsDroppedOnDeath = 0;  

    /// \brief Triggered when the object dies (if soulsDroppedOnDeath > 0),
    /// telling the DataManager that the amount of souls the player has collected has increased.
    public static event Action<int> soulsDropped;

    /// \brief Triggered when the object dies (if godSoulsDroppedOnDeath > 0),
    /// telling the DataManager that the amount of god souls the player has collected has increased.
    public static event Action<int> godSoulsDropped;

    /// If enabled, the object will be invincible for a short duration after taking damage (invincibility frames) </summary>
    public bool canBeInvincible = false;  
    /// In seconds, how long invincibility lasts for after taking damage
    public float invincibleDuration = 3f;  
    /// In seconds, how often the sprite should swap between being opaque and transparent (creating a flashing effect)
    protected float flashDuration = 0.25f;  
    /// The WaitForSeconds variable that is used to implement flashDuration
    protected WaitForSeconds invincibleFlash;  
    /// If the object is currently invincible
    protected bool isInvincible = false;  

    /// Whether or not the object can be set on fire
    public bool canBeOnFire = true;  
    /// Whether or not the object is currently on fire
    protected bool onFire = false;  
    /// Whether or not the object currently has immunity to fire (usually from an ability)
    protected bool fireImmune = false;  

    /// \brief Makes object invincible for invincibleDuration seconds. Toggles transparency every invincibleFlash seconds.
    protected IEnumerator Invincibility()
    {
        isInvincible = true;
        float invincibleElapsed = 0f;
        bool inFlash = true;
        while (invincibleElapsed < invincibleDuration)
        {
            ToggleTransparency(inFlash);
            yield return invincibleFlash;
            invincibleElapsed += flashDuration;
            inFlash = !inFlash;
        }
        isInvincible = false;
    }

    /// \brief Toggles the reduced opacity of the sprite that is used to create the flashing effect when invincible.
    protected void ToggleTransparency(bool isOn)
    {
        if (isOn)
        {
            Color cColor = sRenderer.color;
            sRenderer.color = new Color(cColor.r, cColor.g, cColor.b, 0.30f);
        }
        else
        {
            Color cColor = sRenderer.color;
            sRenderer.color = new Color(cColor.r, cColor.g, cColor.b, 1f);
        }
    }

    /// \brief Initialize invincibleFlash.
    void Awake()
    {
        invincibleFlash = new WaitForSeconds(flashDuration);
    }

    /// \brief Set currentHealth to maxHealth.
    void Start()
    {
        currentHealth = maxHealth;
    }

    /// \brief Set currentHealth back to maxHealth.
    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    /// \brief If something attacks the object, it will run ObjectHealth.TakeDamage()
    /// 
    /// Steps:
    /// <param name="attacker">Reference to the transform of the attacker. Used to determine which direction particles should come from.</param>
    /// <param name="damage">The amount of damage to deal to this object.</param>
    public virtual void TakeDamage(Transform attacker, int damage)
    {
        /// - If the object is currently invincible, skip this function.
        if (isInvincible)
            return;
        
        /// - Subtract the damage done and play the damage animation.
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        /// - Generate hurt particles (if enabled).
        Collider2D objectCollider = transform.GetComponent<Collider2D>();
        if (enableDamageParticles)
        {
            Transform hurtPrefab = Instantiate(hurtEffect,
                    objectCollider.bounds.center,
                    Quaternion.identity);
            hurtPrefab.up = new Vector3(attacker.position.x - objectCollider.transform.position.x, 0f, 0f);
        }

        /// - If the health is now beneath 0, die (respectfully).
        if (currentHealth <= 0)
            Die();

        /// - If not, start invinciblity frames (if enabled).
        if (currentHealth > 0 && canBeInvincible)
            StartCoroutine(Invincibility());
    }

    /// \brief Similar functionality to TakeDamage(), but no invincibility is triggered and no animation players. Used when on fire.
    protected virtual void FireDamage(int damage)
    {
        if (fireImmune || !canBeOnFire)
            return;
        
        currentHealth -= damage;

        // generate hurt particles (if enabled)
        Collider2D objectCollider = transform.GetComponent<Collider2D>();
        if (enableDamageParticles)
        {
            Transform hurtPrefab = Instantiate(hurtEffect,
                    objectCollider.bounds.center,
                    Quaternion.identity);
            hurtPrefab.up = new Vector3(gameObject.transform.position.x - objectCollider.transform.position.x, 0f, 0f);
        }

        if (currentHealth <= 0)
            Die();
    }

    /// \brief Triggered when the object runs out of health. Plays a death sound and animation, drops souls/god souls, and disables the object.
    protected virtual void Die()
    {
        animator.SetBool("IsDead", true);
        AudioManager.Instance.PlaySFX(deathSfxName);

        if (soulsDroppedOnDeath > 0)
            soulsDropped?.Invoke(soulsDroppedOnDeath);
        
        if (godSoulsDroppedOnDeath > 0)
            godSoulsDropped?.Invoke(godSoulsDroppedOnDeath);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        this.enabled = false;
    }

    /// \brief Starts damaging the object (with FireDamage()) repeatedly. A fire sprite will appear over the object while it's on fire.
    /// <param name="damageCount">The amount of times the fire will deal damage before the object stops being on fire.</param>
    /// <param name="damageSpeed">The amount of seconds between each round of dealing damage.</param>
    /// <param name="damage">The amount of damage to deal each time.</param>
    public IEnumerator SetOnFire(int damageCount, float damageSpeed, int damage)
    {
        if (canBeOnFire && !onFire && !isInvincible)
        {
            AudioManager.Instance.PlaySFX("set_on_fire");
            onFire = true;
            if (onFireSprite != null)
                onFireSprite.enabled = true;
            for (int i = 0; i < damageCount && !IsDead; i++)
            {
                FireDamage(damage);
                yield return new WaitForSeconds(damageSpeed);
            }
            onFire = false;
            if (onFireSprite != null)
                onFireSprite.enabled = false;
        }
    }

    /// \brief Start a coroutine for FireImmunityTimer().
    public void FireImmunity(float seconds) { StartCoroutine(FireImmunityTimer(seconds)); }

    /// \brief Make the object immune to fire for the given amount of seconds.
    protected IEnumerator FireImmunityTimer(float seconds)
    {
        fireImmune = true;
        yield return new WaitForSeconds(seconds);
        fireImmune = false;
    }
}
