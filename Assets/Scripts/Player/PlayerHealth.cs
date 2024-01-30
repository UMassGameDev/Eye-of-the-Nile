/**************************************************
Functionality of ObjectHealth.cs specifically for the player.
Inherits from ObjectHealth.

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;
using System;

public class PlayerHealth : ObjectHealth
{
    // The following variable have to be obtained from PlayerStatHolder.cs
    public PlayerStatHolder PStats { get; set; }
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

    public float deadFadeDelay = 1f;
    public float deadFadeLength = 1f;

    // When these events occur, other scripts subscribed to these events can be notified and trigger their functionality
    public static event Action onPlayerDeath;
    public static event Action onPlayerRespawn;
    public static event Action<int> onPlayerDamage;
    public static event Action<int> onPlayerHealthChange;

    DataManager dataManager;

    void Awake()
    {
        invincibleFlash = new WaitForSeconds(flashDuration);
        PStats = GetComponent<PlayerStatHolder>();
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    void Start()
    {
        currentHealth = dataManager.GetPlayerHealth();
        onPlayerHealthChange?.Invoke(currentHealth);
    }

    public override void TakeDamage(Transform attacker, int damage)
    {
        // damage can only be applied if the player is not invincible and the damage is more than the damage resistance
        if (!isInvincible && damage > DamageResistance)
        {
            currentHealth -= damage - DamageResistance;
            animator.SetTrigger("Hurt");
            Collider2D objectCollider = transform.GetComponent<Collider2D>();

            // generate hurt particles (if enabled)
            if (enableDamageParticles)
            {
                Transform hurtPrefab = Instantiate(hurtEffect,
                        objectCollider.bounds.center,
                        Quaternion.identity);
                hurtPrefab.up = new Vector3(attacker.position.x - objectCollider.transform.position.x, 0f, 0f);
            }

            // AudioManager.Instance.PlaySFX("player_take_damage");

            // Let any other objects subscribed to this event know that it has happened
            onPlayerDamage?.Invoke(currentHealth);
            onPlayerHealthChange?.Invoke(currentHealth);

            if (currentHealth <= 0)
                Die();

            if (currentHealth > 0 && canBeInvincible)
                StartCoroutine(Invincibility());
        }
    }

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
            Die();
    }

    public void HealInstant(int healValue)
    {
        if (IsDead)
            return;
        currentHealth = currentHealth + healValue > MaxHealth ? MaxHealth : currentHealth + healValue;
        onPlayerDamage?.Invoke(currentHealth);
        onPlayerHealthChange?.Invoke(currentHealth);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void InvokeHealthChange()
    {
        if (currentHealth > MaxHealth)
            currentHealth = MaxHealth;
        onPlayerHealthChange?.Invoke(currentHealth);
    }

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

    IEnumerator AfterDeath()
    {
        yield return new WaitForSeconds(deadFadeDelay);
        GameObject.Find("StageLoader").GetComponent<StageLoader>().LoadNewStage("this");

        yield return new WaitForSeconds(deadFadeLength);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<PlayerMovement>().enabled = true;
        currentHealth = MaxHealth;

        // Let any other objects subscribed to this event know that it has happened
        onPlayerRespawn?.Invoke();
        onPlayerHealthChange?.Invoke(MaxHealth);
    }
}
