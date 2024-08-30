using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

/** \brief
Functionality of ObjectHealth, specifically for the player.
This script inherits from ObjectHealth. Here are the key changes:
- Some variables get their values from the PlayerStatHolder component instead of inside this script.
- Support for damage resistance and fire resistance stats.
- When the player dies, \ref Scenes_Anubis is loaded, where Anubis tells joke based on how the player died.
- 5 different events are available for other functions in other scripts to subscribe to. Many of these are used by the data manager.
- A function is available mainly for abilities to use that instantly heals the player.

Documentation updated 8/30/2024
\author Stephen Nuttall
*/
public class PlayerHealth : ObjectHealth
{
    /** @name Player Stats
    *  These variables are retrieved from the PlayerStatHolder component, rather than being stored in here. (see StatsAE for more info)
    */
    ///@{
    /// Reference to the PlayerStatHolder itself. Holds various information that ability stat modifiers (StatsAE) can modify.
    public PlayerStatHolder PStats { get; set; }
    /// The maximum health of the player.
    public int MaxHealth { get {
            if (PStats == null)
                PStats = GetComponent<PlayerStatHolder>();
            if (!PStats.IsInitialized)
                return PStats.GetValue("MaxHealth");
            else
            {
                PStats.InitializeDictionary();
                return PStats.GetValue("MaxHealth");
            }
        }}  // Maximum health the player can have
    /// \brief The amount of damage resistance the player currently has.
    /// This value is subtracted from any amount of non-fire damage the player takes.
    /// If the resulting number is <= 0, no damage is done at all.
    public int DamageResistance { get {
            if (PStats == null)
                PStats = GetComponent<PlayerStatHolder>();
            if (!PStats.IsInitialized)
                return PStats.GetValue("DamageResistance");
            else
            {
                PStats.InitializeDictionary();
                return PStats.GetValue("DamageResistance");
            }
        }}  // Subtracted from any damage taken from normal attacks
            // Does not apply to special damage types like fire damage
    /// \brief The amount of fire reistance the player currently has.
    /// The value is subtracted from any amount of fire damage the player takes.
    /// If the resulting number is <= 0, no damage is done at all.
    public int FireResistance { get {
            if (PStats == null)
                PStats = GetComponent<PlayerStatHolder>();
            if (!PStats.IsInitialized)
                return PStats.GetValue("FireResistance");
            else
            {
                PStats.InitializeDictionary();
                return PStats.GetValue("FireResistance");
            }
        }}  // Subtracted from any damage taken from fire
            // negative fire resistance will give total immunity to fire damage
    ///@}

    /** @name Dying and post-death
    *  Information related to the the transition to \ref Scenes_Anubis after the player dies, and what joke Anubis should tell.
    */
    ///@{
    /// The amount of time to wait after death before loading \ref Scenes_Anubis, in seconds.
    public float deadFadeDelay = 1f;
    /// The amount of time given for the fade to black animation to play when loading \ref Scenes_Anubis after death, in seconds.
    public float deadFadeLength = 1f;
    /// The name of the scene to load after the player dies (\ref Scenes_Anubis).
    public string deathSceneName = "Anubis";
    /// If currentDeathMessage is set to this, AnubisJokeTextbox will know to read from the default list of jokes.
    public string defaultDeathMessage = "[DEFAULT]";
    /// If currentDeathMessage is set to this, AnubisJokeTextbox will know to read from the fire list of jokes
    /// (as in fire related. They are fire jokes though).
    public string fireDeathMessage = "[FIRE]";
    /// \brief The current joke for Anubis to read after the player dies.
    /// If the attacking object doesn't have a CustomDeathMessage component, this will either be set to [DEFAULT] or [FIRE].
    public string currentDeathMessage { get; private set; }
    ///@}

    /** @name Events
    *  When these events occur, other scripts subscribed to these events can be notified and trigger their functionality
    */
    ///@{
    /// Triggers any subscribes functions when the player dies.
    public static event Action onPlayerDeath;
    /// Triggers any subscribes functions when the player respawns.
    public static event Action onPlayerRespawn;
    /// Triggers any subscribes functions when the player takes damage.
    public static event Action<int> onPlayerDamage;
    /// Triggers any subscribes functions when the player's health changes.
    public static event Action<int> onPlayerHealthChange;
    /// Triggers any subscribes functions when the player's custom death message changes.
    public static event Action<string> deathMessageChange;
    ///@}

    /// Reference to the data manager.
    DataManager dataManager;

