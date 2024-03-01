/**************************************************
Handles health, damage, and death of an object/entity.

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;
using System;

public class ObjectHealth : MonoBehaviour
{
    public SpriteRenderer sRenderer;
    public Animator animator;
    public Transform hurtEffect;
    public string deathSfxName;

    public bool IsDead { get { return currentHealth <= 0; } }
    public bool enableDamageParticles = true;

    public int maxHealth = 100;
    public int currentHealth { get; protected set; }

    public int soulsDroppedOnDeath = 0;
    public int godSoulsDroppedOnDeath = 0;
    public static event Action<int> soulsDropped;
    public static event Action<int> godSoulsDropped;

    public bool canBeInvincible = false;
    public float invincibleDuration = 3f;
    protected float flashDuration = 0.25f;
    protected WaitForSeconds invincibleFlash;
    protected bool isInvincible = false;

    public SpriteRenderer onFireSprite;
    public bool canBeOnFire = true;
    protected bool onFire = false;
    protected bool fireImmune = false;

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

    void Awake()
    {
        invincibleFlash = new WaitForSeconds(flashDuration);
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(Transform attacker, int damage)
    {
        if (isInvincible)
            return;
        
        currentHealth -= damage;
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

        if (currentHealth <= 0)
            Die();

        if (currentHealth > 0 && canBeInvincible)
            StartCoroutine(Invincibility());
    }

    protected virtual void FireDamage(int damage)
    {
        if (fireImmune || !canBeOnFire)
            return;
        
        currentHealth -= damage;
        Collider2D objectCollider = transform.GetComponent<Collider2D>();

        // generate hurt particles (if enabled)
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

    public void FireImmunity(float seconds) { StartCoroutine(FireImmunityTimer(seconds)); }

    protected IEnumerator FireImmunityTimer(float seconds)
    {
        fireImmune = true;
        yield return new WaitForSeconds(seconds);
        fireImmune = false;
    }
}
