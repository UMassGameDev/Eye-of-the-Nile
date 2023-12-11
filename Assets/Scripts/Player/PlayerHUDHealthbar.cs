using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthbarGUI : MonoBehaviour
{
    public RectTransform healthbarTransform;
    public TMP_Text healthText;
    PlayerHealth playerHealth;

    public float healthbarXLoc;  // this is where the healthbar's x should be "pinned" to
    float healthMultiplier = 5f;

    void OnEnable()
    {
        PlayerHealth.onPlayerHealthChange += updateHealthBar;
    }

    void OnDisable()
    {
        PlayerHealth.onPlayerHealthChange -= updateHealthBar;
    }

    void Awake()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    void Start()
    {
        healthMultiplier = healthbarTransform.sizeDelta.x/playerHealth.maxHealth;
    }
    
    void updateHealthBar(int newHealth)
    {
        healthbarTransform.sizeDelta = new Vector2(newHealth * healthMultiplier, healthbarTransform.sizeDelta.y);

        float XOffset = (playerHealth.maxHealth - newHealth)  * healthMultiplier/2;
        healthbarTransform.anchoredPosition = new Vector2(healthbarXLoc + XOffset, healthbarTransform.anchoredPosition.y);

        if (newHealth > 0) {
            healthText.text = newHealth + "/" + playerHealth.maxHealth;
        } else {
            healthText.text = "0/" + playerHealth.maxHealth;
        }
    }
}