    /// Initialize references and invincibleFlash.
    void Awake()
    {
        invincibleFlash = new WaitForSeconds(flashDuration);
        PStats = GetComponent<PlayerStatHolder>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    /// Set the current health of the player to the data manager's copy of it, and invoke onPlayerHealthChange.
    void Start()
    {
        currentHealth = dataManager.GetPlayerHealth();
        onPlayerHealthChange?.Invoke(currentHealth);
    }

    /// <summary>
    /// Apply damage to the player. If something attacks the player, it will run PlayerHealth.TakeDamage().
    /// Key differences between this an ObjectHealth.TakeDamage():
    /// - Damage cannot be applied if the damage resistance is greater than or equal to the amount of damage being applied
    /// - Events are invoked
    /// 
    /// Steps:
    /// </summary>
    /// <param name="attacker">Reference to the object that is attacking the player.</param>
    /// <param name="damage">The amount of damage being delt to the player.</param>
    public override void TakeDamage(Transform attacker, int damage)
    {
        /// - Damage can only be applied if the player is not invincible and the damage is more than the damage resistance.
        if (!isInvincible && damage > DamageResistance)
        {
            currentHealth -= damage - DamageResistance;
            animator.SetTrigger("Hurt");
            Collider2D objectCollider = transform.GetComponent<Collider2D>();

            /// - Generate hurt particles (if enabled).
            if (enableDamageParticles)
            {
                Transform hurtPrefab = Instantiate(hurtEffect,
                        objectCollider.bounds.center,
                        Quaternion.identity);
                hurtPrefab.up = new Vector3(attacker.position.x - objectCollider.transform.position.x, 0f, 0f);
            }

            // AudioManager.Instance.PlaySFX("player_take_damage");

            /// - Let any other objects subscribed to onPlayerDamage or onPlayerHealthChange know that those have happened.
            onPlayerDamage?.Invoke(currentHealth);
            onPlayerHealthChange?.Invoke(currentHealth);

            /// - If the player is at 0 health, they are dead:
            if (currentHealth <= 0)
            {
                ///  - Using the transform of the attacker passed as a parameter,
                // see if there's a custom death message for Anubis to say.
                if (attacker.TryGetComponent<CustomDeathMessage>(out var cdm)) {
                    currentDeathMessage = cdm.GetRandomJoke();
                } else {
                    currentDeathMessage = defaultDeathMessage;
                }
                deathMessageChange?.Invoke(currentDeathMessage);
                
                ///  - Then, trigger the sequence of events that happens when the player dies
                Die();
            }

            /// - If the player is still alive, start invinciblity frames (if enabled).
            if (currentHealth > 0 && canBeInvincible)
                StartCoroutine(Invincibility());
        }
    }

    /// <summary>
    /// Similar functionality to TakeDamage(), but no invincibility is triggered and no animation players. Used when on fire.
    /// </summary>
    /// <param name="damage">The amount of damage being delt to the player.</param>
    protected override void FireDamage(int damage)
    {
        if (FireResistance < 0 || fireImmune || !canBeOnFire)
            return;

        currentHealth -= damage - FireResistance;
        Collider2D objectCollider = transform.GetComponent<Collider2D>();

        // generate hurt particles (if enabled)
        if (enableDamageParticles)
        {
            Transform hurtPrefab = Instantiate(hurtEffect,
                    objectCollider.bounds.center,
                    Quaternion.identity);
            hurtPrefab.up = new Vector3(gameObject.transform.position.x - objectCollider.transform.position.x, 0f, 0f);
        }

        // AudioManager.Instance.PlaySFX("player_take_damage");

        // Let any other objects subscribed to this event know that it has happened
        onPlayerDamage?.Invoke(currentHealth);
        onPlayerHealthChange?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            currentDeathMessage = fireDeathMessage;
            deathMessageChange?.Invoke(currentDeathMessage);
            Die();
        }
    }

    /// <summary>
    /// Instantly heals the player. Mainly used by abilities.
    /// </summary>
    /// <param name="healValue">Amount of health to heal the player by.</param>
    public void HealInstant(int healValue)
    {
        if (IsDead)
            return;
        currentHealth = currentHealth + healValue > MaxHealth ? MaxHealth : currentHealth + healValue;
        onPlayerDamage?.Invoke(currentHealth);
        onPlayerHealthChange?.Invoke(currentHealth);
    }

    /// Returns the amount of health the player currently has.
    public int GetHealth()
    {
        return currentHealth;
    }

    /// \brief Checks if the player's health is above the max health, and if so, corrects it. Also invokes onPlayerHealthChange.
    /// Used to update the player's health when changed from outside the script
    public void InvokeHealthChange()
    {
        if (currentHealth > MaxHealth)
            currentHealth = MaxHealth;
        onPlayerHealthChange?.Invoke(currentHealth);
    }

    /// \brief Runs when the player dies to stop gameplay and run AfterDeath(). 
    /// Runs an animation, disables collision, physics, and player movement, and starts a corountine for AfterDeath().
    protected override void Die()
    {
        animator.SetBool("IsDead", true);
        AudioManager.Instance.PlaySFX(deathSfxName);
        
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<PlayerMovement>().enabled = false;

        // Let any other objects subscribed to this event know that it has happened
        onPlayerDeath?.Invoke();

        StartCoroutine(AfterDeath());
    }

    /// \brief After the player has died, load \ref Scenes_Anubis and renable critical components.
    /// 
    /// Steps:
    IEnumerator AfterDeath()
    {
        /// - Wait a brief amount of time after dying to allow the user to see what happened.
        yield return new WaitForSeconds(deadFadeDelay);
        /// - Load \ref Scenes_Anubis with the stage loader.
        GameObject.Find("StageLoader").GetComponent<StageLoader>().LoadNewStage(deathSceneName);

        /// - After the fade to black animation is over, re-enable collision, physics, and player movement.
        yield return new WaitForSeconds(deadFadeLength);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<PlayerMovement>().enabled = true;
        currentHealth = MaxHealth;

        /// - Let any other objects subscribed to onPlayerRespawn and onPlayerHealthChange know that these have happened.
        onPlayerRespawn?.Invoke();
        onPlayerHealthChange?.Invoke(MaxHealth);
    }
}
