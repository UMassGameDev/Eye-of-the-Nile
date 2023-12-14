using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthbarGUI : MonoBehaviour
{
    public RectTransform healthbarUnder;
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
        healthMultiplier = healthbarUnder.sizeDelta.x/playerHealth.MaxHealth;
    }
    
    void updateHealthBar(int newHealth)
    {
        healthMultiplier = healthbarUnder.sizeDelta.x / playerHealth.MaxHealth;
        healthbarTransform.sizeDelta = new Vector2(newHealth * healthMultiplier, healthbarTransform.sizeDelta.y);

        float XOffset = (playerHealth.MaxHealth - newHealth)  * healthMultiplier/2;
        healthbarTransform.anchoredPosition = new Vector2(healthbarXLoc - XOffset, healthbarTransform.anchoredPosition.y);

        if (newHealth > 0) {
            healthText.text = newHealth + "/" + playerHealth.MaxHealth;
        } else {
            healthText.text = "0/" + playerHealth.MaxHealth;
        }
    }
}
