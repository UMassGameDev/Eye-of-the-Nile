using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// Handles health, damage, and death of an object/entity.
/// If you want an object to have health, take damage, and die, put this script on it.
/// 
/// Documentation updated 8/12/2024
/// </summary>
/// \author Stephen Nuttall
public class ObjectHealth : MonoBehaviour
{
    /// <summary> Reference to the object’s sprite renderer. </summary>
    public SpriteRenderer sRenderer;
    /// <summary> Reference to the object’s animator. </summary>
    public Animator animator;
    /// <summary> Reference to the particle effects the object should spawn when taking damage. </summary>
    public Transform hurtEffect;  
    /// <summary> Reference to the sprite that should be overlaid the object’s sprite when the object is on fire. </summary>
    public SpriteRenderer onFireSprite;  
    /// <summary> Reference to the sound effect the object should play when it dies. </summary>
    public string deathSfxName;  

    /// <summary> The object is dead if the currentHealth falls to 0. </summary>
    public bool IsDead { get { return currentHealth <= 0; } }  
    /// <summary> If enabled, the object will spawn the given hurtEffect particles when taking damage. </summary>
    public bool enableDamageParticles = true;  

    /// <summary> The maximum health the object can have. </summary>
    public int maxHealth = 100;  
    /// <summary> The amount of health the object currently has. </summary>
    public int currentHealth { get; protected set; }  

    /// <summary> The amount of souls the object should grant the player upon death. To disable, set to <= 0. </summary>
    public int soulsDroppedOnDeath = 0;  
    /// <summary> The amount of god souls the object should grant the player upon death. To disable, set to <= 0. </summary>
    public int godSoulsDroppedOnDeath = 0;  

    /// <summary>
    /// Triggered when the object dies (if soulsDroppedOnDeath > 0),
    /// telling the DataManager that the amount of souls the player has collected has increased.
    /// </summary>
    public static event Action<int> soulsDropped;

    /// <summary>
    /// Triggered when the object dies (if godSoulsDroppedOnDeath > 0),
    /// telling the DataManager that the amount of god souls the player has collected has increased.
    /// </summary>
    public static event Action<int> godSoulsDropped;

    /// <summary> If enabled, the object will be invincible for a short duration after taking damage (invincibility frames) </summary>
    public bool canBeInvincible = false;  
    /// <summary> In seconds, how long invincibility lasts for after taking damage. </summary>
    public float invincibleDuration = 3f;  
    /// <summary> In seconds, how often the sprite should swap between being opaque and transparent (creating a flashing effect). </summary>
    protected float flashDuration = 0.25f;  
    /// <summary> The WaitForSeconds variable that is used to implement flashDuration. </summary>
    protected WaitForSeconds invincibleFlash;  
    /// <summary> If the object is currently invincible. </summary>
    protected bool isInvincible = false;  

    /// <summary> Whether or not the object can be set on fire. </summary>
    public bool canBeOnFire = true;  
    /// <summary> Whether or not the object is currently on fire. </summary>
    protected bool onFire = false;  
    /// <summary> Whether or not the object currently has immunity to fire (usually from an ability). </summary>
    protected bool fireImmune = false;  

    /// <summary>
    /// Makes object invincible for invincibleDuration seconds. Toggles transparency every invincibleFlash seconds.
    /// </summary>
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

    /// <summary>
    /// Toggles the reduced opacity of the sprite that is used to create the flashing effect when invincible.
    /// </summary>
    /// <param name="isOn"></param>
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

    /// <summary>
    /// Initialize invincibleFlash.
    /// </summary>
    void Awake()
    {
        invincibleFlash = new WaitForSeconds(flashDuration);
    }

    /// <summary>
    /// Set currentHealth to maxHealth.
    /// </summary>
    void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Set currentHealth back to maxHealth.
    /// </summary>
    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// If something attacks the object, it will run ObjectHealth.TakeDamage()
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="damage"></param>
    public virtual void TakeDamage(Transform attacker, int damage)
    {
        // if the object is currently invincible, skip this function.
        if (isInvincible)
            return;
        
        // subtract the damage done and play the damage animation.
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        // generate hurt particles (if enabled)
        Collider2D objectCollider = transform.GetComponent<Collider2D>();
        if (enableDamageParticles)
        {
            Transform hurtPrefab = Instantiate(hurtEffect,
                    objectCollider.bounds.center,
                    Quaternion.identity);
            hurtPrefab.up = new Vector3(attacker.position.x - objectCollider.transform.position.x, 0f, 0f);
        }

        // If the health is now beneath 0, die (respectfully)
        if (currentHealth <= 0)
            Die();

        // Start invinciblity frames (if enabled)
        if (currentHealth > 0 && canBeInvincible)
            StartCoroutine(Invincibility());
    }

    /// <summary>
    /// Similar functionality to TakeDamage(), but no invincibility is triggered and no animation players. Used when on fire.
    /// </summary>
    /// <param name="damage"></param>
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

    /// <summary>
    /// Triggered when the object runs out of health. Plays a death sound and animation, drops souls/god souls, and disables the object.
    /// </summary>
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

    /// <summary>
    /// Starts damaging the object (with FireDamage()) repeatedly. A fire sprite will appear over the object while it's on fire.
    /// </summary>
    /// <param name="damageCount"></param>
    /// <param name="damageSpeed"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Start a coroutine for FireImmunityTimer().
    /// </summary>
    /// <param name="seconds"></param>
    public void FireImmunity(float seconds) { StartCoroutine(FireImmunityTimer(seconds)); }

    /// <summary>
    /// Make the object immune to fire for the given amount of seconds.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    protected IEnumerator FireImmunityTimer(float seconds)
    {
        fireImmune = true;
        yield return new WaitForSeconds(seconds);
        fireImmune = false;
    }
}
