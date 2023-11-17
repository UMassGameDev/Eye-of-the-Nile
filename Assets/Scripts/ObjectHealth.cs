using System.Collections;
using System.Collections.Generic;
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
    int currentHealth;
    public bool canBeInvincible = false;
    public float invincibleDuration = 3f;
    private float flashDuration = 0.25f;
    WaitForSeconds invincibleFlash;
    private bool isInvincible = false;

    IEnumerator Invincibility()
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

    void ToggleTransparency(bool isOn)
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

    public void TakeDamage(Transform attacker, int damage)
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

        if (gameObject.name == "Player")
        {
            AudioManager.Instance.PlaySFX("player_take_damage");
        }

        if (currentHealth <= 0)
            Die();

        if (currentHealth > 0 && canBeInvincible)
            StartCoroutine(Invincibility());
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    void Die()
    {
        animator.SetBool("IsDead", true);
        AudioManager.Instance.PlaySFX(deathSfxName);
        /*
        if (gameObject.name == "Player")
        {
            AudioManager.Instance.PlaySFX("player_death");
        }
        else if (gameObject.name.Contains("Breakable Pot"))
        {
            AudioManager.Instance.PlaySFX("breakable_pot_destroy");
        }
        else if (gameObject.name.Contains("Bad Guy"))
        {
            AudioManager.Instance.PlaySFX("enemy_death");
        }
        */
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        this.enabled = false;
    }

}
