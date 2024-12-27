using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthbar : MonoBehaviour
{
    /// The object with the ObjectHealth script attached to be displayed.
    [SerializeField] private ObjectHealth bossHealth;
    /// The red healthbar part of this object.
    [SerializeField] private RectTransform healthbar;

    /// The percentage of the boss's health remaining.
    public float healthbarPercentage;

    private float maxHealth;
    private float currentHealth;
    private float initialHealthbarWidth;
    private float initialHealthbarX;

    void Start()
    {
        maxHealth = bossHealth.GetMaxHealth();
        currentHealth = bossHealth.currentHealth;
        initialHealthbarWidth = healthbar.rect.width;
        initialHealthbarX = healthbar.position.x;
        UpdateHealthbar();
    }
    
    void Update()
    {
        // Detect changes in the boss's health and adjust the healthbar size accordingly.
        if (currentHealth != bossHealth.currentHealth)
        {
            currentHealth = bossHealth.currentHealth;
            UpdateHealthbar();
            if (currentHealth < 0)
            {
                OnBossDefeated();
            }
        }
    }

    void UpdateHealthbar()
    {
        // Update the percentage of boss health remaining.
        healthbarPercentage = currentHealth / maxHealth;
        // Set the healthbar's width to the initial width times the percentage of health remaining.
        healthbar.sizeDelta = new Vector2(initialHealthbarWidth * healthbarPercentage, healthbar.sizeDelta.y);
        // TODO update the healthbar's x position to be left-aligned.
    }

    void OnBossDefeated()
    {
        gameObject.SetActive(false);
    }
}